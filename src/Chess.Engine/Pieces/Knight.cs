using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces;

/// <inheritdoc cref="Piece" />
/// Refer to Piece.cs for description of abstractions.
/// <summary>
///     This class contains knight data and methods that it can make such as moving and calculating legal moves.
/// </summary>
public sealed class Knight : Piece
{
    public Knight(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
        base(PieceType.Knight, piecePosition, pieceCoalition, isFirstMove)
    {
        // Empty
    }

    public override IEnumerable<IMove> GenerateLegalMoves(Board board)
    {
        // Directions that a knight can move in. Stored as position offsets because knights are non-sliding pieces.
        /*
         *     15   17  
         *   6         10
         *        N
         *  -10        -6
         *    -17  -15
         */
        int[] positionOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

        var moves = new List<IMove>();

        foreach (var positionOffset in positionOffsets)
        {
            var destinationCoordinate = PiecePosition + positionOffset;

            if (!IsValidTileCoordinate(destinationCoordinate) ||
                IsColumnExclusion(PiecePosition, positionOffset))
                continue;

            var tile = board.GetTile(destinationCoordinate);

            if (!tile.IsOccupied())
            {
                moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
            }
            else
            {
                if (IsEnemyPieceAtTile(tile))

                    moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.CaptureMove,
                        tile.Piece));
            }
        }

        return moves;
    }

    public override Piece MovePiece(IMove move)
    {
        return PieceUtilities.KnightLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
    }

    /// <summary>
    ///     Checks if the piece is moving out of bounds.
    /// </summary>
    /// <param name="currentPosition">The current position of the piece.</param>
    /// <param name="offset">How many tiles the piece is moving by.</param>
    private static bool IsColumnExclusion(int currentPosition, int offset)
    {
        return FileIndex(currentPosition) == 0 && offset is -17 or -10 or 6 or 15 ||
               FileIndex(currentPosition) == 1 && offset is -10 or 6 ||
               FileIndex(currentPosition) == 6 && offset is -6 or 10 ||
               FileIndex(currentPosition) == 7 && offset is -15 or -6 or 10 or 17;
    }
}