using System.Collections;
using System.Collections.Generic;
using Engine.Enums;
using Engine.Types.MoveGeneration;

namespace Engine.Types.AI
{
    /// <summary>
    /// This class handles the ordering move lists of moves.
    /// </summary>
    public class MoveOrdering
    {
        // Member fields
        private readonly int[] _moveScores;
        private const int MaxMoveCount = 218;
        private const int CapturedPieceValueMultiplier = 10;

        /// <summary>
        /// Constructor
        /// </summary>
        public MoveOrdering()
        {
            // Create a new array of move scores
            _moveScores = new int[MaxMoveCount];
        }

        /// <summary>
        /// Order's a given list of moves.
        /// </summary>
        /// <param name="board">The board for context.</param>
        /// <param name="moves">The list of moves.</param>
        /// <returns>An IEnumerable collection of moves.</returns>
        public IEnumerable<IMove> OrderMoves(Board board, List<IMove> moves)
        {
            // Loop through the list of moves
            for (var i = 0; i < moves.Count; i++)
            {
                // Get the move at the current index
                var move = moves[i];
                
                // Initialise the score to 0
                int score = 0;
                
                // Get the piece type of the moved piece
                PieceType movedPieceType = board.GetTile(move.FromCoordinate).Piece.PieceType;
                
                // Get the piece type of the captured piece (if there isn't one, set as null)
                PieceType? capturedPieceType =
                    move is CaptureMove captureMove ? captureMove.CapturedPiece.PieceType : null;

                // If the captured piece type is not null
                if (capturedPieceType is not null)
                {
                    // Set the score to the captured piece multiplier multiplied by the captured piece. Take away the moved piece.
                    // This score is an estimate for potentially good moves. This allows the alpha beta search to look at these moves
                    // therefore decreasing evaluation because it doesn't need to waste time looking at worse moves later.
                    score = CapturedPieceValueMultiplier * GetPieceValue(capturedPieceType) -
                            GetPieceValue(movedPieceType);
                }

                // If the piece moving is a pawn
                if (movedPieceType == PieceType.Pawn)
                {
                    // If the move is a promotion move
                    if (move is PromotionMove promotionMove)
                    {
                        // Add the value of the piece to promote to because this is classed as a good move
                        score += GetPieceValue(promotionMove.PromotedPiece.PieceType);
                    }
                }

                // Set the score at the index to the calculated score
                _moveScores[i] = score;
            }

            // Sort the moves
            Sort(moves);

            // Return the sorted moves
            return moves;
        }

        /// <summary>
        /// Sorts a list of moves.
        /// </summary>
        /// <param name="moves">The moves to sort.</param>
        private void Sort(IList<IMove> moves)
        {
            // Loop through the length of the list times
            for (int i = 0; i < moves.Count; i++)
            {
                // Loop through until 0 downwards from i + 1
                for (int j = i + 1; j > 0; j--)
                {
                    // Set the swap index to j - 1 (one less)
                    int swapIndex = j - 1;
                    
                    // If the value at the swap index is less than the value at the j index
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
        /// Gets the value for a given piece type.
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