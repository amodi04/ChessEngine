using System.Diagnostics;
using Engine.Enums;
using Engine.Extensions;
using Engine.Types.MoveGeneration;

namespace Engine.Types.AI
{
    /// <summary>
    /// Class responsible for searching through moves.
    /// </summary>
    public class Search
    {
        // Member fields
        private readonly IEvaluator _evaluation;
        private IMove _bestMove;
        private readonly int _searchDepth;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="depth">The depth for the search to use.</param>
        public Search(int depth)
        {
            // Create a new evaluator
            _evaluation = new DefaultEvaluator();
            _bestMove = null;
            _searchDepth = depth;
        }

        /// <summary>
        /// Gets the move to execute by calculating score.
        /// </summary>
        /// <param name="board">The board to execute the move on.</param>
        /// <returns>A move to execute.</returns>
        public IMove ExecuteMove(Board board)
        {
            // Start watch to time speed of minimax algorithm
            var watch = new Stopwatch();
            watch.Start();
            
            // Start the algorithm with the board, search depth and condition for if it is the white player
            Minimax(board, _searchDepth, board.CurrentPlayer.Coalition.IsWhite());
            
            // Stop watch after completion
            watch.Stop();
            
            // Return the best move
            return _bestMove;
        }

        /// <summary>
        /// Minimax algorithm evaluates moves and picks the best one depending on evaluator.
        /// </summary>
        /// <param name="board">The board to evaluate.</param>
        /// <param name="depth">The depth to search to.</param>
        /// <param name="maximisingPlayer">Condition is true if white.</param>
        /// <returns></returns>
        private int Minimax(Board board, int depth, bool maximisingPlayer)
        {
            // If depth is 0 (look at this current move) or the game has ended
            if (depth == 0 || IsEndgame(board))
            {
                // Evaluate current board
                return _evaluation.Evaluate(board);
            }

            // If white (value wants to be as a great as possible)
            if (maximisingPlayer)
            {
                // Initialise the max evaluation to the minimum number
                int maxEval = int.MinValue;
                
                // Loop through player moves
                foreach (IMove move in board.CurrentPlayer.Moves)
                {
                    // Make the move
                    BoardTransition boardTransition = board.CurrentPlayer.MakeMove(move);
                    
                    // If the move has completed
                    if (boardTransition.Status == MoveStatus.Done)
                    {
                        // Call the minimax algorithm again with a lesser depth but this time for the black player
                        // This is so we can evaluate blacks best move and compare moves
                        int currentEval = Minimax(boardTransition.ToBoard, depth - 1, false);
                        
                        // If the current evaluation is higher than any evaluations seen before
                        if (currentEval >= maxEval)
                        {
                            // Set the max to the new evaluation
                            maxEval = currentEval;
                            
                            // Set the best move to this move
                            _bestMove = move;
                        }
                    }
                }

                // Return the max evaluation achieved
                return maxEval;
            }
            else
            {
                // Initialise the min evaluation to the maximum number
                int minEval = int.MaxValue;
                
                // Loop through player moves
                foreach (IMove move in board.CurrentPlayer.Moves)
                {
                    // Make the move
                    BoardTransition boardTransition = board.CurrentPlayer.MakeMove(move);
                    
                    // If the move has completed
                    if (boardTransition.Status == MoveStatus.Done)
                    {
                        // Call the minimax algorithm again with a lesser depth but this time for the white player
                        int currentEval = Minimax(boardTransition.ToBoard, depth - 1, true);
                        
                        // If the current evaluation is less than any evaluations seen before
                        if (currentEval <= minEval)
                        {
                            // Set the minimum evaluation to the new evaluation
                            minEval = currentEval;
                            
                            // Set the best move to this move
                            _bestMove = move;
                        }
                    }
                }

                // Return the minimum evaluation achieved
                return minEval;
            }
        }

        /// <summary>
        /// Checks if the game has ended.
        /// </summary>
        /// <param name="board">The board to check.</param>
        /// <returns>True if the player is in checkmate or in stalemate.</returns>
        private bool IsEndgame(Board board)
        {
            return board.CurrentPlayer.IsInCheckmate() || board.CurrentPlayer.IsInStalemate();
        }
    }
}