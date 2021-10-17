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
    ///     This class contains pawn data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class Pawn : Piece
    {
        public Pawn(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Pawn, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }
        
        public override IEnumerable<IMove> GenerateLegalMoves(Board board)
        {
            // Directions that a pawn can move in. Stored as position offsets because pawns are non-sliding pieces.
            /*
             *     16
             *  7  8  9
             *  -  P  -
             *  -  -  -
             */
            int[] positionOffsets = {7, 8, 9, 16};

            var moves = new List<IMove>();

            foreach (var positionOffset in positionOffsets)
            {
                // Multiplied by direction because pawns are unidirectional pieces
                var destinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * positionOffset;

                // If not in board bounds, skip offset
                if (!IsValidTileCoordinate(destinationCoordinate)) continue;
                
                switch (positionOffset)
                {
                    // Normal move forward
                    case 8 when !board.GetTile(destinationCoordinate).IsOccupied():
                    {
                        if (IsPromotion(destinationCoordinate))
                            AddPromotionMoves(board, moves, destinationCoordinate, null);
                        else
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                        break;
                    }
                    // Pawn jump
                    case 16 when IsFirstMove:
                    {
                        // Get the tile position that is 1 in front of the pawn
                        var behindDestinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * 8;
                        
                        if (!board.GetTile(behindDestinationCoordinate).IsOccupied() &&
                            !board.GetTile(destinationCoordinate).IsOccupied())
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                        break;
                    }
                    // Attacking diagonally
                    case 7 or 9:
                    {
                        // If not on edge case
                        if (!IsColumnExclusion(PiecePosition, positionOffset))
                        {
                            var tile = board.GetTile(destinationCoordinate);
                            
                            if (tile.IsOccupied())
                            {
                                if (IsEnemyPieceAtTile(tile))
                                {
                                    if (IsPromotion(destinationCoordinate))
                                        AddPromotionMoves(board, moves, destinationCoordinate, tile.Piece);
                                    else
                                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                                            MoveType.CaptureMove, tile.Piece));
                                }
                            }
                            
                            else if (board.EnPassantPawn is not null)
                            {
                                // Define method only if an en passant pawn exists
                                void AddEnPassantCaptureMove()
                                {
                                    Piece enPassantPawn = board.EnPassantPawn;
                                    if (PieceCoalition != enPassantPawn.PieceCoalition)
                                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                                            MoveType.CaptureMove, enPassantPawn, isEnPassant: true));
                                }
                                
                                switch (positionOffset)
                                {
                                    case 7:
                                    {
                                        // If the en passant pawn is to the left of the attacking pawn
                                        if (board.EnPassantPawn.PiecePosition ==
                                            PiecePosition + PieceCoalition.GetDirection() * -1)
                                            AddEnPassantCaptureMove();
                                        break;
                                    }
                                    case 9:
                                    {
                                        // If the en passant pawn is to the right of the attacking pawn
                                        if (board.EnPassantPawn.PiecePosition ==
                                            PiecePosition - PieceCoalition.GetDirection() * -1)
                                            AddEnPassantCaptureMove();
                                        break;
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return moves;
        }

        /// <summary>
        ///     Adds promotion moves to a move list.
        /// </summary>
        /// <param name="board">The current board.</param>
        /// <param name="moves">The list of moves to add to.</param>
        /// <param name="destinationCoordinate">The destination coordinate of the promotion move.</param>
        /// <param name="capturedPiece">The captured piece.</param>
        private void AddPromotionMoves(Board board, List<IMove> moves, int destinationCoordinate,
            Piece capturedPiece)
        {
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.QueenLookup[destinationCoordinate, PieceCoalition]));
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.RookLookup[destinationCoordinate, PieceCoalition]));
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.BishopLookup[destinationCoordinate, PieceCoalition]));
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.KnightLookup[destinationCoordinate, PieceCoalition]));
        }

        /// <summary>
        ///     Checks if a tile coordinate is on a promotion rank.
        /// </summary>
        /// <param name="destinationCoordinate">The tile coordinate to check.</param>
        /// <returns>True if it is on the promotion rank.</returns>
        private bool IsPromotion(int destinationCoordinate)
        {
            return PieceCoalition.IsWhite()
                ? RankIndex(destinationCoordinate) == 7
                : RankIndex(destinationCoordinate) == 0;
        }
        
        public override Piece MovePiece(IMove move)
        {
            return PieceUtilities.PawnLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        /// <summary>
        /// Checks if the piece is moving out of bounds.
        /// </summary>
        /// <param name="currentPosition">The current position of the piece.</param>
        /// <param name="offset">How many tiles the piece is moving by.</param>
        /// <returns>True if moving out of bounds. False otherwise.</returns>
        private bool IsColumnExclusion(int currentPosition, int offset)
        {
            // When pawn is on edge files and moving out of bounds
            return FileIndex(currentPosition) == 7 && PieceCoalition.IsWhite() && offset is 9 ||
                   FileIndex(currentPosition) == 0 && !PieceCoalition.IsWhite() && offset is 9 ||
                   FileIndex(currentPosition) == 0 && PieceCoalition.IsWhite() && offset is 7 ||
                   FileIndex(currentPosition) == 7 && !PieceCoalition.IsWhite() && offset is 7;
        }
    }
}