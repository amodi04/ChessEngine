using System;
using System.Collections.Generic;
using System.Linq;
using Engine.BoardRepresentation;
using Engine.Pieces;
using Engine.Player;

namespace Engine.AI
{
	/// <summary>
	/// Class for holding evaluation data
	/// </summary>
    public class BetterEvaluator : IEvaluator
    {
	    // Store piece values for quick access
        public const int PawnValue = 100;
		public const int KnightValue = 300;
		public const int BishopValue = 320;
		public const int RookValue = 500;
		public const int QueenValue = 900;

		// Calculate the endgame material start score
		// This makes the AI value having 2 rooks, 1 bishop, and 1 horse for the endgame.
		private const float EndgameMaterialStart = RookValue * 2 + BishopValue + KnightValue;
		
		/// <summary>
		/// Performs a static evaluation of the current board.
		/// </summary>
		/// <param name="board">The board to evaluate.</param>
		/// <param name="depth">The depth of the current search.</param>
		/// <returns>An integer evaluation score.</returns>
		public int Evaluate (Board board, int depth) {
			
			// Initialise the current evaluations to 0
			int whiteEval = 0;
			int blackEval = 0;

			// Count each player's material score (pieces)
			int whiteMaterial = CountMaterial(board.WhitePlayer);
			int blackMaterial = CountMaterial(board.BlackPlayer);
			
			// Calculate each player's material score without pawns
			int whiteMaterialWithoutPawns = whiteMaterial - board.WhitePieces.OfType<Pawn>().Count() * PawnValue;
			int blackMaterialWithoutPawns = blackMaterial - board.BlackPieces.OfType<Pawn>().Count() * PawnValue;
			
			// Calculate the weighting of the endgame based on each player's material without pawns
			float whiteEndgamePhaseWeight = EndgamePhaseWeight(whiteMaterialWithoutPawns);
			float blackEndgamePhaseWeight = EndgamePhaseWeight(blackMaterialWithoutPawns);
			
			// Add the material scores to evaluations
			whiteEval += whiteMaterial;
			blackEval += blackMaterial;

			// Add the scores calculated from evaluating the piece position tables
			whiteEval += EvaluatePiecePositionTables (board.WhitePlayer, blackEndgamePhaseWeight);
			blackEval += EvaluatePiecePositionTables (board.BlackPlayer, whiteEndgamePhaseWeight);

			// Add the scores calculated from evaluating checkmate
			// For white, if black is in checkmate, add 100000 to it's eval because this will win the game
			// For black, if white is in checkmate, add 100000 to it's eval because this will win the game
			// If there is no checkmate on the current board, add 0, because it is neither good nor bad
			whiteEval += board.BlackPlayer.IsInCheckmate() ? 100000 : 0;
			blackEval += board.WhitePlayer.IsInCheckmate() ? 100000 : 0;

			// Return white's score - black's score
			// If positive, the score is better for white
			// If negative, the score is better for black
			return whiteEval - blackEval;
		}

		/// <summary>
		/// Calculates the endgame phase weight used for the piece position tables.
		/// The score shows how far into the end game the current board is.
		/// </summary>
		/// <param name="materialCountWithoutPawns">The piece score without pawns for the player.</param>
		/// <returns>A weight score stored as a float.</returns>
		float EndgamePhaseWeight (int materialCountWithoutPawns) {
			
			// Calculate the multiplier by dividing 1 by the endgame material start score
			const float multiplier = 1 / EndgameMaterialStart;
			
			// Return 1 - the minimum of 1 or the material count without pawns multiplied by the multiplier
			// The minimum is 0 (denoting that the game has not reached end game yet
			// The maximum tends to 1 denoting that the game has reached the endgame
			return 1 - Math.Min (1, materialCountWithoutPawns * multiplier);
		}
		
		/// <summary>
		/// Counts the material score.
		/// </summary>
		/// <param name="player">The player to count the material score for.</param>
		/// <returns>An integer score.</returns>
		int CountMaterial (Player.Player player)
		{
			// Initialise the sum to 0
			int material = 0;
			
			// Loop over each piece that the player has currently
			foreach (var piece in player.GetActiveAlliedPieces())
			{
				// If the piece is not a king
				// (we don't count the king in the score because the king's score is just used for evaluating checkmate)
				if (piece.PieceType != PieceType.King)
				{
					material += piece.PieceType switch
					{
						PieceType.Pawn => PawnValue,
						PieceType.Knight => KnightValue,
						PieceType.Bishop => BishopValue,
						PieceType.Rook => RookValue,
						PieceType.Queen => QueenValue,
						_ => throw new ArgumentOutOfRangeException()
					};
				}
			}

			// Return the sum
			return material;
		}

		/// <summary>
		/// Evaluates pieces in their piece position tables.
		/// </summary>
		/// <param name="player">The player to evaluate for.</param>
		/// <param name="endgamePhaseWeight">The endgame phase weight (how far into the endgame).</param>
		/// <returns>An integer score.</returns>
		int EvaluatePiecePositionTables (Player.Player player, float endgamePhaseWeight) {
			
			// Initialise the value to 0
			int value = 0;
			
			// Calculate if the player is white
			bool isWhite = player.Coalition.IsWhite();
			
			// Sum the score for each type of piece in the position tables
			value += EvaluatePiecePositionTable (PiecePositionTable.Pawns, player.GetActiveAlliedPieces().OfType<Pawn>(), isWhite);
			value += EvaluatePiecePositionTable (PiecePositionTable.Rooks, player.GetActiveAlliedPieces().OfType<Rook>(), isWhite);
			value += EvaluatePiecePositionTable (PiecePositionTable.Knights, player.GetActiveAlliedPieces().OfType<Knight>(), isWhite);
			value += EvaluatePiecePositionTable (PiecePositionTable.Bishops, player.GetActiveAlliedPieces().OfType<Bishop>(), isWhite);
			value += EvaluatePiecePositionTable (PiecePositionTable.Queens, player.GetActiveAlliedPieces().OfType<Queen>(), isWhite);
			
			// Calculate if the king is in the early phase depending on it's position
			int kingEarlyPhase = PiecePositionTable.Read (PiecePositionTable.KingMiddle, player.King.PiecePosition, isWhite);
			
			// Add the king early phase score * 1 - end game phase weight
			// The value added dictates how important the king is in the end game.
			// Forces the AI to make better decisions about king pieces.
			value += (int) (kingEarlyPhase * (1 - endgamePhaseWeight));
			
			// Add the value of the king position for the end game (allows the AI to put the king in better positions for the end game)
			value += PiecePositionTable.Read (PiecePositionTable.KingEnd, player.King.PiecePosition, isWhite);

			// Return the value
			return value;
		}

		/// <summary>
		/// Evaluates how good a piece is, given it's position
		/// </summary>
		/// <param name="table">The table to read.</param>
		/// <param name="pieceList">The list of pieces to look at.</param>
		/// <param name="isWhite">Flag used for determining the perspective of the player.</param>
		/// <returns>An integer score.</returns>
		static int EvaluatePiecePositionTable (int[] table, IEnumerable<Piece> pieceList, bool isWhite)
		{
			// Returns the sum of the scores for each piece in the list read from the table.
			return pieceList.Sum(t => PiecePositionTable.Read(table, t.PiecePosition, isWhite));
		}
    }
}