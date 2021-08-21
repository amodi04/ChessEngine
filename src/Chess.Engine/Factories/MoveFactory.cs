using System;
using Engine.Enums;
using Engine.Types;
using Engine.Types.MoveGeneration;
using Engine.Types.Pieces;

namespace Engine.Factories
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
        /// <returns>An implementer of IMove dependent on passed in move type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Called when an unhandled move type is passed in.</exception>
        public static IMove CreateMove(Board board, Piece movedPiece, int toCoordinate,
            MoveType moveType = MoveType.NormalMove, Piece capturedPiece = null, Piece castlingRook = default,
            int castlingRookEndPosition = default)
        {
            return moveType switch
            {
                MoveType.NormalMove => CreateNormalMove(board, movedPiece, toCoordinate),
                MoveType.CaptureMove => CreateAttackMove(board, movedPiece, toCoordinate, capturedPiece),
                MoveType.CastleMove => CreateCastlingMove(board, movedPiece, toCoordinate, castlingRook,
                    castlingRookEndPosition),
                MoveType.PromotionMove => null,
                MoveType.EnPassantMove => null,
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
        private static IMove CreateNormalMove(Board board, Piece movedPiece, int toCoordinate)
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
        /// <returns>A CaptureMove containing the passed in move data.</returns>
        private static IMove CreateAttackMove(Board board, Piece movedPiece, int toCoordinate, Piece capturedPiece)
        {
            return new CaptureMove(board, movedPiece.PiecePosition, toCoordinate, movedPiece, capturedPiece);
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
        private static IMove CreateCastlingMove(Board board, Piece movedPiece, int toCoordinate, Piece castlingRook,
            int castlingRookEndPosition)

        {
            return new CastlingMove(board, movedPiece.PiecePosition, toCoordinate, movedPiece, castlingRook,
                castlingRook.PiecePosition, castlingRookEndPosition);
        }
    }
}