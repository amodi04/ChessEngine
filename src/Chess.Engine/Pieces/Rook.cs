using System;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class Rook : Piece
    {
        public Rook(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }

        public override List<Move> GenerateLegalMoves(Board board)
        {
            int[] vectorOffsets = {-8, -1, 1, 8};

            List<Move> moves = new List<Move>();

            foreach (int vectorOffset in vectorOffsets)
            {
                int destinationCoordinate = PiecePosition;
                while (IsValidTileCoordinate(destinationCoordinate))
                {
                    if (IsColumnExclusion(destinationCoordinate, vectorOffset))
                    {
                        break;
                    }

                    destinationCoordinate += vectorOffset;
                    if (!IsValidTileCoordinate(destinationCoordinate)) continue;
                    Tile tile = board.GetTile(destinationCoordinate);
                    if (!tile.IsOccupied())
                    {
                        // Move move
                        moves.Add(new Move());
                    }
                    else
                    {
                        if (IsEnemyPieceAtTile(tile))
                        {
                            // Attack Move
                            moves.Add(new Move());
                        }
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