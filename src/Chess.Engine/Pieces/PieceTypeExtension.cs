using System;

namespace Engine.Pieces
{
    /// <summary>
    /// This extension class contains methods for dealing with piece types.
    /// </summary>
    public static class PieceTypeExtension
    {
        /// <summary>
        /// Gets the string representation of a piece type.
        /// </summary>
        /// <param name="pieceType">The piece type to get the string representation for.</param>
        /// <returns>A letter which is usually the first letter of the piece. Knights return "N" (only difference).</returns>
        public static string ToAbbreviation(this PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.Pawn => "P",
                PieceType.Knight => "N",
                PieceType.Bishop => "B",
                PieceType.Rook => "R",
                PieceType.Queen => "Q",
                PieceType.King => "K",
                // The default case should not occur.
                _ => "-"
            };
        }
    }
}