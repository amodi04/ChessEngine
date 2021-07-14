using System.Collections.Generic;
using System.Linq;

namespace Engine.BoardRepresentation
{
    /// <summary>
    /// A class containing useful values and methods that may be used across the entire class library.
    /// </summary>
    public static class BoardUtilities
    {
        /// <summary>
        /// Constants that are used to reduce the use of magic numbers/values.
        /// </summary>
        public const uint NumTiles = 64;
        public const uint NumTilesPerRank = 8;
        public const uint NumTilesPerFile = 8;
        
        /// <summary>
        /// Lookup array containing the tile coordinate values of the tiles in the respective rank.
        /// </summary>
        public static readonly int[] FirstRank = CreateRank(0);
        public static readonly int[] SecondRank = CreateRank(8);
        public static readonly int[] ThirdRank = CreateRank(16);
        public static readonly int[] FourthRank = CreateRank(24);
        public static readonly int[] FifthRank = CreateRank(32);
        public static readonly int[] SixthRank = CreateRank(40);
        public static readonly int[] SeventhRank = CreateRank(48);
        public static readonly int[] EighthRank = CreateRank(56);

        /// <summary>
        /// Lookup array containing the tile coordinate values of the tiles in the respective file.
        /// </summary>
        public static readonly int[] FirstFile = CreateFile(0);
        public static readonly int[] SecondFile = CreateFile(1);
        public static readonly int[] ThirdFile = CreateFile(2);
        public static readonly int[] FourthFile = CreateFile(3);
        public static readonly int[] FifthFile = CreateFile(4);
        public static readonly int[] SixthFile = CreateFile(5);
        public static readonly int[] SeventhFile = CreateFile(6);
        public static readonly int[] EighthFile = CreateFile(7);

        /// <summary>
        /// Populates the useful lookup arrays based on the starting tile number.
        /// </summary>
        /// <param name="startingTileNumber">The the left most tile coordinate in a rank.</param>
        /// <returns>An array containing the tile coordinates in the rank.</returns>
        private static int[] CreateRank(int startingTileNumber)
        {
            // nth term = n. startingTileNumber <= n <= startingTileNumber + 8.
            return Enumerable.Range(startingTileNumber, 8).ToArray();
        }

        /// <summary>
        /// Populates the useful lookup arrays based on the starting tile number.
        /// </summary>
        /// <param name="startingFileNumber">The the bottom most tile coordinate in a file.</param>
        /// <returns>An array containing the tile coordinates in the file.</returns>
        private static int[] CreateFile(int startingFileNumber)
        {
            // nth term = 8n + k, where k is the starting file number. 0 <= n <= 8.
            return Enumerable.Range(0, 8).Select(n => 8 * n + startingFileNumber).ToArray();
        }

        /// <summary>
        /// Checks whether a tile is in bounds.
        /// </summary>
        /// <param name="tileCoordinate">Tile coordinate to check.</param>
        /// <returns>True if the value is between 0 and 64 inclusive.</returns>
        public static bool IsValidTileCoordinate(int tileCoordinate)
        {
            return tileCoordinate is >= 0 and <= 64;
        }

        /// <summary>
        /// Checks whether a value is in a given array.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="array">IEnumerable array to check in.</param>
        /// <returns>True if value is in array.</returns>
        /// TODO: Check inefficiencies
        public static bool IsInArray(int value, IEnumerable<int> array)
        {
            return array.Any(i => value == i);
        }
    }
}