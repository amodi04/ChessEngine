using System.Collections.Generic;
using Engine.Board;

namespace Engine.Pieces
{
    public abstract class Piece
    {
        public int PiecePosition { get; }

        public Coalition PieceCoalition { get; }

        public Piece(int piecePosition, Coalition pieceCoalition)
        {
            PiecePosition = piecePosition;
            PieceCoalition = pieceCoalition;
        }
        
        public abstract List<Move> GenerateLegalMoves(Board.Board board);
        public abstract Piece MovePiece(Move move);
    }
}