using System;
using Engine.BoardRepresentation;
using Engine.MoveGeneration.Types;
using Engine.Pieces;

namespace Engine.MoveGeneration
{
    public static class MoveFactory
    {
        /// <summary>
        ///     Factory method for handling the creation of different moves.
        /// </summary>
        /// <param name="board">The board that the move is executed on.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="toCoordinate">The end coordinate of the moved piece.</param>
        /// <param name="moveType">The type of move. Default is normal move.</param>
        /// <param name="capturedPiece">The piece being captured. Default is null.</param>
        /// <param name="castlingRook">The rook being castled. Default is null.</param>
        /// <param name="castlingRookEndPosition">The castling rook destination. Default is 0.</param>
        /// <param name="isEnPassant">IsEnPassant flag for the move. True if the move is an en passant move.</param>
        /// <param name="promotedPiece">The piece to promote to. Default is null.</param>
        /// <returns>An implementer of IMove dependent on passed in move type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Called when an unhandled move type is passed in.</exception>
        public static IMove CreateMove(Board board, Piece movedPiece, int toCoordinate,
            MoveType moveType = MoveType.NormalMove, Piece capturedPiece = null, Piece castlingRook = default,
            int castlingRookEndPosition = default, bool isEnPassant = false, Piece promotedPiece = null)
        {
            return moveType switch
            {
                // Creates a normal move
                MoveType.NormalMove => CreateNormalMove(board, movedPiece, toCoordinate),
                
                // Creates a capture move taking in an additional captured piece
                MoveType.CaptureMove => CreateAttackMove(board, movedPiece, toCoordinate, capturedPiece, isEnPassant),
                
                // Creates a castling move taking in additional rook parameters
                MoveType.CastleMove => CreateCastlingMove(board, movedPiece, toCoordinate, castlingRook,
                    castlingRookEndPosition),
                
                // Creates a promotion move which wraps either a normal or attack move depending on if there is a piece to be captured
                MoveType.PromotionMove => capturedPiece == null ? 
                    CreatePromotionMove(CreateNormalMove(board, movedPiece, toCoordinate), promotedPiece) :
                    CreatePromotionMove(CreateAttackMove(board, movedPiece, toCoordinate, capturedPiece, false), promotedPiece),
                
                // Throw argument out of range when unknown move type is passed in
                _ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null)
            };
        }

        /// <summary>
        /// Factory method for getting a piece from the current available moves on the board.
        /// </summary>
        /// <param name="board">The board to get the move from.</param>
        /// <param name="fromCoordinate">The initial piece position.</param>
        /// <param name="toCoordinate">The destination of the piece.</param>
        /// <returns>A move that contains the passed in data. If it isn't a valid move, a null move is returned.</returns>
        public static IMove GetMove(Board board, int fromCoordinate, int toCoordinate)
        {
            // Loop through all moves available
            foreach (var move in board.AllMoves)
            {
                // If the passed in data matches a valid move, return that move
                if (move.FromCoordinate == fromCoordinate && move.ToCoordinate == toCoordinate)
                {
                    return move;
                }
            }

            // Else, return a null move
            return new NormalMove(null, -1, -1, null);
        }

        /// <summary>
        ///     Factory method to create a normal move.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <returns>A NormalMove containing the passed in move data.</returns>
        private static NormalMove CreateNormalMove(Board board, Piece movedPiece, int toCoordinate)
        {
            return new NormalMove(board, movedPiece.PiecePosition, toCoordinate, movedPiece);
        }

        /// <summary>
        ///     Factory method to create a capture move.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <param name="capturedPiece">The piece being captured.</param>
        /// <param name="isEnPassant">Determines whether the move is an en passant move.</param>
        /// <returns>A CaptureMove containing the passed in move data.</returns>
        private static CaptureMove CreateAttackMove(Board board, Piece movedPiece, int toCoordinate, Piece capturedPiece, bool isEnPassant)
        {
            return new CaptureMove(board, movedPiece.PiecePosition, toCoordinate, movedPiece, capturedPiece, isEnPassant);
        }

        /// <summary>
        ///     Factory method to create a castling move.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="movedPiece">The piece to be moved. In this case it will be a king.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <param name="castlingRook">The rook being castled.</param>
        /// <param name="castlingRookEndPosition">The destination coordinate of the rook participating in the move.</param>
        /// <returns>A CastlingMove containing the passed in move data.</returns>
        private static CastlingMove CreateCastlingMove(Board board, Piece movedPiece, int toCoordinate, Piece castlingRook,
            int castlingRookEndPosition)

        {
            return new CastlingMove(board, movedPiece.PiecePosition, toCoordinate, movedPiece, castlingRook, 
                castlingRook.PiecePosition, castlingRookEndPosition);
        }

        /// <summary>
        /// Factory method to create a promotion move.
        /// </summary>
        /// <param name="move">The move to wrap.</param>
        /// <param name="promotedPiece">The piece to promote to.</param>
        /// <returns>A move wrapped in a promotion move.</returns>
        private static PromotionMove CreatePromotionMove(IMove move, Piece promotedPiece)
        {
            return new PromotionMove(move, promotedPiece);
        }
    }
}