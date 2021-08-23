using System.Collections.Generic;
using Engine.Enums;
using Engine.Factories;
using Engine.Types.MoveGeneration;
using Engine.Util;
using static Engine.Util.BoardUtilities;

namespace Engine.Types.Pieces
{
    /// <inheritdoc cref="Piece" />
    public sealed class Bishop : Piece
    {
        public Bishop(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Bishop, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }

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

        public override Piece MovePiece(IMove move)
        {
            return PieceUtilities.BishopLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Bishop is on special edge case when its position is on the first file
            // AND the offset is -9 or 7 (going left).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is -7 or 9 (going right) 
            return IsInArray(currentPosition, FirstFile)
                   && offset is -9 or 7 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is -7 or 9;
        }
    }
}