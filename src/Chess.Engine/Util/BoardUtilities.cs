using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Engine.Enums;

namespace Engine.Util
{
    /// <summary>
    ///     A class containing useful values and methods that may be used across the entire class library.
    /// </summary>
    public static class BoardUtilities
    {
        /// <summary>
        ///     Constants that are used to reduce the use of magic numbers/values.
        /// </summary>
        public const int NumTiles = 64;
        public const int NumTilesPerRank = 8;
        public const int NumTilesPerFile = 8;
        public const string FileNames = "abcdefgh";
        public const string RankNames = "12345678";

        // Stores a reflected mapping of tile coordinates so that the board starts at the bottom left
        // Rather than the bottom right
        public static readonly int[] ReflectBoard = GenerateReflectedPositions();

        // Stores the respective player types for the players that can be accessed throughout the game
        public static PlayerType WhitePlayerType = PlayerType.Human;
        public static PlayerType BlackPlayerType = PlayerType.Computer;

        /// <summary>
        /// Generates the reflected mapping of tile positions.
        /// </summary>
        /// <returns>A dictionary containing the key to value mapping.</returns>
        private static int[] GenerateReflectedPositions()
        {
            int[] mapping = new int[64];
            
            // Loop over 64 times
            for (var i = 0; i < NumTiles; i++)
            {
                // Store at each key position the calculated value
                // This calculated value produces a reflected mapping of coordinate
                mapping[i] = (int) (16 * Math.Floor((double) (i / 8))+ 7 - i);
            }
            
            // Return the mapping
            return mapping;
        }
        
        /// <summary>
        ///     Populates the useful lookup arrays based on the starting tile number.
        /// </summary>
        /// <param name="startingTileNumber">The the left most tile coordinate in a rank.</param>
        /// <returns>An array containing the tile coordinates in the rank.</returns>
        private static int[] CreateRank(int startingTileNumber)
        {
            // nth term = n. startingTileNumber <= n <= startingTileNumber + 8.
            return Enumerable.Range(startingTileNumber, 8).ToArray();
        }

        /// <summary>
        ///     Populates the useful lookup arrays based on the starting tile number.
        /// </summary>
        /// <param name="startingFileNumber">The the bottom most tile coordinate in a file.</param>
        /// <returns>An array containing the tile coordinates in the file.</returns>
        private static int[] CreateFile(int startingFileNumber)
        {
            // nth term = 8n + k, where k is the starting file number. 0 <= n <= 8.
            return Enumerable.Range(0, 8).Select(n => 8 * n + startingFileNumber).ToArray();
        }

        /// <summary>
        ///     Checks whether a tile is in bounds.
        /// </summary>
        /// <param name="tileCoordinate">Tile coordinate to check.</param>
        /// <returns>True if the value is between 0 and 64 inclusive.</returns>
        public static bool IsValidTileCoordinate(int tileCoordinate)
        {
            // Tiles should be within the board so 0 to 63 inclusive.
            return tileCoordinate is >= 0 and <= 63;
        }

        /// <summary>
        /// Gets the rank number based on the tile coordinate.
        /// </summary>
        /// <param name="tileCoordinate">The tile coordinate to get the rank from.</param>
        /// <returns>An integer rank position.</returns>
        public static int RankIndex(int tileCoordinate)
        {
            // Return the tile coordinate value bit shifted right by 3
            return tileCoordinate >> 3;
        }

        /// <summary>
        /// Gets the file number based on the tile coordinate.
        /// </summary>
        /// <param name="tileCoordinate">The tile coordinate to get the file from.</param>
        /// <returns>An integer file position.</returns>
        public static int FileIndex(int tileCoordinate)
        {
            // Return the tile coordinate value ANDed with the binary value of 7
            return tileCoordinate & 0b000111;
        }

        /// <summary>
        /// Generates a string representation of the algebraic positions on the board.
        /// </summary>
        /// <returns>An IEnumerable of 2 character strings representing the file and rank position.</returns>
        public static IEnumerable<string> GenerateAlgebraicNotation()
        {
            var positions = new string[64];
            int[] numbers = {1, 2, 3, 4, 5, 6, 7, 8};
            string[] letters = {"a", "b", "c", "d", "e", "f", "g", "h"};
            int count = 0;
            
            // Loop over each number
            foreach (var number in numbers)
            {
                // Loop over each letter
                foreach (var letter in letters)
                {
                    // Store the letter and number at the index
                    positions[count] = $"{letter}{number}";
                    
                    // Increase index
                    count++;
                }
            }

            // Return the array of positions
            return positions;
        }
    }
}