using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;

namespace Engine.AI
{
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
        /// <returns>An IEnumerable collection of moves.</returns>
        public IEnumerable<IMove> OrderMoves(Board board, List<IMove> moves, TranspositionTable transpositionTable)
        {
            IMove? hashMove = transpositionTable.GetStoredMove();
            _useQuickSort = moves.Count > MoveLengthThreshold;
            _moveScores = _useQuickSort ? new int[moves.Count] : new int[MaxMoveCount];
            
            for (var i = 0; i < moves.Count; i++)
            {
                var move = moves[i];
                int score = 0;
                
                PieceType movedPieceType = board.GetTile(move.FromCoordinate).Piece.Type;

                // Get the piece type of the captured piece (if there isn't one, set as null)
                PieceType? capturedPieceType =
                    move is CaptureMove captureMove ? captureMove.CapturedPiece.Type : null;
                
                if (capturedPieceType is not null)
                {
                    // Set the score to the captured piece multiplier multiplied by the captured piece take away the moved piece.
                    // This score is an estimate for potentially good moves. This allows the alpha beta search to look at these moves
                    // therefore decreasing evaluation because it doesn't need to waste time looking at worse moves later.
                    score = CapturedPieceValueMultiplier * GetPieceValue(capturedPieceType) -
                            GetPieceValue(movedPieceType);
                }
                
                if (movedPieceType == PieceType.Pawn)
                {
                    if (move is PromotionMove promotionMove)
                    {
                        // Add the value of the piece to promote to because this is classed as a good move
                        score += GetPieceValue(promotionMove.PromotedPiece.Type);
                    }
                }

                // If the move is the same as the best move stored from a previous search, prioritise it
                if (move.FromCoordinate == hashMove?.FromCoordinate && move.ToCoordinate == hashMove?.FromCoordinate)
                {
                    score += 10000;
                }
                
                // Set the score
                _moveScores[i] = score;
            }

            // Sort the moves
            if (_useQuickSort)
            {
                QuickSort(moves);
            }
            else
            {
                InsertionSort(moves);
            }

            // Return the sorted moves
            return moves;
        }
        
        /// <summary>
        ///     Sorts a list of moves using insertion sort.
        /// </summary>
        /// <param name="moves">The moves to sort.</param>
        private void InsertionSort(IList<IMove> moves)
        {
            for (var i = 0; i < moves.Count; i++)
            {
                for (var j = i + 1; j > 0; j--)
                {
                    var swapIndex = j - 1;
                    if (_moveScores[swapIndex] < _moveScores[j])
                    {
                        // Swap the moves
                        (moves[j], moves[swapIndex]) = (moves[swapIndex], moves[j]);
                        
                        // Swap the move scores
                        (_moveScores[j], _moveScores[swapIndex]) = (_moveScores[swapIndex], _moveScores[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Auxiliary QuickSort function for readability.
        /// Makes the initial call to the recursive function.
        /// </summary>
        /// <param name="moves">The moves to sort.</param>
        private void QuickSort(IList<IMove> moves)
        {
            QuickSort(moves, 0, moves.Count - 1);
        }
        
        /// <summary>
        /// Sorts a list of moves using quicksort.
        /// Sorts a portion of an array, divides it into partitions, then sorts those.
        /// </summary>
        /// <param name="moves">The moves to sort.</param>
        /// <param name="low">The lower bound of the partition.</param>
        /// <param name="high">The higher bound of the partition.</param>
        private void QuickSort(IList<IMove> moves, int low, int high)
        {
            if (low >= 0 && high >= 0 && low < high)
            {
                // Pivot is the partitioning index
                var pivot = Partition(moves, low, high); 
                
                // Sort left sub array
                QuickSort(moves, low, pivot);
                
                // Sort right sub array
                QuickSort(moves, pivot + 1, high);
            }
        }

        /// <summary>
        /// Partitions an array into sub arrays and sorts the sub array.
        /// </summary>
        /// <param name="moves">The list of moves to sort.</param>
        /// <param name="low">The lower bound of the sub array.</param>
        /// <param name="high">The upper bound of the sub array.</param>
        /// <returns>A partition index signifying where to split the array.</returns>
        private int Partition(IList<IMove> moves, int low, int high)
        {
            // Set the pivot to the middle of the array
            int pivot = _moveScores[(int) Math.Floor((high + low) / 2d)];
            
            // Left index
            int i = low - 1;
            
            // Right index
            int j = high + 1;

            while (true)
            {
                // Move the left index to the right at least once and while the element
                // at the left index is greater than the pivot
                do { i++; } while (_moveScores[i] > pivot);
                
                // Move the right index to the left at least once and while the element
                // at the right index is less than the pivot
                do { j--; } while (_moveScores[j] < pivot);

                // If the indices have crossed return the new pivot point (point of crossing)
                if (i >= j) return j;
                
                // Swap elements at left and right indices
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
            // Get the value from the evaluator and return
            return pieceType switch
            {
                PieceType.Bishop => BetterEvaluator.BishopValue,
                PieceType.Pawn => BetterEvaluator.PawnValue,
                PieceType.Knight => BetterEvaluator.KnightValue,
                PieceType.Rook => BetterEvaluator.RookValue,
                PieceType.Queen => BetterEvaluator.QueenValue,

                // Value for a king is 0
                _ => 0
            };
        }
    }
}