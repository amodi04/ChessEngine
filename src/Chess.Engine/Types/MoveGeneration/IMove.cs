using Engine.Types.Pieces;

namespace Engine.Types.MoveGeneration
{
    /// <summary>
    ///     The base interface that all move structures will implement with their own logic
    /// </summary>
    public interface IMove
    {
        /// <summary>
        ///     Must have struct value types
        /// </summary>
        public Board Board { get; }

        public int FromCoordinate { get; }
        public int ToCoordinate { get; }
        public Piece MovedPiece { get; }

        /// <summary>
        ///     Creates a new board with the moved piece.
        /// </summary>
        /// <returns>A new board with the piece moved.</returns>
        public Board ExecuteMove();
    }
}