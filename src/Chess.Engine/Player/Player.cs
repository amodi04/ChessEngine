using System;
using System.Collections.Generic;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;

namespace Engine.Player
{
    /// <summary>
    ///     This class contains player data and functions that the chess player will utilise to play the game.
    /// </summary>
    public class Player
    {
        private readonly Board _board;
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
            PlayerType = coalition.IsWhite() ? BoardUtilities.WhitePlayerType : BoardUtilities.BlackPlayerType;
            _board = board;
            King = GetKingOnBoard();
            _isInCheck = CalculateAttacksOnTile(King.PiecePosition, opponentMoves).Any();
            Moves = moves.Concat(ComputeCastleMoves(opponentMoves)).ToList();
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
            foreach (var activePiece in GetActiveAlliedPieces())
                if (activePiece.Type == PieceType.King)
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
            foreach (var move in Moves)
            {
                var transition = MakeMove(move);
                if (transition.Status == MoveStatus.Done) return true;
            }
            
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
            if (!IsMoveLegal(move)) return new BoardTransition(_board, _board, MoveStatus.Illegal);
            
            var toBoard = move.ExecuteMove();

            // If there are any attacking moves on the king, return the current board as the move made would leave the
            // player in check. Otherwise return the new board because the move is valid.
            return toBoard.CurrentPlayer.GetOpponent().IsInCheck()
                ? new BoardTransition(_board, _board, MoveStatus.PlayerInCheck)
                : new BoardTransition(_board, toBoard, MoveStatus.Done);
        }

        /// <summary>
        ///     Checks whether a move is legal or not.
        /// </summary>
        /// <param name="move">The move to check.</param>
        /// <returns>True if legal, false otherwise.</returns>
        private bool IsMoveLegal(IMove move)
        {
            // Move is identified by to and from coordinates. True if found
            foreach (var potentialMove in Moves)
                if (potentialMove.FromCoordinate == move.FromCoordinate &&
                    potentialMove.ToCoordinate == move.ToCoordinate)
                    return true;

            return false;
        }

        /// <summary>
        ///     Gets the current active allied piece
        /// </summary>
        /// <returns>Active white pieces if the player is white and active black pieces if the player is black.</returns>
        public IEnumerable<Piece> GetActiveAlliedPieces()
        {
            return Coalition.IsWhite() ? _board.WhitePieces : _board.BlackPieces;
        }

        /// <summary>
        ///     Gets the player's opponent.
        /// </summary>
        /// <returns>If player is white, get the black player, otherwise get the white player.</returns>
        public Player GetOpponent()
        {
            return Coalition.IsWhite() ? _board.BlackPlayer : _board.WhitePlayer;
        }

        /// <summary>
        ///     Calculates the potential castling moves for the player.
        /// </summary>
        /// <param name="opponentMoves">The moves that the opponent player can make.</param>
        /// <returns>An IEnumerable of castling moves.</returns>
        private IEnumerable<IMove> ComputeCastleMoves(IEnumerable<IMove> opponentMoves)
        {
            var castleMoves = new List<IMove>();

            // Set the king position dependent on coalition
            var kingPosition = Coalition.IsWhite() ? 4 : 60;
            
            if (!HasCastlingRights()) return Array.Empty<IMove>();
            
            // Enumerate to array so multiple enumerations are avoided (array accesses are far quicker)
            var opponentMovesArray = opponentMoves as IMove[] ?? opponentMoves.ToArray();

            // King side castle
            
            // If the adjacent tile next to the king is empty and the 2nd consecutive adjacent tile is empty
            if (!_board.GetTile(kingPosition + 1).IsOccupied() && !_board.GetTile(kingPosition + 2).IsOccupied())
            {
                var rookTile = _board.GetTile(Coalition.IsWhite() ? 7 : 63);
                if (rookTile.IsOccupied() && rookTile.Piece.IsFirstMove)
                    // If there are no opponent attacks on the empty tiles between the rook and the king
                    if (!CalculateAttacksOnTile(kingPosition + 1, opponentMovesArray).Any() &&
                        !CalculateAttacksOnTile(kingPosition + 2, opponentMovesArray).Any() &&
                        rookTile.Piece.Type == PieceType.Rook)
                        castleMoves.Add(new CastlingMove(_board, kingPosition, kingPosition + 2,
                            King, (Rook) rookTile.Piece, rookTile.TileCoordinate,
                            kingPosition + 1));
            }

            // Queen side castle
            
            // Return the current list of castling moves because queen side castling is invalid
            if (_board.GetTile(kingPosition - 1).IsOccupied() || _board.GetTile(kingPosition - 2).IsOccupied() ||
                _board.GetTile(kingPosition - 3).IsOccupied()) return castleMoves;
            {
                var rookTile = _board.GetTile(Coalition.IsWhite() ? 0 : 56);

                // Castling is invalid here, return the calculated moves so far.
                if (!rookTile.IsOccupied() || !rookTile.Piece.IsFirstMove) return castleMoves;

                // If there are no opponent attacks on the empty tiles between the rook and the king
                if (!CalculateAttacksOnTile(kingPosition - 1, opponentMovesArray).Any() &&
                    !CalculateAttacksOnTile(kingPosition - 2, opponentMovesArray).Any() &&
                    !CalculateAttacksOnTile(kingPosition - 3, opponentMovesArray).Any() &&
                    rookTile.Piece.Type == PieceType.Rook)
                    castleMoves.Add(new CastlingMove(_board,
                        kingPosition,
                        kingPosition - 2,
                        King,
                        (Rook) rookTile.Piece,
                        rookTile.TileCoordinate,
                        kingPosition - 1));
            }

            return castleMoves;
        }

        /// <summary>
        ///     Checks if the king can castle.
        /// </summary>
        /// <returns>True if the king can castle false if not.</returns>
        private bool HasCastlingRights()
        {
            return !_isInCheck && !King.IsCastled && (King.KingsideCastleCapable || King.QueensideCastleCapable);
        }

        public override string ToString()
        {
            return Coalition.IsWhite() ? "White" : "Black";
        }
    }
}