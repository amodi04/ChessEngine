using Engine.Enums;
using Engine.Types;
using Engine.Types.Pieces;

namespace Engine.Factories
{
    public static class MoveFactory
    {
        /// <summary>
        ///     Factory method to create a normal move for all pieces.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <returns>A move containing the passed in move data.</returns>
        public static Move CreateNormalMove(Board board, Piece movedPiece, int toCoordinate)
        {
            return new(MoveType.NormalMove, board, movedPiece.PiecePosition, toCoordinate, movedPiece);
        }

        /// <summary>
        ///     Factory method to create a capture move for all pieces.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <param name="capturedPiece">The piece being captured.</param>
        /// <returns>A move containing the passed in move data.</returns>
        public static Move CreateAttackMove(Board board, Piece movedPiece, int toCoordinate, Piece capturedPiece)
        {
            return new(board, movedPiece.PiecePosition, toCoordinate, movedPiece, capturedPiece);
        }
    }
}