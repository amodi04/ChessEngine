using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <inheritdoc cref="Piece" />
    /// Refer to Piece.cs for description of abstractions.
    /// <summary>
    ///     This class contains king data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class King : Piece
    {
        public King(int piecePosition, Coalition pieceCoalition, bool kingsideCastleCapable,
            bool queensideCastleCapable) :
            base(PieceType.King, piecePosition, pieceCoalition, true)
        {
            IsCastled = false;
            KingsideCastleCapable = kingsideCastleCapable;
            QueensideCastleCapable = queensideCastleCapable;
        }
        
        private King(int piecePosition, Coalition pieceCoalition, bool isFirstMove, bool isCastled,
            bool kingsideCastleCapable, bool queensideCastleCapable) :
            base(PieceType.King, piecePosition, pieceCoalition, isFirstMove)
        {
            IsCastled = isCastled;
            KingsideCastleCapable = kingsideCastleCapable;
            QueensideCastleCapable = queensideCastleCapable;
        }
        
        public bool IsCastled { get; }
        public bool KingsideCastleCapable { get; }
        public bool QueensideCastleCapable { get; }
        
        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a king can move in. Stored as position offsets because kings are non-sliding pieces.
            /*
             *  7  8  9
             * -1  K  1
             * -9 -8 -7
             */
            int[] positionOffsets = {-9, -8, -7, -1, 1, 7, 8, 9};

            var moves = new List<IMove>();

            foreach (var positionOffset in positionOffsets)
            {
                var destinationCoordinate = PiecePosition + positionOffset;

                // If tile not in board or is on an edge case, skip offset.
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
            // Return a new king instance at the moved position
            return new King(move.ToCoordinate, move.MovedPiece.PieceCoalition, false, move is CastlingMove, false,
                false);
        }

        /// <summary>
        /// Checks if the piece is moving out of bounds.
        /// </summary>
        /// <param name="currentPosition">The current position of the piece.</param>
        /// <param name="offset">How many tiles the piece is moving by.</param>
        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // When the king is on the edge files and moving out of bounds
            return FileIndex(currentPosition) == 0 && offset is -9 or -1 or 7 ||
                   FileIndex(currentPosition) == 7 && offset is -7 or 1 or 9;
        }
    }
}