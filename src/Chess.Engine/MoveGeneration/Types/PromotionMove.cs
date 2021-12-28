using System;
using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveGeneration.Types;

/// <inheritdoc cref="IMove" />
/// Refer to IMove.cs for description of abstractions.
/// <summary>
///     This struct stores promotion move data.
/// </summary>
public readonly struct PromotionMove : IMove, IEquatable<PromotionMove>
{
    public Board Board { get; }
    public int FromCoordinate { get; }
    public int ToCoordinate { get; }
    public Piece MovedPiece { get; }
    public Piece? CapturedPiece { get; }
    private readonly IMove _wrappedMove;
    private readonly Pawn _promotingPawn;
    public Piece PromotedPiece { get; }

    /// <summary>
    ///     Constructor creates a promotion move
    /// </summary>
    /// <param name="wrappedMove">The move to wrap around.</param>
    /// <param name="promotedPiece">The piece to promote to.</param>
    public PromotionMove(IMove wrappedMove, Piece promotedPiece)
    {
        Board = wrappedMove.Board;
        FromCoordinate = wrappedMove.FromCoordinate;
        ToCoordinate = wrappedMove.ToCoordinate;
        MovedPiece = wrappedMove.MovedPiece;
        _wrappedMove = wrappedMove;
        _promotingPawn = (Pawn)MovedPiece;
        PromotedPiece = promotedPiece;
        CapturedPiece = wrappedMove is CaptureMove captureMove ? captureMove.CapturedPiece : null;
    }

    public Board ExecuteMove()
    {
        // Get the board state including the pawn movement to the promotion rank
        var intermediaryBoard = _wrappedMove.ExecuteMove();
        var boardBuilder = new BoardBuilder();

        foreach (var piece in intermediaryBoard.AllPieces)
            if (!_promotingPawn.Equals(piece))
                boardBuilder.SetPieceAtTile(piece);

        boardBuilder.SetPieceAtTile(PromotedPiece.MovePiece(this));

        // Set player to move to current board player because the intermediary board player to move is the opponent
        // This was set in the previous move execution
        boardBuilder.SetCoalitionToMove(intermediaryBoard.CurrentPlayer.Coalition);

        return boardBuilder.BuildBoard();
    }

    public bool Equals(PromotionMove other)
    {
        return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
               ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
               Equals(_wrappedMove, other._wrappedMove) && Equals(_promotingPawn, other._promotingPawn) &&
               Equals(PromotedPiece, other.PromotedPiece);
    }

    public override bool Equals(object? obj)
    {
        return obj is PromotionMove other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece, _wrappedMove, _promotingPawn,
            PromotedPiece);
    }

    public static bool operator ==(PromotionMove left, PromotionMove right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PromotionMove left, PromotionMove right)
    {
        return !left.Equals(right);
    }
}