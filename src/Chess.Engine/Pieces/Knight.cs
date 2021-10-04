using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <summary>
    ///     This class contains knight data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class Knight : Piece
    {
        /// <summary>
        ///     Constructor to create a knight.
        /// </summary>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="isFirstMove">Sets whether this is the pieces first move.</param>
        public Knight(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Knight, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }

        /// <summary>
        ///     This method generates the legal moves for the knight, given the board.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <returns>An IList of moves that can be made.</returns>
        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a knight can move in. Stored as position offsets because knights are non-sliding pieces.
            /*
             *     15   17  
             *   6         10
             *        N
             *  -10        -6
             *    -17  -15
             */
            int[] positionOffsets = {-17, -15, -10, -6, 6, 10, 15, 17};

            var moves = new List<IMove>();

            foreach (var positionOffset in positionOffsets)
            {
                // Initialise destination to piece position plus position offset
                var destinationCoordinate = PiecePosition + positionOffset;

                // If tile not in board or is on an edge case, skip offset.
                if (!IsValidTileCoordinate(destinationCoordinate) ||
                    IsColumnExclusion(PiecePosition, positionOffset))
                    continue;

                var tile = board.GetTile(destinationCoordinate);
                // If tile is empty, add normal move
                if (!tile.IsOccupied())
                {
                    moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                }
                else
                {
                    // If enemy at tile, create attacking move
                    if (IsEnemyPieceAtTile(tile))

                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.CaptureMove,
                            tile.Piece));
                }
            }

            return moves;
        }

        /// <summary>
        ///     This method moves the knight by utilising passed in move data.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public override Piece MovePiece(IMove move)
        {
            // Return the knight at the lookup table location given the two indexes passed in
            return PieceUtilities.KnightLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Knight is on special edge case when its position is on the first file
            // AND the offset is -17, -10, 6 or 15 (going left).
            // The second special edge case is when its position is on the second file
            // AND the offset is -10 or 6 (going left).
            // The third special edge case is when its position is on the seventh file
            // AND the offset is -6 or 10 (going right).
            // The fourth special edge case is when its position is on the eighth file
            // AND the offset is -15, -6, 10 or 17 (going right).
            return FileIndex(currentPosition) == 0 && offset is -17 or -10 or 6 or 15 ||
                   FileIndex(currentPosition) == 1 && offset is -10 or 6 ||
                   FileIndex(currentPosition) == 6 && offset is -6 or 10 ||
                   FileIndex(currentPosition) == 7 && offset is -15 or -6 or 10 or 17;
        }
    }
}