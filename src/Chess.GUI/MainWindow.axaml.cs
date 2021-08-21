using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Engine.Enums;
using Engine.Extensions;
using Engine.Types;
using Engine.Types.Pieces;
using Engine.Util;

namespace Chess.GUI
{
    /// <summary>
    /// The main window class.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Member fields
        private readonly UniformGrid _boardView;
        private readonly List<TilePanel> _tilePanels;
        public Board BoardModel { get; set; }
        public Tile? FromTile { get; set; }
        public Tile? DestinationTile { get; set; }
        public Piece? MovedPiece { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            _tilePanels = new List<TilePanel>();
            // Create a normal chess board
            BoardModel = Board.CreateStandardBoard();
            // Find the grid from the xaml and store it
            _boardView = this.Find<UniformGrid>("BoardGrid");
            GenerateBoard();
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
                
                // Add the panel to the board
                _boardView.Children.Add(tilePanel);
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
            foreach (TilePanel tilePanel in _tilePanels)
            {
                // Draw the piece for each tile
                tilePanel.DrawPiece();
                
                // Re add the tile with the new piece graphics
                _boardView.Children.Add(tilePanel);
            }
        }

        /// <summary>
        /// Quit button handler method
        /// </summary>
        /// <param name="sender">Object that owns the event handler</param>
        /// <param name="e">The event</param>
        private void QuitMenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            // Close the application
            Close();
        }
    }
}