using System;
using Engine.Enums;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.Util
{
    public static class PieceUtilities
    {
        public static DoubleKeyDictionary<int, Coalition, Piece> PawnLookup = GeneratePiece(PieceType.Pawn);
        public static DoubleKeyDictionary<int, Coalition, Piece> KnightLookup = GeneratePiece(PieceType.Knight);
        public static DoubleKeyDictionary<int, Coalition, Piece> BishopLookup = GeneratePiece(PieceType.Bishop);
        public static DoubleKeyDictionary<int, Coalition, Piece> RookLookup = GeneratePiece(PieceType.Rook);
        public static DoubleKeyDictionary<int, Coalition, Piece> QueenLookup = GeneratePiece(PieceType.Queen);

        private static DoubleKeyDictionary<int, Coalition, Piece> GeneratePiece(PieceType pieceType)
        {
            Func<int, Coalition, Piece> makePiece = pieceType switch
            {
                PieceType.Pawn => (i, coalition) => new Pawn(i, coalition),
                PieceType.Knight => (i, coalition) => new Knight(i, coalition),
                PieceType.Bishop => (i, coalition) => new Bishop(i, coalition),
                PieceType.Rook => (i, coalition) => new Rook(i, coalition),
                PieceType.Queen => (i, coalition) => new Queen(i, coalition),
                _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null)
            };
            var pieces = new DoubleKeyDictionary<int, Coalition, Piece>();
            {
                foreach (Coalition coalition in Enum.GetValues(typeof(Coalition)))
                    for (var i = 0; i < BoardUtilities.NumTiles; i++)
                        pieces.Add(i, coalition, makePiece(i, coalition));
            }
            return pieces;
        }
    }
}