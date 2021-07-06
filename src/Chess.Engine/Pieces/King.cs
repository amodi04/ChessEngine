using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class King : Piece
    {
        public King(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }
        
        public override List<Move> GenerateLegalMoves(Board board)
        {
            int[] positionOffsets = {-9, -8, -7, -1, 1, 7, 8, 9};

            List<Move> moves = new List<Move>();
            foreach (int positionOffset in positionOffsets)
            {
                int destinationCoordinate = PiecePosition + positionOffset;

                if (IsColumnExclusion(PiecePosition, destinationCoordinate))
                {
                    continue;
                }

                if (IsValidTileCoordinate(destinationCoordinate))
                {
                    Tile tile = board.GetTile(destinationCoordinate);
                    if (!tile.IsOccupied())
                    {
                        // Move move
                        moves.Add(CreateNormalMove(board, destinationCoordinate));
                    }
                    else
                    {
                        if (IsEnemyPieceAtTile(tile))
                        {
                            // Attack Move
                            moves.Add(CreateAttackMove(board, destinationCoordinate, tile.Piece));
                        }
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