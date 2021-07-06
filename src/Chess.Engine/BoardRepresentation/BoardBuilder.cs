using System.Collections.Generic;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.BoardRepresentation
{
    public class BoardBuilder
    {
        public BoardBuilder()
        {
            BoardConfiguration = new Dictionary<int, Piece>();
        }

        public Dictionary<int, Piece> BoardConfiguration { get; }

        public Coalition CoalitionToMove { get; private set; }

        public BoardBuilder SetPieceAtTile(Piece piece)
        {
            BoardConfiguration[piece.PiecePosition] = piece;
            return this;
        }

        public BoardBuilder SetCoalitionToMove(Coalition coalitionToMove)
        {
            CoalitionToMove = coalitionToMove;
            return this;
        }

        public Board BuildBoard()
        {
            return new(this);
        }
    }
}