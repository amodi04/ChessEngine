using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Pieces;
using Engine.Player;
using Engine.Util;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.BoardRepresentation;

/// <summary>
///     Utility class for hashing boards using the Zobrist Hash function.
/// </summary>
public static class Zobrist
{
    private const int Seed = 34;
    private static readonly Random _prng = new(Seed);

    private static DoubleKeyDictionary<PieceType, Coalition, int> _pieceTable = GeneratePieceTable();
    private static readonly ulong[,] Table = InitialiseZobrist();

    /// <summary>
    ///     Generates random 64-bit numbers for each possible element of the board.
    /// </summary>
    /// <returns>A 64x12 table of bitstrings.</returns>
    private static ulong[,] InitialiseZobrist()
    {
        var table = new ulong[NumTiles, 12];

        for (var i = 0; i < NumTiles; i++)
        for (var j = 0; j < 12; j++)
            table[i, j] = GenerateRandom64BitNumber();

        return table;
    }

    /// <summary>
    /// Generates a double key dictionary of pieces. Allows the obtaining of a number from 1 to 12 based on
    /// piece type and coalition
    /// </summary>
    /// <returns>A table of integers accessed via PieceType and Coalition.</returns>
    private static DoubleKeyDictionary<PieceType, Coalition, int> GeneratePieceTable()
    {
        var pieces = new DoubleKeyDictionary<PieceType, Coalition, int>();
        int index = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                PieceType pieceType = (PieceType)i;
                Coalition coalition = (Coalition)j;
                pieces[pieceType, coalition] = index;
                index++;
            }
        }

        return pieces;
    }

    /// <summary>
    ///     Zobrist Hash Function.
    /// </summary>
    /// <param name="board">The board to hash.</param>
    /// <returns>A 64-bit hash of the board.</returns>
    public static ulong Hash(Board board)
    {
        var hash = 0UL;
        for (var i = 0; i < NumTiles; i++)
        {
            if (!board.GetTile(i).IsOccupied()) continue;
            var piece = board.GetTile(i).Piece;
            if (piece is null) continue;
            var type = piece.Type;
            var coalition = piece.PieceCoalition;
            var j =_pieceTable[type,coalition];

            hash ^= Table[i, j];
        }

        return hash;
    }

    /// <summary>
    ///     Generates a random 64-bit bitstring.
    /// </summary>
    /// <returns>A 64-bit number.</returns>
    private static ulong GenerateRandom64BitNumber()
    {
        var buffer = new byte[8];
        _prng.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }
}