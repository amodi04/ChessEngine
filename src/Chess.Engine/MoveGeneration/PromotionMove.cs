using System;
using Engine.Builders;
using Engine.Types.Pieces;

namespace Engine.Types.MoveGeneration
{
    /// <inheritdoc cref="IMove" />
    /// <summary>
    ///     This struct stores promotion move data.
    /// </summary>
    public readonly struct PromotionMove : IMove, IEquatable<PromotionMove>
    {
        /// <summary>
        /// Move Data.
        /// </summary>
        public Board Board { get; }
        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }
        public Piece? CapturedPiece { get; }
        public IMove WrappedMove { get; }
        public Pawn PromotingPawn { get; }
        public Piece PromotedPiece { get; }

        /// <summary>
        /// Constructor creates a promotion move
        /// </summary>
        /// <param name="wrappedMove">The move to wrap around.</param>
        /// <param name="promotedPiece">The piece to promote to.</param>
        public PromotionMove(IMove wrappedMove, Piece promotedPiece)
        {
            Board = wrappedMove.Board;
            FromCoordinate = wrappedMove.FromCoordinate;
            ToCoordinate = wrappedMove.ToCoordinate;
            MovedPiece = wrappedMove.MovedPiece;
            WrappedMove = wrappedMove;
            PromotingPawn = (Pawn) MovedPiece;
            PromotedPiece = promotedPiece;
            
            // If the wrapped move is a capture move then set the captured piece
            if (wrappedMove is CaptureMove captureMove)
            {
                CapturedPiece = captureMove.CapturedPiece;
            }
            // Otherwise, set to null
            else
            {
                CapturedPiece = null;
            }
        }
        
        public Board ExecuteMove()
        {
            // Get the board state including the pawn movement to the promotion rank
            var intermediaryBoard = WrappedMove.ExecuteMove();
            var boardBuilder = new BoardBuilder();
            
            // Set all pieces except the moved piece
            foreach (var piece in intermediaryBoard.AllPieces)
                if (!PromotingPawn.Equals(piece))
                    boardBuilder.SetPieceAtTile(piece);
            
            
            // Move the promoted piece to where the pawn ins
            boardBuilder.SetPieceAtTile(PromotedPiece.MovePiece(this));

            // Set player to move to current board player because the intermediary board player to move is the opponent
            // This was set in the previous move execution
            boardBuilder.SetCoalitionToMove(intermediaryBoard.CurrentPlayer.Coalition);

            // Build the board
            return boardBuilder.BuildBoard();
        }

        public bool Equals(PromotionMove other)
        {
            return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
                   ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
                   Equals(WrappedMove, other.WrappedMove) && Equals(PromotingPawn, other.PromotingPawn) &&
                   Equals(PromotedPiece, other.PromotedPiece);
        }

        public override bool Equals(object obj)
        {
            return obj is PromotionMove other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece, WrappedMove, PromotingPawn, PromotedPiece);
        }

        public static bool operator ==(PromotionMove left, PromotionMove right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PromotionMove left, PromotionMove right)
        {
            return !left.Equals(right);
        }
    }
}