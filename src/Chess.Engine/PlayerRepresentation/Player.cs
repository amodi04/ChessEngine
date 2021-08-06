﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.Enums;
using Engine.Extensions;
using Engine.MoveGeneration;
using Engine.Opposition;
using Engine.Pieces;

namespace Engine.PlayerRepresentation
{
    /// <summary>
    ///     This class contains player data and functions that the chess player will utilise to play the game.
    /// </summary>
    public class Player
    {
        // Member fields
        private readonly Board _board;
        private readonly bool _isInCheck;

        /// <summary>
        ///     Constructor to create a player
        /// </summary>
        /// <param name="coalition">The coalition that the player will be controlling.</param>
        /// <param name="board">The current board in play.</param>
        /// <param name="moves">An iterable collection of moves available to the player.</param>
        /// <param name="opponentMoves">An iterable collection of opponent moves available to the player.</param>
        public Player(Coalition coalition, Board board, IEnumerable<Move> moves, IEnumerable<Move> opponentMoves)
        {
            Coalition = coalition;
            _board = board;
            King = GetKingOnBoard();
            Moves = moves;
            // TODO: Check LINQ performance
            // True if there are any elements in the collection
            _isInCheck = CalculateAttacksOnTile(King.PiecePosition, opponentMoves).Any();
        }

        public King King { get; }
        public IEnumerable<Move> Moves { get; }
        public Coalition Coalition { get; }

        /// <summary>
        ///     Gets all the moves that are attacking the tile passed in.
        /// </summary>
        /// <param name="tilePosition">The tile to check.</param>
        /// <param name="opponentMoves">All the possible moves that the opponent can make.</param>
        /// <returns>An iterable collection of attacking moves on the tile.</returns>
        private IEnumerable<Move> CalculateAttacksOnTile(int tilePosition, IEnumerable<Move> opponentMoves)
        {
            var attackMoves = new List<Move>();
            foreach (var opponentMove in opponentMoves)
                if (tilePosition == opponentMove.ToCoordinate)
                    attackMoves.Add(opponentMove);

            return attackMoves;
        }

        /// <summary>
        ///     Gets the player king on the board.
        /// </summary>
        /// <returns>The king of the same coalition as the player.</returns>
        /// <exception cref="Exception">Called if no king is found which should never happen.</exception>
        private King GetKingOnBoard()
        {
            // Iterate through each allied piece
            foreach (var activePiece in GetActiveAlliedPieces())
                // If the piece is a king, return it.
                if (activePiece.PieceType == PieceType.King)
                {
                    return (King) activePiece;
                }
            
            // No king is found
            throw new Exception("Not a valid board because the player must always have a king!");
        }

        /// <summary>
        ///     Checks whether the player is in check.
        /// </summary>
        /// <returns>True if the king is in check, false otherwise.</returns>
        public bool IsInCheck()
        {
            return _isInCheck;
        }

        /// <summary>
        ///     Checks whether the player is in checkmate.
        /// </summary>
        /// <returns>True if the king is in check and there are no possible escape moves to be made.</returns>
        public bool IsInCheckmate()
        {
            return _isInCheck && !HasEscapeMoves();
        }

        /// <summary>
        ///     Checks whether the player is in stalemate
        /// </summary>
        /// <returns>True if the king is not in check and there are no possible escape moves to be made.</returns>
        public bool IsInStalemate()
        {
            return !_isInCheck && !HasEscapeMoves();
        }

        /// <summary>
        ///     Checks whether the player has escape moves for the king.
        /// </summary>
        /// <returns>True if there are available moves, false otherwise.</returns>
        private bool HasEscapeMoves()
        {
            // Loop through each available move
            foreach (var move in Moves)
            {
                // Make the move
                var transition = MakeMove(move);
                // If the move completed successfully, return true
                if (transition.Status == MoveStatus.Done) return true;
            }

            // Return false because no moves completed successfully
            return false;
        }

        /// <summary>
        ///     Gets the board transition data to be passed between boards.
        /// </summary>
        /// <param name="move">The move to be made.</param>
        /// <returns>A board transition struct containing relevant data for the next board.</returns>
        private BoardTransition MakeMove(Move move)
        {
            // If the move is illegal, return the current board data.
            if (!IsMoveLegal(move)) return new BoardTransition(_board, _board, move, MoveStatus.Illegal);

            // Make the move
            var toBoard = move.ExecuteMove();
            // Calculate all attacking moves on the current player king
            var attacksOnKing =
                CalculateAttacksOnTile(toBoard.CurrentPlayer.King.PiecePosition, toBoard.CurrentPlayer.Moves);

            // If there are any attacking moves on the king, return the current board as the move made would leave the
            // player in check. Otherwise return the new board because the move is valid.
            return !attacksOnKing.Any()
                ? new BoardTransition(_board, _board, move, MoveStatus.PlayerInCheck)
                : new BoardTransition(_board, toBoard, move, MoveStatus.Done);
        }

        /// <summary>
        ///     Checks whether a move is legal or not.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <returns>True if legal, false otherwise.</returns>
        private bool IsMoveLegal(Move move)
        {
            // TODO: Check LINQ performance
            // True if the member field Moves contains the move passed in
            // All moves in the member field are legal
            return Moves.Contains(move);
        }

        /// <summary>
        ///     Gets the current active allied piece
        /// </summary>
        /// <returns>Active white pieces if the player is white and active black pieces if the player is black.</returns>
        public IEnumerable<Piece> GetActiveAlliedPieces()
        {
            // If white return white pieces, else return black pieces
            return Coalition.IsWhite() ? _board.WhitePieces : _board.BlackPieces;
        }

        public Player GetOpponent()
        {
            // If white return black, else white,
            return Coalition.IsWhite() ? _board.BlackPlayer : _board.WhitePlayer;
        }
    }
}