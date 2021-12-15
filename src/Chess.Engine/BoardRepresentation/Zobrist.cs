using System;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.BoardRepresentation
{
    /// <summary>
    /// Utility class for hashing boards using the Zobrist Hash function.
    /// </summary>
    public static class Zobrist
    {
        // Constants
        private const int Seed = 34;
        private static Random _prng = new(Seed);
        
        private static ulong[,] _table = InitialiseZobrist();
        
        /// <summary>
        /// Generates random 64-bit numbers for each possible element of the board.
        /// </summary>
        /// <returns>A 64x12 table of bitstrings.</returns>
        private static ulong[,] InitialiseZobrist()
        {
            // Create 64x12 matrix (64 tiles x 12 pieces)
            ulong[,] table = new ulong[NumTiles, 12];
            
            for (int i = 0; i < NumTiles; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    // Store a random 64 bit number at each element in the matrix
                    table[i, j] = GenerateRandom64BitNumber();
                }
            }

            return table;
        }

        /// <summary>
        /// Zobrist Hash Function.
        /// </summary>
        /// <param name="board">The board to hash.</param>
        /// <returns>A 64-bit hash of the board.</returns>
        public static ulong Hash(Board board)
        {
            // Initialise hash to 0
            ulong hash = 0UL;
            for (int i = 0; i < NumTiles; i++)
            {
                if (board.GetTile(i).IsOccupied())
                {
                    Piece piece = board.GetTile(i).Piece;
                    PieceType type = piece.Type;
                    Coalition coalition = piece.PieceCoalition;
                    
                    // Get the y index of the matrix
                    int j = (int) type * (int) coalition;
                    
                    // XOR bitstrings with hash for each piece on the board
                    hash ^= _table[i, j];
                }
            }

            return hash;
        }

        /// <summary>
        /// Generates a random 64-bit bitstring.
        /// </summary>
        /// <returns>A 64-bit number.</returns>
        private static ulong GenerateRandom64BitNumber()
        {
            // Create a length 8 byte array for storing the bits of the bit string
            byte[] buffer = new byte[8];
            
            // Generate random bytes and fill buffer with them
            _prng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}