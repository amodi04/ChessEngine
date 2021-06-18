using System.Collections.Generic;
using Engine.Board;

namespace Engine.Pieces
{
    public class Knight : Piece
    {
        private readonly int[] _positionOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

        public Knight(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition) { }

        public override List<Move> GenerateLegalMoves(Board.Board board)
        {
            List<Move> moves = new List<Move>();

            foreach (int positionOffset in _positionOffsets)
            {
                int destinationCoordinate = PiecePosition + positionOffset;
                if (BoardUtilities.IsValidTileCoordinate(destinationCoordinate) &&
                    IsKnightInColumnExclusion(PiecePosition, positionOffset))
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
                    Piece pieceAtTile = tile.Piece;

                    if (PieceCoalition != pieceAtTile.PieceCoalition)
                    {
                        // Attack Move
                        moves.Add(new Move());
                    }
                }

                return moves;
            }

            return moves;
        }

        public override Piece MovePiece(Move move)
        {
            throw new System.NotImplementedException();
        }
        
        private bool IsKnightInColumnExclusion(int currentPosition, int positionOffset)
        {
            return IsInArray(currentPosition, BoardUtilities.FirstFile) && positionOffset is -17 or -10 or 6 or 15 ||
                   IsInArray(currentPosition, BoardUtilities.SecondFile) && positionOffset is -10 or 6 ||
                   IsInArray(currentPosition, BoardUtilities.SeventhFile) && positionOffset is -6 or 10 ||
                   IsInArray(currentPosition, BoardUtilities.EighthFile) && positionOffset is -15 or -6 or 10 or 15;
        }

        private bool IsInArray(int value, int[] array)
        {
            foreach (int i in array)
            {
                if (value == i)
                {
                    return true;
                }
            }

            return false;
        }
    }
}