using System.ComponentModel;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;

namespace Engine.AI
{
    /// <summary>
    /// This class houses the AIPlayer abstraction.
    /// </summary>
    public class AIPlayer
    {
        // Member fields
        private AlphaBetaSearch Search { get; }
        private readonly Board _board;
        
        // Background worker used for running a task on another thread asynchronously
        public BackgroundWorker Worker { get; }

        /// <summary>
        /// Constructor creates AIPlayer.
        /// </summary>
        /// <param name="board">The initial board to play on.</param>
        public AIPlayer(Board board)
        {
            // Create a new search object with a depth
            Search = new AlphaBetaSearch();
            
            // Assign variables
            _board = board;
            Worker = new BackgroundWorker();
            
            // Subscribe the StartThreadedSearch method to the BackgroundWorker DoWork event
            Worker.DoWork += MakeMove;
        }

        /// <summary>
        /// Runs the search for the best move.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="args">Arguments passed into the method</param>
        private void MakeMove(object sender, DoWorkEventArgs args)
        {
            // If the AI is in checkmate or stalemate then return because the game is over
            if (_board.CurrentPlayer.IsInCheckmate() || _board.CurrentPlayer.IsInStalemate()) return;
            
            // Get the board from the passed in argument
            Board board = (Board) args.Argument;
            
            // Search for the best move
            IMove bestMove = Search.SearchMove(board);
            
            // Store the result in the argument which will be passed on once the method has exited
            args.Result = bestMove;
        }
    }
}