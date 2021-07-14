﻿using System;
using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{

    /// <inheritdoc cref="Piece"/>
    public class Bishop : Piece
    {
        public Bishop(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }
        public override IList GenerateLegalMoves(Board board)
        {
            int[] vectorOffsets = {-9, -7, 7, 9};

            var moves = new List<Move>();

            foreach (var vectorOffset in vectorOffsets)
            {
                var destinationCoordinate = PiecePosition;
                while (IsValidTileCoordinate(destinationCoordinate))
                {
                    if (IsColumnExclusion(destinationCoordinate, vectorOffset)) break;

                    destinationCoordinate += vectorOffset;
                    if (!IsValidTileCoordinate(destinationCoordinate)) continue;

                    var tile = board.GetTile(destinationCoordinate);
                    if (!tile.IsOccupied())
                    {
                        // Move move
                        moves.Add(CreateNormalMove(board, destinationCoordinate));
                    }
                    else
                    {
                        if (IsEnemyPieceAtTile(tile))
                            // Attacking move
                            moves.Add(CreateAttackMove(board, destinationCoordinate, tile.Piece));
                        break;
                    }
                }
            }

            return moves;
        }

        public override Piece MovePiece(Move move)
        {
            throw new NotImplementedException();
        }

        protected override bool IsColumnExclusion(int currentPosition, int offset)
        {
            return IsInArray(currentPosition, FirstFile)
                   && offset is -9 or -7 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is -7 or 9;
        }
    }
}