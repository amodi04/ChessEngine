using System;
using Engine.Enums;

namespace Engine.Extensions
{
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
                // Default case will never be processed
                _ => "-"
            };
        }

        /// <summary>
        /// Gets the value of the piece.
        /// </summary>
        /// <param name="pieceType">The piece type to get the value for.</param>
        /// <returns>An integer value of the piece type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown piece type is used</exception>
        public static int ToValue(this PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.Pawn => 100,
                PieceType.Knight => 300,
                PieceType.Bishop => 320,
                PieceType.Rook => 500,
                PieceType.Queen => 900,
                PieceType.King => 100000,
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null)
            };
        }
    }
}