using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <summary>
    ///     This class contains bishop data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class Bishop : Piece
    {
        /// <summary>
        ///     Constructor to create a bishop.
        /// </summary>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="isFirstMove">Sets whether this is the pieces first move.</param>
        public Bishop(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Bishop, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }

        /// <summary>
        ///     This method generates the legal moves for the bishop, given the board.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <returns>An IList of moves that can be made.</returns>
        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a bishop can move in. Stored as vector offsets because bishops are sliding pieces.
            /*
             *  7     9
             *     B   
             * -9    -7
             */
            int[] vectorOffsets = {-9, -7, 7, 9};

            var moves = new List<IMove>();

            foreach (var vectorOffset in vectorOffsets)
            {
                // Initialise destination coordinate to piece position
                var destinationCoordinate = PiecePosition;

                // While the destination coordinate is within the board range
                // (we want to check all moves in this direction)
                while (IsValidTileCoordinate(destinationCoordinate))
                {
                    // If the bishop is on an edge tile then exit while loop
                    if (IsColumnExclusion(destinationCoordinate, vectorOffset)) break;

                    // Step the destination coordinate by the offset
                    destinationCoordinate += vectorOffset;

                    // If outside of board, then skip to next offset.
                    // This will be when all valid diagonals in one direction has been covered.
                    if (!IsValidTileCoordinate(destinationCoordinate)) continue;

                    var tile = board.GetTile(destinationCoordinate);

                    // Tile is empty, add a normal move
                    if (!tile.IsOccupied())
                    {
                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                    }
                    else
                    {
                        // If enemy at tile, add attacking move
                        if (IsEnemyPieceAtTile(tile))
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.CaptureMove,
                                tile.Piece));
                        break;
                    }
                }
            }

            return moves;
        }

        /// <summary>
        ///     This method moves the bishop by utilising passed in move data.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public override Piece MovePiece(IMove move)
        {
            // Return the bishop at the lookup table location given the two indexes passed in
            return PieceUtilities.BishopLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Bishop is on special edge case when its position is on the first file
            // AND the offset is -9 or 7 (going left).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is -7 or 9 (going right) 
            return FileIndex(currentPosition) == 0 && offset is -9 or 7 ||
                   FileIndex(currentPosition) == 7 && offset is -7 or 9;
        }
    }
}