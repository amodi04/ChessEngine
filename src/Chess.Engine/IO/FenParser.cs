using System;
using System.Collections.Generic;
using System.Text;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using Engine.Player;
using static Engine.BoardRepresentation.BoardUtilities;

namespace Engine.IO;

/// <summary>
///     This class handles the parsing of FEN strings.
///     FEN strings allow the board state to be stored as a string so that it can be played on any FEN compliant engine.
///     It is a form of saving board state.
/// </summary>
public static class FenParser
{
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
        var castlingRights = sections.Length > 2 ? sections[2] : "KQkq";

        var whiteCastleKingside = castlingRights.Contains("K");
        var whiteCastleQueenside = castlingRights.Contains("Q");
        var blackCastleKingside = castlingRights.Contains("k");
        var blackCastleQueenside = castlingRights.Contains("q");

        var file = 0;
        var rank = 7;

        foreach (var symbol in sections[0])

            // Should the parser now look at the next rank (below)
            if (symbol == '/')
            {
                file = 0;
                rank--;
            }
            else
            {
                // Parse empty ranks
                if (char.IsDigit(symbol))
                {
                    // Numbers denote how many empty tiles between either pieces or the start/end of the rank
                    file += (int)char.GetNumericValue(symbol);
                }
                else
                {
                    // Parse pieces
                    var pieceCoalition = char.IsUpper(symbol) ? Coalition.White : Coalition.Black;
                    var pieceType = PieceTypeFromCharacter[char.ToUpper(symbol)];

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
                            boardBuilder.SetPieceAtTile(new King(rank * 8 + file, pieceCoalition,
                                whiteCastleKingside, whiteCastleQueenside));
                            file++;
                            break;
                        case PieceType.King:
                            boardBuilder.SetPieceAtTile(new King(rank * 8 + file, pieceCoalition,
                                blackCastleKingside, blackCastleQueenside));
                            file++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

        // Set current player
        boardBuilder.SetCoalitionToMove(sections[1] == "w" ? Coalition.White : Coalition.Black);
        
        // Set en passant pawn
        var enPassant = sections[3];
        try
        {
            // If there is an en passant pawn
            if (enPassant != "-")
            {
                var fileIndex = FileNames.IndexOf(enPassant[0]);
                var rankIndex = RankNames.IndexOf(enPassant[1]);
                var tileCoordinate = fileIndex + rankIndex * 8;
                boardBuilder.SetEnPassantPawn((Pawn) boardBuilder.BoardConfiguration[tileCoordinate]!);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return boardBuilder.BuildBoard();
    }

    /// <summary>
    ///     Creates a FEN string from the current board state.
    /// </summary>
    /// <param name="board">The board to parse.</param>
    /// <param name="moveStack">The stack of moves played on the board.</param>
    /// <returns>A string representation of the board.</returns>
    public static string FenFromPosition(Board board, IEnumerable<IMove> moveStack)
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
                    if (numEmptyTiles > 0)
                    {
                        // Add the number of empty to the string to denote how many empty adjacent tiles there are
                        fen += numEmptyTiles.ToString();
                        numEmptyTiles = 0;
                    }

                    fen += piece.Type.ToAbbreviation(piece.PieceCoalition);
                }
                else
                {
                    numEmptyTiles++;
                }
            }

            if (numEmptyTiles > 0)
                fen += numEmptyTiles;

            if (rank != 0)
                fen += '/';
        }

        // Current player
        fen += $" {board.CurrentPlayer.Coalition.ToAbbreviation().ToLower()} ";

        // Castling
        var stringBuilder = new StringBuilder();
        if (board.WhitePlayer.King.KingsideCastleCapable) stringBuilder.Append('K');
        if (board.WhitePlayer.King.QueensideCastleCapable) stringBuilder.Append('Q');
        if (board.BlackPlayer.King.KingsideCastleCapable) stringBuilder.Append('k');
        if (board.BlackPlayer.King.QueensideCastleCapable) stringBuilder.Append('q');
        var castleString = stringBuilder.ToString();
        fen += string.IsNullOrEmpty(castleString) ? '-' : castleString;
        fen += ' ';

        var enPassantPawn = board.EnPassantPawn;
        if (enPassantPawn is null)
            fen += '-';
        else
            fen += "" + FileNames[FileIndex(enPassantPawn.PiecePosition)] +
                   RankNames[RankIndex(enPassantPawn.PiecePosition)];

        // Moves since last attack
        var count = 0;
        foreach (var move in moveStack)
        {
            if (move is CaptureMove) count = 0;

            count++;
        }

        count = (int)Math.Floor(count / 2d);

        var plyCount = (int)Math.Floor(board.PlyCount / 2d) + 1;
        fen += $" {count} {plyCount}";
        return fen;
    }
}