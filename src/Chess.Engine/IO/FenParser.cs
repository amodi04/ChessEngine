using System;
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
    ///     This class handles the parsing of FEN strings.
    ///     FEN strings allow the board state to be stored as a string so that it can be played on any FEN compliant engine.
    ///     It is a form of saving board state.
    /// </summary>
    public static class FenParser
    {
        // New game string. This is the string used for creating a standard game.
        public const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        ///     This dictionary stores the conversion from a character representing a piece to its relative piece type.
        /// </summary>
        private static readonly Dictionary<char, PieceType> PieceTypeFromCharacter = new()
        {
            ['K'] = PieceType.King, ['P'] = PieceType.Pawn, ['N'] = PieceType.Knight, ['B'] = PieceType.Bishop,
            ['R'] = PieceType.Rook, ['Q'] = PieceType.Queen
        };

        /// <summary>
        ///     Creates a board object by parsing a FEN string.
        /// </summary>
        /// <param name="fen">The FEN string to parse.</param>
        /// <returns>A board object with state parsed from the FEN string.</returns>
        public static Board PositionFromFen(string fen)
        {
            var boardBuilder = new BoardBuilder();
            
            var sections = fen.Split(' ');

            // The castling rights are defined in the 3rd section (index 2) of the fen string
            // If the length is not greater than 2 (e.g. incomplete fen string), then just store default castling rights
            var castlingRights = sections.Length > 2 ? sections[2] : "KQkq";
            
            var whiteCastleKingside = castlingRights.Contains("K");
            var whiteCastleQueenside = castlingRights.Contains("Q");
            var blackCastleKingside = castlingRights.Contains("k");
            var blackCastleQueenside = castlingRights.Contains("q");
            
            var file = 0;
            var rank = 7;

            // Loop over each character in the first section (the main board representation)
            foreach (var symbol in sections[0])
                // Should the parser now look at the next rank (below)
                if (symbol == '/')
                {
                    file = 0;
                    rank--;
                }
                else
                {
                    if (char.IsDigit(symbol))
                    {
                        // Numbers denote how many empty tiles between either pieces or the start/end of the rank
                        file += (int) char.GetNumericValue(symbol);
                    }
                    else
                    {
                        var pieceCoalition = char.IsUpper(symbol) ? Coalition.White : Coalition.Black;
                        var pieceType = PieceTypeFromCharacter[char.ToUpper(symbol)];

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
                                boardBuilder.SetPieceAtTile(new King(rank * 8 + file, pieceCoalition,
                                    whiteCastleKingside, whiteCastleQueenside));
                                file++;
                                break;
                            case PieceType.King when pieceCoalition is Coalition.Black:
                                // When the king is black then create a king but pass in the black player castling rights
                                boardBuilder.SetPieceAtTile(new King(rank * 8 + file, pieceCoalition,
                                    blackCastleKingside, blackCastleQueenside));
                                file++;
                                break;
                        }
                    }
                }

            // Set the next player to move to the value of section 2
            // If it is 'w' denoting white then set it to white otherwise set it to black
            boardBuilder.SetCoalitionToMove(sections[1] == "w" ? Coalition.White : Coalition.Black);
            
            return boardBuilder.BuildBoard();
        }

        /// <summary>
        ///     Creates a FEN string from the current board state.
        /// </summary>
        /// <param name="board">The board to parse.</param>
        /// <param name="moveStack">The stack of moves played on the board.</param>
        /// <returns>A string representation of the board.</returns>
        public static string FenFromPosition(Board board, Stack<IMove> moveStack)
        {
            var fen = "";
            
            for (var rank = 7; rank >= 0; rank--)
            {
                var numEmptyTiles = 0;
                
                for (var file = 0; file < NumTilesPerFile; file++)
                {
                    var i = rank * 8 + file;
                    var piece = board.GetTile(i).Piece;
                    
                    if (piece is not null)
                    {
                        // If the number of empty tiles on the rank is more than zero (so there are some empty tiles) 
                        if (numEmptyTiles > 0)
                        {
                            // Add the number of empty to the string to denote how many empty adjacent tiles there are
                            fen += numEmptyTiles;
                            numEmptyTiles = 0;
                        }
                        
                        fen += piece.Type.ToAbbreviation(piece.PieceCoalition);
                    }
                    else
                    {
                        numEmptyTiles++;
                    }
                }

                // If there are more than 0 empty tiles on the rank (adds any remaining ones not accounted for in the loop)
                if (numEmptyTiles > 0)
                    fen += numEmptyTiles;
                
                if (rank != 0)
                    // Append a '/' to denote the next rank down
                    fen += '/';
            }
            fen += $" {board.CurrentPlayer.Coalition.ToAbbreviation().ToLower()} ";
            
            var stringBuilder = new StringBuilder();

            // Append each player's castling capabilities to the string
            if (board.WhitePlayer.King.KingsideCastleCapable) stringBuilder.Append('K');
            if (board.WhitePlayer.King.QueensideCastleCapable) stringBuilder.Append('Q');
            if (board.BlackPlayer.King.KingsideCastleCapable) stringBuilder.Append('k');
            if (board.BlackPlayer.King.QueensideCastleCapable) stringBuilder.Append('q');
            
            var castleString = stringBuilder.ToString();

            // If the string is null or empty (no castling capabilities) then append a dash to denote no castling
            fen += string.IsNullOrEmpty(castleString) ? '-' : castleString;
            
            fen += ' ';
            
            // Get enPassant data
            var enPassantPawn = board.EnPassantPawn;
            if (enPassantPawn is null)
                fen += '-';
            else
                fen += "" + FileNames[FileIndex(enPassantPawn.PiecePosition)] +
                       RankNames[RankIndex(enPassantPawn.PiecePosition)];

            // Calculate the number of moves since last attack
            var count = 0;
            foreach (var move in moveStack)
            {
                if (move is CaptureMove) count = 0;

                count++;
            }
            count = (int) Math.Floor(count / 2d);

            // Calculate the total moves played (2 plys per move)
            var plyCount = (int) Math.Floor(board.PlyCount / 2d) + 1;
            fen += $" {count} {plyCount}";
 
            return fen;
        }
    }
}