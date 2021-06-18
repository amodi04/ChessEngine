using System.Collections.Generic;
using Engine.BoardRepresentation.TileRepresentation;

namespace Engine.BoardRepresentation
{
    public class Board
    {
        private List<Tile> _board;

        internal Board(BoardBuilder boardBuilder)
        {
            _board = InitialiseBoard(boardBuilder);
        }

        private List<Tile> InitialiseBoard(BoardBuilder boardBuilder)
        {
            Tile[] tiles = new Tile[BoardUtilities.NumTiles];
            for (int i = 0; i < BoardUtilities.NumTiles; i++)
            {
                tiles[i] = Tile.CreateTile(i, boardBuilder.BoardConfiguration[i]);
            }

            return new List<Tile>(tiles);
        }

        public Tile GetTile(int tileCoordinate)
        {
            return _board[tileCoordinate];
        }
    }
}