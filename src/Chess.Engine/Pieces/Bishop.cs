using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <inheritdoc cref="Piece" />
    /// Refer to Piece.cs for description of abstractions.
    /// <summary>
    ///     This class contains bishop data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class Bishop : Piece
    {
        public Bishop(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Bishop, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }
        
        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a bishop can move in. Stored as vector offsets because bishops are sliding pieces.
            /*
             *  7     9
             *     B   
             * -9    -7
             */
            int[] vectorOffsets = {-9, -7, 7, 9};

            var moves = new List<IMove>();

            foreach (var vectorOffset in vectorOffsets)
            {
                var destinationCoordinate = PiecePosition;

                // Keep looping while the destination coordinate is on the board
                while (IsValidTileCoordinate(destinationCoordinate))
                {
                    // Skip offset evaluation of tile if the bishop is dealing with an edge case
                    if (IsColumnExclusion(destinationCoordinate, vectorOffset)) break;
                    
                    destinationCoordinate += vectorOffset;
                    
                    // Skip to next offset when all valid diagonals in one direction has been covered.
                    if (!IsValidTileCoordinate(destinationCoordinate)) continue;

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
                        break;
                    }
                }
            }
            
            return moves;
        }
        
        public override Piece MovePiece(IMove move)
        {
            return PieceUtilities.BishopLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        /// <summary>
        /// Checks if the piece is moving out of bounds.
        /// </summary>
        /// <param name="currentPosition">The current position of the piece.</param>
        /// <param name="offset">How many tiles the piece is moving by.</param>
        /// <returns>True if moving out of bounds. False otherwise.</returns>
        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // When the bishop is on the edge files and moving out of bounds
            return FileIndex(currentPosition) == 0 && offset is -9 or 7 ||
                   FileIndex(currentPosition) == 7 && offset is -7 or 9;
        }
    }
}