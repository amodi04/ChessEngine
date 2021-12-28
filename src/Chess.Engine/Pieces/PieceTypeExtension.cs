using Engine.AI;
using Engine.Player;

namespace Engine.Pieces;

/// <summary>
///     This extension class contains methods for dealing with piece types.
/// </summary>
public static class PieceTypeExtension
{
    /// <summary>
    ///     Gets the string representation of a piece type.
    /// </summary>
    /// <param name="pieceType">The piece type to get the string representation for.</param>
    /// <param name="coalition">The colour of the piece will change the case output of the string.</param>
    /// <returns>A letter which is usually the first letter of the piece. Knights return "N" (only difference).</returns>
    public static string ToAbbreviation(this PieceType pieceType, Coalition coalition)
    {
        return pieceType switch
        {
            PieceType.Pawn => coalition.IsWhite() ? "P" : "p",
            PieceType.Bishop => coalition.IsWhite() ? "B" : "b",
            PieceType.Knight => coalition.IsWhite() ? "N" : "n",
            PieceType.Rook => coalition.IsWhite() ? "R" : "r",
            PieceType.Queen => coalition.IsWhite() ? "Q" : "q",
            PieceType.King => coalition.IsWhite() ? "K" : "k",
            _ => "-"
        };
    }

    /// <summary>
    ///     Gets the value of the piece.
    /// </summary>
    /// <param name="pieceType">The piece type to get the value for.</param>
    /// <returns>An integer value of the piece type.</returns>
    public static int ToValue(this PieceType pieceType)
    {
        return pieceType switch
        {
            PieceType.Pawn => AISettings.PawnValue,
            PieceType.Knight => AISettings.KnightValue,
            PieceType.Bishop => AISettings.BishopValue,
            PieceType.Rook => AISettings.RookValue,
            PieceType.Queen => AISettings.QueenValue,
            PieceType.King => 0,
            _ => 0
        };
    }
}