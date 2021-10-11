using System.Text;
using Engine.BoardRepresentation;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.MoveGeneration
{
    /// <summary>
    /// This class houses all utilities related to moves.
    /// </summary>
    public static class MoveUtilities
    {
        /// <summary>
        /// Utility method to convert a move struct into the SAN (Standard Algebraic Notation) format.
        /// </summary>
        /// <param name="move">The move to convert.</param>
        /// <param name="boardTransition">The from and to board data for getting move context.</param>
        /// <returns>A string representation of the move.</returns>
        public static string ToSan(IMove move, BoardTransition boardTransition)
        {
            /*
             * --- SAN Format ---
             *
             * <SAN move descriptor piece moves>   ::= <Piece symbol>[<from file>|<from rank>|<from square>]['x']<to square>
             * <SAN move descriptor pawn captures> ::= <from file>[<from rank>] 'x' <to square>[<promoted to>]
             * <SAN move descriptor pawn push>     ::= <to square>[<promoted to>]
             *
             * ------------------
             */
            
            // Initialise string builder
            var notation = new StringBuilder();
            var fromBoard = boardTransition.FromBoard;
            var toBoard = boardTransition.ToBoard;
            
            // Get the piece type of the moved piece
            var movedPieceType = move.MovedPiece.PieceType;
            
            // If the move is a castling move
            if (move is CastlingMove castlingMove)
            {
                // Get the change in position of the king
                int delta = castlingMove.ToCoordinate - castlingMove.FromCoordinate;
                
                // If the delta is negative (e.g. king side castle)
                if (delta > 0)
                {
                    // Return the string "O-O"
                    return "O-O";
                }

                // Else Return the string "O-O-O because it is a queen side castle"
                return "O-O-O";
            }
            
            // If it is a pawn, then append an empty string, otherwise append the uppercase abbreviation for the piece.
            // This is because the SAN format does not use an abbreviation for pawns
            // Passing in Coalition.White forces the uppercase abbreviation
            notation.Append(
                movedPieceType == PieceType.Pawn ? "" : movedPieceType.ToAbbreviation(Coalition.White));
            
            // If the piece is not a pawn and is not a king
            if (movedPieceType != PieceType.Pawn && movedPieceType != PieceType.King)
            {
                
                // Loop through all moves in the board that could be made
                foreach (var altMove in fromBoard.AllMoves)
                {
                    // If the from coordinates are equal but the destination coordinates are not, loop to next move.
                    // This is the case when a piece could make more than one move
                    if (altMove.FromCoordinate == move.FromCoordinate ||
                        altMove.ToCoordinate != move.ToCoordinate) continue;
                    
                    // If the moved piece type is not equal then continue
                    // This is because we want to check for similar pieces making a move to the same destination to check for ambiguities
                    if (altMove.MovedPiece.PieceType != movedPieceType) continue;
                    
                    // Get the from file index
                    var fromFileIndex = FileIndex(move.FromCoordinate);
                    
                    // Get the comparing move from file index
                    var altFromFileIndex = FileIndex(altMove.FromCoordinate);
                    
                    // Get the from rank index
                    var fromRankIndex = RankIndex(move.FromCoordinate);
                    
                    // Get the comparing move from rank index
                    var altFromRankIndex = RankIndex(move.FromCoordinate);

                    // If the moves are on different files
                    if (fromFileIndex != altFromFileIndex)
                    {
                        // Append the file letter and break because we have found a solution to the ambiguity
                        notation.Append(FileNames[fromFileIndex]);
                        break;
                    }

                    // If the moves are on different ranks
                    if (fromRankIndex != altFromRankIndex)
                    {
                        // Append the rank number and break because we have found a solution to the ambiguity
                        notation.Append(RankNames[fromRankIndex]);
                        break;
                    }

                    // Append the file letter and number for from coordinate if no solution to the ambiguity was found
                    // Break because we have exhausted all options
                    notation.Append(FileNames[fromFileIndex]);
                    notation.Append(RankNames[fromRankIndex]);
                    break;
                }
            }

            // Temporary variable to check for en passant
            var enPassant = false;
            
            // If the move is a capture move
            if (move is CaptureMove captureMove)
            {
                // If the capturing piece is a pawn
                if (captureMove.MovedPiece.PieceType == PieceType.Pawn)
                {
                    // Append the from file letter because this is the SAN format for pawn captures
                    notation.Append(FileNames[FileIndex(captureMove.FromCoordinate)]);
                }

                // Append an x to denote a capture
                notation.Append('x');

                // Store if the move is en passant
                enPassant = captureMove.IsEnPassant;
            }

            // If the move is a promotion move
            if (move is PromotionMove promotionMove)
            {
                // Promotion move format is:
                // {toSquare}={PieceType} or
                // {fromFile} 'x' {toSquare}={PieceType} for capture promotions
                // e.g. e8=Q means pawn to e8 has promoted to a queen
                notation.Append(FileNames[FileIndex(promotionMove.FromCoordinate)]);

                // If there is a captured piece (is a Capture Promotion Move)
                if (promotionMove.CapturedPiece != null)
                {
                    // Append 'x' for capture move
                    notation.Append('x');
                    
                    // Append the file letter and rank number for the destination coordinate
                    notation.Append(FileNames[FileIndex(move.ToCoordinate)]);
                    notation.Append(RankNames[RankIndex(move.ToCoordinate)]);
                }
                // Else, append the rank letter because it is not a capture move
                else
                {
                    notation.Append(RankNames[RankIndex(move.FromCoordinate)]);
                }
                
                // Passing in Coalition.White forces the uppercase abbreviation
                notation.Append($"={promotionMove.PromotedPiece.PieceType.ToAbbreviation(Coalition.White)}");
            }
            else
            {
                // Append the file letter and rank number for the destination coordinate
                notation.Append(FileNames[FileIndex(move.ToCoordinate)]);
                notation.Append(RankNames[RankIndex(move.ToCoordinate)]);
            }

            // If the move is an en passant move
            if (enPassant)
            {
                // Append 'e.p.' which signifies en passant
                notation.Append(" e.p.");
            }

            // Append'#' if checkmate move
            if (toBoard.CurrentPlayer.IsInCheckmate())
            {
                notation.Append('#');
            }
            
            // Append '+' if check move
            else if (toBoard.CurrentPlayer.IsInCheck())
            {
                notation.Append('+');
            }
            // Append "½-½" if stalemate (no official symbol for stalemate)
            else if (toBoard.CurrentPlayer.IsInStalemate())
            {
                notation.Append("½-½");
            }

            // Return the string
            return notation.ToString();
        }
    }
}