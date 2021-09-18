namespace Engine.Types.AI
{
    /// <summary>
    /// Base interface that all evaluators will inherit from.
    /// </summary>
    public interface IEvaluator
    {
        /// <summary>
        /// Evaluates a board.
        /// </summary>
        /// <param name="board">The board to evaluate.</param>
        /// <returns>An integer score for the value of the board.</returns>
        int Evaluate(Board board, int depth);
    }
}