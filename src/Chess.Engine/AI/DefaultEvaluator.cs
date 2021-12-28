using Engine.BoardRepresentation;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using static Engine.AI.AISettings;

namespace Engine.AI;

/// <summary>
///     Class for holding default evaluation data.
/// </summary>
public class DefaultEvaluator : IEvaluator
{
    /// <summary>
    ///     Gets the integer value of the move.
    /// </summary>
    /// <param name="board">The board for move context.</param>
    /// <param name="depth">The current depth of the search.</param>
    /// <returns>A score value for the board.</returns>
    public int Evaluate(Board board, int depth)
    {
        // A positive score is more beneficial for white
        // A negative score is more beneficial for black
        return Score(board.WhitePlayer, depth) - Score(board.BlackPlayer, depth);
    }

    /// <summary>
    ///     Scores a player.
    /// </summary>
    /// <param name="player">The player to score.</param>
    /// <param name="depth">The current depth of the search.</param>
    /// <returns>An integer score value for the player.</returns>
    private int Score(Player.Player player, int depth)
    {
        return EvaluateMobility(player)
               + EvaluateKingThreats(player, depth)
               + EvaluateAttacks(player)
               + EvaluateCastled(player)
               + EvaluatePieces(player);
    }

    /// <summary>
    ///     Evaluates the piece values of the player.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <returns>An integer evaluation score.</returns>
    private int EvaluatePieces(Player.Player player)
    {
        var pieceEvaluationScore = 0;
        var numBishops = 0;
        var numRooks = 0;

        foreach (var piece in player.GetActiveAlliedPieces())
        {
            pieceEvaluationScore += piece.Type.ToValue();

            switch (piece.Type)
            {
                case PieceType.Bishop:
                    numBishops++;
                    break;
                case PieceType.Rook:
                    numRooks++;
                    break;
            }
        }

        pieceEvaluationScore += numBishops == 2 ? TwoBishopsBonus : 0;
        pieceEvaluationScore += numRooks == 2 ? TwoRooksBonus : 0;
        return pieceEvaluationScore * PieceMultiplier;
    }

    /// <summary>
    ///     Evaluates whether the player is castled.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <returns>An integer evaluation score.</returns>
    private int EvaluateCastled(Player.Player player)
    {
        return player.King.IsCastled ? CastleBonus : 0;
    }

    /// <summary>
    ///     Evaluates the sum of possible attacks.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <returns>An integer evaluation score.</returns>
    private int EvaluateAttacks(Player.Player player)
    {
        var attackScore = 0;

        foreach (var move in player.Moves)
            if (move is CaptureMove captureMove)
                if (captureMove.MovedPiece.Type.ToValue() <= captureMove.CapturedPiece.Type.ToValue())
                    attackScore++;
        return attackScore * AttackMultiplier;
    }

    /// <summary>
    ///     Evaluates the threats on the opponent king.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <param name="depth">The current search depth.</param>
    /// <returns>An integer evaluation score.</returns>
    private int EvaluateKingThreats(Player.Player player, int depth)
    {
        return player.GetOpponent().IsInCheckmate()
            ? CheckmateBonus * ComputeDepthBonus(depth)
            : ComputeCheck(player);
    }

    /// <summary>
    ///     Evaluates check on the opponent king.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <returns>An integer evaluation score.</returns>
    private int ComputeCheck(Player.Player player)
    {
        return player.GetOpponent().IsInCheck() ? CheckBonus : 0;
    }

    /// <summary>
    ///     Computes the bonus to apply depending on the depth.
    /// </summary>
    /// <param name="depth">The current depth of the search.</param>
    /// <returns>An integer evaluation score.</returns>
    private int ComputeDepthBonus(int depth)
    {
        return depth == 0 ? 1 : DepthMultiplier * depth;
    }

    /// <summary>
    ///     Evaluate the moves that can be made by the player.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <returns>An integer evaluation score.</returns>
    private int EvaluateMobility(Player.Player player)
    {
        return MobilityMultiplier * ComputeMobilityRatio(player);
    }

    /// <summary>
    ///     Computes the mobility ratio.
    /// </summary>
    /// <param name="player">The player to evaluate for.</param>
    /// <returns>An integer evaluation score.</returns>
    private int ComputeMobilityRatio(Player.Player player)
    {
        return (int)(player.Moves.Count * 10.0f / player.GetOpponent().Moves.Count);
    }
}