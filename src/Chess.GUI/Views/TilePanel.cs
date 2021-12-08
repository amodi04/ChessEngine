using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Chess.GUI.Models;
using Chess.GUI.Util;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Chess.GUI.Views
{
    /// <summary>
    ///     Custom panel class so that it can hold state.
    /// </summary>
    public class TilePanel : Panel
    {
        private readonly MainWindow _mainWindow;

        /// <summary>
        ///     Creates a tile panel.
        /// </summary>
        /// <param name="tileIndex">The coordinate of the tile in the grid.</param>
        /// <param name="mainWindow">The main windows that the board is drawn on.</param>
        public TilePanel(int tileIndex, MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            TileIndex = tileIndex;

            // Set colour depending on coordinate in grid
            Background = (TileIndex + TileIndex / 8) % 2 == 0 ? Brushes.DarkSlateGray : Brushes.Ivory;
            DrawPiece();
            
            // Add a press handler for handling clicks
            AddHandler(PointerPressedEvent, MouseDownHandler, handledEventsToo: true);
        }
        
        public int TileIndex { get; }

        /// <summary>
        ///     Draws the piece on the tile.
        /// </summary>
        public void DrawPiece()
        {
            Children.Clear();
            
            if (!_mainWindow.BoardModel.GetTile(TileIndex).IsOccupied()) return;
            
            Piece? piece = _mainWindow.BoardModel.GetTile(TileIndex).Piece;
            Image image = IOUtilities.GenerateImage(piece);
            Children.Add(image);
        }

        /// <summary>
        ///     Draws the algebraic letters and numbers on the board.
        /// </summary>
        public void DrawAlgebraicNotation()
        {
            // If in black's perspective
            if (!_mainWindow.BoardFlipped)
            {
                // Add labels from black perspective
                if (FileIndex(TileIndex) is 7) AddLabel(false);

                if (RankIndex(TileIndex) is 7) AddLabel(true);
            }
            else
            {
                // Add labels from white perspective
                if (FileIndex(TileIndex) is 0) AddLabel(false);

                if (RankIndex(TileIndex) is 0) AddLabel(true);
            }
        }

        /// <summary>
        ///     Adds labels to the tile
        /// </summary>
        /// <param name="drawFile">Are we adding a file label (letter)</param>
        private void AddLabel(bool drawFile)
        {
            Children.Add(new Label
            {
                // Draw letter if drawing file otherwise draw number
                Content = drawFile ? $"{FileNames[FileIndex(TileIndex)]}" : $"{RankNames[RankIndex(TileIndex)]}",
                FontSize = 50,
                Foreground = (TileIndex + TileIndex / 8) % 2 == 0 ? Brushes.White : Brushes.Black,
                FontWeight = FontWeight.Bold,

                // Position letters and numbers accordingly in the corners of the tiles
                HorizontalAlignment = drawFile ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                VerticalAlignment = drawFile ? VerticalAlignment.Bottom : VerticalAlignment.Top,
                Margin = new Thickness(5)
            });
        }

        /// <summary>
        ///     Handles click events on the tile.
        /// </summary>
        /// <param name="sender">The object that owns the event handler.</param>
        /// <param name="e">The event.</param>
        private async void MouseDownHandler(object? sender, PointerPressedEventArgs e)
        {
            switch (e.GetCurrentPoint(null).Properties.PointerUpdateKind)
            {
                case PointerUpdateKind.LeftButtonPressed:
                    // If there has not been a tile selected yet
                    if (_mainWindow.FromTile == null)
                    {
                        // Handle first click

                        // Set the initial tile to the panel
                        _mainWindow.FromTile = _mainWindow.BoardModel.GetTile(TileIndex);

                        // Set the piece to be moved to the piece on the tile
                        _mainWindow.MovedPiece = _mainWindow.FromTile.Piece;

                        // If there is no piece, there is nothing to move, so reset selected tile to null
                        if (_mainWindow.MovedPiece == null) _mainWindow.FromTile = null;
                    }
                    else
                    {
                        // Handle second click

                        // Get the move that matches the tiles selected
                        IMove move = MoveFactory.GetMove(_mainWindow.BoardModel, _mainWindow.FromTile.TileCoordinate,
                            TileIndex);
                        
                        // Wait until promotion moves have been resolved
                        move = await HandlePromotion(move);
                        
                        var boardTransition = _mainWindow.BoardModel.CurrentPlayer.MakeMove(move);
                        
                        if (boardTransition.Status == MoveStatus.Done)
                        {
                            // Set the current board in memory to the new board
                            _mainWindow.BoardModel = boardTransition.ToBoard;
                            _mainWindow.MoveStack.Push(move);
                            _mainWindow.MoveLogViewModel.UpdateMoveLog(move, boardTransition);
                            
                            _mainWindow.DrawBoard();
                            _mainWindow.PlaySound(move);

                            // Scroll move log for user
                            _mainWindow.MoveLogView.DataGrid.ScrollIntoView(
                                _mainWindow.MoveLogView.DataGrid.Items.Cast<MoveModel>().Last(), null);
                            _mainWindow.MoveMadeUpdate();

                            // Game over scenario
                            if (_mainWindow.BoardModel.CurrentPlayer.IsInCheckmate() ||
                                _mainWindow.BoardModel.CurrentPlayer.IsInStalemate())
                                _mainWindow.ShowEndgameWindow();
                        }

                        // Reset tiles and pieces ready for a new move to be made
                        _mainWindow.FromTile = null;
                        _mainWindow.MovedPiece = null;
                    }

                    break;
                case PointerUpdateKind.RightButtonPressed:
                    // Reset tiles and pieces. Right mouse button clears current selection
                    _mainWindow.FromTile = null;
                    _mainWindow.MovedPiece = null;
                    break;
            }

            // Redraw board asynchronously
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _mainWindow.DrawBoard();
            });
        }

        private async Task<IMove> HandlePromotion(IMove move)
        {
            if (move is not PromotionMove) return move;
            
            PromotionWindow promotionWindow = new();
            // Await the result from the user input, default is queen.
            var pieceType = await promotionWindow.ShowDialog<PieceType>(_mainWindow);
            
            foreach (var potentialPromotionMove in _mainWindow.BoardModel.AllMoves)
            {
                if (potentialPromotionMove is not PromotionMove promotionMove) continue;
                if (promotionMove.PromotedPiece.Type != pieceType || promotionMove.ToCoordinate != TileIndex) continue;
                // Set the move to the promotion move
                move = promotionMove;

                // Break because we have found the correct move to use
                break;
            }

            return move;
        }

        /// <summary>
        ///     Highlights legal moves on click.
        /// </summary>
        public void HighlightLegalMoves()
        {
            // If the user has not clicked the button, then moves will not be highlighted
            if (!_mainWindow.HighlightLegalMoves) return;
            
            foreach (var move in GetPieceMoves())
                // If a piece can move to the tile
                if (move.ToCoordinate == TileIndex)
                    // Draw a grey circle to indicate it is a destination
                    Children.Add(new Ellipse
                    {
                        Width = 100,
                        Height = 100,
                        Fill = Brushes.Gray
                    });
        }

        /// <summary>
        ///     Gets the list of moves that the current piece can make.
        /// </summary>
        /// <returns>An enumerable list of moves.</returns>
        private IEnumerable<IMove> GetPieceMoves()
        {
            if (_mainWindow.MovedPiece == null || _mainWindow.MovedPiece.PieceCoalition !=
                _mainWindow.BoardModel.CurrentPlayer.Coalition) return new List<IMove>();
            
            List<IMove> pieceMoves = new();
            Board board = _mainWindow.BoardModel;
            foreach (var move in board.CurrentPlayer.Moves)
                if (move.MovedPiece == _mainWindow.MovedPiece)
                {
                    var boardTransition = board.CurrentPlayer.MakeMove(move);
                    if (boardTransition.Status == MoveStatus.Done) pieceMoves.Add(move);
                }

            return pieceMoves;
        }
    }
}