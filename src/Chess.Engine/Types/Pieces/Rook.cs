﻿using System.Collections.Generic;
using Engine.Enums;
using Engine.Factories;
using Engine.Types.MoveGeneration;
using Engine.Util;
using static Engine.Util.BoardUtilities;

namespace Engine.Types.Pieces
{
    public sealed class Rook : Piece
    {
        /// <inheritdoc cref="Piece" />
        public Rook(int piecePosition, Coalition pieceCoalition) :
            base(PieceType.Rook, piecePosition, pieceCoalition)
        {
            // Empty
        }

        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a rook can move in. Stored as vector offset because rooks are sliding pieces.
            /*
             *  -  8  -
             * -1  R  1
             *  - -8  -
             */
            int[] vectorOffsets = {-8, -1, 1, 8};

            var moves = new List<IMove>();

            foreach (var vectorOffset in vectorOffsets)
            {
                // Initialise destination coordinate to piece position
                var destinationCoordinate = PiecePosition;

                // While the destination coordinate is within the board range
                // (we want to check all moves in this direction)
                while (IsValidTileCoordinate(destinationCoordinate))
                {
                    // If rook is on an edge case, skip any further generation of moves in this vector offset
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
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                                MoveType.CaptureMove, tile.Piece));
                        break;
                    }
                }
            }

            return moves;
        }

        public override Piece MovePiece(IMove move)
        {
            return PieceUtilities.RookLookup[move.MovedPiece.PiecePosition, move.MovedPiece.PieceCoalition];
        }


        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Rook is on special edge case when its position is on the first file
            // AND the offset is -1 (going left).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is 1 (going right)
            return IsInArray(currentPosition, FirstFile)
                   && offset is -1 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is 1;
        }
    }
}