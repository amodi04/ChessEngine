using System.Collections.Generic;
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
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Chess.GUI.Views;

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
        Background = (TileIndex + TileIndex / 8) % 2 == 0 ? Brushes.DarkSlateGray : Brushes.Ivory;
        DrawPiece();
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

        var piece = _mainWindow.BoardModel.GetTile(TileIndex).Piece;
        var image = IOUtilities.GenerateImage(piece);
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
            Content = drawFile ? $"{FileNames[FileIndex(TileIndex)]}" : $"{RankNames[RankIndex(TileIndex)]}",
            FontSize = 50,
            Foreground = (TileIndex + TileIndex / 8) % 2 == 0 ? Brushes.White : Brushes.Black,
            FontWeight = FontWeight.Bold,

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
        // Disable user input if computer is taking it's turn
        if (_mainWindow.BoardModel.CurrentPlayer.PlayerType == PlayerType.Computer) return;

        switch (e.GetCurrentPoint(null).Properties.PointerUpdateKind)
        {
            case PointerUpdateKind.LeftButtonPressed:
                if (_mainWindow.FromTile == null)
                {
                    // Handle first click

                    _mainWindow.FromTile = _mainWindow.BoardModel.GetTile(TileIndex);
                    _mainWindow.MovedPiece = _mainWindow.FromTile.Piece;
                    if (_mainWindow.MovedPiece == null) _mainWindow.FromTile = null;
                }
                else
                {
                    // Handle second click

                    var move = MoveFactory.GetMove(_mainWindow.BoardModel, _mainWindow.FromTile.TileCoordinate,
                        TileIndex);

                    // Wait until promotion moves have been resolved
                    move = await HandlePromotion(move);

                    var boardTransition = _mainWindow.BoardModel.CurrentPlayer.MakeMove(move);

                    if (boardTransition.Status == MoveStatus.Done)
                    {
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

                    _mainWindow.FromTile = null;
                    _mainWindow.MovedPiece = null;
                }

                break;
            case PointerUpdateKind.RightButtonPressed:
                // Right mouse button clears current selection
                _mainWindow.FromTile = null;
                _mainWindow.MovedPiece = null;
                break;
        }

        await Dispatcher.UIThread.InvokeAsync(() => { _mainWindow.DrawBoard(); });
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
            move = promotionMove;
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
            if (move.ToCoordinate == TileIndex)
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
        var board = _mainWindow.BoardModel;
        foreach (var move in board.CurrentPlayer.Moves)
            if (move.MovedPiece == _mainWindow.MovedPiece)
            {
                var boardTransition = board.CurrentPlayer.MakeMove(move);
                if (boardTransition.Status == MoveStatus.Done) pieceMoves.Add(move);
            }

        return pieceMoves;
    }
}