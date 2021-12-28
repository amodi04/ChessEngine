using System;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;

namespace Engine.AI;

/// <summary>
///     This class handles the ordering move lists of moves.
/// </summary>
public class MoveOrdering
{
    // Maximum number of moves in any given board state
    private const int MaxMoveCount = 218;
    private const int CapturedPieceValueMultiplier = 10;

    // Threshold at which to switch to quick sort rather than insertion sort
    private const int MoveLengthThreshold = 30;
    private int[] _moveScores;
    private bool _useQuickSort;

    public MoveOrdering()
    {
        _moveScores = new int[MaxMoveCount];
    }

    /// <summary>
    ///     Orders a given list of moves.
    /// </summary>
    /// <param name="board">The board for context.</param>
    /// <param name="moves">The list of moves.</param>
    /// <param name="transpositionTable">The history of boards searched.</param>
    /// <returns>An IEnumerable collection of moves.</returns>
    public IEnumerable<IMove> OrderMoves(Board board, List<IMove> moves, TranspositionTable transpositionTable)
    {
        var hashMove = transpositionTable.GetStoredMove();
        _useQuickSort = moves.Count > MoveLengthThreshold;
        _moveScores = _useQuickSort ? new int[moves.Count] : new int[MaxMoveCount];

        for (var i = 0; i < moves.Count; i++)
        {
            var move = moves[i];
            var score = 0;

            var piece = board.GetTile(move.FromCoordinate).Piece;
            if (piece is not null)
            {
                var movedPieceType = piece.Type;
                PieceType? capturedPieceType =
                    move is CaptureMove captureMove ? captureMove.CapturedPiece.Type : null;

                if (capturedPieceType is not null)
                    score = CapturedPieceValueMultiplier * GetPieceValue(capturedPieceType) -
                            GetPieceValue(movedPieceType);

                if (movedPieceType == PieceType.Pawn)
                    if (move is PromotionMove promotionMove)
                        score += GetPieceValue(promotionMove.PromotedPiece.Type);
            }

            // If the move is the same as the best move stored from a previous search, prioritise it
            if (move.FromCoordinate == hashMove?.FromCoordinate && move.ToCoordinate == hashMove.FromCoordinate)
                score += 10000;

            _moveScores[i] = score;
        }

        if (_useQuickSort)
            QuickSort(moves);
        else
            InsertionSort(moves);
        return moves;
    }

    /// <summary>
    ///     Sorts a list of moves using insertion sort.
    /// </summary>
    /// <param name="moves">The moves to sort.</param>
    private void InsertionSort(IList<IMove> moves)
    {
        for (var i = 0; i < moves.Count; i++)
        for (var j = i + 1; j > 0; j--)
        {
            var swapIndex = j - 1;
            if (_moveScores[swapIndex] >= _moveScores[j]) continue;
            (moves[j], moves[swapIndex]) = (moves[swapIndex], moves[j]);
            (_moveScores[j], _moveScores[swapIndex]) = (_moveScores[swapIndex], _moveScores[j]);
        }
    }

    /// <summary>
    ///     Auxiliary QuickSort function for readability.
    ///     Makes the initial call to the recursive function.
    /// </summary>
    /// <param name="moves">The moves to sort.</param>
    private void QuickSort(IList<IMove> moves)
    {
        QuickSort(moves, 0, moves.Count - 1);
    }

    /// <summary>
    ///     Sorts a list of moves using quicksort.
    ///     Sorts a portion of an array, divides it into partitions, then sorts those.
    /// </summary>
    /// <param name="moves">The moves to sort.</param>
    /// <param name="low">The lower bound of the partition.</param>
    /// <param name="high">The higher bound of the partition.</param>
    private void QuickSort(IList<IMove> moves, int low, int high)
    {
        if (low < 0 || high < 0 || low >= high) return;

        // Pivot is the partitioning index
        var pivot = Partition(moves, low, high);

        // Sort left sub array
        QuickSort(moves, low, pivot);

        // Sort right sub array
        QuickSort(moves, pivot + 1, high);
    }

    /// <summary>
    ///     Partitions an array into sub arrays and sorts the sub array.
    /// </summary>
    /// <param name="moves">The list of moves to sort.</param>
    /// <param name="low">The lower bound of the sub array.</param>
    /// <param name="high">The upper bound of the sub array.</param>
    /// <returns>A partition index signifying where to split the array.</returns>
    private int Partition(IList<IMove> moves, int low, int high)
    {
        // Set the pivot to the middle of the array
        var pivot = _moveScores[(int)Math.Floor((high + low) / 2d)];

        // Left index
        var i = low - 1;

        // Right index
        var j = high + 1;

        while (true)
        {
            // Move the left index to the right at least once and while the element
            // at the left index is greater than the pivot
            do
            {
                i++;
            } while (_moveScores[i] > pivot);

            // Move the right index to the left at least once and while the element
            // at the right index is less than the pivot
            do
            {
                j--;
            } while (_moveScores[j] < pivot);

            // If the indices have crossed return the new pivot point (point of crossing)
            if (i >= j) return j;

            (_moveScores[i], _moveScores[j]) = (_moveScores[j], _moveScores[i]);
            (moves[i], moves[j]) = (moves[j], moves[i]);
        }
    }

    /// <summary>
    ///     Gets the value for a given piece type.
    /// </summary>
    /// <param name="pieceType">The piece type to get the value for.</param>
    /// <returns>An integer value.</returns>
    private static int GetPieceValue(PieceType? pieceType)
    {
        return pieceType switch
        {
            PieceType.Bishop => BetterEvaluator.BishopValue,
            PieceType.Pawn => BetterEvaluator.PawnValue,
            PieceType.Knight => BetterEvaluator.KnightValue,
            PieceType.Rook => BetterEvaluator.RookValue,
            PieceType.Queen => BetterEvaluator.QueenValue,
            _ => 0
        };
    }
}