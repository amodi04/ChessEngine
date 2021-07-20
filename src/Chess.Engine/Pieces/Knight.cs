using System;
using System.Collections;
using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveRepresentation;
using Engine.Opposition;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <inheritdoc cref="Piece"/>
    public class Knight : Piece
    {
        public Knight(int piecePosition, Coalition pieceCoalition) : base(piecePosition, pieceCoalition)
        {
            // Empty
        }

        public override IList GenerateLegalMoves(Board board)
        {
            // Directions that a knight can move in. Stored as position offsets because knights are non-sliding pieces.
            /*
             *     15   17  
             *   6         10
             *        N
             *  -10        -6
             *    -17  -15
             */
            int[] positionOffsets = {-17, -15, -10, -6, 6, 10, 15, 17};

            var moves = new List<Move>();

            foreach (var positionOffset in positionOffsets)
            {
                // Initialise destination to piece position plus position offset
                var destinationCoordinate = PiecePosition + positionOffset;
                
                // If tile not in board or is on an edge case, skip offset.
                if (!IsValidTileCoordinate(destinationCoordinate) ||
                    IsColumnExclusion(PiecePosition, positionOffset))
                    continue;

                var tile = board.GetTile(destinationCoordinate);
                // If tile is empty, add normal move
                if (!tile.IsOccupied())
                {
                    moves.Add(CreateNormalMove(board, destinationCoordinate));
                }
                else
                {
                    // If enemy at tile, create attacking move
                    if (IsEnemyPieceAtTile(tile))
                        
                        moves.Add(CreateAttackMove(board, destinationCoordinate, tile.Piece));
                }
            }

            return moves;
        }

        // TODO: Implement this
        public override Piece MovePiece(Move move)
        {
            throw new NotImplementedException();
        }

        protected override bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Knight is on special edge case when its position is on the first file
            // AND the offset is -17, -10, 6 or 15 (going left).
            // The second special edge case is when its position is on the second file
            // AND the offset is -10 or 6 (going left).
            // The third special edge case is when its position is on the seventh file
            // AND the offset is -6 or 10 (going right).
            // The fourth special edge case is when its position is on the eighth file
            // AND the offset is -15, -6, 10 or 15 (going right).
            return IsInArray(currentPosition, FirstFile)
                   && offset is -17 or -10 or 6 or 15 ||
                   IsInArray(currentPosition, SecondFile)
                   && offset is -10 or 6 ||
                   IsInArray(currentPosition, SeventhFile)
                   && offset is -6 or 10 ||
                   IsInArray(currentPosition, EighthFile)
                   && offset is -15 or -6 or 10 or 15;
        }
    }
}