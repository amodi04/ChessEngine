using System;
using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveGeneration.Types
{
    /// <inheritdoc cref="IMove" />
    /// Refer to IMove.cs for description of abstractions.
    /// <summary>
    ///     This struct stores castling move data.
    /// </summary>
    public readonly struct CastlingMove : IMove, IEquatable<CastlingMove>
    {
        public Board Board { get; }
        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }
        private readonly Piece _castlingRook;
        private readonly int _castlingRookStartPosition;
        private readonly int _castlingRookEndPosition;

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
            _castlingRook = castlingRook;
            _castlingRookStartPosition = castlingRookStartPosition;
            _castlingRookEndPosition = castlingRookEndPosition;
        }

        public Board ExecuteMove()
        {
            var boardBuilder = new BoardBuilder();

            // Set all pieces except the moved piece
            foreach (var piece in Board.AllPieces)
                if (!MovedPiece.Equals(piece))
                    boardBuilder.SetPieceAtTile(piece);
            
            boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this))
                .SetPieceAtTile(PieceUtilities.RookLookup[_castlingRookEndPosition, _castlingRook.PieceCoalition])
                .RemovePieceAtTile(_castlingRook).SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition)
                .SetPlyCount(Board.PlyCount + 1);
            
            return boardBuilder.BuildBoard();
        }
        
        public bool Equals(CastlingMove other)
        {
            return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
                   ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
                   Equals(_castlingRook, other._castlingRook) &&
                   _castlingRookStartPosition == other._castlingRookStartPosition &&
                   _castlingRookEndPosition == other._castlingRookEndPosition;
        }

        public override bool Equals(object obj)
        {
            return obj is CastlingMove other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece, _castlingRook,
                _castlingRookStartPosition, _castlingRookEndPosition);
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