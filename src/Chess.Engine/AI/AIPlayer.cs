using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.IO;
using Engine.MoveGeneration;

namespace Engine.AI;

/// <summary>
///     This class houses the AIPlayer which plays against human players.
/// </summary>
public class AIPlayer
{
    private bool _useBook;

    public AIPlayer()
    {
        Worker = new BackgroundWorker();
        Search = new Search();
        _useBook = false;
        Worker.DoWork += InitialiseSearch;
    }

    private Search Search { get; }
    public BackgroundWorker Worker { get; }


    /// <summary>
    ///     Sets up data in order to search for a move.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="args">Arguments passed into the method</param>
    private void InitialiseSearch(object? sender, DoWorkEventArgs args)
    {
        var (board, prevMoves, openingBook) = (Tuple<Board, List<string>, Stream?>)args.Argument!;
        if (board.CurrentPlayer.IsInCheckmate() || board.CurrentPlayer.IsInStalemate()) return;

        _useBook = _useBook || board.PlyCount < 2 && AISettings.UseBook;
        var bestMove = _useBook
            ? PickRandomWeightedBookMove(board, prevMoves, openingBook)
            : Search.SearchMove(board);
        args.Result = bestMove;
    }

    /// <summary>
    ///     Picks a book move randomly weighted.
    /// </summary>
    /// <param name="board">The board to apply the move to.</param>
    /// <param name="prevMoves">The list of previous moves played.</param>
    /// <param name="openingBook"></param>
    /// <returns>The move to play.</returns>
    private IMove? PickRandomWeightedBookMove(Board board, List<string> prevMoves, Stream? openingBook)
    {
        List<string?> lines = new();
        var possibleMoves = new List<string?>();
        if (openingBook != null)
        {
            using StreamReader reader = new(openingBook);
            while (!reader.EndOfStream) lines.Add(reader.ReadLine());
        }

        FindPossibleMoves(board, prevMoves, lines, possibleMoves);

        if (possibleMoves.Count == 0)
        {
            _useBook = false;

            // Search for a move using AB pruning instead - can no longer make book moves
            return Search.SearchMove(board);
        }

        var random = new Random();
        var index = random.Next(0, possibleMoves.Count);

        return PgnParser.FromSan(board, possibleMoves[index]);
    }

    /// <summary>
    ///     Finds the possible moves that could be made given the previous moves.
    /// </summary>
    /// <param name="board">The board to make the move on.</param>
    /// <param name="prevMoves">The list of previous moves played.</param>
    /// <param name="lines">The list of games to pick the move from.</param>
    /// <param name="possibleMoves">The list to append to.</param>
    private void FindPossibleMoves(Board board, List<string> prevMoves, List<string?> lines,
        List<string?> possibleMoves)
    {
        foreach (var stringMoves in lines.Select(line => line?.Split(' ')))
            if (board.PlyCount == 0)
            {
                // Add the first move of the line to the list of potential moves
                possibleMoves.Add(stringMoves?[0]);
            }
            else
            {
                // If previous moves match with the sequence of moves from the current line of the text file,
                // Add the move following the matched sequence to the list of potential moves
                if (stringMoves != null && PreviousMoveIsSame(board.PlyCount - 1, prevMoves, stringMoves))
                    possibleMoves.Add(stringMoves[board.PlyCount]);
            }
    }

    /// <summary>
    ///     Checks if the move at the passed in ply is the same as a move in a sequence at the same ply.
    /// </summary>
    /// <param name="ply">The ply to check.</param>
    /// <param name="prevMoves">The list of previous moves played.</param>
    /// <param name="stringMoves">The line of moves to check against.</param>
    /// <returns></returns>
    private bool PreviousMoveIsSame(int ply, IReadOnlyList<string> prevMoves, IReadOnlyList<string> stringMoves)
    {
        if (ply > 0)
            if (!PreviousMoveIsSame(ply - 1, prevMoves, stringMoves))
                return false;

        return prevMoves[ply] == stringMoves[ply];
    }
}