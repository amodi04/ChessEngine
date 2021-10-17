using System.Text;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.IO
{
    /// <summary>
    /// Handles the parsing of PGN strings
    /// </summary>
    public class PgnParser
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
        
        /// <summary>
        ///     Utility method to convert a move struct into the SAN (Standard Algebraic Notation) format.
        /// </summary>
        /// <param name="move">The move to convert.</param>
        /// <param name="boardTransition">The from and to board data for getting move context.</param>
        /// <returns>A string representation of the move.</returns>
        public static string ToSan(IMove move, BoardTransition boardTransition)
        {
            var notation = new StringBuilder();
            var fromBoard = boardTransition.FromBoard;
            var toBoard = boardTransition.ToBoard;
            var movedPieceType = move.MovedPiece.Type;
            
            if (move is CastlingMove castlingMove)
            {
                // Get the change in position of the king
                var delta = castlingMove.ToCoordinate - castlingMove.FromCoordinate;
                return delta > 0 ? "O-O" : "O-O-O";
            }

            // If it is a pawn, then append an empty string, otherwise append the uppercase abbreviation for the piece.
            // This is because the SAN format does not use an abbreviation for pawns
            // Passing in Coalition.White forces the uppercase abbreviation
            notation.Append(
                movedPieceType == PieceType.Pawn ? "" : movedPieceType.ToAbbreviation(Coalition.White));
            
            if (movedPieceType != PieceType.Pawn && movedPieceType != PieceType.King)
                foreach (var altMove in fromBoard.AllMoves)
                {
                    // Move to next move if coordinates are different
                    if (altMove.FromCoordinate == move.FromCoordinate ||
                        altMove.ToCoordinate != move.ToCoordinate) continue;
                    
                    if (altMove.MovedPiece.Type != movedPieceType) continue;
                    
                    var fromFileIndex = FileIndex(move.FromCoordinate);
                    var altFromFileIndex = FileIndex(altMove.FromCoordinate);
                    var fromRankIndex = RankIndex(move.FromCoordinate);
                    var altFromRankIndex = RankIndex(move.FromCoordinate);
                    
                    if (fromFileIndex != altFromFileIndex)
                    {
                        // Append the file letter and break because we have found a solution to the ambiguity
                        notation.Append(FileNames[fromFileIndex]);
                        break;
                    }
                    
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
            
            var enPassant = false;
            
            if (move is CaptureMove captureMove)
            {
                if (captureMove.MovedPiece.Type == PieceType.Pawn)
                    notation.Append(FileNames[FileIndex(captureMove.FromCoordinate)]);

                // Append an x to denote a capture
                notation.Append('x');
                enPassant = captureMove.IsEnPassant;
            }
            
            if (move is PromotionMove promotionMove)
            {
                // Promotion move format is:
                // {toSquare}={PieceType} or
                // {fromFile} 'x' {toSquare}={PieceType} for capture promotions
                // e.g. e8=Q means pawn to e8 has promoted to a queen
                notation.Append(FileNames[FileIndex(promotionMove.FromCoordinate)]);
                
                if (promotionMove.CapturedPiece != null)
                {
                    // Append 'x' for capture move
                    notation.Append('x');
                    
                    notation.Append(FileNames[FileIndex(move.ToCoordinate)]);
                    notation.Append(RankNames[RankIndex(move.ToCoordinate)]);
                }
                else
                {
                    notation.Append(RankNames[RankIndex(move.FromCoordinate)]);
                }

                // Passing in Coalition.White forces the uppercase abbreviation
                notation.Append($"={promotionMove.PromotedPiece.Type.ToAbbreviation(Coalition.White)}");
            }
            else
            {
                notation.Append(FileNames[FileIndex(move.ToCoordinate)]);
                notation.Append(RankNames[RankIndex(move.ToCoordinate)]);
            }
            
            if (enPassant)
                notation.Append(" e.p.");
            
            if (toBoard.CurrentPlayer.IsInCheckmate())
                notation.Append('#');
            
            else if (toBoard.CurrentPlayer.IsInCheck())
                notation.Append('+');
            // Append "½-½" if stalemate (no official symbol for stalemate)
            else if (toBoard.CurrentPlayer.IsInStalemate()) notation.Append("½-½");
            
            return notation.ToString();
        }

        /// <summary>
        /// Utility method to convert a SAN string to a valid move.
        /// </summary>
        /// <param name="board">The board that the move will be executed on.</param>
        /// <param name="san">The string to parse.</param>
        /// <returns>The move matching the SAN string.</returns>
        public static IMove FromSan(Board board, string san)
        {
            IMove move = new NormalMove();
            foreach (var moveToTest in board.CurrentPlayer.Moves)
            {
                move = moveToTest;

                var fromCoordinate = move.FromCoordinate;
                var toCoordinate = move.ToCoordinate;
                var pieceType = move.MovedPiece.Type;

                if (san == "O-O")
                {
                    // Return the king side castle move
                    if (pieceType == PieceType.King && toCoordinate - fromCoordinate == 2) return move;
                }
                else if (san == "O-O-O")
                {
                    // Return the queen side castle move
                    if (pieceType == PieceType.King && toCoordinate - fromCoordinate == -2) return move;
                }

                // Is pawn move if starts with any file indicator (e.g. 'e'4). Uppercase B is used for bishops 
                else if (FileNames.Contains(san[0].ToString()))
                {
                    if (pieceType != PieceType.Pawn) continue;

                    // Correct starting file
                    if (FileNames.IndexOf(san[0]) == FileIndex(fromCoordinate))
                    {
                        // Is promotion
                        if (san.Contains("="))
                        {
                            if (RankIndex(toCoordinate) == 0 || RankIndex(toCoordinate) == 7)
                            {
                                // Pawn is capturing to promote
                                if (san.Length == 5)
                                {
                                    var targetFile = san[1];
                                    // Skip if not moving to correct file
                                    if (FileNames.IndexOf(targetFile) != FileIndex(toCoordinate)) continue;
                                }

                                var promotionChar = san[^1];

                                // Skip this move, incorrect promotion type
                                if (pieceType.ToAbbreviation(Coalition.White) != promotionChar.ToString()) continue;

                                return move;
                            }
                        }
                        else
                        {
                            var targetFile = san[^2];
                            var targetRank = san[^1];

                            // Correct ending file
                            if (FileNames.IndexOf(targetFile) == FileIndex(toCoordinate))
                                // Correct ending rank
                                if (RankNames.IndexOf(targetRank) == RankIndex(toCoordinate))
                                    break;
                        }
                    }
                }
                // Regular piece move
                else
                {
                    var movePieceChar = san[0];
                    // Skip this move, incorrect move piece type
                    if (pieceType.ToAbbreviation(Coalition.White) != movePieceChar.ToString()) continue;

                    var targetFile = san[^2];
                    var targetRank = san[^1];
                    // Correct ending file
                    if (FileNames.IndexOf(targetFile) == FileIndex(toCoordinate))
                        // Correct ending rank
                        if (RankNames.IndexOf(targetRank) == RankIndex(toCoordinate))
                        {
                            // Additional char present for disambiguation
                            if (san.Length == 4)
                            {
                                var disambiguationChar = san[1];
                                
                                // Is file disambiguation
                                if (FileNames.Contains(disambiguationChar.ToString()))
                                {
                                    // Incorrect starting file
                                    if (FileNames.IndexOf(disambiguationChar) != FileIndex(fromCoordinate)) continue;
                                }
                                // Is rank disambiguation
                                else
                                {
                                    // Incorrect starting rank
                                    if (RankNames.IndexOf(disambiguationChar) != RankIndex(fromCoordinate)) continue;
                                }
                            }

                            break;
                        }
                }
            }

            return move;
        }
    }
}