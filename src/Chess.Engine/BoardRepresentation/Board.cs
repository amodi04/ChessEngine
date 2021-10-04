﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.IO;
using Engine.MoveGeneration;
using Engine.Pieces;
using Engine.Player;

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
            AllPieces = WhitePieces.Concat(BlackPieces);

            // Get the en passant pawn from the board builder and set it internally
            EnPassantPawn = boardBuilder.EnPassantPawn;
            
            // Calculate each colour's legal moves for any board instance
            var whiteMoves = CalculateLegalMoves(WhitePieces);
            var blackMoves = CalculateLegalMoves(BlackPieces);

            // Initialise players
            WhitePlayer = new Player.Player(Coalition.White, this, whiteMoves, blackMoves);
            BlackPlayer = new Player.Player(Coalition.Black, this, blackMoves, whiteMoves);
            
            // Store all possible moves for the current board
            AllMoves = WhitePlayer.Moves.Concat(BlackPlayer.Moves);
            
            // Set the current player via the board builder
            CurrentPlayer = boardBuilder.CoalitionToMove.ChoosePlayer(WhitePlayer, BlackPlayer);
        }

        // Properties
        public IEnumerable<Piece> BlackPieces { get; }
        public IEnumerable<Piece> WhitePieces { get; }
        public IEnumerable<Piece> AllPieces { get; }
        public Player.Player WhitePlayer { get; }
        public Player.Player BlackPlayer { get; }
        public Player.Player CurrentPlayer { get; }
        public IEnumerable<IMove> AllMoves { get; }
        public Pawn EnPassantPawn { get; }

        /// <summary>
        ///     Gets all pieces on the current board.
        /// </summary>
        /// <param name="board">The array of tile data which may contain pieces.</param>
        /// <param name="coalition">Tile state that depicts what colour pieces are returned.</param>
        /// <returns>Returns an iterable collection so that it cannot be modified without casting to list.</returns>
        private static IEnumerable<Piece> CalculateActivePieces(IEnumerable<Tile> board, Coalition coalition)
        {
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
        private IEnumerable<IMove> CalculateLegalMoves(IEnumerable<Piece> pieces)
        {
            var legalMoves = new List<IMove>();
            
            // Loop through all pieces
            foreach (var piece in pieces)
                
                // Loop through each move the piece can make
                foreach (var legalMove in piece.GenerateLegalMoves(this))
                    
                    // Add it to the list
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
            
            // Create board by looping through and setting tile based on board builder configuration
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
                tiles[i] = Tile.CreateTile(i, boardBuilder.BoardConfiguration[i]);
            return tiles;
        }
        
        /// <summary>
        ///     Generates the standard board of chess.
        /// </summary>
        /// <returns>Returns a board object containing tiles with pieces at their starting positions.</returns>
        public static Board CreateStandardBoard()
        {
            return FenParser.PositionFromFen(FenParser.StartFen);
        }

        /// <summary>
        /// Creates a board from a custom fen string.
        /// </summary>
        /// <param name="fen">The fen string to use.</param>
        /// <returns>A board parsed from the fen string.</returns>
        public static Board CreateBoardFromFen(string fen)
        { 
            return FenParser.PositionFromFen(fen);
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
        ///     Converts the board object to a useful string for debugging purposes.
        /// </summary>
        /// <returns>A string of letters and dashed representing the current board layout from white's perspective.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
            {
                // Get the string of the tile at current index
                var tile = _board[BoardUtilities.ReflectBoard[i]].ToString();

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