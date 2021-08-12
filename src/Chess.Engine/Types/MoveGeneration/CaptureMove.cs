using System;
using Engine.Builders;
using Engine.Types.Pieces;

namespace Engine.Types.MoveGeneration
{
    /// <inheritdoc cref="IMove" />
    /// <summary>
    ///     This struct stores capture move data
    /// </summary>
    public readonly struct CaptureMove : IMove, IEquatable<CaptureMove>
    {
        /// <summary>
        ///     Move Data
        /// </summary>
        public Board Board { get; }

        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }

        public Piece CapturedPiece { get; }

        /// <summary>
        ///     Constructor creates a capture move.
        /// </summary>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="capturedPiece">The piece to be captured.</param>
        public CaptureMove(Board board, int fromCoordinate, int toCoordinate, Piece movedPiece, Piece capturedPiece)
        {
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;
        }

        public Board ExecuteMove()
        {
            var boardBuilder = new BoardBuilder();

            // Set all pieces except the moved piece
            foreach (var piece in Board.AllPieces)
                if (!MovedPiece.Equals(piece))
                    boardBuilder.SetPieceAtTile(piece);

            // Move the moved piece
            boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this));

            // Set next player to move
            boardBuilder.SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition);

            // Build the board
            return boardBuilder.BuildBoard();
        }

        /// <summary>
        ///     IEquatable Implementation of Equals.
        /// </summary>
        /// <param name="other">The Capture Move struct to compare to.</param>
        /// <returns>True if equal, false if not.</returns>
        public bool Equals(CaptureMove other)
        {
            // Return true if all value types are equal
            return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
                   ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
                   Equals(CapturedPiece, other.CapturedPiece);
        }

        public override bool Equals(object obj)
        {
            // Return true if object is of type CaptureMove and they are equal
            return obj is CaptureMove other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Combine the hash codes of all value types stored
            return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece, CapturedPiece);
        }

        public static bool operator ==(CaptureMove left, CaptureMove right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CaptureMove left, CaptureMove right)
        {
            return !left.Equals(right);
        }
    }
}