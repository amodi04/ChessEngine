using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveRepresentation
{
    /// <summary>
    /// This struct stores Move Data.
    /// </summary>
    public readonly struct Move
    {
        /// <summary>
        /// Move Data
        /// </summary>
        public MoveType MoveType { get; }
        public Board Board { get; }
        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }
        public Piece CapturedPiece { get; }

        /// <summary>
        /// Constructor creates a non capture move.
        /// </summary>
        /// <param name="moveType">This will be confined to non-capturing states.</param>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        public Move(MoveType moveType, Board board, int fromCoordinate, int toCoordinate, Piece movedPiece)
        {
            MoveType = moveType;
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = null;
        }

        /// <summary>
        /// Constructor creates a capture move.
        /// </summary>
        /// <param name="moveType">This will be confined to capturing states.</param>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="capturedPiece">The piece at the destination coordinate being captured.</param>
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

        // TODO: Implement this
        public Board ExecuteMove()
        {
            return null;
        }
    }
}