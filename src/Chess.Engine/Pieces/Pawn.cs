using System.Collections.Generic;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.Pieces
{
    /// <summary>
    ///     This class contains pawn data and methods that it can make such as moving and calculating legal moves.
    /// </summary>
    public sealed class Pawn : Piece
    {
        /// <summary>
        ///     Constructor to create a pawn.
        /// </summary>
        /// <param name="piecePosition">The position on the board to create the piece at.</param>
        /// <param name="pieceCoalition">The colour of the piece.</param>
        /// <param name="isFirstMove">Sets whether this is the pieces first move.</param>
        public Pawn(int piecePosition, Coalition pieceCoalition, bool isFirstMove) :
            base(PieceType.Pawn, piecePosition, pieceCoalition, isFirstMove)
        {
            // Empty
        }

        /// <summary>
        ///     This method generates the legal moves for the pawn, given the board.
        /// </summary>
        /// <param name="board">The current board state.</param>
        /// <returns>An IList of moves that can be made.</returns>
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
                // Initialise destination coordinate to piece position plus the direction * position offset.
                // Multiplied by direction because pawns are unidirectional pieces
                var destinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * positionOffset;

                // If not in range, skip offset
                if (!IsValidTileCoordinate(destinationCoordinate)) continue;

                // There are many cases for a pawn to move so a switch statement is more efficient
                switch (positionOffset)
                {
                    // If the position offset is 8 and the tile is empty,
                    case 8 when !board.GetTile(destinationCoordinate).IsOccupied():
                    {
                        // Add a move depending on if the destination is a pawn promotion square
                        // If it is, then add promotion move, otherwise add a normal move
                        if (IsPromotion(destinationCoordinate))
                        {
                            AddNonCapturePromotionMoves(board, moves, destinationCoordinate);
                        }
                        else
                        {
                            // Add a normal move
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                        }
                        break;
                    }
                    // If the position offset is 16 and it is the pawns first move (pawn jump move),
                    case 16 when IsFirstMove:
                    {
                        // Get the tile position that is 1 in front of the pawn
                        var behindDestinationCoordinate = PiecePosition + PieceCoalition.GetDirection() * 8;
                        // If neither of the tiles are occupied, then add a normal move
                        if (!board.GetTile(behindDestinationCoordinate).IsOccupied() &&
                            !board.GetTile(destinationCoordinate).IsOccupied())
                            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate));
                        break;
                    }
                    // If the position offset is neither 16 or 8, then generate moves like other pieces.
                    case 7 or 9:
                    {
                        // If not in special file case, 
                        if (!IsColumnExclusion(PiecePosition, positionOffset))
                        {
                            var tile = board.GetTile(destinationCoordinate);
                            // Pawns can only attack diagonally forward so we check for enemy at tile and then create an attacking move if there is.
                            if (tile.IsOccupied())
                            {
                                // If there is an enemy piece
                                if (IsEnemyPieceAtTile(tile))
                                {
                                    // Add a move depending on if the destination is a pawn promotion square
                                    // If it is, then add a promotion attack move, otherwise add a normal attack move
                                    if (IsPromotion(destinationCoordinate))
                                    {
                                        AddCapturePromotionMoves(board, moves, destinationCoordinate, tile.Piece);
                                    }
                                    else
                                    {
                                        // Add normal capture move
                                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                                            MoveType.CaptureMove, tile.Piece));
                                    }
                                }
                            }
                            
                            // If the tile is empty but the en passant pawn exists
                            else if (board.EnPassantPawn is not null)
                            {
                                // Method scope method for adding the capture move
                                void AddEnPassantCaptureMove()
                                {
                                    // Get the en passant pawn
                                    Piece pieceAtTile = board.EnPassantPawn;
                                    
                                    // If they are enemy pieces
                                    if (PieceCoalition != pieceAtTile.PieceCoalition)
                                    {
                                        // Add a capture move
                                        moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.CaptureMove, pieceAtTile, isEnPassant: true));
                                    }   
                                }
                                
                                // Switch based on capture direction (7 - left, 9 - right)
                                switch (positionOffset)
                                {
                                    // If 7
                                    case 7:
                                    {
                                        // If the en passant pawn is to the left of the attacking pawn
                                        if (board.EnPassantPawn.PiecePosition == PiecePosition + PieceCoalition.GetDirection() * -1)
                                        {
                                            // Add the capture move
                                            AddEnPassantCaptureMove();
                                        }
                                        break;
                                    }
                                    // If 9
                                    case 9:
                                    {
                                        // If the en passant pawn is to the right of the attacking pawn
                                        if (board.EnPassantPawn.PiecePosition == PiecePosition - PieceCoalition.GetDirection() * -1)
                                        {
                                            // Add the capture move
                                            AddEnPassantCaptureMove();
                                        }
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
        /// Adds capture promotion moves to a move list.
        /// </summary>
        /// <param name="board">The current board.</param>
        /// <param name="moves">The list of moves to add to.</param>
        /// <param name="destinationCoordinate">The destination coordinate of the promotion move.</param>
        /// <param name="capturedPiece">The captured piece.</param>
        private void AddCapturePromotionMoves(Board board, List<IMove> moves, int destinationCoordinate, Piece capturedPiece)
        {
            // Add a queen promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.QueenLookup[destinationCoordinate, PieceCoalition]));

            // Add a rook promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.RookLookup[destinationCoordinate, PieceCoalition]));

            // Add a bishop promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.BishopLookup[destinationCoordinate, PieceCoalition]));

            // Add a knight promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate,
                MoveType.PromotionMove, capturedPiece,
                promotedPiece: PieceUtilities.KnightLookup[destinationCoordinate, PieceCoalition]));
        }

        /// <summary>
        /// Adds capture promotion moves to a move list.
        /// </summary>
        /// <param name="board">The current board.</param>
        /// <param name="moves">The list of moves to add to.</param>
        /// <param name="destinationCoordinate">The destination coordinate of the promotion move.</param>
        private void AddNonCapturePromotionMoves(Board board, List<IMove> moves, int destinationCoordinate)
        {
            // Add a queen promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.PromotionMove,
                promotedPiece: PieceUtilities.QueenLookup[destinationCoordinate, PieceCoalition]));

            // Add a rook promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.PromotionMove,
                promotedPiece: PieceUtilities.RookLookup[destinationCoordinate, PieceCoalition]));

            // Add a bishop promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.PromotionMove,
                promotedPiece: PieceUtilities.BishopLookup[destinationCoordinate, PieceCoalition]));

            // Add a knight promotion
            moves.Add(MoveFactory.CreateMove(board, this, destinationCoordinate, MoveType.PromotionMove,
                promotedPiece: PieceUtilities.KnightLookup[destinationCoordinate, PieceCoalition]));
        }

        /// <summary>
        /// Checks if a tile coordinate is on a promotion rank.
        /// </summary>
        /// <param name="destinationCoordinate">The tile coordinate to check.</param>
        /// <returns>True if it is on the promotion rank.</returns>
        private bool IsPromotion(int destinationCoordinate)
        {
            // If the pawn is white, return true if the coordinate is on the 7th index rank (0 offset so 8th rank)
            // Otherwise return true if the the coordinate is on the 0th index (1st rank)
            // Each colour is trying to reach the other side of the board to promote
            return PieceCoalition.IsWhite()
                ? RankIndex(destinationCoordinate) == 7
                : RankIndex(destinationCoordinate) == 0;
        }

        /// <summary>
        ///     This method moves the pawn by utilising passed in move data.
        /// </summary>
        /// <param name="move">The move struct containing the data needed to make a move.</param>
        /// <returns>A piece at the destination location.</returns>
        public override Piece MovePiece(IMove move)
        {
            // Return the pawn at the lookup table location given the two indexes passed in
            return PieceUtilities.PawnLookup[move.ToCoordinate, move.MovedPiece.PieceCoalition];
        }

        private bool IsColumnExclusion(int currentPosition, int offset)
        {
            // Pawn is on special edge case when its position is on the eighth file
            // AND the offset is 9 AND it is white (going left).
            // The second special edge case is when its position is on the first file
            // AND the offset is 9 AND it is black (going left)

            // Pawn is on special edge case when its position is on the first file
            // AND the offset is 7 AND it is white (going right).
            // The second special edge case is when its position is on the eighth file
            // AND the offset is 7 AND it is black (going right) 
            return FileIndex(currentPosition) == 7 && PieceCoalition.IsWhite() && offset is  9||
                   FileIndex(currentPosition) == 0 && !PieceCoalition.IsWhite() && offset is 9||
                   FileIndex(currentPosition) == 0 && PieceCoalition.IsWhite() && offset is 7||
                   FileIndex(currentPosition) == 7 && !PieceCoalition.IsWhite() && offset is 7;
        }
    }
}