using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine.Enums;
using Engine.Extensions;
using Engine.Types.MoveGeneration;
using Engine.Types.Pieces;
using Engine.Util;

namespace Engine.Types
{
    /// <summary>
    ///     This class contains player data and functions that the chess player will utilise to play the game.
    /// </summary>
    public class Player
    {
        // Member fields
        protected readonly Board Board;
        private readonly bool _isInCheck;

        /// <summary>
        ///     Constructor to create a player
        /// </summary>
        /// <param name="coalition">The coalition that the player will be controlling.</param>
        /// <param name="board">The current board in play.</param>
        /// <param name="moves">An iterable collection of moves available to the player.</param>
        /// <param name="opponentMoves">An iterable collection of opponent moves available to the player.</param>
        public Player(Coalition coalition, Board board, IEnumerable<IMove> moves, IEnumerable<IMove> opponentMoves)
        {
            Coalition = coalition;

            // Set the player type depending on if the player is white or black
            PlayerType = coalition.IsWhite() ? BoardUtilities.WhitePlayerType : BoardUtilities.BlackPlayerType;
            
            Board = board;
            King = GetKingOnBoard();

            // Store as array to reduce multiple enumerations
            var opponentMovesArray = opponentMoves as IMove[] ?? opponentMoves.ToArray();

            Moves = moves.Concat(ComputeCastleMoves(opponentMovesArray)).ToList();
            // TODO: Check LINQ performance
            // True if there are any elements in the collection
            _isInCheck = CalculateAttacksOnTile(King.PiecePosition, opponentMovesArray).Any();
        }

        public King King { get; }
        public ICollection<IMove> Moves { get; }
        public Coalition Coalition { get; }
        public PlayerType PlayerType { get; }

