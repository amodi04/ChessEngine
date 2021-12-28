using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Chess.GUI.Models;
using Chess.GUI.Util;
using Chess.GUI.Views;
using Engine.AI;
using Engine.MoveGeneration;

namespace Chess.GUI;

/// <summary>
///     This class observes the current game in progress.
/// </summary>
public class GameObserver
{
    private readonly AIPlayer _aiPlayer;
    private readonly MainWindow _mainWindow;

    /// <summary>
    ///     Constructor creates a GameObserver object.
    /// </summary>
    /// <param name="mainWindow">The main window to observer.</param>
    public GameObserver(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _aiPlayer = new AIPlayer();

        _mainWindow.OnGuiUpdate += HandleUpdate;
        _aiPlayer.Worker.RunWorkerCompleted += OnSearchComplete;
    }

    /// <summary>
    ///     Handles an update in the GUI.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="args">Arguments passed into the event.</param>
    private void HandleUpdate(object? sender, EventArgs args)
    {
        if (_aiPlayer.Worker.IsBusy)
        {
            _aiPlayer.Worker.CancelAsync();
        }
        else
        {
            var prevMoves = new List<string?>();
            foreach (var move in _mainWindow.MoveLogViewModel.Moves)
            {
                prevMoves.Add(move.WhiteMove);
                if (move.BlackMove is not null)
                    prevMoves.Add(move.BlackMove);
            }

            _aiPlayer.Worker.RunWorkerAsync(Tuple.Create(_mainWindow.BoardModel, prevMoves,
                AISettings.UseBook ? IOUtilities.GetOpeningBook() : null));
        }
    }

    /// <summary>
    ///     Called when the search for the move has completed.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="args">Arguments passed into the event.</param>
    private void OnSearchComplete(object? sender, RunWorkerCompletedEventArgs args)
    {
        // Game over
        if (_mainWindow.BoardModel.CurrentPlayer.IsInCheckmate() ||
            _mainWindow.BoardModel.CurrentPlayer.IsInStalemate()) return;

        if (args.Error != null)
            Debug.WriteLine(args.Error.ToString());

        var bestMove = (IMove)args.Result!;

        var boardTransition = _mainWindow.BoardModel.CurrentPlayer.MakeMove(bestMove);

        if (boardTransition.Status == MoveStatus.Done)
        {
            _mainWindow.BoardModel = boardTransition.ToBoard;
            _mainWindow.MoveStack.Push(bestMove);

            _mainWindow.MoveLogViewModel.UpdateMoveLog(bestMove, boardTransition);
            _mainWindow.MoveLogView.DataGrid.ScrollIntoView(
                _mainWindow.MoveLogView.DataGrid.Items.Cast<MoveModel>().Last(), null);

            _mainWindow.MoveMadeUpdate();
        }

        _mainWindow.DrawBoard();
        _mainWindow.PlaySound(bestMove);

        // Game over
        if (_mainWindow.BoardModel.CurrentPlayer.IsInCheckmate() ||
            _mainWindow.BoardModel.CurrentPlayer.IsInStalemate())
            _mainWindow.ShowEndgameWindow();
    }
}