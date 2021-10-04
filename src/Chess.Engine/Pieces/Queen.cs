using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <summary>
    ///     This class contains queen data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class Queen : Piece
    {
        /// <summary>
        ///     Constructor to create a queen.
        /// </summary>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="isFirstMove">Sets whether this is the pieces first move.</param>
        public Queen(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Queen, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }

        /// <summary>
        ///     This method generates the legal moves for the queen, given the board.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <returns>An IList of moves that can be made.</returns>
        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a queen can move in. Stored as vector offset because queens are sliding pieces.
            /*
             *  7  8  9
             * -1  Q  1
             * -9 -8 -7
             */
            int[] vectorOffsets = {-9, -8, -7, -1, 1, 7, 8, 9};

            var moves = new List<IMove>();

            foreach (var vectorOffset in vectorOffsets)
            {
                // Initialise destination coordinate to piece position
                var destinationCoordinate = PiecePosition;

                // While the destination coordinate is within the board range
                // (we want to check all moves in this direction)
                while (IsValidTileCoordinate(destinationCoordinate))
                {
                    // If queen is on an edge case, skip any further generation of moves in this vector offset
                    if (IsColumnExclusion(destinationCoordinate, vectorOffset)) break;

                    // Step the destination coordinate by the vector offset
                    destinationCoordinate += vectorOffset;

                    // Skip if not in board range
                    if (!IsValidTileCoordinate(destinationCoordinate)) continue;
                    var tile = board.GetTile(destinationCoordinate);

                    // If tile is empty, add normal move
                    if (!tile.IsOccupied())
                    {
                        // Move move
                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                    }
                    else
                    {
                        // If enemy at tile, add attacking move
                        if (IsEnemyPieceAtTile(tile))
                            // Attack Move
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.CaptureMove,
                                tile.Piece));
                        break;
                    }
                }
            }

            return moves;
        }

        /// <summary>
        ///     This method moves the queen by utilising passed in move data.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public override Piece MovePiece(IMove move)
        {
            return PieceUtilities.QueenLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Queen is on special edge case when its position is on the first file
            // AND the offset is -9, -1 or 7 (going left).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is -7, 1 or 9 (going right) 
            return FileIndex(currentPosition) == 0 && offset is -9 or -1 or 7 ||
                   FileIndex(currentPosition) == 7 && offset is -7 or 1 or 9;
        }
    }
}