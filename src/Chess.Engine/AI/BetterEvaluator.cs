using System;
using System.Collections.Generic;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.Pieces;
using Engine.Player;

namespace Engine.AI;

/// <summary>
///     Class for holding evaluation data
/// </summary>
public class BetterEvaluator : IEvaluator
{
    public const int PawnValue = 100;
    public const int KnightValue = 300;
    public const int BishopValue = 320;
    public const int RookValue = 500;
    public const int QueenValue = 900;

    // Calculate the endgame material start score
    // This makes the AI try to at least keep 2 rooks, 1 bishop, and 1 knight for the endgame.
    private const float EndgameMaterialStart = RookValue * 2 + BishopValue + KnightValue;

    /// <summary>
    ///     Performs a static evaluation of the current board.
    /// </summary>
    /// <param name="board">The board to evaluate.</param>
    /// <param name="depth">The depth of the current search.</param>
    /// <returns>An integer evaluation score.</returns>
    public int Evaluate(Board board, int depth)
    {
        var whiteEval = 0;
        var blackEval = 0;

        var whiteMaterial = CountMaterial(board.WhitePlayer);
        var blackMaterial = CountMaterial(board.BlackPlayer);

        var whiteMaterialWithoutPawns = whiteMaterial - board.WhitePieces.OfType<Pawn>().Count() * PawnValue;
        var blackMaterialWithoutPawns = blackMaterial - board.BlackPieces.OfType<Pawn>().Count() * PawnValue;

        var whiteEndgamePhaseWeight = EndgamePhaseWeight(whiteMaterialWithoutPawns);
        var blackEndgamePhaseWeight = EndgamePhaseWeight(blackMaterialWithoutPawns);

        whiteEval += whiteMaterial;
        blackEval += blackMaterial;

        whiteEval += EvaluatePiecePositionTables(board.WhitePlayer, blackEndgamePhaseWeight);
        blackEval += EvaluatePiecePositionTables(board.BlackPlayer, whiteEndgamePhaseWeight);

        whiteEval += board.BlackPlayer.IsInCheckmate() ? 100000 : 0;
        blackEval += board.WhitePlayer.IsInCheckmate() ? 100000 : 0;

        // If positive, the score is better for white
        // If negative, the score is better for black
        return whiteEval - blackEval;
    }

    /// <summary>
    ///     Calculates the endgame phase weight used for the piece position tables.
    ///     The score shows how far into the end game the current board is.
    /// </summary>
    /// <param name="materialCountWithoutPawns">The piece score without pawns for the player.</param>
    /// <returns>A weight score stored as a float.</returns>
    private float EndgamePhaseWeight(int materialCountWithoutPawns)
    {
        const float multiplier = 1 / EndgameMaterialStart;

        // The minimum is 0 (denoting that the game has not reached end game yet)
        // The maximum tends to 1 denoting that the game has reached the endgame
        return 1 - Math.Min(1, materialCountWithoutPawns * multiplier);
    }

    /// <summary>
    ///     Counts the material score.
    /// </summary>
    /// <param name="player">The player to count the material score for.</param>
    /// <returns>An integer score.</returns>
    private int CountMaterial(Player.Player player)
    {
        var material = 0;

        foreach (var piece in player.GetActiveAlliedPieces())
            if (piece.Type != PieceType.King)
                material += piece.Type switch
                {
                    PieceType.Pawn => PawnValue,
                    PieceType.Knight => KnightValue,
                    PieceType.Bishop => BishopValue,
                    PieceType.Rook => RookValue,
                    PieceType.Queen => QueenValue,
                    _ => throw new ArgumentOutOfRangeException()
                };
        return material;
    }

    /// <summary>
    ///     Evaluates pieces in their piece position tables.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <param name="endgamePhaseWeight">The endgame phase weight (how far into the endgame).</param>
    /// <returns>An integer score.</returns>
    private int EvaluatePiecePositionTables(Player.Player player, float endgamePhaseWeight)
    {
        var value = 0;
        var isWhite = player.Coalition.IsWhite();

        value += EvaluatePiecePositionTable(PiecePositionTable.Pawns, player.GetActiveAlliedPieces().OfType<Pawn>(),
            isWhite);
        value += EvaluatePiecePositionTable(PiecePositionTable.Rooks, player.GetActiveAlliedPieces().OfType<Rook>(),
            isWhite);
        value += EvaluatePiecePositionTable(PiecePositionTable.Knights,
            player.GetActiveAlliedPieces().OfType<Knight>(), isWhite);
        value += EvaluatePiecePositionTable(PiecePositionTable.Bishops,
            player.GetActiveAlliedPieces().OfType<Bishop>(), isWhite);
        value += EvaluatePiecePositionTable(PiecePositionTable.Queens,
            player.GetActiveAlliedPieces().OfType<Queen>(), isWhite);

        var kingEarlyPhase =
            PiecePositionTable.Read(PiecePositionTable.KingMiddle, player.King.PiecePosition, isWhite);

        value += (int)(kingEarlyPhase * (1 - endgamePhaseWeight));

        // Add the value of the king position for the end game (allows the AI to put the king in better positions for the end game)
        value += PiecePositionTable.Read(PiecePositionTable.KingEnd, player.King.PiecePosition, isWhite);

        return value;
    }

    /// <summary>
    ///     Evaluates how good a piece is, given it's position
    /// </summary>
    /// <param name="table">The table to read.</param>
    /// <param name="pieceList">The list of pieces to look at.</param>
    /// <param name="isWhite">Flag used for determining the perspective of the player.</param>
    /// <returns>An integer score.</returns>
    private static int EvaluatePiecePositionTable(int[] table, IEnumerable<Piece> pieceList, bool isWhite)
    {
        // Returns the sum of the scores for each piece in the list read from the table.
        return pieceList.Sum(t => PiecePositionTable.Read(table, t.PiecePosition, isWhite));
    }
}