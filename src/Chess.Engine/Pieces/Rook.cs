using System;
using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class Rook : Piece
    {
        public Rook(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }

        public override IList GenerateLegalMoves(Board board)
        {
            int[] vectorOffsets = {-8, -1, 1, 8};

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
                            // Attack Move
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
                   && offset is -1 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is 1;
        }
    }
}