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
using Chess.GUI.Util;
using Chess.GUI.ViewModels;
using Engine.BoardRepresentation;
using Engine.IO;
using Engine.MoveGeneration;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Chess.GUI.Views;

/// <summary>
///     The main window class responsible for user interaction with the engine.
/// </summary>
public class MainWindow : Window
{
    private readonly UniformGrid _boardView;
    private readonly List<TilePanel> _tilePanels;
    public EventHandler? OnGuiUpdate;

    public MainWindow()
    {
        InitializeComponent();

        BoardModel = Board.CreateStandardBoard();
        _tilePanels = new List<TilePanel>();
        _boardView = this.Find<UniformGrid>("BoardGrid");
        CapturedPiecesPanel = this.Find<CapturedPiecesPanel>("CapturedPiecesPanel");
        MoveLogView = this.Find<MoveLogView>("MoveLogView");
        MoveLogViewModel = new MoveLogViewModel();
        MoveLogView.DataContext = MoveLogViewModel;
        MoveStack = new Stack<IMove>();

        GenerateBoard();

        // Flip the board so that white is on the bottom by default
        FlipBoardMenuItem_OnClick(null, null);

        HighlightLegalMoves = true;

        new GameObserver(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private CapturedPiecesPanel CapturedPiecesPanel { get; }
    public MoveLogView MoveLogView { get; }
    public MoveLogViewModel MoveLogViewModel { get; }
    public Board BoardModel { get; set; }
    public Tile? FromTile { get; set; }
    public Piece? MovedPiece { get; set; }
    public bool HighlightLegalMoves { get; private set; }
    public bool BoardFlipped { get; private set; }
    public Stack<IMove> MoveStack { get; }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    ///     Generates the board at the beginning of the application.
    /// </summary>
    private void GenerateBoard()
    {
        for (var i = 0; i < NumTiles; i++)
        {
            TilePanel tilePanel = new(i, this);
            _tilePanels.Add(tilePanel);
        }

        for (var i = 0; i < NumTiles; i++)
            _boardView.Children.Add(_tilePanels[ReflectBoard[i]]);
    }

    /// <summary>
    ///     Draws the board graphics
    /// </summary>
    public void DrawBoard()
    {
        _boardView.Children.Clear();

        for (var i = 0; i < _tilePanels.Count; i++)
        {
            var tilePanel = _tilePanels[i];

            tilePanel.DrawPiece();
            tilePanel.HighlightLegalMoves();
            tilePanel.DrawAlgebraicNotation();

            _boardView.Children.Add(_tilePanels[ReflectBoard[i]]);
        }

        CapturedPiecesPanel.DrawPanels(MoveStack);
    }

    /// <summary>
    ///     Quit button handler method.
    /// </summary>
    /// <param name="sender">Object that owns the event handler.</param>
    /// <param name="e">The event.</param>
    private void QuitMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    ///     Flip board button handler method.
    /// </summary>
    /// <param name="sender">Object that owns the event handler.</param>
    /// <param name="e">The event.</param>
    private void FlipBoardMenuItem_OnClick(object? sender, RoutedEventArgs? e)
    {
        // This is equal to a 180 degree rotation of the board
        _tilePanels.Reverse();
        BoardFlipped = !BoardFlipped;
        DrawBoard();
    }

    /// <summary>
    ///     Highlight legal moves button handler method.
    /// </summary>
    /// <param name="sender">Object that owns the event handler.</param>
    /// <param name="e">The event.</param>
    private void HighlightLegalMovesMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        HighlightLegalMoves = !HighlightLegalMoves;
    }

    /// <summary>
    ///     Called when the New Game button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The arguments passed in.</param>
    private async void NewGame_OnClick(object? sender, RoutedEventArgs e)
    {
        GameSetupWindow gameSetupWindow = new();
        var result =
            await gameSetupWindow.ShowDialog<Tuple<PlayerType, PlayerType>>(this);

        if (result != null) SetupGame(result);
    }

    /// <summary>
    ///     Sets up a new game.
    /// </summary>
    /// <param name="gameConfig">The configuration for the new game.</param>
    private void SetupGame(Tuple<PlayerType, PlayerType> gameConfig)
    {
        WhitePlayerType = gameConfig.Item1;
        BlackPlayerType = gameConfig.Item2;

        BoardModel = Board.CreateStandardBoard();
        ResetGui();
        DrawBoard();

        MoveMadeUpdate();
    }

    /// <summary>
    ///     Resets the GUI.
    /// </summary>
    private void ResetGui()
    {
        MoveLogViewModel.Moves.Clear();
        MoveStack.Clear();
        CapturedPiecesPanel.DrawPanels(MoveStack);
    }

    /// <summary>
    ///     Called when a move is made.
    /// </summary>
    public void MoveMadeUpdate()
    {
        if (BoardModel.CurrentPlayer.PlayerType == PlayerType.Human)
            return;

        OnGuiUpdate?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///     Called when the To Fen menu button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private async void ToFEN_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var fen = FenParser.FenFromPosition(BoardModel, MoveStack);
            FenOutputWindow fenOutputWindow = new(fen);
            await fenOutputWindow.ShowDialog(this);
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex.Message);
            await errorWindow.ShowDialog(this);
        }
    }

    /// <summary>
    ///     Called when the From Fen menu button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private async void FromFEN_OnClick(object? sender, RoutedEventArgs e)
    {
        FenInputWindow fenInputWindow = new();

        var fen = await fenInputWindow.ShowDialog<string>(this);

        try
        {
            BoardModel = Board.CreateBoardFromFen(fen);
            ResetGui();
            DrawBoard();
            MoveMadeUpdate();
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex.Message);
            await errorWindow.ShowDialog(this);
        }
    }

    /// <summary>
    ///     Called when the Configure AI Settings menu button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private async void ConfigureAISettings_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            AISettingsWindow aiSettingsWindow = new();
            await aiSettingsWindow.ShowDialog(this);
        }
        catch (Exception ex)
        {
            var errorWindow = new ErrorWindow(ex.Message);
            await errorWindow.ShowDialog(this);
        }
    }

    /// <summary>
    ///     Called when the change tile colours menu button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private async void ChangeTileColours_OnClick(object? sender, RoutedEventArgs e)
    {
        TileColourPickerWindow tileColourPickerWindow = new();

        var color = await tileColourPickerWindow.ShowDialog<RGBColor>(this);

        var converter = new RGBColorToBrushConverter();
        var brush = (Brush)converter.Convert(color, typeof(RGBColor), null, CultureInfo.CurrentCulture);

        var menuItem = (MenuItem)sender!;

        var changingLightColour = ReferenceEquals(menuItem.Header, "Change Light Tile Colour");
        foreach (var tilePanel in _tilePanels)
            if (changingLightColour)
            {
                // Change the light coloured tiles to the brush colour
                if ((tilePanel.TileIndex + tilePanel.TileIndex / 8) % 2 != 0) tilePanel.Background = brush;
            }
            else
            {
                // Change the dark coloured tiles to the brush colour
                if ((tilePanel.TileIndex + tilePanel.TileIndex / 8) % 2 == 0) tilePanel.Background = brush;
            }
    }

    /// <summary>
    ///     Called when the reset tile colours menu button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private void ResetTileColours_OnClick(object? sender, RoutedEventArgs e)
    {
        foreach (var tilePanel in _tilePanels)
            tilePanel.Background = (tilePanel.TileIndex + tilePanel.TileIndex / 8) % 2 == 0
                ? Brushes.DarkSlateGray
                : Brushes.Ivory;
    }

    /// <summary>
    ///     Called when the current player is either in checkmate of stalemate.
    /// </summary>
    public async void ShowEndgameWindow()
    {
        EndgameWindow endgameWindow = new();
        var endgameStatus = BoardModel.CurrentPlayer.IsInCheckmate()
            ? $"{BoardModel.CurrentPlayer.GetOpponent()} wins by Checkmate!"
            : "Draw by Stalemate!";
        endgameWindow.ViewModel.EndgameStatus = endgameStatus;
        await endgameWindow.ShowDialog(this);
    }

    /// <summary>
    ///     Plays audio for moves played.
    /// </summary>
    /// <param name="move">The move to play the audio for.</param>
    public void PlaySound(IMove move)
    {
        var vm = DataContext as MainWindowViewModel;
        vm?.Play(IOUtilities.GetSoundStream(move));
    }
}