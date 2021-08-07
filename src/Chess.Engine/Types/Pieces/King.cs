﻿using System.Collections;
using System.Collections.Generic;
using Engine.Enums;
using Engine.Factories;
using static Engine.Util.BoardUtilities;

namespace Engine.Types.Pieces
{
    /// <inheritdoc cref="Piece" />
    public sealed class King : Piece
    {
        public King(int piecePosition, Coalition pieceCoalition) :
            base(PieceType.King, piecePosition, pieceCoalition)
        {
            // Empty
        }

        public override IList GenerateLegalMoves(Board board)
        {
            // Directions that a king can move in. Stored as position offsets because kings are non-sliding pieces.
            /*
             *  7  8  9
             * -1  K  1
             * -9 -8 -7
             */
            int[] positionOffsets = {-9, -8, -7, -1, 1, 7, 8, 9};

            var moves = new List<Move>();

            foreach (var positionOffset in positionOffsets)
            {
                // Initialise destination coordinate to piece position plus it's position offset
                var destinationCoordinate = PiecePosition + positionOffset;

                // If tile not in board or is on an edge case, skip offset.
                if (!IsValidTileCoordinate(destinationCoordinate) ||
                    IsColumnExclusion(PiecePosition, positionOffset))
                    continue;

                var tile = board.GetTile(destinationCoordinate);

                // If empty, create normal move
                if (!tile.IsOccupied())
                {
                    moves.Add(MoveFactory.CreateNormalMove(board, this, destinationCoordinate));
                }
                else
                {
                    // If enemy at tile, create attack move
                    if (IsEnemyPieceAtTile(tile))
                        moves.Add(MoveFactory.CreateAttackMove(board, this, destinationCoordinate, tile.Piece));
                }
            }

            return moves;
        }

        public override Piece MovePiece(Move move)
        {
            return new King(move.MovedPiece.PiecePosition, move.MovedPiece.PieceCoalition);
        }

        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // King is on special edge case when its position is on the first file
            // AND the offset is -9, -1 or 7 (going left).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is -7, 1 or 9 (going right) 
            return IsInArray(currentPosition, FirstFile)
                   && offset is -9 or -1 or 7 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is -7 or 1 or 9;
        }
    }
}