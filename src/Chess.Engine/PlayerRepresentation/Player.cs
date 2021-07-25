using System;
using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.PlayerRepresentation
{
    /// <summary>
    /// This class contains player data and functions that the chess player will utilise to play the game.
    /// </summary>
    public class Player
    {
        // Member fields
        private readonly Board _board;
        private King _king;
        private IEnumerable<Move> _moves;
        private readonly Coalition _coalition;

        /// <summary>
        /// Constructor to create a player
        /// </summary>
        /// <param name="coalition">The coalition that the player will be controlling.</param>
        /// <param name="board">The current board in play.</param>
        /// <param name="moves">An iterable collection of moves available to the player.</param>
        /// <param name="opponentMoves">An iterable collection of opponent moves available to the player.</param>
        public Player(Coalition coalition, Board board, IEnumerable<Move> moves, IEnumerable<Move> opponentMoves)
        {
            _coalition = coalition;
            _board = board;
            _king = GetKingOnBoard();
            _moves = moves;
        }

        /// <summary>
        /// Gets the player king on the board.
        /// </summary>
        /// <returns>The king of the same coalition as the player.</returns>
        /// <exception cref="Exception">Called if no king is found which should never happen.</exception>
        private King GetKingOnBoard()
        {
            // Iterate through each allied piece
            foreach (Piece activePiece in GetActiveAlliedPieces())
            {
                // If the piece is a king, return it.
                if (activePiece.PieceType == PieceType.King)
                {
                    return (King) activePiece;
                }
            }

            // No king is found
            throw new Exception("Not a valid board because the player must always have a king!");
        }

        /// <summary>
        /// Gets the current active allied piece
        /// </summary>
        /// <returns>Active white pieces if the player is white and active black pieces if the player is black.</returns>
        private IEnumerable<Piece> GetActiveAlliedPieces()
        {
            return _coalition.IsWhite() ? _board.WhitePieces : _board.BlackPieces;
        }
    }
}