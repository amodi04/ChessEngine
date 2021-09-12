using System;
using System.ComponentModel;
using System.Diagnostics;
using Chess.GUI.Views;
using Engine.Enums;
using Engine.Types;
using Engine.Types.AI;
using Engine.Types.MoveGeneration;

namespace Chess.GUI
{
    /// <summary>
    /// This class observes the current game in progress.
    /// </summary>
    public class GameObserver
    {
        // Member fields
        private MainWindow _mainWindow;
        public AIPlayer AIPlayer { get; }

        /// <summary>
        /// Constructor creates a GameObserver object.
        /// </summary>
        /// <param name="mainWindow">The main window to observer.</param>
        public GameObserver(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            
            // Create a new AIPlayer passing in the current board in play
            AIPlayer = new AIPlayer(mainWindow.BoardModel);
            
            // Subscribe the HandleUpdate method to the main window OnGUIUpdate event
            _mainWindow.OnGUIUpdate += HandleUpdate;
            
            // Subscribe the OnSearchComplete method to the AIPlayer BackgroundWorker RunWorkerCompleted event
            AIPlayer.Worker.RunWorkerCompleted += OnSearchComplete;
        }
        
        /// <summary>
        /// Handles an update in the GUI.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="args">Arguments passed into the event.</param>
        public void HandleUpdate(object sender, EventArgs args)
        {
            // If the ai player is calculating move
            if (AIPlayer.Worker.IsBusy)
            {
                // Cancel its process
                AIPlayer.Worker.CancelAsync();
            }
            else
            {
                // Run the BackgroundWorker (invokes the DoWork event)
                // Pass in the Board as an argument
                AIPlayer.Worker.RunWorkerAsync(_mainWindow.BoardModel);
            }
        }

        /// <summary>
        /// Called when the search for the move has completed.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="args">Arguments passed into the event.</param>
        public void OnSearchComplete(object sender, RunWorkerCompletedEventArgs args)
        {
            // If the Current player is in checkmate or stalemate, return because the game is over.
            if (_mainWindow.BoardModel.CurrentPlayer.IsInCheckmate() ||
                _mainWindow.BoardModel.CurrentPlayer.IsInStalemate()) return;
            
            // If there is an error
            if (args.Error != null)
            {
                // TODO: Debug
                Debug.WriteLine(args.Error.ToString());
            }
            
            // Get the best move from the argument passed in
            IMove bestMove = (IMove) args.Result;
            
            // Make the move on the main board
            BoardTransition boardTransition =
                _mainWindow.BoardModel.CurrentPlayer.MakeMove(bestMove);

            // If the move completed
            if (boardTransition.Status == MoveStatus.Done)
            {
                // Set the new board
                _mainWindow.BoardModel = boardTransition.ToBoard;
                
                // Call a move made update event on the main window
                _mainWindow.MoveMadeUpdate();
                
                // Update the move log
                _mainWindow.MoveLogViewModel.UpdateMoveLog(bestMove, boardTransition);
            }
            
            // Draw the board again
            _mainWindow.DrawBoard();
        }
    }
}