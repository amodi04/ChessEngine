using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class Queen : Piece
    {
        public Queen(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }

        public override List<Move> GenerateLegalMoves(Board board)
        {
            int[] vectorOffsets = {-9, -8, -7, -1, 1, 7, 8, 9};

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
            throw new System.NotImplementedException();
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