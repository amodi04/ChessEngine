
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;
using Engine.Enums;
using Engine.Types;
using Engine.Types.AI;
using Engine.Types.MoveGeneration;
using Engine.Types.Pieces;
using Engine.Util;

namespace Chess.GUI.Views
{
    /// <summary>
    /// The main window class.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Member fields
        private readonly UniformGrid _boardView;
        private readonly List<TilePanel> _tilePanels;
        private static MainWindow instance;
        public CapturedPiecesPanel CapturedPiecesPanel { get; private set; }
        public MoveLogView MoveLogView { get; }
        public MoveLogViewModel MoveLogViewModel { get; private set; }
        public Board BoardModel { get; set; }
        public Tile? FromTile { get; set; }
        public Piece? MovedPiece { get; set; }
        public bool HighlightLegalMoves { get; private set; }
        public Stack<IMove> MoveStack { get; }
        public EventHandler? OnGUIUpdate;

        public MainWindow()
        {
            InitializeComponent();
            _tilePanels = new List<TilePanel>();
            // Create a normal chess board
            BoardModel = Board.CreateStandardBoard();
            // Find the front end components from the xaml and store it
            _boardView = this.Find<UniformGrid>("BoardGrid");
            CapturedPiecesPanel = this.Find<CapturedPiecesPanel>("CapturedPiecesPanel");
            MoveLogView = this.Find<MoveLogView>("MoveLogView");
            
            // Initialise the move log view model data context
            MoveLogViewModel = new MoveLogViewModel();
            MoveLogView.DataContext = MoveLogViewModel;
            
            // Create a new stack of moves to keep track of moves made
            MoveStack = new Stack<IMove>();
            
            // Generate front end board
            GenerateBoard();
            
            // Flip the board so that white is on the bottom by default
            FlipBoardMenuItem_OnClick(null, null);
            
            // Turn of highlighting legal moves by default
            HighlightLegalMoves = true;
            
            // Create a new GameObserver
            new GameObserver(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Loads the xaml front end.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Generates the board at the beginning of the application.
        /// </summary>
        private void GenerateBoard()
        {
            // Loop over all 64 tiles
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
            {
                // Create a front end tile panel for each tile on the board
                TilePanel tilePanel = new TilePanel(i, this);
                
                // Add the panel to the list
                _tilePanels.Add(tilePanel);
            }

            // Loop again because reflected indexes can only be used when list is populated
            for (var i = 0; i < BoardUtilities.NumTiles; i++)
            {
                // Add the tile panel at the correct position (starting from bottom left via reflected mapping)
                _boardView.Children.Add(_tilePanels[BoardUtilities.ReflectBoard[i]]);
            }
        }

        /// <summary>
        /// Draws the board graphics
        /// </summary>
        public void DrawBoard()
        {
            // Clear existing tiles on the board
            _boardView.Children.Clear();
            
            // Loop over stored tiles
            for (int i = 0; i < _tilePanels.Count; i++)
            {
                // Draw the piece
                _tilePanels[i].DrawPiece();
                
                // Highlight legal moves
                _tilePanels[i].HighlightLegalMoves();
                
                // Re-add panels
                _boardView.Children.Add(_tilePanels[BoardUtilities.ReflectBoard[i]]);
            }
            
            // Update the captured pieces panel
            CapturedPiecesPanel.DrawPanels(MoveStack);
        }

        /// <summary>
        /// Quit button handler method.
        /// </summary>
        /// <param name="sender">Object that owns the event handler.</param>
        /// <param name="e">The event.</param>
        private void QuitMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            // Close the application
            Close();
        }

        /// <summary>
        /// Flip board button handler method.
        /// </summary>
        /// <param name="sender">Object that owns the event handler.</param>
        /// <param name="e">The event.</param>
        private void FlipBoardMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            // Reverse the array of tile panels
            // This is equal to a 180 degree rotation of the board
            _tilePanels.Reverse();
            
            // Redraw the board
            DrawBoard();
        }

        /// <summary>
        /// Highlight legal moves button handler method.
        /// </summary>
        /// <param name="sender">Object that owns the event handler.</param>
        /// <param name="e">The event.</param>
        private void HighlightLegalMovesMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            // Set the boolean to the not of itself. Button therefore acts as a toggle
            HighlightLegalMoves = !HighlightLegalMoves;
        }

        /// <summary>
        /// Called when the New Game button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The arguments passed in.</param>
        private async void NewGame_OnClick(object? sender, RoutedEventArgs e)
        {
            // Create a new game setup window
            GameSetupWindow gameSetupWindow = new GameSetupWindow();
            
            // Try catch because input is involved
            try
            {
                // Await the result from the dialog pop up
                Tuple<PlayerType, PlayerType> result = await gameSetupWindow.ShowDialog<Tuple<PlayerType, PlayerType>>(this);
                
                // Setup a new game
                SetupGame(result);
            }
            
            catch
            {
                // ignored 
            }
        }

        /// <summary>
        /// Sets up a new game.
        /// </summary>
        /// <param name="gameConfig">The configuration for the new game.</param>
        private void SetupGame(Tuple<PlayerType, PlayerType> gameConfig)
        {
            // Set the player types in board utilities so they can be accessed throughout the game
            BoardUtilities.WhitePlayerType = gameConfig.Item1;
            BoardUtilities.WhitePlayerType = gameConfig.Item2;
            
            // Create a new standard board
            BoardModel = Board.CreateStandardBoard();

            // Reset the GUI
            ResetGUI();
            
            // Draw the board
            DrawBoard();
        }

        /// <summary>
        /// Resets the GUI.
        /// </summary>
        private void ResetGUI()
        {
            // Remove all moves from the move log
            MoveLogViewModel.Moves.Clear();
            
            // Clear move history
            MoveStack.Clear();
            
            // Pass in an empty move stack into the captured pieces panel to reset it
            CapturedPiecesPanel.DrawPanels(new Stack<IMove>());
        }

        /// <summary>
        /// Called when a move is made.
        /// </summary>
        public void MoveMadeUpdate()
        {
            // If the current player is human
            if (BoardModel.CurrentPlayer.PlayerType == PlayerType.Human)
            {
                // Return
                return;
            }
            
            // Else if the player is a computer, invoke the OnGUIUpdate event
            OnGUIUpdate?.Invoke(this, EventArgs.Empty);
        }
    }
}