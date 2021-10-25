using System;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;

namespace Engine.Pieces
{
    /// <summary>
    ///     This class contains piece data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public abstract class Piece : IEquatable<Piece>
    {
        /// <summary>
        ///     Constructor to create a piece.
        /// </summary>
        /// <param name="type">The type of piece to create.</param>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="isFirstMove">Sets whether this is the pieces first move.</param>
        protected Piece(PieceType type, int piecePosition, Coalition pieceCoalition, bool isFirstMove)
        {
            Type = type;
            PiecePosition = piecePosition;
            PieceCoalition = pieceCoalition;
            IsFirstMove = isFirstMove;
        }
        
        public PieceType Type { get; }
        public int PiecePosition { get; }
        public Coalition PieceCoalition { get; }
        public bool IsFirstMove { get; }

        /// <summary>
        ///     Checks if there is an enemy piece at a given tile.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if the piece at the tile is enemy. False if ally.</returns>
        protected bool IsEnemyPieceAtTile(Tile tile)
        {
            return PieceCoalition != tile.Piece.PieceCoalition;
        }

        /// <summary>
        ///     Gets the string representation of the piece.
        /// </summary>
        /// <returns>A single character representing the piece type.</returns>
        public override string ToString()
        {
            return Type.ToAbbreviation(PieceCoalition);
        }

        /// <summary>
        ///     This method generates the legal moves for the piece, given the board.
        ///     Each piece will have it's own implementation.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <returns>An IList of moves that can be made.</returns>
        public abstract IEnumerable<IMove> GenerateLegalMoves(Board board);

        /// <summary>
        ///     This method moves the piece by utilising passed in move data. Each piece will have it's own implementation.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public abstract Piece MovePiece(IMove move);
        
        public bool Equals(Piece other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return Type == other.Type && PiecePosition == other.PiecePosition &&
                   PieceCoalition == other.PieceCoalition && IsFirstMove == other.IsFirstMove;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return obj.GetType() == GetType() && Equals((Piece) obj);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine((int) Type, PiecePosition, (int) PieceCoalition, IsFirstMove);
        }
        
        public static bool operator ==(Piece left, Piece right)
        {
            return Equals(left, right);
        }
        
        public static bool operator !=(Piece left, Piece? right)
        {
            return !Equals(left, right);
        }
    }
}