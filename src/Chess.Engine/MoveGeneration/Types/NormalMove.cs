using System;
using Engine.BoardRepresentation;
using Engine.Pieces;

namespace Engine.MoveGeneration.Types;

/// <inheritdoc cref="IMove" />
/// Refer to IMove.cs for description of abstractions.
/// <summary>
///     This struct stores normal move data.
/// </summary>
public readonly struct NormalMove : IMove, IEquatable<NormalMove>
{
    public Board Board { get; }
    public int FromCoordinate { get; }
    public int ToCoordinate { get; }
    public Piece MovedPiece { get; }

    /// <summary>
    ///     Constructor creates a non capture move.
    /// </summary>
    /// <param name="board">The current board that the move is to be executed on.</param>
    /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
    /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
    /// <param name="movedPiece">The piece to be moved.</param>
    public NormalMove(Board board, int fromCoordinate, int toCoordinate, Piece movedPiece)
    {
        Board = board;
        FromCoordinate = fromCoordinate;
        ToCoordinate = toCoordinate;
        MovedPiece = movedPiece;
    }

    public Board ExecuteMove()
    {
        var boardBuilder = new BoardBuilder();
        foreach (var piece in Board.AllPieces)
            if (!MovedPiece.Equals(piece))
                boardBuilder.SetPieceAtTile(piece);

        boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this));
        if ((ToCoordinate - FromCoordinate == 16 || ToCoordinate - FromCoordinate == -16) && MovedPiece is Pawn)
            boardBuilder.SetEnPassantPawn((Pawn)MovedPiece.MovePiece(this));

        boardBuilder.SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition)
            .SetPlyCount(Board.PlyCount + 1);

        return boardBuilder.BuildBoard();
    }

    public bool Equals(NormalMove other)
    {
        return Equals(Board, other.Board) && FromCoordinate == other.FromCoordinate &&
               ToCoordinate == other.ToCoordinate && Equals(MovedPiece, other.MovedPiece);
    }

    public override bool Equals(object? obj)
    {
        return obj is NormalMove other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Board, FromCoordinate, ToCoordinate, MovedPiece);
    }

    public static bool operator ==(NormalMove left, NormalMove right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NormalMove left, NormalMove right)
    {
        return !left.Equals(right);
    }
}