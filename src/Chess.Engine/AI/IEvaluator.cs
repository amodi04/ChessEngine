﻿using Engine.BoardRepresentation;

namespace Engine.AI;

/// <summary>
///     Base interface that all evaluators will implement.
/// </summary>
public interface IEvaluator
{
    /// <summary>
    ///     Evaluates a board.
    /// </summary>
    /// <param name="board">The board to evaluate.</param>
    /// <param name="depth">The curren depth of the search.</param>
    /// <returns>An integer score for the value of the board.</returns>
    int Evaluate(Board board, int depth);
}