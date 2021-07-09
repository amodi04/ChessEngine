using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;

namespace Engine.Pieces
{
    public abstract class Piece
    {
        public Piece(int piecePosition, Coalition pieceCoalition)
        {
            PiecePosition = piecePosition;
            PieceCoalition = pieceCoalition;
        }

        public int PiecePosition { get; }
        public Coalition PieceCoalition { get; }
        public bool IsFirstMove { get; }

        protected bool IsEnemyPieceAtTile(Tile tile)
        {
            return PieceCoalition == tile.Piece.PieceCoalition;
        }

        protected Move CreateNormalMove(Board board, int toCoordinate)
        {
            return new(MoveType.NormalMove, board, PiecePosition,
                toCoordinate, this);
        }

        protected Move CreateAttackMove(Board board, int toCoordinate, Piece capturedPiece)
        {
            return new(MoveType.CaptureMove, board, PiecePosition, toCoordinate, this, capturedPiece);
        }

        public abstract IList GenerateLegalMoves(Board board);
        public abstract Piece MovePiece(Move move);
        protected abstract bool IsColumnExclusion(int currentPosition, int offset);
    }
}