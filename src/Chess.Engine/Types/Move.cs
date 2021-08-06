using System;
using Engine.BoardRepresentation;
using Engine.Enums;
using Engine.Pieces;

namespace Engine.MoveGeneration
{
    /// <summary>
    ///     This struct stores Move Data.
    /// </summary>
    public readonly struct Move : IEquatable<Move>
    {
        /// <summary>
        ///     Move Data
        /// </summary>
        public readonly MoveType Type;

        public readonly Board Board;
        public readonly int FromCoordinate;
        public readonly int ToCoordinate;
        public readonly Piece MovedPiece;
        public readonly Piece CapturedPiece;

        /// <summary>
        ///     Constructor creates a non capture move.
        /// </summary>
        /// <param name="type">This will be confined to non-capturing states.</param>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        public Move(MoveType type, Board board, int fromCoordinate, int toCoordinate, Piece movedPiece)
        {
            Type = type;
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = null;
        }

        /// <summary>
        ///     Constructor creates a capture move.
        /// </summary>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="capturedPiece">The piece at the destination coordinate being captured.</param>
        public Move(Board board, int fromCoordinate, int toCoordinate, Piece movedPiece,
            Piece capturedPiece)
        {
            Type = MoveType.CaptureMove;
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

        /// <summary>
        /// IEquatable Implementation of Equals.
        /// </summary>
        /// <param name="other">The Move struct to compare to.</param>
        /// <returns>True if equal, false if not.</returns>
        public bool Equals(Move other)
        {
            // Return true if all value types are equal
            return Type == other.Type && Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
                   ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
                   Equals(CapturedPiece, other.CapturedPiece);
        }

        /// <summary>
        /// Checks if two objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            // Return true if object is of type move and they are equal
            return obj is Move other && Equals(other);
        }

        /// <summary>
        /// Gets the hash code of the current move struct in memory.
        /// </summary>
        /// <returns>The hash code combination of all value types within the struct.</returns>
        public override int GetHashCode()
        {
            // Combine hash codes of all value types
            return HashCode.Combine((int) Type, Board, FromCoordinate, ToCoordinate, MovedPiece, CapturedPiece);
        }

        /// <summary>
        /// Shorthand operator for equal comparison.
        /// </summary>
        /// <param name="left">The object to compare.</param>
        /// <param name="right">The object to compare against.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool operator == (Move left, Move right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Shorthand operator for not equal comparison.
        /// </summary>
        /// <param name="left">The object to compare.</param>
        /// <param name="right">The object to compare against.</param>
        /// <returns>True if not equal, false if equal.</returns>
        public static bool operator != (Move left, Move right)
        {
            return !left.Equals(right);
        }
    }
}