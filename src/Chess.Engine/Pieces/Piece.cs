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
        public bool IsFirstMove { get; }

        public Piece(int piecePosition, Coalition pieceCoalition)
        {
            PiecePosition = piecePosition;
            PieceCoalition = pieceCoalition;
        }
        
        protected bool IsEnemyPieceAtTile(Tile tile)
        {
            return PieceCoalition == tile.Piece.PieceCoalition;
        }

        protected Move CreateNormalMove(Board board, int toCoordinate)
        {
            return new Move(MoveType.NormalMove, board, PiecePosition,
                toCoordinate, this);
        }
        
        protected Move CreateAttackMove(Board board, int toCoordinate, Piece capturedPiece)
        {
            return new Move(MoveType.CaptureMove, board, PiecePosition, toCoordinate, this, capturedPiece);
        }

        public abstract List<Move> GenerateLegalMoves(Board board);
        public abstract Piece MovePiece(Move move);
        protected abstract bool IsColumnExclusion(int currentPosition, int offset);
    }
}