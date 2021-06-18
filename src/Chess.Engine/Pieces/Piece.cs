using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;

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
        
        protected bool IsEnemyPieceAtTile(Tile tile)
        {
            return PieceCoalition == tile.Piece.PieceCoalition;
        }
        
        public abstract List<Move> GenerateLegalMoves(Board board);
        protected abstract bool IsColumnExclusion(int currentPosition, int offset);
        public abstract Piece MovePiece(Move move);
    }
}