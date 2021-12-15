using Engine.Pieces;
using Engine.Player;

namespace Engine.BoardRepresentation
{
    /// <summary>
    ///     This class enables the building of board and board data.
    /// </summary>
    public class BoardBuilder
    {
        public BoardBuilder()
        {
            BoardConfiguration = new Piece[BoardUtilities.NumTiles];
        }
        
        public Piece?[] BoardConfiguration { get; }
        public Coalition CoalitionToMove { get; private set; }
        public Pawn? EnPassantPawn { get; private set; }
        public int PlyCount { get; private set; }

        /// <summary>
        ///     Maps a tile position to a piece on the board.
        /// </summary>
        /// <param name="piece">Piece to be mapped.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetPieceAtTile(Piece piece)
        {
            BoardConfiguration[piece.PiecePosition] = piece;
            return this;
        }

        /// <summary>
        ///     Removes a piece from the board.
        /// </summary>
        /// <param name="piece">The piece to remove.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder RemovePieceAtTile(Piece piece)
        {
            BoardConfiguration[piece.PiecePosition] = null;
            return this;
        }

        /// <summary>
        ///     Sets the next coalition(colour) to move.
        /// </summary>
        /// <param name="coalitionToMove">The coalition that will move next.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetCoalitionToMove(Coalition coalitionToMove)
        {
            CoalitionToMove = coalitionToMove;
            return this;
        }

        /// <summary>
        ///     Sets the current en passant pawn
        /// </summary>
        /// <param name="enPassantPawn">The pawn to set.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetEnPassantPawn(Pawn enPassantPawn)
        {
            EnPassantPawn = enPassantPawn;
            return this;
        }

        /// <summary>
        ///     Sets the number of ply's played so far.
        /// </summary>
        /// <param name="plyCount">The number of ply's to set</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetPlyCount(int plyCount)
        {
            PlyCount = plyCount;
            return this;
        }

        /// <summary>
        ///     Builds the chess board.
        ///     This is done bypassing the board builder instance into the board constructor so that the configuration is available
        ///     for usage.
        /// </summary>
        /// <returns>The created board object.</returns>
        public Board BuildBoard()
        {
            return new Board(this);
        }
    }
}