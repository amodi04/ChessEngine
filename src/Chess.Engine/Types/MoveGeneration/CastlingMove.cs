using System;
using Engine.Builders;
using Engine.Types.Pieces;
using Engine.Util;

namespace Engine.Types.MoveGeneration
{
    /// <inheritdoc cref="IMove" />
    /// <summary>
    ///     This struct stores castling move data.
    /// </summary>
    public readonly struct CastlingMove : IMove, IEquatable<CastlingMove>
    {
        /// <summary>
        ///     Move Data
        /// </summary>
        public Board Board { get; }
        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }
        public Piece CastlingRook { get; }
        public int CastlingRookStartPosition { get; }
        public int CastlingRookEndPosition { get; }

        /// <summary>
        ///     Constructor creates a capture move.
        /// </summary>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="castlingRook">The rook that will participate in the castling move.</param>
        /// <param name="castlingRookStartPosition">The start position of the rook.</param>
        /// <param name="castlingRookEndPosition">The end position of the rook.</param>
        public CastlingMove(Board board, int fromCoordinate, int toCoordinate, Piece movedPiece, Piece castlingRook,
            int castlingRookStartPosition, int castlingRookEndPosition)
        {
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CastlingRook = castlingRook;
            CastlingRookStartPosition = castlingRookStartPosition;
            CastlingRookEndPosition = castlingRookEndPosition;
        }

        public Board ExecuteMove()
        {
            var boardBuilder = new BoardBuilder();

            // Set all pieces except the moved piece
            foreach (var piece in Board.AllPieces)
                if (!MovedPiece.Equals(piece))
                    boardBuilder.SetPieceAtTile(piece);

            // Move the king piece, move the rook and set the next player to move
            boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this)).
                SetPieceAtTile(PieceUtilities.RookLookup[CastlingRookEndPosition, CastlingRook.PieceCoalition]).RemovePieceAtTile(CastlingRook).
                SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition);

            // Build the board
            return boardBuilder.BuildBoard();
        }

        /// <summary>
        ///     IEquatable Implementation of Equals.
        /// </summary>
        /// <param name="other">The CastlingMove struct to compare to.</param>
        /// <returns>True if equal, false if not.</returns>
        public bool Equals(CastlingMove other)
        {
            // Return true if all value types are equal
            return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
                   ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
                   Equals(CastlingRook, other.CastlingRook) &&
                   CastlingRookStartPosition == other.CastlingRookStartPosition &&
                   CastlingRookEndPosition == other.CastlingRookEndPosition;
        }

        public override bool Equals(object obj)
        {
            // Return true if object is of type CaptureMove and they are equal
            return obj is CastlingMove other && Equals(other);
        }

        public override int GetHashCode()
        {
            // Combine the hash codes of all value types stored
            return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece, CastlingRook,
                CastlingRookStartPosition, CastlingRookEndPosition);
        }

        public static bool operator ==(CastlingMove left, CastlingMove right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CastlingMove left, CastlingMove right)
        {
            return !left.Equals(right);
        }
    }
}