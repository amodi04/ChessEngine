using System;
using System.Diagnostics;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;

namespace Engine.AI
{
    /// <summary>
    ///     Class responsible for searching through moves using Alpha Beta pruning.
    /// </summary>
    public class AlphaBetaSearch
    {
        private int _searchDepth;
        private IEvaluator _evaluation;
        private readonly MoveOrdering _moveOrdering;
        private IMove _bestMove;
        private bool _isWhite;
        
        // Debug values
        private int _cutoffsProduced;
        private int _movesEvaluated;
        
        public AlphaBetaSearch()
        {
            _searchDepth = AISettings.Depth;
            _moveOrdering = new MoveOrdering();
            _movesEvaluated = 0;
            _cutoffsProduced = 0;
            _isWhite = false;
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

            // Create evaluator based on the user value in the AISettings class
            _evaluation = AISettings.UseBetterEvaluator ? new BetterEvaluator() : new DefaultEvaluator();

            // Search for the best move
            _searchDepth = AISettings.Depth;
            AlphaBeta(board, _searchDepth, int.MinValue, int.MaxValue, _isWhite);
            
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
        /// <param name="maximisingPlayer">Flag for running the algorithm as either black or white</param>
        /// <returns>An integer board value that is propagated up the game tree.</returns>
        private int AlphaBeta(Board board, int depth, int alpha, int beta, bool maximisingPlayer)
        {
            // If the depth is 0 (leaf node) or the game has ended
            if (depth == 0 || IsEndgame(board))
            {
                _movesEvaluated++;

                // Return the static evaluation of the board
                return _evaluation.Evaluate(board, depth);
            }

            // Sort the moves
            var orderedMoves = _moveOrdering.OrderMoves(board, board.CurrentPlayer.Moves.ToList());

            // If the algorithm is running from white's perspective
            if (maximisingPlayer)
            {
                // Initialise the maximum evaluation to -Infinity (we want this to increase)
                var maxEval = int.MinValue;

                // Loop through the sorted moves for the current player
                foreach (var move in orderedMoves)
                {
                    // Make the move
                    var boardTransition = board.CurrentPlayer.MakeMove(move);
                    if (boardTransition.Status == MoveStatus.Done)
                    {
                        // Evaluate the children of the node and get the minimum value (black's best score)
                        // Decrease depth, pass alpha and beta, and run evaluation of children as black because 
                        // it becomes black's turn after white.
                        var eval = AlphaBeta(boardTransition.ToBoard, depth - 1, alpha, beta, false);

                        // If black's best score is greater than white's best score
                        if (eval > maxEval)
                        {
                            // Set white's best score to the new score because it has a higher value
                            maxEval = eval;

                            // If we are at the root node (maximal search depth)
                            if (depth == _searchDepth)
                                // Set the best move
                                _bestMove = move;
                        }

                        // Set alpha to the max of the current evaluation and alpha 
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

                // Return the maximum evaluation
                return maxEval;
            }
            // Else if the algorithm is running as black

            // Initialise the minimum evaluation to +Infinity (we want this to decrease)
            var minEval = int.MaxValue;

            // Loop through the sorted moves for the current player
            foreach (var move in orderedMoves)
            {
                // Make the move
                var boardTransition = board.CurrentPlayer.MakeMove(move);
                if (boardTransition.Status == MoveStatus.Done)
                {
                    // Evaluate the children of the node and get the maximum value (white's best score)
                    // Decrease depth, pass alpha and beta, and run evaluation of children as white because 
                    // it becomes white's turn after black.
                    var eval = AlphaBeta(boardTransition.ToBoard, depth - 1, alpha, beta, true);

                    // If white's best score is less than black's best score
                    if (eval < minEval)
                    {
                        // Set black's best score to the new score because it has a smaller value
                        minEval = eval;

                        // If we are at the root node (maximal search depth)
                        if (depth == _searchDepth)
                            // Set the best move
                            _bestMove = move;
                    }

                    // Set beta to the min of the current evaluation and beta 
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

            // Return the minimum evaluation
            return minEval;
        }

        /// <summary>
        ///     Checks if the game has ended.
        /// </summary>
        /// <param name="board">The board to check.</param>
        /// <returns>True if the player is in checkmate or in stalemate.</returns>
        private bool IsEndgame(Board board)
        {
            // Return true if the current player is in checkmate or stalemate(draw)
            return board.CurrentPlayer.IsInCheckmate() || board.CurrentPlayer.IsInStalemate();
        }
    }
}