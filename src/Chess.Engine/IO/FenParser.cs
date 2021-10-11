using System;
using System.Collections;
using System.Collections.Generic;
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
    /// This class handles the parsing of FEN strings.
    /// FEN strings allow the board state to be stored as a string so that it can be played on any FEN compliant engine.
    /// It is a form of saving board state.
    /// </summary>
    public static class FenParser
    {
        /// <summary>
        /// This dictionary stores the conversion from a character representing a piece to its relative piece type.
        /// </summary>
        private static readonly Dictionary<char, PieceType> PieceTypeFromCharacter = new Dictionary<char, PieceType> () {
            ['K'] = PieceType.King, ['P'] = PieceType.Pawn, ['N'] = PieceType.Knight, ['B'] = PieceType.Bishop, ['R'] = PieceType.Rook, ['Q'] = PieceType.Queen
        };
        
        // New game string. This is the string used for creating a standard game.
        public const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        /// Creates a board object by parsing a FEN string.
        /// </summary>
        /// <param name="fen">The FEN string to parse.</param>
        /// <returns>A board object with state parsed from the FEN string.</returns>
        public static Board PositionFromFen(string fen)
        {
            // Create a new board builder
            BoardBuilder boardBuilder = new BoardBuilder();
            
            // Split the string by spaces
            string[] sections = fen.Split(' ');

            // The castling rights are defined in the 3rd section (index 2) of the fen string
            // If the length is not greater than 2 (e.g. incomplete fen string), then just store default castling rights
            string castlingRights = sections.Length > 2 ? sections[2] : "KQkq";
            
            // Store if each player has it's castling rights
            bool whiteCastleKingside = castlingRights.Contains ("K");
            bool whiteCastleQueenside = castlingRights.Contains ("Q");
            bool blackCastleKingside = castlingRights.Contains ("k");
            bool blackCastleQueenside = castlingRights.Contains ("q");

            // Initialise file to 0 and rank to 7 
            int file = 0;
            int rank = 7;
            
            // Loop over each character in the first section (the main board representation)
            foreach (char symbol in sections[0])
            {
                // If the symbol is a '/' (denoting that the parser should now look at the next rank (below)
                if (symbol == '/')
                {
                    // Set the file to 0 (start at the left)
                    file = 0;
                    
                    // Decrease rank (go down to next rank)
                    rank--;
                }
                else
                {
                    // If the character is a decimal digit
                    if (char.IsDigit(symbol))
                    {
                        // Increase the file by that value
                        // Numbers denote how many empty tiles between either pieces or the start/end of the rank
                        file += (int) char.GetNumericValue(symbol);
                    }
                    else
                    { 
                        // If the character is an uppercase letter,
                        // then set the coalition to white (white pieces are uppercase, black pieces are lowercase)
                        Coalition pieceCoalition = (char.IsUpper(symbol)) ? Coalition.White : Coalition.Black;
                        
                        // Set the piece type depending on the letter from the dictionary
                        PieceType pieceType = PieceTypeFromCharacter[char.ToUpper(symbol)];
                        
                        // Switch depending on piece type
                        // Create a new piece at the coordinate (calculated via the equation i = rank * 8 + file, where i is the index to place the piece on the board
                        // Create the piece at the specified index with correct coalition
                        // Since FEN doesn't store game history then assume it is the pieces first move
                        // Step the file by one since there can't be more than 1 piece per tile
                        switch (pieceType)
                        {
                            case PieceType.Pawn:
                                boardBuilder.SetPieceAtTile(new Pawn(rank * 8 + file, pieceCoalition, true));
                                file++;
                                break;
                            case PieceType.Knight:
                                boardBuilder.SetPieceAtTile(new Knight(rank * 8 + file, pieceCoalition, true));
                                file++;
                                break;
                            case PieceType.Bishop:
                                boardBuilder.SetPieceAtTile(new Bishop(rank * 8 + file, pieceCoalition, true));
                                file++;
                                break;
                            case PieceType.Rook:
                                boardBuilder.SetPieceAtTile(new Rook(rank * 8 + file, pieceCoalition, true));
                                file++;
                                break;
                            case PieceType.Queen:
                                boardBuilder.SetPieceAtTile(new Queen(rank * 8 + file, pieceCoalition, true));
                                file++;
                                break;
                            case PieceType.King when pieceCoalition is Coalition.White:
                                // When the king is white then create a king but pass in the white player castling rights
                                boardBuilder.SetPieceAtTile(new King(rank * 8 + file, pieceCoalition, whiteCastleKingside, whiteCastleQueenside));
                                file++;
                                break;
                            case PieceType.King when pieceCoalition is Coalition.Black:
                                // When the king is black then create a king but pass in the black player castling rights
                                boardBuilder.SetPieceAtTile(new King(rank * 8 + file, pieceCoalition, blackCastleKingside, blackCastleQueenside));
                                file++;
                                break;
                        }
                    }
                }
            }
            
            // Set the next player to move to the value of section 2 (index 1)
            // If it is 'w' denoting white then set it to white otherwise set it to black
            boardBuilder.SetCoalitionToMove(sections[1] == "w" ? Coalition.White : Coalition.Black);
            
            // Build the board
            return boardBuilder.BuildBoard();
        }

        /// <summary>
        /// Creates a FEN string from the current board state.
        /// </summary>
        /// <param name="board">The board to parse.</param>
        /// <param name="moveStack">The stack of moves played on the board.</param>
        /// <returns>A string representation of the board.</returns>
        public static string FenFromPosition(Board board, Stack<IMove> moveStack)
        {
            // Initialise the fen string
            string fen = "";
            
            // Loop from the top rank down to the first rank
            for (int rank = 7; rank >= 0; rank--)
            {
                // Initialise the number of empty tiles on this rank to 0
                int numEmptyTiles = 0;
                
                // Loop through each file on the rank
                for (int file = 0; file < NumTilesPerFile; file++)
                {
                    // Get the board index from the current file and rank using the equation i = rank * 8 + file
                    int i = rank * 8 + file;
                    
                    // Get the piece on the tile
                    Piece piece = board.GetTile(i).Piece;
                    
                    // If the piece is not null
                    if (piece is not null)
                    {
                        // If the number of empty tiles on the rank is more than zero (so there are some empty tiles) 
                        if (numEmptyTiles > 0)
                        {
                            // Add the number of empty to the string to denote how many empty adjacent tiles there are
                            fen += numEmptyTiles;
                            
                            // Reset the number of empty tiles
                            numEmptyTiles = 0;
                        }

                        // Add the piece type abbreviation to the string
                        fen += piece.PieceType.ToAbbreviation(piece.PieceCoalition);
                        
                    }
                    // Else (if the tile is empty)
                    else
                    {
                        // Increase the number of empty tiles
                        numEmptyTiles++;
                    }
                }

                // If there are more than 0 empty tiles on the rank (adds any remaining ones not accounted for in the loop)
                if (numEmptyTiles > 0)
                {
                    // Add the number of empty tiles
                    fen += numEmptyTiles;
                }

                // If the rank is not 0 (so not the first rank)
                if (rank != 0)
                {
                    // Append a '/' to denote the next rank down
                    fen += '/';
                }
            }

            // Append whitespace and then the the current player colour (the player to move) to the string.
            // Append more white space to separate sections.
            fen += $" {board.CurrentPlayer.Coalition.ToAbbreviation().ToLower()} ";

            // Create a new temporary string builder
            StringBuilder stringBuilder = new StringBuilder();
            
            // Append each player's castling capabilities to the string
            if (board.WhitePlayer.King.KingsideCastleCapable)
            {
                stringBuilder.Append('K');
            }
            if (board.WhitePlayer.King.QueensideCastleCapable)
            {
                stringBuilder.Append('Q');
            }
            if (board.BlackPlayer.King.KingsideCastleCapable)
            {
                stringBuilder.Append('k');
            }
            if (board.BlackPlayer.King.QueensideCastleCapable)
            {
                stringBuilder.Append('q');
            }

            // Build the castling string
            string castleString = stringBuilder.ToString();
            
            // If the string is null or empty (no castling capabilities) then append a dash to denote no castling
            // Otherwise append the castling string
            fen += string.IsNullOrEmpty(castleString) ? '-' : castleString;

            // Add whitespace for new section
            fen += ' ';
            
            // Get the current en passant pawn
            Pawn enPassantPawn = board.EnPassantPawn;
            
            // If there is no en passant pawn
            if (enPassantPawn is null)
            {
                // Append a dash to denote that there is no en passant pawn
                fen += '-';
            }
            // Else (if there is an en passant pawn)
            else
            {
                // Append the algebraic coordinate that the en passant pawn is on
                fen += "" + FileNames[FileIndex(enPassantPawn.PiecePosition)] +
                       RankNames[RankIndex(enPassantPawn.PiecePosition)];
            }
            
            int count = 0;
            foreach (var move in moveStack)
            {
                if (move is CaptureMove)
                {
                    count = 0;
                }

                count++;
            }

            count = (int) Math.Floor(count / 2d);

            int plyCount = (int) Math.Floor(board.PlyCount / 2d) + 1;
            fen += $" {count} {plyCount}";

            // Return the string
            return fen;
        }
    }
}