using System.Linq;
using Engine.Extensions;

namespace Engine.Types.AI
{
    /// <summary>
    /// Structure for holding default evaluation data.
    /// </summary>
    public struct DefaultEvaluator : IEvaluator
    {
        // Member fields
        private const int CheckBonus = 50;
        
        /// <summary>
        /// Gets the integer value of the move.
        /// </summary>
        /// <param name="board">The board for move context.</param>
        /// <returns>A score value for the board.</returns>
        public int Evaluate(Board board)
        {
            // Return the white player score minus the black player score
            // A positive score is more beneficial for white
            // A negative score is more beneficial for black
            return Score(board.WhitePlayer) - Score(board.BlackPlayer);
        }

        /// <summary>
        /// Scores a player.
        /// </summary>
        /// <param name="player">The player to score.</param>
        /// <returns>An integer score value for the player.</returns>
        private int Score(Player player)
        {
            // Return the sum of all evaluations
            return EvaluateMaterial(player)
                   + EvaluateMobility(player)
                   + EvaluateCheck(player)
                   + EvaluateCheckmate(player);
        }

        /// <summary>
        /// Evaluates checkmate.
        /// </summary>
        /// <param name="player">The player to evaluate.</param>
        /// <returns>If the opponent is in checkmate, the value is large because this will win the game. Otherwise return 0.</returns>
        private int EvaluateCheckmate(Player player)
        {
            return player.GetOpponent().IsInCheckmate() ? 10000 : 0;
        }

        /// <summary>
        /// Evaluates check
        /// </summary>
        /// <param name="player">The player to evaluate.</param>
        /// <returns>Gives a bonus for putting the opposite player in check.</returns>
        private int EvaluateCheck(Player player)
        {
            return player.GetOpponent().IsInCheck() ? CheckBonus : 0;
        }

        /// <summary>
        /// Evaluates the mobility of the player.
        /// </summary>
        /// <param name="player">The player to evaluate.</param>
        /// <returns>The more moves that can be made, the higher the value.</returns>
        private int EvaluateMobility(Player player)
        {
            return player.Moves.Count * 50;
        }
        
        /// <summary>
        /// Evaluates the piece values of the player.
        /// </summary>
        /// <param name="player">The player to evaluate.</param>
        /// <returns>The sum of piece values.s</returns>
        private int EvaluateMaterial(Player player)
        {
            int material = 0;
            foreach (var piece in player.GetActiveAlliedPieces())
            {
                // Add the piece value to the sum
                material += piece.PieceType.ToValue();
            }

            // Return sum
            return material;
        }
    }
}