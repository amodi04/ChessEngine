using Engine.Enums;
using Engine.Types;
using Engine.Types.Pieces;
using Engine.Util;

namespace Engine.Builders
{
    /// <summary>
    ///     This class enables the building of board and board data.
    /// </summary>
    public class BoardBuilder
    {
        /// <summary>
        ///     Constructor initialises board configuration.
        /// </summary>
        public BoardBuilder()
        {
            BoardConfiguration = new Piece[BoardUtilities.NumTiles];
        }

        // Member fields
        public Piece[] BoardConfiguration { get; }
        public Coalition CoalitionToMove { get; private set; }

        /// <summary>
        ///     Maps a tile position to a piece on the board.
        /// </summary>
        /// <param name="piece">Piece to be mapped.</param>
        /// <returns>A board builder instance containing board configuration. Useful for chaining method calls.</returns>
        public BoardBuilder SetPieceAtTile(Piece piece)
        {
            // Updates board config with a new piece at position
            BoardConfiguration[piece.PiecePosition] = piece;
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
        ///     Builds the chess board.
        ///     This is done bypassing the board builder instance into the board constructor so that the configuration is available
        ///     for usage.
        /// </summary>
        /// <returns>The created board object.</returns>
        public Board BuildBoard()
        {
            // Create a new board, passing in the board builder object to the board constructor.
            return new(this);
        }
    }
}