using Engine.MoveGeneration;

namespace Engine.BoardRepresentation
{
    /// <summary>
    ///     This struct holds data for moving between immutable boards.
    /// </summary>
    public readonly struct BoardTransition
    {
        // Member fields
        public Board FromBoard { get; }
        public Board ToBoard { get; }
        public MoveStatus Status { get; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="fromBoard">The previous board to move from.</param>
        /// <param name="toBoard">The board to move to.</param>
        /// <param name="moveStatus">The status of the move.</param>
        public BoardTransition(Board fromBoard, Board toBoard, MoveStatus moveStatus)
        {
            FromBoard = fromBoard;
            ToBoard = toBoard;
            Status = moveStatus;
        }
    }
}