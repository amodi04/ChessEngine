using System.Collections.Generic;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.BoardRepresentation
{
    /// <summary>
    /// This class enables the building of board and board data.
    /// </summary>
    public class BoardBuilder
    {
        // Member fields
        public Dictionary<int, Piece> BoardConfiguration { get; }
        public Coalition CoalitionToMove { get; private set; }
        
        /// <summary>
        /// Constructor initialises board configuration.
        /// </summary>
        public BoardBuilder()
        {
            BoardConfiguration = new Dictionary<int, Piece>();
        }

        /// <summary>
        /// Maps a tile position to a piece on the board.
        /// </summary>
        /// <param name="piece">Piece to be mapped.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetPieceAtTile(Piece piece)
        {
            BoardConfiguration[piece.PiecePosition] = piece;
            return this;
        }

        /// <summary>
        /// Sets the next coalition(colour) to move.
        /// </summary>
        /// <param name="coalitionToMove">The coalition that will move next.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetCoalitionToMove(Coalition coalitionToMove)
        {
            CoalitionToMove = coalitionToMove;
            return this;
        }

        /// <summary>
        /// Builds the chess board.
        /// This is done bypassing the board builder instance into the board constructor so that the configuration is available for usage.
        /// </summary>
        /// <returns>The created board object.</returns>
        public Board BuildBoard()
        {
            return new Board(this);
        }
    }
}