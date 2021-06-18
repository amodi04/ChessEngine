using System.Collections.Generic;
using Engine.Pieces;

namespace Engine.Board
{
    public class BoardBuilder
    {
        public Dictionary<int, Piece> BoardConfiguration { get; }

        public BoardBuilder()
        {
            BoardConfiguration = new Dictionary<int, Piece>();
        }

        public BoardBuilder SetPieceAtTile(Piece piece)
        {
            BoardConfiguration[piece.PiecePosition] = piece;
            return this;
        }

        public Board BuildBoard()
        {
            return new Board(this);
        }
    }
}