using System;
using System.Diagnostics;
using Engine.Enums;
using Engine.Extensions;
using Engine.Types.MoveGeneration;

namespace Engine.Types.AI
{
    /// <summary>
    /// Class responsible for searching through moves.
    /// </summary>
    public class MinimaxSearch
    {
        // Member fields
        private readonly IEvaluator _evaluation;
        private readonly int _searchDepth;
        private int _movesEvaluated;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="depth">The depth for the search to use.</param>
        public MinimaxSearch(int depth)
        {
            // Create a new evaluator
            _evaluation = new DefaultEvaluator();
            _searchDepth = depth;
            _movesEvaluated = 0;
        }

        /// <summary>
        /// Gets the move to execute by calculating score.
        /// </summary>
        /// <param name="board">The board to execute the move on.</param>
        /// <returns>A move to execute.</returns>
        public IMove SearchMove(Board board)
        {
            // Initialise the max evaluation to a minimum
            int maxEval = int.MinValue;
            
            // Initialise the min evaluation to a maximum
            int minEval = int.MaxValue;
            
            IMove bestMove = null;
            _movesEvaluated = 0;

            // TODO: Remove DEBUG
            var watch = new Stopwatch();
            watch.Start();
            
            // Loop through each legal move that can be made for the current player
            foreach (var move in board.CurrentPlayer.Moves)
            {
                // Make the move
                BoardTransition boardTransition = board.CurrentPlayer.MakeMove(move);
                
                // If the move was completed
                if (boardTransition.Status == MoveStatus.Done)
                {
                    // Evaluate the board based on the next player to move
                    // If white is to move, then they want to maximise the score for black
                    // If black is to move, then they want to minimise the score 
                    var currentEval = board.CurrentPlayer.Coalition.IsWhite()
                        ? Minimise(boardTransition.ToBoard, _searchDepth - 1)
                        : Maximise(boardTransition.ToBoard, _searchDepth - 1);

                    // If the current player is white and current evaluation is more than the max evaluation
                    if (board.CurrentPlayer.Coalition.IsWhite() && currentEval >= maxEval)
                    {
                        // Set the new max evaluation
                        maxEval = currentEval;
                        
                        // Set the best move
                        bestMove = move;
                        
                    // Else if the player is black and the current evaluation is less than the min evaluation
                    } else if (!board.CurrentPlayer.Coalition.IsWhite() && currentEval <= minEval)
                    {
                        // Set the new min evaluation
                        minEval = currentEval;
                        
                        // Set the best move
                        bestMove = move;
                    }
                }
            }

            // TODO: DEBUG
            watch.Stop();
            Debug.WriteLine($"Evaluated {_movesEvaluated} moves in {watch.ElapsedMilliseconds}ms");
            
            // Return the best move
            return bestMove;
        }

        /// <summary>
        /// Minimises the score on the board.
        /// </summary>
        /// <param name="board">The board to minimise.</param>
        /// <param name="depth">The depth to search at.</param>
        /// <returns>An integer value of the minimised board.</returns>
        private int Minimise(Board board, int depth)
        {
            // If the depth is 0 or the game has ended
            if (depth == 0 || IsEndgame(board))
            {
                _movesEvaluated++;
                // Return the evaluation of the current board
                return _evaluation.Evaluate(board, depth);
            }

            // Initialise the minimum value to the maximum value
            int minEval = int.MaxValue;
            
            // Loop through each available move
            foreach (IMove move in board.CurrentPlayer.Moves)
            {
                // Make the move
                BoardTransition boardTransition = board.CurrentPlayer.MakeMove(move);
                
                // If the move completed
                if (boardTransition.Status == MoveStatus.Done)
                {
                    // Maximise the board
                    int currentEval = Maximise(boardTransition.ToBoard, depth - 1);
                    
                    // If the evaluation is less than the minimum evaluation
                    if (currentEval <= minEval)
                    {
                        // Set the current minimum to the new minimum 
                        minEval = currentEval;
                    }
                }
            }

            // Return the min evaluation
            return minEval;
        }

        /// <summary>
        /// Maximises the score on the board.
        /// </summary>
        /// <param name="board">The board to maximise.</param>
        /// <param name="depth">The depth to search at.</param>
        /// <returns>An integer value of the maximised board.</returns>
        private int Maximise(Board board, int depth)
        {
            // If the depth is 0 or the game has ended
            if (depth == 0 || IsEndgame(board))
            {
                _movesEvaluated++;
                return _evaluation.Evaluate(board, depth);
            }

            // Initialise the maximum value to the minimum value
            int maxEval = int.MinValue;
            
            // Loop through each available move
            foreach (IMove move in board.CurrentPlayer.Moves)
            {
                // Make the move
                BoardTransition boardTransition = board.CurrentPlayer.MakeMove(move);
                
                // If the move completed
                if (boardTransition.Status == MoveStatus.Done)
                {
                    // Minimise the board
                    int currentEval = Minimise(boardTransition.ToBoard, depth - 1);
                    
                    // If the evaluation is more than the maximum evaluation
                    if (currentEval >= maxEval)
                    {
                        // Set the current maximum to the new maximum 
                        maxEval = currentEval;
                    }
                }
            }

            // Return the max evaluation
            return maxEval;
        }

        /// <summary>
        /// Checks if the game has ended.
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