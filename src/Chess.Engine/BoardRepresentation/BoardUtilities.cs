using System;
using Engine.Player;

namespace Engine.BoardRepresentation;

/// <summary>
///     A class containing useful values and methods that may be used across the entire class library.
/// </summary>
public static class BoardUtilities
{
    public const int NumTiles = 64;
    public const int NumTilesPerRank = 8;
    public const int NumTilesPerFile = 8;
    public const string FileNames = "abcdefgh";
    public const string RankNames = "12345678";

    // Stores a reflected mapping of tile coordinates so that the board starts at the bottom left
    // rather than the bottom right
    public static readonly int[] ReflectBoard = GenerateReflectedPositions();

    public static PlayerType WhitePlayerType = PlayerType.Human;
    public static PlayerType BlackPlayerType = PlayerType.Computer;

    /// <summary>
    ///     Generates the reflected mapping of tile positions.
    /// </summary>
    /// <returns>An array containing the index to value mapping.</returns>
    private static int[] GenerateReflectedPositions()
    {
        var mapping = new int[64];
        for (var i = 0; i < NumTiles; i++)
            mapping[i] = (int)(16 * Math.Floor(i / 8d) + 7 - i);

        return mapping;
    }

    /// <summary>
    ///     Checks whether a tile is in bounds.
    /// </summary>
    /// <param name="tileCoordinate">Tile coordinate to check.</param>
    /// <returns>True if the value is between 0 and 64 inclusive.</returns>
    public static bool IsValidTileCoordinate(int tileCoordinate)
    {
        return tileCoordinate is >= 0 and <= 63;
    }

    /// <summary>
    ///     Gets the rank number based on the tile coordinate.
    /// </summary>
    /// <param name="tileCoordinate">The tile coordinate to get the rank from.</param>
    /// <returns>An integer rank position.</returns>
    public static int RankIndex(int tileCoordinate)
    {
        return tileCoordinate >> 3;
    }

    /// <summary>
    ///     Gets the file number based on the tile coordinate.
    /// </summary>
    /// <param name="tileCoordinate">The tile coordinate to get the file from.</param>
    /// <returns>An integer file position.</returns>
    public static int FileIndex(int tileCoordinate)
    {
        return tileCoordinate & 0b000111;
    }
}