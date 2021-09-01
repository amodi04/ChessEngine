using System.Text;
using Engine.Enums;
using Engine.Extensions;
using Engine.Types;
using Engine.Types.MoveGeneration;
using static Engine.Util.BoardUtilities;

namespace Engine.Util
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
        /// <param name="board">The board that the move was executed on.</param>
        /// <returns>A string representation of the move.</returns>
        public static string ToSAN(IMove move, Board board)
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
            
            // Get the piece type of the moved piece
            var movedPieceType = move.MovedPiece.PieceType;
            
            // If it is a pawn, then append an empty string, otherwise append the normal abbreviation for the piece
            // This is because the SAN format does not use an abbreviation for pawns
            notation.Append(
                movedPieceType == PieceType.Pawn ? "" : movedPieceType.ToAbbreviation(move.MovedPiece.PieceCoalition));
            
            // If the piece is not a pawn and is not a king
            if (movedPieceType != PieceType.Pawn && movedPieceType != PieceType.King)
            {
                
                // Loop through all moves in the board that could be made
                foreach (var altMove in board.AllMoves)
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

            // If the move is a capture move
            if (move is CaptureMove)
            {
                // If the capturing piece is a pawn
                if (move.MovedPiece.PieceType == PieceType.Pawn)
                {
                    // Append the from file letter because this is the SAN format for pawn captures
                    notation.Append(FileNames[FileIndex(move.FromCoordinate)]);
                }

                // Append an x to denote a capture
                notation.Append('x');
            }
            // TODO: check for en passant
            
            
            // Append the file letter and rank number for the destination coordinate
            notation.Append(FileNames[FileIndex(move.ToCoordinate)]);
            notation.Append(RankNames[RankIndex(move.ToCoordinate)]);
            
            // TODO: check for pawn promotion
            
            // TODO: Check for check and checkmate

            // Return the string
            return notation.ToString();
        }
    }
}