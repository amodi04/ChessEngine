using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveRepresentation
{
    public struct Move
    {
        public MoveType MoveType { get; }
        public Board Board { get; }
        public int ToCoordinate { get; }
        public int FromCoordinate { get; }
        public Piece MovedPiece { get; }
        public Piece CapturedPiece { get; }

        public Move(MoveType moveType, Board board, int toCoordinate, int fromCoordinate, Piece movedPiece, Piece capturedPiece)
        {
            MoveType = moveType;
            Board = board;
            ToCoordinate = toCoordinate;
            FromCoordinate = fromCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;
        }
    }
}