        /// <summary>
        ///     Gets all the moves that are attacking the tile passed in.
        /// </summary>
        /// <param name="tilePosition">The tile to check.</param>
        /// <param name="opponentMoves">All the possible moves that the opponent can make.</param>
        /// <returns>An iterable collection of attacking moves on the tile.</returns>
        private static IEnumerable<IMove> CalculateAttacksOnTile(int tilePosition, IEnumerable<IMove> opponentMoves)
        {
            var attackMoves = new List<IMove>();
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
                    return (King) activePiece;

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
        public BoardTransition MakeMove(IMove move)
        {
            // If the move is illegal, return the current board data.
            if (!IsMoveLegal(move)) return new BoardTransition(Board, Board, move, MoveStatus.Illegal);

            // Make the move
            var toBoard = move.ExecuteMove();

            // If there are any attacking moves on the king, return the current board as the move made would leave the
            // player in check. Otherwise return the new board because the move is valid.
            return toBoard.CurrentPlayer.GetOpponent().IsInCheck()
                ? new BoardTransition(Board, Board, move, MoveStatus.PlayerInCheck)
                : new BoardTransition(Board, toBoard, move, MoveStatus.Done);
        }

        /// <summary>
        ///     Checks whether a move is legal or not.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <returns>True if legal, false otherwise.</returns>
        private bool IsMoveLegal(IMove move)
        {
            // TODO: Check LINQ performance
            // True if the member field Moves contains the move passed in
            // All moves in the member field are legal
            foreach (var potentialMove in Moves)
            {
                if (potentialMove.FromCoordinate == move.FromCoordinate &&
                    potentialMove.ToCoordinate == move.ToCoordinate)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets the current active allied piece
        /// </summary>
        /// <returns>Active white pieces if the player is white and active black pieces if the player is black.</returns>
        public IEnumerable<Piece> GetActiveAlliedPieces()
        {
            // If white return white pieces, else return black pieces
            return Coalition.IsWhite() ? Board.WhitePieces : Board.BlackPieces;
        }

        /// <summary>
        ///     Gets the player's opponent.
        /// </summary>
        /// <returns>If player is white, get the black player, otherwise get the white player.</returns>
        public Player GetOpponent()
        {
            // If white return black, else white,
            return Coalition.IsWhite() ? Board.BlackPlayer : Board.WhitePlayer;
        }

        /// <summary>
        ///     Calculates the potential castling moves for the player.
        /// </summary>
        /// <param name="playerMoves">The moves that the player can make.</param>
        /// <param name="opponentMoves">The moves that the opponent player can make.</param>
        /// <returns>An IEnumerable of castling moves.</returns>
        private IEnumerable<IMove> ComputeCastleMoves(IEnumerable<IMove> opponentMoves)
        {
            // Initialise list
            var castleMoves = new List<IMove>();

            // Set the king position depending on if the player is white or black
            var kingPosition = Coalition.IsWhite() ? 4 : 60;

            // If the king has moved or the king is in check, no castling moves can be made so an empty list is returned
            if (!King.IsFirstMove || _isInCheck) return castleMoves;

            // Convert the IEnumerable to an array to reduce multiple enumerations
            var opponentMovesArray = opponentMoves as IMove[] ?? opponentMoves.ToArray();

            // King side castle
            // If the adjacent tile next to the king is empty and the 2nd consecutive adjacent tile is empty
            if (!Board.GetTile(kingPosition + 1).IsOccupied() && !Board.GetTile(kingPosition + 2).IsOccupied())
            {
                // Get the rook on the king side. Position is dependent on coalition
                var rookTile = Board.GetTile(Coalition.IsWhite() ? 7 : 63);

                // If the rook tile is occupied and it is the pieces first move
                if (rookTile.IsOccupied() && rookTile.Piece.IsFirstMove)
                    // If there are no opponent attacks on the empty tiles between the rook and the king
                    // and the piece is a rook
                    if (!CalculateAttacksOnTile(kingPosition + 1, opponentMovesArray).Any() &&
                        !CalculateAttacksOnTile(kingPosition + 2, opponentMovesArray).Any() &&
                        rookTile.Piece.PieceType == PieceType.Rook)
                        // Add a king side castling move
                        castleMoves.Add(new CastlingMove(Board,
                            kingPosition,
                            kingPosition + 2,
                            King,
                            (Rook) rookTile.Piece,
                            rookTile.TileCoordinate,
                            kingPosition + 1));
            }

            // Queen side castle
            // If the adjacent tile next to the king is occupied or the 2nd consecutive adjacent tile is occupied or
            // the 3rd consecutive adjacent tile is occupied,
            // return the current list of castling moves because queen side castling is invalid here.
            if (Board.GetTile(kingPosition - 1).IsOccupied() || Board.GetTile(kingPosition - 2).IsOccupied() ||
                Board.GetTile(kingPosition - 3).IsOccupied()) return castleMoves;
            {
                // Get the rook on the queen side. Position is dependent on coalition
                // TODO: Change hard coding rooks
                var rookTile = Board.GetTile(Coalition.IsWhite() ? 0 : 56);

                // If the rook tile is not occupied or the rook has moved, castling is invalid so return the calculated 
                // moves so far.
                if (!rookTile.IsOccupied() || !rookTile.Piece.IsFirstMove) return castleMoves;

                // If there are no opponent attacks on the empty tiles between the rook and the king
                // and the piece is a rook
                if (!CalculateAttacksOnTile(kingPosition - 1, opponentMovesArray).Any() &&
                    !CalculateAttacksOnTile(kingPosition - 2, opponentMovesArray).Any() &&
                    !CalculateAttacksOnTile(kingPosition - 3, opponentMovesArray).Any() &&
                    rookTile.Piece.PieceType == PieceType.Rook)
                    // Add a queen side castling move
                    castleMoves.Add(new CastlingMove(Board,
                        kingPosition,
                        kingPosition - 2,
                        King,
                        (Rook) rookTile.Piece,
                        rookTile.TileCoordinate,
                        kingPosition - 1));
            }

            return castleMoves;
        }
    }
}