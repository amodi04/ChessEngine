using System.Collections.Generic;
using Engine.BoardRepresentation;
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
            int[] positionOffsets = {7, 8, 9};

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
                }
                // TODO: Implement pawn attacks
            }

            return moves;
        }

        public override Piece MovePiece(Move move)
        {
            throw new System.NotImplementedException();
        }

        protected override bool IsColumnExclusion(int currentPosition, int offset)
        {
            throw new System.NotImplementedException();
        }
    }
}