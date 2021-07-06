using System;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
        }

        public override List<Move> GenerateLegalMoves(Board board)
        {
            int[] positionOffsets = {7, 8, 9, 16};

            List<Move> moves = new List<Move>();

            foreach (int positionOffset in positionOffsets)
            {
                int destinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * positionOffset;
                if (IsValidTileCoordinate(destinationCoordinate))
                {
                    continue;;
                }

                if (positionOffset == 8 && !board.GetTile(destinationCoordinate).IsOccupied())
                {
                    // TODO: Check for promotion
                    // Move move
                    moves.Add(new Move());
                } else if (positionOffset == 16 && IsFirstMove)
                {
                    int behindDestinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * 8;
                    if (!board.GetTile(behindDestinationCoordinate).IsOccupied() &&
                        !board.GetTile(destinationCoordinate).IsOccupied())
                    {
                        moves.Add(CreateNormalMove(board, destinationCoordinate));
                    }
                } else if (!IsColumnExclusion(PiecePosition, positionOffset))
                {
                    Tile tile = board.GetTile(destinationCoordinate);
                    if (!tile.IsOccupied())
                    {
                        if (IsEnemyPieceAtTile(tile))
                        {
                            // TODO: Check for promotion
                            moves.Add(CreateAttackMove(board, destinationCoordinate, tile.Piece));
                        }
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
            return IsInArray(currentPosition, EighthFile) && PieceCoalition.IsWhite() && offset is 7 ||
                   IsInArray(currentPosition, FirstFile) && !PieceCoalition.IsWhite() && offset is 7 ||
                   IsInArray(currentPosition, FirstFile) && PieceCoalition.IsWhite() && offset is 9 ||
                   IsInArray(currentPosition, EighthFile) && !PieceCoalition.IsWhite() && offset is 9;
        }
    }
}