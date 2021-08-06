using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.Enums;
using Engine.Extensions;
using Engine.MoveGeneration;
using Engine.Opposition;
using Engine.Util;
using static Engine.Util.BoardUtilities;

namespace Engine.Pieces
{
    public class Pawn : Piece
    {
        /// <inheritdoc cref="Piece" />
        public Pawn(int piecePosition, Coalition pieceCoalition) :
            base(PieceType.Pawn, piecePosition, pieceCoalition)
        {
            // Empty
        }

        public override IList GenerateLegalMoves(Board board)
        {
            // Directions that a pawn can move in. Stored as position offsets because pawns are non-sliding pieces.
            /*
             *     16
             *  7  8  9
             *  -  P  -
             *  -  -  -
             */
            int[] positionOffsets = {7, 8, 9, 16};

            var moves = new List<Move>();

            foreach (var positionOffset in positionOffsets)
            {
                // Initialise destination coordinate to piece position plus the direction * position offset.
                // Multiplied by direction because pawns are unidirectional pieces
                var destinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * positionOffset;

                // If not in range, skip offset
                if (!IsValidTileCoordinate(destinationCoordinate)) continue;

                // There are many cases for a pawn to move so a switch statement is more efficient
                switch (positionOffset)
                {
                    // If the position offset is 8 and the tile is empty, create a normal move
                    case 8 when !board.GetTile(destinationCoordinate).IsOccupied():
                        // TODO: Check for promotion
                        moves.Add(new Move());
                        break;
                    // If the position offset is 16 and it is the pawns first move (pawn jump move),
                    case 16 when IsFirstMove:
                    {
                        // Get the tile position that is 1 in front of the pawn
                        var behindDestinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * 8;
                        // If neither of the tiles are occupied, then add a normal move
                        if (!board.GetTile(behindDestinationCoordinate).IsOccupied() &&
                            !board.GetTile(destinationCoordinate).IsOccupied())
                            moves.Add(CreateNormalMove(board, destinationCoordinate));
                        break;
                    }
                    // If the position offset is neither 16 or 8, then generate moves like other pieces.
                    default:
                    {
                        // If not in special file case, 
                        if (!IsColumnExclusion(PiecePosition, positionOffset))
                        {
                            var tile = board.GetTile(destinationCoordinate);
                            // Pawns can only attack diagonally forward so we check for enemy at tile and then create an attacking move if there is.
                            if (tile.IsOccupied())
                                if (IsEnemyPieceAtTile(tile))
                                    // TODO: Check for promotion
                                    moves.Add(CreateAttackMove(board, destinationCoordinate, tile.Piece));
                        }

                        break;
                    }
                }
            }

            return moves;
        }

        public override Piece MovePiece(Move move)
        {
            return PieceUtilities.PawnLookup[move.MovedPiece.PiecePosition, move.MovedPiece.PieceCoalition];
        }

        // Pawn is on special edge case when its position is on the eighth file
        // AND the offset is 7 AND it is white (going left).
        // The second special edge case is when its position is on the first file
        // AND the offset is 7 AND it is black (going left)

        // Pawn is on special edge case when its position is on the first file
        // AND the offset is 9 AND it is white (going right).
        // The second special edge case is when its position is on the eighth file
        // AND the offset is 9 AND it is black (going right) 
        protected virtual bool IsColumnExclusion(int currentPosition, int offset)
        {
            return IsInArray(currentPosition, EighthFile) && PieceCoalition.IsWhite() && offset is 7 ||
                   IsInArray(currentPosition, FirstFile) && !PieceCoalition.IsWhite() && offset is 7 ||
                   IsInArray(currentPosition, FirstFile) && PieceCoalition.IsWhite() && offset is 9 ||
                   IsInArray(currentPosition, EighthFile) && !PieceCoalition.IsWhite() && offset is 9;
        }
    }
}