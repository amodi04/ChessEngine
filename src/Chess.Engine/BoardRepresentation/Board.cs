using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            
            WhitePieces = CalculateActivePieces(_board, Coalition.White);
            BlackPieces = CalculateActivePieces(_board, Coalition.Black);
            AllPieces = WhitePieces.Concat(BlackPieces);
            
            EnPassantPawn = boardBuilder.EnPassantPawn;
            
            var whiteMoves = CalculateLegalMoves(WhitePieces);
            var blackMoves = CalculateLegalMoves(BlackPieces);
            
            WhitePlayer = new Player.Player(Coalition.White, this, whiteMoves, blackMoves);
            BlackPlayer = new Player.Player(Coalition.Black, this, blackMoves, whiteMoves);
            AllMoves = WhitePlayer.Moves.Concat(BlackPlayer.Moves);
            CurrentPlayer = boardBuilder.CoalitionToMove.ChoosePlayer(WhitePlayer, BlackPlayer);
            
            PlyCount = boardBuilder.PlyCount;

            // Hash board state and store with board
            Zobristkey = Zobrist.Hash(this);
        }

        public IEnumerable<Piece> WhitePieces { get; }
        public IEnumerable<Piece> BlackPieces { get; }
        public IEnumerable<Piece> AllPieces { get; }
        public Player.Player WhitePlayer { get; }
        public Player.Player BlackPlayer { get; }
        public Player.Player CurrentPlayer { get; }
        public IEnumerable<IMove> AllMoves { get; }
        public Pawn EnPassantPawn { get; }
        public int PlyCount { get; }
        public ulong Zobristkey { get; }

        /// <summary>
        ///     Gets all pieces on the current board.
        /// </summary>
        /// <param name="board">The array of tile data which may contain pieces.</param>
        /// <param name="coalition">Tile state that depicts what colour pieces are returned.</param>
        /// <returns>Returns an iterable collection so that it cannot be modified without casting to list.</returns>
        private static IEnumerable<Piece> CalculateActivePieces(IEnumerable<Tile> board, Coalition coalition)
        {
            var pieces = new List<Piece>();
            
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
            
            // Foreach piece, generate their moves and add it to the list of legal moves
            foreach (var piece in pieces)
                foreach (var legalMove in piece.GenerateLegalMoves(this))
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
            
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
                // Create tile at index based on board builder configuration
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
        ///     Creates a board from a custom fen string.
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
                // Get the string of the tile at current index (reflected so that a1 starts bottom left)
                var tile = _board[BoardUtilities.ReflectBoard[i]].ToString();
                
                stringBuilder.Append(tile.PadRight(3));

                // If the current index + 1 (because of zero offset) mod 8 (tiles per rank) is 0 then create new line
                // This represents a new rank on the board
                if ((i + 1) % BoardUtilities.NumTilesPerRank == 0) stringBuilder.Append('\n');
            }

            // Reverse string so that it is from white's perspective
            var charArray = stringBuilder.ToString().ToCharArray();
            Array.Reverse(charArray);
            
            // Return string representation of board
            return new string(charArray);
        }
    }
}