using System.Collections.Generic;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.BoardRepresentation
{
    public class Board
    {
        private readonly Tile[] _board;

        internal Board(BoardBuilder boardBuilder)
        {
            _board = InitialiseBoard(boardBuilder);
        }

        private Tile[] InitialiseBoard(BoardBuilder boardBuilder)
        {
            var tiles = new Tile[BoardUtilities.NumTiles];
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
                tiles[i] = Tile.CreateTile(i, boardBuilder.BoardConfiguration[i]);

            return tiles;
        }

        // TODO: Use FEN
        public static Board CreateStandardBoard()
        {
            int[] boardConstants = {0, 56};
            var boardBuilder = new BoardBuilder();
            var coalition = Coalition.White;
            for (var i = 0; i <= 1; i++)
            {
                if (i != 0) coalition = Coalition.Black;

                int index;
                for (index = boardConstants[i]; index <= boardConstants[i] + 7; index++)
                    if (index == boardConstants[i] || index == boardConstants[i] + 7)
                        boardBuilder.SetPieceAtTile(new Rook(index, coalition));
                    else if (index == boardConstants[i] + 1 || index == boardConstants[i] + 6)
                        boardBuilder.SetPieceAtTile(new Knight(index, coalition));
                    else if (index == boardConstants[i] + 2 || index == boardConstants[i] + 5)
                        boardBuilder.SetPieceAtTile(new Bishop(index, coalition));
                    else if (index == boardConstants[i] + 3)
                        boardBuilder.SetPieceAtTile(new Queen(index, coalition));
                    else if (index == boardConstants[i] + 4) boardBuilder.SetPieceAtTile(new King(index, coalition));

                // coalition == coalition.WHITE
                if (i == 0)
                    for (index = boardConstants[0] + 8; index <= 15; index++)
                        boardBuilder.SetPieceAtTile(new Pawn(index, coalition));
                else
                    for (index = boardConstants[1] - 8; index <= 55; index++)
                        boardBuilder.SetPieceAtTile(new Pawn(index, coalition));
            }

            return boardBuilder.BuildBoard();
        }

        public Tile GetTile(int tileCoordinate)
        {
            return _board[tileCoordinate];
        }
    }
}