using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Enums;
using Engine.Extensions;
using Engine.Types.MoveGeneration;

namespace Engine.Types.Pieces
{
    /// <summary>
    ///     This class contains piece data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public abstract class Piece : IEquatable<Piece>
    {
        /// <summary>
        ///     Constructor to create a piece. This class is abstract so this constructor can only be called from inheriting
        ///     classes.
        /// </summary>
        /// <param name="pieceType">The type of piece to create.</param>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        protected Piece(PieceType pieceType, int piecePosition, Coalition pieceCoalition)
        {
            PieceType = pieceType;
            PiecePosition = piecePosition;
            PieceCoalition = pieceCoalition;
        }

        // Member fields
        public PieceType PieceType { get; }
        public int PiecePosition { get; }
        public Coalition PieceCoalition { get; }
        public bool IsFirstMove { get; }

        /// <summary>
        ///     IEquatable Implementation of Equals.
        /// </summary>
        /// <param name="other">The Piece object to compare to.</param>
        /// <returns>True if equal, false if not.</returns>
        public bool Equals(Piece other)
        {
            // Referential equality checks
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Return true if all value types are equal
            return PieceType == other.PieceType && PiecePosition == other.PiecePosition &&
                   PieceCoalition == other.PieceCoalition && IsFirstMove == other.IsFirstMove;
        }

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
        /// <returns>A letter which is uppercase if white or lowercase if black. The letters are defined in PieceTypeExtension.</returns>
        public override string ToString()
        {
            return PieceType.ToAbbreviation(PieceCoalition);
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

        /// <summary>
        ///     Checks if two objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>True if equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            // Referential equality checks
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // If both types are equal and they are equal then return true
            return obj.GetType() == GetType() && Equals((Piece) obj);
        }

        /// <summary>
        ///     Gets the hash code of the current move struct in memory.
        /// </summary>
        /// <returns>The hash code combination of all value types within the struct.</returns>
        public override int GetHashCode()
        {
            // Combine hash codes of all value types
            return HashCode.Combine((int) PieceType, PiecePosition, (int) PieceCoalition, IsFirstMove);
        }

        /// <summary>
        ///     Shorthand operator for equal comparison.
        /// </summary>
        /// <param name="left">The object to compare.</param>
        /// <param name="right">The object to compare against.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool operator ==(Piece left, Piece right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Shorthand operator for not equal comparison.
        /// </summary>
        /// <param name="left">The object to compare.</param>
        /// <param name="right">The object to compare against.</param>
        /// <returns>True if not equal, false if equal.</returns>
        public static bool operator !=(Piece left, Piece right)
        {
            return !Equals(left, right);
        }
    }
}