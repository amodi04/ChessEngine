using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Chess.GUI.Libraries.ColorPicker.Converters;
using Chess.GUI.Libraries.ColorPicker.Structures;
using Chess.GUI.ViewModels;
using Engine.BoardRepresentation;
using Engine.IO;
using Engine.MoveGeneration;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Chess.GUI.Views
{
    /// <summary>
    /// The main window class.
    /// </summary>
    public class MainWindow : Window
    {
        // Member fields
        private readonly UniformGrid _boardView;
        private readonly List<TilePanel> _tilePanels;
        private CapturedPiecesPanel CapturedPiecesPanel { get; }
        public MoveLogView MoveLogView { get; }
        public MoveLogViewModel MoveLogViewModel { get; }
        public Board BoardModel { get; set; }
        public Tile? FromTile { get; set; }
        public Piece? MovedPiece { get; set; }
        public bool HighlightLegalMoves { get; private set; }
        public bool BoardFlipped { get; private set; }
        public Stack<IMove> MoveStack { get; }
        public EventHandler? OnGuiUpdate;

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
            for (var i = 0; i < NumTiles; i++)
            {
                // Create a front end tile panel for each tile on the board
                TilePanel tilePanel = new TilePanel(i, this);
                
                // Add the panel to the list
                _tilePanels.Add(tilePanel);
            }

            // Loop again because reflected indexes can only be used when list is populated
            for (var i = 0; i < NumTiles; i++)
            {
                // Add the tile panel at the correct position (starting from bottom left via reflected mapping)
                _boardView.Children.Add(_tilePanels[ReflectBoard[i]]);
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
                var tilePanel = _tilePanels[i];
                // Draw the piece
                tilePanel.DrawPiece();
                
                // Highlight legal moves
                tilePanel.HighlightLegalMoves();
                
                // Re-draw letter and number coordinates on the board
                tilePanel.DrawAlgebraicNotation();

                // Re-add panels
                _boardView.Children.Add(_tilePanels[ReflectBoard[i]]);
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
        private void FlipBoardMenuItem_OnClick(object? sender, RoutedEventArgs? e)
        {
            // Reverse the array of tile panels
            // This is equal to a 180 degree rotation of the board
            _tilePanels.Reverse();

            BoardFlipped = !BoardFlipped;
            
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
                // ignored. Carry on if the game could not be setup (does not replace board)
            }
        }

        /// <summary>
        /// Sets up a new game.
        /// </summary>
        /// <param name="gameConfig">The configuration for the new game.</param>
        private void SetupGame(Tuple<PlayerType, PlayerType> gameConfig)
        {
            // Set the player types in board utilities so they can be accessed throughout the game
            WhitePlayerType = gameConfig.Item1;
            BlackPlayerType = gameConfig.Item2;
            
            // Create a new standard board
            BoardModel = Board.CreateStandardBoard();

            // Reset the GUI
            ResetGui();
            
            // Draw the board
            DrawBoard();
            MoveMadeUpdate();
        }

        /// <summary>
        /// Resets the GUI.
        /// </summary>
        private void ResetGui()
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
            OnGuiUpdate?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the To Fen menu button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private async void ToFEN_OnClick(object? sender, RoutedEventArgs e)
        {
            // Get the FEN string of the current board
            string fen = FenParser.FenFromPosition(BoardModel, MoveStack);
            
            // Create a new FenOutputWindow object
            FenOutputWindow fenOutputWindow = new FenOutputWindow(fen);
            
            // Wait for the dialog to close asynchronously so the main thread isn't blocked
            await fenOutputWindow.ShowDialog(this);
        }

        /// <summary>
        /// Called when the From Fen menu button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private async void FromFEN_OnClick(object? sender, RoutedEventArgs e)
        {
            // Create a new FenInputWindowObject
            FenInputWindow fenInputWindow = new FenInputWindow();
            
            // Wait for the result of the dialog when it closes. Expecting a string
            string fen = await fenInputWindow.ShowDialog<string>(this);
            
            // Try parse
            try
            {
                // Parse the board
                BoardModel = Board.CreateBoardFromFen(fen);
                
                // Redraw GUI
                ResetGui();
                DrawBoard();
                MoveMadeUpdate();
            }
            catch
            {
                // ignored. Carry on if string not parsed (does not replace the board model)
            }
        }

        /// <summary>
        /// Called when the Configure AI Settings menu button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private async void ConfigureAISettings_OnClick(object? sender, RoutedEventArgs e)
        {
            // Create a new window
            AISettingsWindow aiSettingsWindow = new AISettingsWindow();
            
            // Await asynchronously for the window to be closed by the user
            await aiSettingsWindow.ShowDialog(this);
        }

        /// <summary>
        /// Called when the change tile colours menu button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private async void ChangeTileColours_OnClick(object? sender, RoutedEventArgs e)
        {
            // Create a new tile colour picker window
            TileColourPickerWindow tileColourPickerWindow = new TileColourPickerWindow();
            
            // Wait asynchronously for the window to close and store the passed out value
            RGBColor color = await tileColourPickerWindow.ShowDialog<RGBColor>(this);
            
            // Create a new colour converter and convert rgb color data to a brush object
            var converter = new RGBColorToBrushConverter();
            var brush = (Brush) converter.Convert(color, typeof(RGBColor), null, CultureInfo.CurrentCulture);
            
            // Cast the sender to menu item so that the header can be accessed
            var menuItem = (MenuItem) sender!;
            
            // Are we changing the light colour tiles?
            bool changingLightColour = ReferenceEquals(menuItem?.Header, "Change Light Tile Colour");
            foreach (var tilePanel in _tilePanels)
            {
                // If the user is changing the light tile colours
                if (changingLightColour)
                {
                    // Change the light coloured tiles to the brush colour
                    if ((tilePanel.TileIndex + tilePanel.TileIndex / 8) % 2 != 0)
                    {
                        tilePanel.Background = brush;
                    }
                }
                // Else (user is changing dark tile colours
                else
                {
                    // Change the dark coloured tiles to the brush colour
                    if ((tilePanel.TileIndex + tilePanel.TileIndex / 8) % 2 == 0)
                    {
                        tilePanel.Background = brush;
                    }
                }
            }
        }

        /// <summary>
        /// Called when the reset tile colours menu button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void ResetTileColours_OnClick(object? sender, RoutedEventArgs e)
        {
            // Loop through each tile on the board and set respective colours based on index
            foreach (var tilePanel in _tilePanels)
            {
                tilePanel.Background =(tilePanel.TileIndex + tilePanel.TileIndex / 8) % 2 == 0 ? Brushes.DarkSlateGray : Brushes.Ivory;
            }
        }

        /// <summary>
        /// Called when the current player is either in checkmate of stalemate.
        /// </summary>
        public async void ShowEndgameWindow()
        {
            EndgameWindow endgameWindow = new EndgameWindow();
            string endgameStatus = BoardModel.CurrentPlayer.IsInCheckmate() ?
                $"{BoardModel.CurrentPlayer.GetOpponent()} wins by Checkmate!" : "Draw by Stalemate!";
            endgameWindow.ViewModel.EndgameStatus = endgameStatus;
            await endgameWindow.ShowDialog(this);
        }
    }
}