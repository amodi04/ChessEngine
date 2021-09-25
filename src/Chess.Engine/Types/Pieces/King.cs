using System.Collections.Generic;
using Engine.Enums;
using Engine.Factories;
using Engine.Types.MoveGeneration;
using static Engine.Util.BoardUtilities;

namespace Engine.Types.Pieces
{
    /// <summary>
    ///     This class contains king data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class King : Piece
    {
        // Member fields
        public bool IsCastled { get; }
        public bool KingsideCastleCapable { get; }
        public bool QueensideCastleCapable { get; }

        /// <summary>
        ///     Constructor to create a king.
        /// </summary>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="kingsideCastleCapable">Can the king castle kingside.</param>
        /// <param name="queensideCastleCapable">Can the king castle queenside.</param>
        public King(int piecePosition, Coalition pieceCoalition, bool kingsideCastleCapable, bool queensideCastleCapable) :
            base(PieceType.King, piecePosition, pieceCoalition, true)
        {
            IsCastled = false;
            KingsideCastleCapable = kingsideCastleCapable;
            QueensideCastleCapable = queensideCastleCapable;
        }
        
        /// <summary>
        /// Overloaded constructor. Allows the setting of the king being castled.
        /// </summary>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="isFirstMove">Sets whether this is the pieces first move.</param>
        /// <param name="isCastled">Sets whether the king is castled or not</param>
        /// <param name="kingsideCastleCapable">Can the king castle kingside.</param>
        /// <param name="queensideCastleCapable">Can the king castle queenside.</param>
        public King(int piecePosition, Coalition pieceCoalition, bool isFirstMove, bool isCastled,  bool kingsideCastleCapable, bool queensideCastleCapable) :
            base(PieceType.King, piecePosition, pieceCoalition, isFirstMove)
        {
            // Set the IsCastled property to the value of the passed in parameter isCastled
            IsCastled = isCastled;
            KingsideCastleCapable = kingsideCastleCapable;
            QueensideCastleCapable = queensideCastleCapable;
        }

        /// <summary>
        ///     This method generates the legal moves for the king, given the board.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <returns>An IList of moves that can be made.</returns>
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
                // Initialise destination coordinate to piece position plus it's position offset
                var destinationCoordinate = PiecePosition + positionOffset;

                // If tile not in board or is on an edge case, skip offset.
                if (!IsValidTileCoordinate(destinationCoordinate) ||
                    IsColumnExclusion(PiecePosition, positionOffset))
                    continue;

                var tile = board.GetTile(destinationCoordinate);

                // If empty, create normal move
                if (!tile.IsOccupied())
                {
                    moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                }
                else
                {
                    // If enemy at tile, create attack move
                    if (IsEnemyPieceAtTile(tile))
                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.CaptureMove,
                            tile.Piece));
                }
            }

            return moves;
        }

        /// <summary>
        ///     This method moves the king by utilising passed in move data.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public override Piece MovePiece(IMove move)
        {
            // Return a new king instance at the moved position
            return new King(move.ToCoordinate, move.MovedPiece.PieceCoalition, false, move is CastlingMove, false, false);
        }

        private static bool IsColumnExclusion(int currentPosition, int offset)
        {
            // King is on special edge case when its position is on the first file
            // AND the offset is -9, -1 or 7 (going left).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is -7, 1 or 9 (going right) 
            return FileIndex(currentPosition) == 0 && offset is -9 or -1 or 7 ||
                   FileIndex(currentPosition) == 7 && offset is -7 or 1 or 9;
        }
    }
}