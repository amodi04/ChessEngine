using Engine.Enums;
using Engine.Types.MoveGeneration;

namespace Engine.Types
{
    /// <summary>
    ///     This struct holds data for moving between immutable boards.
    /// </summary>
    public readonly struct BoardTransition
    {
        // Member fields
        private Board FromBoard { get; }
        public Board ToBoard { get; }
        public IMove TransitionMove { get; }
        public MoveStatus Status { get; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="fromBoard">The previous board to move from.</param>
        /// <param name="toBoard">The board to move to.</param>
        /// <param name="transitionMove">The move that causes the change in board.</param>
        /// <param name="moveStatus">The status of the move.</param>
        public BoardTransition(Board fromBoard, Board toBoard, IMove transitionMove, MoveStatus moveStatus)
        {
            FromBoard = fromBoard;
            ToBoard = toBoard;
            TransitionMove = transitionMove;
            Status = moveStatus;
        }
    }
}