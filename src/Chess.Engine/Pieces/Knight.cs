using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class Knight : Piece
    {
        public Knight(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }

        public override List<Move> GenerateLegalMoves(Board board)
        {
            int[] positionOffsets = {-17, -15, -10, -6, 6, 10, 15, 17};
            
            List<Move> moves = new List<Move>();

            foreach (int positionOffset in positionOffsets)
            {
                int destinationCoordinate = PiecePosition + positionOffset;
                if (IsValidTileCoordinate(destinationCoordinate) &&
                    IsColumnExclusion(PiecePosition, positionOffset))
                {
                    continue;
                }

                Tile tile = board.GetTile(destinationCoordinate);
                if (!tile.IsOccupied())
                {
                    // Move
                    moves.Add(new Move());
                }
                else
                {
                    if (IsEnemyPieceAtTile(tile))
                    {
                        // Attacking move
                        moves.Add(new Move());
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
                   && offset is -17 or -10 or 6 or 15 ||
                   IsInArray(currentPosition, SecondFile)
                   && offset is -10 or 6 ||
                   IsInArray(currentPosition, SeventhFile)
                   && offset is -6 or 10 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is -15 or -6 or 10 or 15;
        }
    }
}