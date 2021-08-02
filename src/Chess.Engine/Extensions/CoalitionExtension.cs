using Engine.Opposition;
using Engine.PlayerRepresentation;

namespace Engine.Extensions
{
    /// <summary>
    ///     This extension class contains methods for dealing with coalition state.
    /// </summary>
    public static class CoalitionExtension
    {
        /// <summary>
        ///     Gets the direction that a coalition moves in.
        /// </summary>
        /// <param name="coalition">The colour that the direction is going to be returned for.</param>
        /// <returns>Positive 1 if white and negative 1 if black.</returns>
        public static int GetDirection(this Coalition coalition)
        {
            // white, +1 because traversing towards the 64th tile
            // black, -1 because traversing towards the 0th tile
            return coalition.IsWhite() ? 1 : -1;
        }

        /// <summary>
        ///     Checks whether the argument is white.
        /// </summary>
        /// <param name="coalition">The colour to check.</param>
        /// <returns>True if white. False if black.</returns>
        public static bool IsWhite(this Coalition coalition)
        {
            return coalition == Coalition.White;
        }

        /// <summary>
        ///     Utility method for choosing the player based on a coalition.
        /// </summary>
        /// <param name="coalition">The coalition used to select the player.</param>
        /// <param name="whitePlayer">The white player on the current board in play.</param>
        /// <param name="blackPlayer">The black player on the current board in play.</param>
        /// <returns>A white player if the coalition is white, otherwise the black player is returned.</returns>
        public static Player ChoosePlayer(this Coalition coalition, Player whitePlayer, Player blackPlayer)
        {
            return coalition.IsWhite() ? whitePlayer : blackPlayer;
        }
    }
}