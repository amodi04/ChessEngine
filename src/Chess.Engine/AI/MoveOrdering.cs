using System;
using System.Collections;
using System.Collections.Generic;
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
        // Constants
        // Maximum number of moves in any given board state
        private const int MaxMoveCount = 218;
        private const int CapturedPieceValueMultiplier = 10;
        private const int MoveLengthThreshold = 10;

        private int[] _moveScores;
        private bool _useQuickSort;

        public MoveOrdering()
        {
            _moveScores = new int[MaxMoveCount];
        }

        /// <summary>
        ///     Order's a given list of moves.
        /// </summary>
        /// <param name="board">The board for context.</param>
        /// <param name="moves">The list of moves.</param>
        /// <returns>An IEnumerable collection of moves.</returns>
        public IEnumerable<IMove> OrderMoves(Board board, List<IMove> moves)
        {
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
        /// Auxiliary QuickSort function for readability
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
            // Set the pivot to the right of the sub array.
            int pivot = _moveScores[high];
            
            // Temporary pivot index
            int i = low - 1;

            for (int j = low; j <= high - 1; j++)
            {
                // If current element is larger than the pivot element
                if (_moveScores[j] > pivot)
                {
                    // Increment index of smaller element
                    i++;
                    
                    // Swap current value with value at temporary pivot index
                    (_moveScores[i], _moveScores[j]) = (_moveScores[j], _moveScores[i]);
                    (moves[i], moves[j]) = (moves[j], moves[i]);
                }
            }

            // Move the pivot element to the correct pivot position (between smaller and larger elements)
            (_moveScores[i + 1], _moveScores[high]) = (_moveScores[high], _moveScores[i + 1]);
            (moves[i + 1], moves[high]) = (moves[high], moves[i + 1]);
            
            // The pivot index
            return i + 1;
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