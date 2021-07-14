using System.Collections.Generic;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.BoardRepresentation
{
    /// <summary>
    /// This class stores current game state in form of the chess board.
    /// </summary>
    public class Board
    {
        // Member fields
        private readonly Tile[] _board;
        private IEnumerable<Piece> _blackPieces;
        private IEnumerable<Piece> _whitePieces;

        /// <summary>
        /// Internal constructor - only used by board builder.
        /// </summary>
        /// <param name="boardBuilder">The BoardBuilder object that builds the board.</param>
        internal Board(BoardBuilder boardBuilder)
        {
            _board = InitialiseBoard(boardBuilder);
            _whitePieces = CalculateActivePieces(_board, Coalition.White);
            _blackPieces = CalculateActivePieces(_board, Coalition.Black);
        }

        /// <summary>
        /// Gets all pieces on the current board.
        /// </summary>
        /// <param name="board">The array of tile data which may contain pieces.</param>
        /// <param name="coalition">Tile state that depicts what colour pieces are returned.</param>
        /// <returns>Returns an iterable collection so that it cannot be modified.</returns>
        private IEnumerable<Piece> CalculateActivePieces(Tile[] board, Coalition coalition)
        {
            // TODO: Test LINQ performance
            var pieces = new List<Piece>();
            foreach (var tile in board)
            {
                if (!tile.IsOccupied()) continue;
                var piece = tile.Piece;
                if (piece.PieceCoalition == coalition) pieces.Add(piece);
            }

            return pieces;
        }

        /// <summary>
        /// Creates the board based on the BoardBuilder configuration.
        /// </summary>
        /// <param name="boardBuilder">The board builder containing the current board configuration.</param>
        /// <returns>A length 64 array of tiles.</returns>
        private Tile[] InitialiseBoard(BoardBuilder boardBuilder)
        {
            var tiles = new Tile[BoardUtilities.NumTiles];
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
                tiles[i] = Tile.CreateTile(i, boardBuilder.BoardConfiguration[i]);

            return tiles;
        }

        // TODO: Use FEN
        /// <summary>
        /// Generates the standard board of chess.
        /// </summary>
        /// <returns>Returns a board object containing tiles with pieces at their starting positions.</returns>
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

        /// <summary>
        /// Gets the tile data at the given tileCoordinate.
        /// </summary>
        /// <param name="tileCoordinate">The tile position on the board.</param>
        /// <returns>Tile data at given position.</returns>
        public Tile GetTile(int tileCoordinate)
        {
            return _board[tileCoordinate];
        }
    }
}