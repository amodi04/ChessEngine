using Engine.BoardRepresentation;
using Engine.Enums;
using Engine.Pieces;

namespace Engine.MoveGeneration
{
    /// <summary>
    ///     This struct stores Move Data.
    /// </summary>
    public readonly struct Move
    {
        /// <summary>
        ///     Move Data
        /// </summary>
        public readonly MoveType MoveType;

        public readonly Board Board;
        public readonly int FromCoordinate;
        public readonly int ToCoordinate;
        public readonly Piece MovedPiece;
        public readonly Piece CapturedPiece;

        /// <summary>
        ///     Constructor creates a non capture move.
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
        ///     Constructor creates a capture move.
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

        /// <summary>
        ///     Creates a new board with the moved piece.
        /// </summary>
        /// <returns>A new board with the piece moved.</returns>
        public Board ExecuteMove()
        {
            var boardBuilder = new BoardBuilder();

            // Set all pieces except the moved piece for the current player
            foreach (var piece in Board.CurrentPlayer.GetActiveAlliedPieces())
                if (!MovedPiece.Equals(piece))
                    boardBuilder.SetPieceAtTile(piece);

            // Set all the pieces for the opponent player
            foreach (var piece in Board.CurrentPlayer.GetOpponent().GetActiveAlliedPieces())
                boardBuilder.SetPieceAtTile(piece);

            // Move the moved piece
            boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this));

            // Set next player to move
            boardBuilder.SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition);

            // Build the board
            return boardBuilder.BuildBoard();
        }
    }
}