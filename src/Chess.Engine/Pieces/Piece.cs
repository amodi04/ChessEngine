using System.Collections;
using Engine.BoardRepresentation;
using Engine.BoardRepresentation.TileRepresentation;
using Engine.Enums;
using Engine.Extensions;
using Engine.MoveGeneration;
using Engine.Opposition;

namespace Engine.Pieces
{
    /// <summary>
    ///     This class contains piece data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public abstract class Piece
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
        ///     Checks if there is an enemy piece at a given tile.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if the piece at the tile is enemy. False if ally.</returns>
        protected bool IsEnemyPieceAtTile(Tile tile)
        {
            return PieceCoalition != tile.Piece.PieceCoalition;
        }

        /// <summary>
        ///     Helper method to create a normal move for all pieces.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <returns>A move containing the passed in move data.</returns>
        protected Move CreateNormalMove(Board board, int toCoordinate)
        {
            return new(MoveType.NormalMove, board, PiecePosition,
                toCoordinate, this);
        }

        /// <summary>
        ///     Helper method to create a capture move for all pieces.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <param name="toCoordinate">The destination coordinate of the move.</param>
        /// <param name="capturedPiece">The piece being captured.</param>
        /// <returns>A move containing the passed in move data.</returns>
        protected Move CreateAttackMove(Board board, int toCoordinate, Piece capturedPiece)
        {
            return new(MoveType.CaptureMove, board, PiecePosition, toCoordinate, this, capturedPiece);
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
        public abstract IList GenerateLegalMoves(Board board);

        /// <summary>
        ///     This method moves the piece by utilising passed in move data. Each piece will have it's own implementation.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public abstract Piece MovePiece(Move move);

        /// <summary>
        ///     This method checks for pieces being on certain columns where considerations must be taken into account.
        ///     This is so that the correct destination location is computed and the piece does not go off the board.
        /// </summary>
        /// <param name="currentPosition">The current position of the piece.</param>
        /// <param name="offset">The integer offset that the piece will be moving.</param>
        /// <returns>True if the piece is in a exclusion column. False if not.</returns>
        protected abstract bool IsColumnExclusion(int currentPosition, int offset);
    }
}