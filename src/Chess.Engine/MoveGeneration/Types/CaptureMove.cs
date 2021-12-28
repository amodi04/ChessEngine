using System;
using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveGeneration.Types;

/// <inheritdoc cref="IMove" />
/// Refer to IMove.cs for description of abstractions.
/// <summary>
///     This struct stores capture move data.
/// </summary>
public readonly struct CaptureMove : IMove, IEquatable<CaptureMove>
{
    public Board Board { get; }
    public int FromCoordinate { get; }
    public int ToCoordinate { get; }
    public Piece MovedPiece { get; }
    public Piece CapturedPiece { get; }
    public bool IsEnPassant { get; }

    /// <summary>
    ///     Constructor creates a capture move.
    /// </summary>
    /// <param name="board">The current board that the move is to be executed on.</param>
    /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
    /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
    /// <param name="movedPiece">The piece to be moved.</param>
    /// <param name="capturedPiece">The piece to be captured.</param>
    /// <param name="isEnPassant">Used for special en passant logic in move execution.</param>
    public CaptureMove(Board board, int fromCoordinate, int toCoordinate, Piece movedPiece, Piece capturedPiece,
        bool isEnPassant)
    {
        Board = board;
        FromCoordinate = fromCoordinate;
        ToCoordinate = toCoordinate;
        MovedPiece = movedPiece;
        CapturedPiece = capturedPiece;
        IsEnPassant = isEnPassant;
    }

    public Board ExecuteMove()
    {
        var boardBuilder = new BoardBuilder();
        foreach (var piece in Board.AllPieces)
            if (!MovedPiece.Equals(piece))
                boardBuilder.SetPieceAtTile(piece);

        if (IsEnPassant)
            // Most capture moves overwrite the captured piece with the moved piece
            // So we only need to remove the piece for an en passant move.
            boardBuilder.RemovePieceAtTile(CapturedPiece);

        boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this))
            .SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition)
            .SetPlyCount(Board.PlyCount + 1);

        return boardBuilder.BuildBoard();
    }

    public bool Equals(CaptureMove other)
    {
        return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
               ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece) &&
               Equals(CapturedPiece, other.CapturedPiece) && IsEnPassant == other.IsEnPassant;
    }

    public override bool Equals(object? obj)
    {
        return obj is CaptureMove other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece, CapturedPiece, IsEnPassant);
    }

    public static bool operator ==(CaptureMove left, CaptureMove right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CaptureMove left, CaptureMove right)
    {
        return !left.Equals(right);
    }
}