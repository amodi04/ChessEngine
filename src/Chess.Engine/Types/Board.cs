using System;
using System.Collections.Generic;
using System.Text;
using Engine.Extensions;
using Engine.MoveGeneration;
using Engine.Opposition;
using Engine.Pieces;
using Engine.PlayerRepresentation;
using Engine.Util;

namespace Engine.BoardRepresentation
{
    /// <summary>
    ///     This class stores current game state in form of the chess board.
    /// </summary>
    public class Board
    {
        // Member fields
        private readonly Tile[] _board;

        /// <summary>
        ///     Internal constructor - only used by board builder.
        /// </summary>
        /// <param name="boardBuilder">The BoardBuilder object that builds the board.</param>
        internal Board(BoardBuilder boardBuilder)
        {
            _board = InitialiseBoard(boardBuilder);

            // Calculate each colour's pieces for any board instance
            WhitePieces = CalculateActivePieces(_board, Coalition.White);
            BlackPieces = CalculateActivePieces(_board, Coalition.Black);

            // Calculate each colour's legal moves for any board instance
            var whiteLegalMoves = CalculateLegalMoves(WhitePieces);
            var blackLegalMoves = CalculateLegalMoves(BlackPieces);

            // Initialise players
            WhitePlayer = new Player(Coalition.White, this, whiteLegalMoves, blackLegalMoves);
            BlackPlayer = new Player(Coalition.Black, this, blackLegalMoves, whiteLegalMoves);
            CurrentPlayer = boardBuilder.CoalitionToMove.ChoosePlayer(WhitePlayer, BlackPlayer);
        }

        public IEnumerable<Piece> BlackPieces { get; }
        public IEnumerable<Piece> WhitePieces { get; }
        public Player WhitePlayer { get; }
        public Player BlackPlayer { get; }
        public Player CurrentPlayer { get; }

        /// <summary>
        ///     Gets all pieces on the current board.
        /// </summary>
        /// <param name="board">The array of tile data which may contain pieces.</param>
        /// <param name="coalition">Tile state that depicts what colour pieces are returned.</param>
        /// <returns>Returns an iterable collection so that it cannot be modified without casting to list.</returns>
        private IEnumerable<Piece> CalculateActivePieces(Tile[] board, Coalition coalition)
        {
            // TODO: Test LINQ performance
            var pieces = new List<Piece>();

            // Loop through each tile
            foreach (var tile in board)
            {
                // If empty, skip to next tile
                if (!tile.IsOccupied()) continue;
                var piece = tile.Piece;
                // If piece is ally to passed in colour, then add it.
                if (piece.PieceCoalition == coalition) pieces.Add(piece);
            }

            return pieces;
        }

        /// <summary>
        ///     Calculates the legal moves from an enumerable collection of pieces.
        /// </summary>
        /// <param name="pieces">The pieces to iterate over.</param>
        /// <returns>An IEnumerable collection of legal moves generated from each piece passed in.</returns>
        private IEnumerable<Move> CalculateLegalMoves(IEnumerable<Piece> pieces)
        {
            var legalMoves = new List<Move>();
            foreach (var piece in pieces)
            foreach (Move legalMove in piece.GenerateLegalMoves(this))
                legalMoves.Add(legalMove);

            return legalMoves;
        }

        /// <summary>
        ///     Creates the board based on the BoardBuilder configuration.
        /// </summary>
        /// <param name="boardBuilder">The board builder containing the current board configuration.</param>
        /// <returns>A length 64 array of tiles.</returns>
        private static Tile[] InitialiseBoard(BoardBuilder boardBuilder)
        {
            var tiles = new Tile[BoardUtilities.NumTiles];
            // Create board by looping through and setting tile based on board builder config
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
                tiles[i] = Tile.CreateTile(i, boardBuilder.BoardConfiguration[i]);
            return tiles;
        }

        // TODO: Use FEN
        /// <summary>
        ///     Generates the standard board of chess.
        /// </summary>
        /// <returns>Returns a board object containing tiles with pieces at their starting positions.</returns>
        public static Board CreateStandardBoard()
        {
            var boardBuilder = new BoardBuilder();
            boardBuilder.SetPieceAtTile(new Rook(0, Coalition.White));
            boardBuilder.SetPieceAtTile(new Knight(1, Coalition.White));
            boardBuilder.SetPieceAtTile(new Bishop(2, Coalition.White));
            boardBuilder.SetPieceAtTile(new Queen(3, Coalition.White));
            boardBuilder.SetPieceAtTile(new King(4, Coalition.White));
            boardBuilder.SetPieceAtTile(new Bishop(5, Coalition.White));
            boardBuilder.SetPieceAtTile(new Knight(6, Coalition.White));
            boardBuilder.SetPieceAtTile(new Rook(7, Coalition.White));
            for (var i = 8; i < 16; i++) boardBuilder.SetPieceAtTile(new Pawn(i, Coalition.White));

            for (var i = 48; i < 56; i++) boardBuilder.SetPieceAtTile(new Pawn(i, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Rook(56, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Knight(57, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Bishop(58, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Queen(59, Coalition.Black));
            boardBuilder.SetPieceAtTile(new King(60, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Bishop(61, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Knight(62, Coalition.Black));
            boardBuilder.SetPieceAtTile(new Rook(63, Coalition.Black));

            return boardBuilder.BuildBoard();
        }

        /// <summary>
        ///     Gets the tile data at the given tileCoordinate.
        /// </summary>
        /// <param name="tileCoordinate">The tile position on the board.</param>
        /// <returns>Tile data at given position.</returns>
        public Tile GetTile(int tileCoordinate)
        {
            // Return the tile at passed in index
            return _board[tileCoordinate];
        }

        /// <summary>
        ///     Converts the board object to a useful string.
        /// </summary>
        /// <returns>A string of letters and dashed representing the current board layout from white's perspective.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
            {
                // Get the string of the tile at current index
                var tile = _board[i].ToString();

                // Append the tile string with a padding of 3
                stringBuilder.Append(tile.PadRight(3));

                // If the current index + 1 (because of zero offset) mod 8 (tiles per rank) is 0 then create new line
                // This represents a new rank on the board
                if ((i + 1) % BoardUtilities.NumTilesPerRank == 0) stringBuilder.Append('\n');
            }

            // Reverse string so that it is from white's perspective
            var charArray = stringBuilder.ToString().ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}