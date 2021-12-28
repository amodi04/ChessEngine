using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces;

/// <inheritdoc cref="Piece" />
/// Refer to Piece.cs for description of abstractions.
/// <summary>
///     This class contains rook data and methods that it can make such as moving and calculating legal moves.
/// </summary>
public sealed class Rook : Piece
{
    public Rook(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
        base(PieceType.Rook, piecePosition, pieceCoalition, isFirstMove)
    {
        // Empty
    }

    public override IEnumerable<IMove> GenerateLegalMoves(Board board)
    {
        // Directions that a rook can move in. Stored as vector offset because rooks are sliding pieces.
        /*
         *  -  8  -
         * -1  R  1
         *  - -8  -
         */
        int[] vectorOffsets = { -8, -1, 1, 8 };

        var moves = new List<IMove>();

        foreach (var vectorOffset in vectorOffsets)
        {
            var destinationCoordinate = PiecePosition;
            while (IsValidTileCoordinate(destinationCoordinate))
            {
                if (IsColumnExclusion(destinationCoordinate, vectorOffset)) break;
                destinationCoordinate += vectorOffset;

                if (!IsValidTileCoordinate(destinationCoordinate)) continue;

                var tile = board.GetTile(destinationCoordinate);
                if (!tile.IsOccupied())
                {
                    moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                }
                else
                {
                    if (IsEnemyPieceAtTile(tile))
                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                            MoveType.CaptureMove, tile.Piece));
                    break;
                }
            }
        }

        return moves;
    }

    public override Piece MovePiece(IMove move)
    {
        return PieceUtilities.RookLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
    }

    /// <summary>
    ///     Checks if the piece is moving out of bounds.
    /// </summary>
    /// <param name="currentPosition">The current position of the piece.</param>
    /// <param name="offset">How many tiles the piece is moving by.</param>
    /// <returns>True if moving out of bounds. False otherwise.</returns>
    private static bool IsColumnExclusion(int currentPosition, int offset)
    {
        return FileIndex(currentPosition) == 0 && offset is -1 ||
               FileIndex(currentPosition) == 7 && offset is 1;
    }
}