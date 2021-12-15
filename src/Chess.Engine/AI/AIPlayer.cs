﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.IO;
using Engine.MoveGeneration;

namespace Engine.AI
{
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

            // Subscribe the StartThreadedSearch method to the BackgroundWorker DoWork event
            Worker.DoWork += InitialiseSearch;
        }
        
        private Search Search { get; }
        public BackgroundWorker Worker { get; }


        /// <summary>
        ///     Sets up data in order to search for a move.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="args">Arguments passed into the method</param>
        private void InitialiseSearch(object sender, DoWorkEventArgs args)
        {
            // Get the board from the passed in argument and unpack
            var (board, prevMoves, openingBook) = (Tuple<Board, List<string>, Stream?>) args.Argument;

            // If the AI is in checkmate or stalemate then return because the game is over
            if (board.CurrentPlayer.IsInCheckmate() || board.CurrentPlayer.IsInStalemate()) return;
            
            _useBook = _useBook || board.PlyCount < 2 && AISettings.UseBook;
            
            var bestMove = _useBook ? PickRandomWeightedBookMove(board, prevMoves, openingBook) : Search.SearchMove(board);
            
            // Store the result in the argument which will be passed on once the method has exited
            args.Result = bestMove;
        }

        /// <summary>
        /// Picks a book move randomly weighted.
        /// </summary>
        /// <param name="board">The board to apply the move to.</param>
        /// <param name="prevMoves">The list of previous moves played.</param>
        /// <param name="openingBook"></param>
        /// <returns>The move to play.</returns>
        private IMove PickRandomWeightedBookMove(Board board, List<string> prevMoves, Stream? openingBook)
        {
            List<string?> lines = new();
            var possibleMoves = new List<string?>();
            if (openingBook != null)
            {
                using StreamReader reader = new(openingBook);
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }

            // Loop and split each line to get each move seperately
            foreach (var stringMoves in lines.Select(line => line?.Split(' ')))
            {
                // If the AI makes the first move
                if (board.PlyCount == 0)
                {
                    // Add the first move of the line to the list of potential moves
                    possibleMoves.Add(stringMoves?[0]);
                }
                else
                {
                    // If previous moves match with the sequence of moves from the current line of the text file,
                    // Add the move following the matched sequence to the list of potential moves
                    if (PreviousMoveIsSame(board.PlyCount - 1, prevMoves, stringMoves))
                        possibleMoves.Add(stringMoves?[board.PlyCount]);
                }
            }

            // If no potential moves were found
            if (possibleMoves.Count == 0)
            {
                // We don't want to evaluate book moves anymore because there are no more matches
                _useBook = false;
                
                // Search for a move using AB pruning
                return Search.SearchMove(board);
            }

            // Generate a random index for the list of moves
            var random = new Random(DateTime.Now.GetHashCode());
            var index = random.Next(0, possibleMoves.Count);
            
            // Convert the move and return it
            return PgnParser.FromSan(board, possibleMoves[index]);
        }

        /// <summary>
        /// Checks if the move at the passed in ply is the same as a move in a sequence at the same ply.
        /// </summary>
        /// <param name="ply">The ply to check.</param>
        /// <param name="prevMoves">The list of previous moves played.</param>
        /// <param name="stringMoves">The line of moves to check against.</param>
        /// <returns></returns>
        private bool PreviousMoveIsSame(int ply, IReadOnlyList<string> prevMoves, IReadOnlyList<string> stringMoves)
        {
            if (ply > 0)
                // Return false if the previous move is not the same (the sequence does not match)
                if (!PreviousMoveIsSame(ply - 1, prevMoves, stringMoves))
                    return false;
            
            // Return the match of moves at the same index
            return prevMoves[ply] == stringMoves[ply];
        }
    }
}