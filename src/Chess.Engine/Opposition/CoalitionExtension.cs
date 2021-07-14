namespace Engine.Opposition
{
    /// <summary>
    /// This extension class contains methods for dealing with coalition state.
    /// </summary>
    public static class CoalitionExtension
    {
        /// <summary>
        /// Gets the direction that a coalition moves in.
        /// </summary>
        /// <param name="coalition">The colour that the direction is going to be returned for.</param>
        /// <returns>Positive 1 if white and negative 1 if black.</returns>
        public static int GetDirection(this Coalition coalition)
        {
            return coalition == Coalition.White ? 1 : -1;
        }

        /// <summary>
        /// Checks whether the argument is white.
        /// </summary>
        /// <param name="coalition">The colour to check.</param>
        /// <returns>True if white. False if black.</returns>
        public static bool IsWhite(this Coalition coalition)
        {
            return coalition == Coalition.White;
        }
    }
}