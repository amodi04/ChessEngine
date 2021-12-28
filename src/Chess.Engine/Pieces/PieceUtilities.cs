using System;
using Engine.BoardRepresentation;
using Engine.Player;
using Engine.Util;

namespace Engine.Pieces;

/// <summary>
///     This utility class contains some useful methods for pieces.
/// </summary>
public static class PieceUtilities
{
    /// <summary>
    ///     Lookup values for increased efficiency when generating moves.
    /// </summary>
    public static readonly DoubleKeyDictionary<int, Coalition, Piece>
        PawnLookup = GeneratePiece(PieceType.Pawn);

    public static readonly DoubleKeyDictionary<int, Coalition, Piece>
        KnightLookup = GeneratePiece(PieceType.Knight);

    public static readonly DoubleKeyDictionary<int, Coalition, Piece>
        BishopLookup = GeneratePiece(PieceType.Bishop);

    public static readonly DoubleKeyDictionary<int, Coalition, Piece>
        RookLookup = GeneratePiece(PieceType.Rook);

    public static readonly DoubleKeyDictionary<int, Coalition, Piece>
        QueenLookup = GeneratePiece(PieceType.Queen);

    /// <summary>
    ///     Generates a double key dictionary of pieces at each possible location that they can be.
    /// </summary>
    /// <param name="pieceType">The piece to generate the positions for.</param>
    /// <returns>
    ///     A double key dictionary containing the position and coalition as keys.
    ///     The value is the piece object at that index.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">If an undefined piece type is passed in, the exception will occur.</exception>
    private static DoubleKeyDictionary<int, Coalition, Piece> GeneratePiece(PieceType pieceType)
    {
        Func<int, Coalition, Piece> makePiece = pieceType switch
        {
            PieceType.Pawn => (i, coalition) => new Pawn(i, coalition, false),
            PieceType.Knight => (i, coalition) => new Knight(i, coalition, false),
            PieceType.Bishop => (i, coalition) => new Bishop(i, coalition, false),
            PieceType.Rook => (i, coalition) => new Rook(i, coalition, false),
            PieceType.Queen => (i, coalition) => new Queen(i, coalition, false),
            _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null)
        };

        var pieces = new DoubleKeyDictionary<int, Coalition, Piece>();
        {
            // Loop through each coalition (white and black)
            foreach (Coalition coalition in Enum.GetValues(typeof(Coalition)))
                for (var i = 0; i < BoardUtilities.NumTiles; i++)
                    pieces.Add(i, coalition, makePiece(i, coalition));
        }

        return pieces;
    }
}