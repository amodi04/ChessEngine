using System;
using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class King : Piece
    {
        public King(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }

        public override IList GenerateLegalMoves(Board board)
        {
            int[] positionOffsets = {-9, -8, -7, -1, 1, 7, 8, 9};

            var moves = new List<Move>();
            foreach (var positionOffset in positionOffsets)
            {
                var destinationCoordinate = PiecePosition + positionOffset;

                if (IsColumnExclusion(PiecePosition, positionOffset)) continue;

                if (IsValidTileCoordinate(destinationCoordinate))
                {
                    var tile = board.GetTile(destinationCoordinate);
                    if (!tile.IsOccupied())
                    {
                        // Move move
                        moves.Add(CreateNormalMove(board, destinationCoordinate));
                    }
                    else
                    {
                        if (IsEnemyPieceAtTile(tile))
                            // Attack Move
                            moves.Add(CreateAttackMove(board, destinationCoordinate, tile.Piece));
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
                   && offset is -9 or -1 or 7 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is -7 or 1 or 9;
        }
    }
}