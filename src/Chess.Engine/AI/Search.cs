using System;
using System.Diagnostics;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Player;

namespace Engine.AI;

/// <summary>
///     Class responsible for searching through moves using Alpha Beta pruning.
/// </summary>
public class Search
{
    private readonly MoveOrdering _moveOrdering;
    private IMove? _bestMove;

    // Debug values
    private int _cutoffsProduced;
    private IEvaluator _evaluation;
    private bool _isWhite;
    private int _movesEvaluated;
    private int _searchDepth;
    private int _targetDepth;

    public Search()
    {
        _searchDepth = AISettings.Depth;
        _moveOrdering = new MoveOrdering();
        _movesEvaluated = 0;
        _cutoffsProduced = 0;
        _isWhite = false;
        _evaluation = new DefaultEvaluator();
        _bestMove = new NormalMove();
    }

    /// <summary>
    ///     Gets the move to execute by calculating score.
    /// </summary>
    /// <param name="board">The board to execute the move on.</param>
    /// <returns>A move to execute.</returns>
    public IMove? SearchMove(Board board)
    {
        var watch = new Stopwatch();
        watch.Start();

        _movesEvaluated = 0;
        _cutoffsProduced = 0;
        _bestMove = null;
        _isWhite = board.CurrentPlayer.Coalition.IsWhite();
        _searchDepth = AISettings.Depth;
        TranspositionTable transpositionTable = new(board, 64000);
        _evaluation = AISettings.UseBetterEvaluator ? new BetterEvaluator() : new DefaultEvaluator();

        if (AISettings.UseIterativeDeepening)
        {
            IterativeDeepeningSearch(board, transpositionTable);
        }
        else
        {
            _targetDepth = _searchDepth;
            AlphaBeta(board, _targetDepth, int.MinValue, int.MaxValue, _isWhite, transpositionTable);
        }

        watch.Stop();
        Debug.WriteLine($"Evaluated {_movesEvaluated} moves in {watch.ElapsedMilliseconds}ms");
        Debug.WriteLine($"Cutoffs Produced: {_cutoffsProduced}");
        return _bestMove;
    }

    /// <summary>
    ///     Performs a search via iterative deepening.
    ///     This incrementally performs a depth first search at increasing depths
    /// </summary>
    /// <param name="board">The board to search.</param>
    /// <param name="transpositionTable">The hash table of moves searched</param>
    private void IterativeDeepeningSearch(Board board, TranspositionTable transpositionTable)
    {
        Stopwatch iterativeDeepeningTimer = new();
        iterativeDeepeningTimer.Start();

        for (var i = 1; i <= _searchDepth; i++)
        {
            if (iterativeDeepeningTimer.ElapsedMilliseconds > 1000) break;
            _targetDepth = i;
            Debug.WriteLine(i);

            AlphaBeta(board, _targetDepth, int.MinValue, int.MaxValue, _isWhite, transpositionTable);
        }

        iterativeDeepeningTimer.Stop();
    }

    /// <summary>
    ///     Alpha beta search algorithm.
    ///     Iterates through a game tree and picks a move based on evaluation score.
    ///     Decreases the number of search nodes evaluated by the minimax algorithm.
    ///     Outputs the same move as a standard minimax algorithm but evaluates less nodes
    ///     because they can never influence the outcome.
    /// </summary>
    /// <param name="board">The board to evaluate.</param>
    /// <param name="depth">The depth at which to search at.</param>
    /// <param name="alpha">Minimum score of the maximising player (worst score).</param>
    /// <param name="beta">Maximum score of the minimising player (worst score).</param>
    /// <param name="maximisingPlayer">Flag for running the algorithm as either black or white.</param>
    /// <param name="transpositionTable">Table of previously searched boards.</param>
    /// <returns>An integer board value that is propagated up the game tree.</returns>
    private int AlphaBeta(Board board, int depth, int alpha, int beta, bool maximisingPlayer,
        TranspositionTable transpositionTable)
    {
        var transpositionEvaluation = transpositionTable.LookupEvaluation(depth, alpha, beta);

        // If there is an evaluation stored
        if (transpositionEvaluation != int.MinValue)
        {
            _bestMove = transpositionTable.GetStoredMove();
            return transpositionEvaluation;
        }

        if (depth == 0 || IsEndgame(board))
        {
            _movesEvaluated++;
            return _evaluation.Evaluate(board, depth);
        }

        var orderedMoves = _moveOrdering.OrderMoves(board, board.CurrentPlayer.Moves.ToList(), transpositionTable);

        if (maximisingPlayer)
        {
            var maxEval = int.MinValue;

            foreach (var move in orderedMoves)
            {
                var boardTransition = board.CurrentPlayer.MakeMove(move);
                if (boardTransition.Status == MoveStatus.Done)
                {
                    // Evaluate the children of the node and get the minimum value (black's best score)
                    var eval = AlphaBeta(boardTransition.ToBoard, depth - 1, alpha, beta, false,
                        transpositionTable);
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        if (depth == _targetDepth)
                            _bestMove = move;
                    }

                    alpha = Math.Max(alpha, eval);

                    if (eval >= beta)
                    {
                        // Beta cutoff
                        _cutoffsProduced++;
                        break;
                    }
                }
            }

            transpositionTable.StoreEvaluation(depth, alpha, TranspositionTable.UpperBound, _bestMove);
            return maxEval;
        }

        var minEval = int.MaxValue;

        foreach (var move in orderedMoves)
        {
            var boardTransition = board.CurrentPlayer.MakeMove(move);
            if (boardTransition.Status == MoveStatus.Done)
            {
                // Evaluate the children of the node and get the maximum value (white's best score)
                var eval = AlphaBeta(boardTransition.ToBoard, depth - 1, alpha, beta, true, transpositionTable);

                if (eval < minEval)
                {
                    minEval = eval;

                    if (depth == _targetDepth)
                        _bestMove = move;
                }

                beta = Math.Min(beta, eval);

                if (eval <= alpha)
                {
                    // Beta cutoff
                    _cutoffsProduced++;
                    break;
                }
            }
        }

        // Store evaluation in transposition table
        transpositionTable.StoreEvaluation(depth, beta, TranspositionTable.LowerBound, _bestMove);
        return minEval;
    }

    /// <summary>
    ///     Checks if the game has ended.
    /// </summary>
    /// <param name="board">The board to check.</param>
    /// <returns>True if the player is in checkmate or in stalemate.</returns>
    private bool IsEndgame(Board board)
    {
        return board.CurrentPlayer.IsInCheckmate() || board.CurrentPlayer.IsInStalemate();
    }
}