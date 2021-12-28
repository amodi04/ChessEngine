using System.Runtime.InteropServices;
using Engine.MoveGeneration;

namespace Engine.BoardRepresentation;

/// <summary>
///     Hash Table for storing board states
/// </summary>
public class TranspositionTable
{
    public const int LookupFailed = int.MinValue;

    // The value for this position is the exact evaluation
    public const int Exact = 0;

    // A move was found during the search that was too good, meaning the opponent will play a different move earlier on,
    // not allowing the position where this move was available to be reached. Because the search cuts off at
    // this point (beta cut-off), an even better move may exist. This means that the evaluation for the
    // position could be even higher, making the stored value the lower bound of the actual value.
    public const int LowerBound = 1;

    // No move during the search resulted in a position that was better than the current player could get from playing a
    // different move in an earlier position (i.e eval was <= alpha for all moves in the position).
    // Due to the way alpha-beta search works, the value we get here won't be the exact evaluation of the position,
    // but rather the upper bound of the evaluation. This means that the evaluation is, at most, equal to this value.
    public const int UpperBound = 2;
    private readonly Board _board;

    private readonly Entry[] _entries;
    private readonly ulong _size;

    public TranspositionTable(Board board, int size)
    {
        _board = board;
        _size = (ulong)size;

        _entries = new Entry[size];
    }

    private ulong Index => _board.Zobristkey % _size;

    /// <summary>
    ///     Clears the hash table of all entries
    /// </summary>
    public void Clear()
    {
        for (var i = 0; i < _entries.Length; i++) _entries[i] = new Entry();
    }

    public IMove? GetStoredMove()
    {
        return _entries[Index].Move;
    }

    /// <summary>
    ///     Attempts to find the evaluation of the board being searched.
    /// </summary>
    /// <param name="depth">The depth we are searching at.</param>
    /// <param name="alpha">The maximum value.</param>
    /// <param name="beta">The minimum value.</param>
    /// <returns>An integer score of the board. Minimum integer number returned if board was not stored in the table.</returns>
    public int LookupEvaluation(int depth, int alpha, int beta)
    {
        var entry = _entries[Index];

        if (entry.Key == _board.Zobristkey)
            // Only use the stored evaluation if it has been searched to at least the same depth as would be searched now
            if (entry.Depth >= depth)
            {
                var score = entry.Value;

                switch (entry.NodeType)
                {
                    // We have stored the exact evaluation, so return it
                    case Exact:
                        return score;
                    // We have stored the upper bound of the evaluation.
                    // If it's less than alpha then we don't need to search the moves in this position because they are worse.
                    // Otherwise a search must take place to find the exact value
                    case UpperBound when score <= alpha:
                        return score;
                    // We have stored the lower bound of the evaluation.
                    // Only return if it causes a beta cut-off.
                    case LowerBound when score >= beta:
                        return score;
                }
            }

        return LookupFailed;
    }

    /// <summary>
    ///     Stores the evaluation of the current board.
    /// </summary>
    /// <param name="depth">The depth at which the board was searched at.</param>
    /// <param name="eval">The value of the evaluation.</param>
    /// <param name="evalType">The node type.</param>
    /// <param name="move">The move to store</param>
    public void StoreEvaluation(int depth, int eval, int evalType, IMove? move)
    {
        _entries[Index] = new Entry(_board.Zobristkey, eval, move, (byte)depth, (byte)evalType);
    }

    /// <summary>
    ///     Holds evaluation data in the transposition table
    /// </summary>
    public readonly struct Entry
    {
        public readonly ulong Key;
        public readonly int Value;
        public readonly IMove? Move;
        public readonly byte Depth;
        public readonly byte NodeType;

        public Entry(ulong key, int value, IMove? move, byte depth, byte nodeType)
        {
            Key = key;
            Value = value;
            Move = move;
            Depth = depth;
            NodeType = nodeType;
        }

        /// <summary>
        ///     Gets the size of the Entry struct.
        /// </summary>
        /// <returns>The number of bytes that the Entry struct uses.</returns>
        public static int GetSize()
        {
            return Marshal.SizeOf<Entry>();
        }
    }
}