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
        // Maximum number of moves in any given board state
        private const int MaxMoveCount = 218;
        private const int CapturedPieceValueMultiplier = 10;
        
        private readonly int[] _moveScores;
        
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
            InsertionSort(moves);

            // Return the sorted moves
            return moves;
        }
        
        /// <summary>
        ///     Sorts a list of moves.
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