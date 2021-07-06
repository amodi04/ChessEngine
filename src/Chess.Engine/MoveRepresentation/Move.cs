using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveRepresentation
{
    public struct Move
    {
        public MoveType MoveType { get; }
        public Board Board { get; }
        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }
        public Piece CapturedPiece { get; }

        public Move(MoveType moveType, Board board, int fromCoordinate, int toCoordinate, Piece movedPiece)
        {
            MoveType = moveType;
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = null;
        }

        public Move(MoveType moveType, Board board, int fromCoordinate, int toCoordinate, Piece movedPiece,
            Piece capturedPiece)
        {
            MoveType = moveType;
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;
        }
    }
}