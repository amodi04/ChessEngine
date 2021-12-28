using System;
using System.Diagnostics;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Player;

namespace Engine.AI
{
    /// <summary>
    ///     Class responsible for searching through moves using Alpha Beta pruning.
    /// </summary>
    public class Search
    {
        private int _searchDepth;
        private int _targetDepth;
        private IEvaluator _evaluation;
        private readonly MoveOrdering _moveOrdering;
        private IMove? _bestMove;
        private bool _isWhite;
        
        // Debug values
        private int _cutoffsProduced;
        private int _movesEvaluated;
        
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
        public IMove SearchMove(Board board)
        {
            var watch = new Stopwatch();
            watch.Start();

            // Reset values every time the search is run
            _movesEvaluated = 0;
            _cutoffsProduced = 0;
            _bestMove = null;
            _isWhite = board.CurrentPlayer.Coalition.IsWhite();
            _searchDepth = AISettings.Depth;
            
            TranspositionTable transpositionTable = new(board, 64000);

            // Create evaluator based on the user value in the AISettings class
            _evaluation = AISettings.UseBetterEvaluator ? new BetterEvaluator() : new DefaultEvaluator();

            if (AISettings.UseIterativeDeepening)
            {
                // Start timer for iterative deepening
                Stopwatch iterativeDeepeningTimer = new();
                iterativeDeepeningTimer.Start();
            
                // Loop until max search depth defined by user is reached
                for (int i = 1; i <= _searchDepth; i++)
                {
                    // If time exceeded for search, break and use best move found so far
                    if (iterativeDeepeningTimer.ElapsedMilliseconds > 1000) break;
                
                    // Set the current depth iteration that is being searched (used for depth checks in the alpha beta search)
                    _targetDepth = i;
                    Debug.WriteLine(i);
                
                    // Search at target depth
                    AlphaBeta(board, _targetDepth, int.MinValue, int.MaxValue, _isWhite, transpositionTable);
                }
                iterativeDeepeningTimer.Stop();
            }
            else
            {
                // Normal search
                _targetDepth = _searchDepth;
                AlphaBeta(board, _targetDepth, int.MinValue, int.MaxValue, _isWhite, transpositionTable);
            }
            watch.Stop();
            Debug.WriteLine($"Evaluated {_movesEvaluated} moves in {watch.ElapsedMilliseconds}ms");
            Debug.WriteLine($"Cutoffs Produced: {_cutoffsProduced}");

            // Return the best move
            return _bestMove;
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
        private int AlphaBeta(Board board, int depth, int alpha, int beta, bool maximisingPlayer, TranspositionTable transpositionTable)
        {
            // Try to find the evaluation score for the board we are attempting to evaluate
            int transpositionEvaluation = transpositionTable.LookupEvaluation(depth, alpha, beta);
            
            // If there is an evaluation stored
            if (transpositionEvaluation != int.MinValue)
            {
                _bestMove = transpositionTable.GetStoredMove();
                return transpositionEvaluation;
            }
            
            if (depth == 0 || IsEndgame(board))
            {
                // Return the static evaluation of the board
                _movesEvaluated++;
                return _evaluation.Evaluate(board, depth);
            }
            
            var orderedMoves = _moveOrdering.OrderMoves(board, board.CurrentPlayer.Moves.ToList(), transpositionTable);

            // If the algorithm is running from white's perspective
            if (maximisingPlayer)
            {
                // Initialise the maximum evaluation to -Infinity (we want this to increase)
                var maxEval = int.MinValue;
                
                foreach (var move in orderedMoves)
                {
                    var boardTransition = board.CurrentPlayer.MakeMove(move);
                    if (boardTransition.Status == MoveStatus.Done)
                    {
                        // Evaluate the children of the node and get the minimum value (black's best score)
                        var eval = AlphaBeta(boardTransition.ToBoard,  depth - 1, alpha, beta, false, transpositionTable);

                        // If black's best score is greater than white's best score
                        if (eval > maxEval)
                        {
                            // Set white's best score to the new score because it has a higher value
                            maxEval = eval;

                            // If we are at the root node
                            if (depth == _targetDepth)
                                _bestMove = move;
                        }
                        
                        alpha = Math.Max(alpha, eval);

                        // If the current evaluation is more than black's best move
                        if (eval >= beta)
                        {
                            // Beta cutoff
                            // Prune branch because white has already found a better move
                            _cutoffsProduced++;
                            break;
                        }
                    }
                }
                
                transpositionTable.StoreEvaluation(depth, alpha, TranspositionTable.UpperBound, _bestMove);
                return maxEval;
            }
            // Else if the algorithm is running as black
            else
            {
                // Initialise the minimum evaluation to +Infinity (we want this to decrease)
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
                            // Set black's best score to the new score because it has a smaller value
                            minEval = eval;

                            // If we are at the root node
                            if (depth == _targetDepth)
                                // Set the best move
                                _bestMove = move;
                        }
                    
                        beta = Math.Min(beta, eval);

                        // If the current evaluation is less than white's best move
                        if (eval <= alpha)
                        {
                            // Beta cutoff
                            // Prune branch because white has already found a better move
                            _cutoffsProduced++;
                            break;
                        }
                    }
                }
            
                transpositionTable.StoreEvaluation(depth, beta, TranspositionTable.LowerBound, _bestMove);
                return minEval;
            }
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
}