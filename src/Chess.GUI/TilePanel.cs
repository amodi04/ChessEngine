﻿using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Engine.Enums;
using Engine.Extensions;
using Engine.Factories;
using Engine.Types.MoveGeneration;
using Engine.Types.Pieces;
using Engine.Util;

namespace Chess.GUI
{
    /// <summary>
    /// Custom panel class so that it can hold state.
    /// </summary>
    public class TilePanel : Panel
    {
        // Member fields
        private readonly int _tileIndex;
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Creates a tile panel.
        /// </summary>
        /// <param name="tileIndex">The coordinate of the tile in the grid.</param>
        /// <param name="mainWindow">The main windows that the board is drawn on.</param>
        public TilePanel(int tileIndex, MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _tileIndex = tileIndex;
            
            // Set colour depending on coordinate in grid
            // TODO: Change to allow user defined colour
            Background = (_tileIndex + _tileIndex / 8) % 2 == 0 ? Brushes.DarkSlateGray : Brushes.Ivory;
            // Draw the piece on the tile
            DrawPiece();
            // Add a press handler for handling clicks
            AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
        }
        
        /// <summary>
        /// Draws the piece on the tile.
        /// </summary>
        public void DrawPiece()
        {
            // Clear all children from the current tile panel object
            Children.Clear();
            
            // If the internal tile representation does have a piece on it
            if (_mainWindow.BoardModel.GetTile(_tileIndex).IsOccupied())
            {
                // Get the piece
                Piece piece = _mainWindow.BoardModel.GetTile(_tileIndex).Piece;
                
                // Find the image
                Image image = new Image
                {
                    // Image format: "{Coalition}{Piece}.png"
                    // Example: WB.png => White Bishop
                    // Example: BK => Black King
                    Source = new Bitmap(GUIUtilities.AssetLoader.Open(new Uri(
                        $"avares://Chess.GUI/Assets/{piece.PieceCoalition.ToAbbreviation()}{piece.PieceType.ToAbbreviation(Coalition.White)}.png")))
                };
                // Add the image to the panel
                Children.Add(image);
            }
        }

        /// <summary>
        /// Handles click events on the tile.
        /// </summary>
        /// <param name="sender">The object that owns the event handler.</param>
        /// <param name="e">The event.</param>
        private async void MouseDownHandler(object? sender, PointerPressedEventArgs e)
        {
            // Switch depending on what mouse button was clicked
            switch (e.GetCurrentPoint(null).Properties.PointerUpdateKind)
            {
                // Left mouse button case
                case PointerUpdateKind.LeftButtonPressed:
                    // If there has not been a tile selected yet
                    if (_mainWindow.FromTile == null)
                    {
                        // Handle first click
                        
                        // Set the initial tile to the panel
                        _mainWindow.FromTile = _mainWindow.BoardModel.GetTile(_tileIndex);

                        // Set the piece to be moved to the piece on the tile
                        _mainWindow.MovedPiece = _mainWindow.FromTile.Piece;
                        
                        // If there is no piece, there is nothing to move, so reset selected tile to null
                        if (_mainWindow.MovedPiece == null)
                        {
                            _mainWindow.FromTile = null;
                        }
                    }
                    else
                    {
                        // Handle second click

                        // Get the move that matches the tiles selected
                        IMove move = MoveFactory.GetMove(_mainWindow.BoardModel, _mainWindow.FromTile.TileCoordinate, _tileIndex);
                        
                        // If the move is a promotion move,
                        if (move is PromotionMove)
                        {
                            // Create a new window for selecting the piece to promote to.
                            PromotionWindow promotionWindow = new PromotionWindow();
                            
                            // Await the result from the user input, default is queen.
                            PieceType pieceType = await promotionWindow.ShowDialog<PieceType>(_mainWindow);
                            
                            // Loop through all moves in the board
                            foreach (var potentialPromotionMove in _mainWindow.BoardModel.AllMoves)
                            {
                                // If the move is not a promotion move, skip
                                if (potentialPromotionMove is not PromotionMove promotionMove) continue;
                                
                                // If the move is the correct piece type and the to coordinate is correct
                                if (promotionMove.PromotedPiece.PieceType == pieceType && promotionMove.ToCoordinate == _tileIndex)
                                {
                                    // Set the move to the promotion move
                                    move = promotionMove;
                                    
                                    // Break because we have found the correct move to use
                                    break;
                                }
                            }
                        }
                        
                        // Get the new board representation
                        var boardTransition = _mainWindow.BoardModel.CurrentPlayer.MakeMove(move);
                        
                        // If the status is done, e.g. is a valid move
                        if (boardTransition.Status == MoveStatus.Done)
                        {
                            // Set the current board in memory to the new board
                            _mainWindow.BoardModel = boardTransition.ToBoard;
                            
                            // Add move to move history log
                            _mainWindow.MoveStack.Push(move);
                            _mainWindow.MoveLogViewModel.UpdateMoveLog(move, boardTransition);
                        }

                        // Reset tiles and pieces ready for a new move to be made
                        _mainWindow.FromTile = null;
                        _mainWindow.MovedPiece = null;
                    }
                    break;
                // If the right button was pressed
                case PointerUpdateKind.RightButtonPressed:
                    // Reset tiles and pieces. Right mouse button clears current selection
                    _mainWindow.FromTile = null;
                    _mainWindow.MovedPiece = null;
                    break;
            }
            // Redraw board asynchronously
            await Dispatcher.UIThread.InvokeAsync(() => { _mainWindow.DrawBoard(); });
        }

        /// <summary>
        /// Highlights legal moves on click.
        /// </summary>
        public void HighlightLegalMoves()
        {
            // If the user has not clicked the button, then moves will not be highlighted
            if (!_mainWindow.HighlightLegalMoves) return;
            
            // Loop through moves in the current pieces move
            foreach (var move in GetPieceMoves())
            {
                // If the tile is a potential move
                if (move.ToCoordinate == _tileIndex)
                {
                    // Draw a grey circle above to indicate it is a move
                    Children.Add(new Ellipse
                    {
                        Width = 20,
                        Height = 20,
                        Fill = Brushes.Gray,
                    });
                }
            }
        }
        
        /// <summary>
        /// Gets the list of moves that the current piece can make.
        /// </summary>
        /// <returns>An enumerable list of moves.</returns>
        private IEnumerable<IMove> GetPieceMoves()
        {
            // If the piece is not null and the clicked piece is the same colour as the player
            if (_mainWindow.MovedPiece != null && _mainWindow.MovedPiece.PieceCoalition == _mainWindow.BoardModel.CurrentPlayer.Coalition)
            {
                // Generate the moves for the piece and return it
                return _mainWindow.MovedPiece.GenerateLegalMoves(_mainWindow.BoardModel);
            }

            // Return an empty list if conditions not met
            return new List<IMove>();
        }
    }
}