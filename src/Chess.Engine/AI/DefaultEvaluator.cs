using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using static Engine.AI.AISettings;

namespace Engine.AI
{
    /// <summary>
    /// Class for holding default evaluation data.
    /// </summary>
    public class DefaultEvaluator : IEvaluator
    {
        /// <summary>
        /// Gets the integer value of the move.
        /// </summary>
        /// <param name="board">The board for move context.</param>
        /// <param name="depth">The current depth of the search.</param>
        /// <returns>A score value for the board.</returns>
        public int Evaluate(Board board, int depth)
        {
            // Return the white player score minus the black player score
            // A positive score is more beneficial for white
            // A negative score is more beneficial for black
            return Score(board.WhitePlayer, depth) - Score(board.BlackPlayer, depth);
        }

        /// <summary>
        /// Scores a player.
        /// </summary>
        /// <param name="player">The player to score.</param>
        /// <param name="depth">The current depth of the search.</param>
        /// <returns>An integer score value for the player.</returns>
        private int Score(Player.Player player, int depth)
        {
            // Return the sum of all evaluations
            return EvaluateMobility(player)
                   + EvaluateKingThreats(player, depth)
                   + EvaluateAttacks(player)
                   + EvaluateCastled(player)
                   + EvaluatePieces(player);
        }

        /// <summary>
        /// Evaluates the piece values of the player.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <returns>An integer evaluation score.</returns>
        private int EvaluatePieces(Player.Player player)
        {
            // Initialise values
            int pieceEvaluationScore = 0;
            int numBishops = 0;
            int numRooks = 0;
            
            // Loop through each active piece for the player
            foreach (var piece in player.GetActiveAlliedPieces())
            {
                // Increase the score by the piece value
                pieceEvaluationScore += piece.PieceType.ToValue();
                
                // If the piece is a bishop, increase the number of bishops score
                if (piece.PieceType == PieceType.Bishop)
                {
                    numBishops++;
                } 
                else if (piece.PieceType == PieceType.Rook)
                {
                    numRooks++;
                }
            }

            // Return the piece evaluation score
            // If the number of bishops is two then add the two bishops bonus.
            // This is because two bishops are better than 1 when playing
            pieceEvaluationScore += numBishops == 2 ? TwoBishopsBonus : 0;
            pieceEvaluationScore += numRooks == 2 ? TwoRooksBonus : 0;
            return pieceEvaluationScore * PieceMultiplier;
        }

        /// <summary>
        /// Evaluates whether the player is castled.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <returns>An integer evaluation score.</returns>
        private int EvaluateCastled(Player.Player player)
        {
            // Return the castle bonus if the player is castled
            return player.King.IsCastled ? CastleBonus : 0;
        }

        /// <summary>
        /// Evaluates the sum of possible attacks.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <returns>An integer evaluation score.</returns>
        private int EvaluateAttacks(Player.Player player)
        {
            // Initialise the attack score to 0.
            int attackScore = 0;
            
            // Loop through each move the player can make
            foreach (var move in player.Moves)
            {
                // If the move is an attak score
                if (move is CaptureMove captureMove)
                {
                    // If the moved piece value is less than the captured piece value
                    if (captureMove.MovedPiece.PieceType.ToValue() <= captureMove.CapturedPiece.PieceType.ToValue())
                    {
                        // Increase the attack score by 1
                        attackScore++;
                    }
                }
            }

            // Return the attack score multiplied by the attack multiplier
            return attackScore * AttackMultiplier;
        }

        /// <summary>
        /// Evaluates the threats on the opponent king.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <param name="depth">The current search depth.</param>
        /// <returns>An integer evaluation score.</returns>
        private int EvaluateKingThreats(Player.Player player, int depth)
        {
            // If the opponent is in checkmate (when searching for a move in the game tree),
            // Return the checkmate bonus (large value because checkmate is the best possible move since it wins the game
            // This is multiplied by the depth bonus which incentives' the AI to go for the checkmate move the further away it is
            // Else evaluate a check move on the opponent
            return player.GetOpponent().IsInCheckmate()
                ? CheckmateBonus * ComputeDepthBonus(depth)
                : ComputeCheck(player);
        }

        /// <summary>
        /// Evaluates check on the opponent king.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <returns>An integer evaluation score.</returns>
        private int ComputeCheck(Player.Player player)
        {
            // Return the check bonus if the player is in check otherwise return 0.
            return player.GetOpponent().IsInCheck() ? CheckBonus : 0;
        }

        /// <summary>
        /// Computes the bonus to apply depending on the depth.
        /// </summary>
        /// <param name="depth">The current depth of the search.</param>
        /// <returns>An integer evaluation score.</returns>
        private int ComputeDepthBonus(int depth)
        {
            // If the depth is 0, return 1
            // Otherwise return 100 multiplied by the depth
            // Higher depth values achieve a higher score
            return depth == 0 ? 1 : DepthMultiplier * depth;
        }
        
        /// <summary>
        /// Evaluate the moves that can be made by the player.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <returns>An integer evaluation score.</returns>
        private int EvaluateMobility(Player.Player player)
        {
            // Return the mobility multiplier by the calculated ratio of player mobilities
            return MobilityMultiplier * ComputeMobilityRatio(player);
        }

        /// <summary>
        /// Computes the mobility ratio.
        /// </summary>
        /// <param name="player">The player to evaluate for.</param>
        /// <returns>An integer evaluation score.</returns>
        private int ComputeMobilityRatio(Player.Player player)
        {
            // Return the number of moves that the player can make multiplied by ten divided by the number of moves the opponent can make
            // This gives the ratio of moves that the player can make compared to the number of moves the opponent can make.
            // This maximises the AIs moves compared to the player (a good chess strategy)
            // Cast to an int and return
            return (int) (player.Moves.Count * 10.0f / player.GetOpponent().Moves.Count);
        }
    }
}