using Engine.BoardRepresentation;

namespace Engine.AI;

/// <summary>
///     This class holds values for preferred piece positions during a game.
/// </summary>
public static class PiecePositionTable
{
	/// <summary>
	///     This is the pawn position table
	///     Pawns prefer to promote, so higher values are seen towards the other side of the board
	/// </summary>
	public static readonly int[] Pawns =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
        5, 5, 10, 25, 25, 10, 5, 5,
        0, 0, 0, 20, 20, 0, 0, 0,
        5, -5, -10, 0, 0, -10, -5, 5,
        5, 10, 10, -20, -20, 10, 10, 5,
        0, 0, 0, 0, 0, 0, 0, 0
    };

	/// <summary>
	///     This is the knight position table
	///     Knights can access the their most moves at the center of the board so higher values are seen in the center
	/// </summary>
	public static readonly int[] Knights =
    {
        -50, -40, -30, -30, -30, -30, -40, -50,
        -40, -20, 0, 0, 0, 0, -20, -40,
        -30, 0, 10, 15, 15, 10, 0, -30,
        -30, 5, 15, 20, 20, 15, 5, -30,
        -30, 0, 15, 20, 20, 15, 0, -30,
        -30, 5, 10, 15, 15, 10, 5, -30,
        -40, -20, 0, 5, 5, 0, -20, -40,
        -50, -40, -30, -30, -30, -30, -40, -50
    };

	/// <summary>
	///     This is the bishop position table
	///     Bishops can access the their most moves at the center of the board so higher values are seen in the center
	/// </summary>
	public static readonly int[] Bishops =
    {
        -20, -10, -10, -10, -10, -10, -10, -20,
        -10, 0, 0, 0, 0, 0, 0, -10,
        -10, 0, 5, 10, 10, 5, 0, -10,
        -10, 5, 5, 10, 10, 5, 5, -10,
        -10, 0, 10, 10, 10, 10, 0, -10,
        -10, 10, 10, 10, 10, 10, 10, -10,
        -10, 5, 0, 0, 0, 0, 5, -10,
        -20, -10, -10, -10, -10, -10, -10, -20
    };

	/// <summary>
	///     This is the rook position table
	///     Rooks are most effective near the opponent king so higher values are seen on the opposite side of the board
	/// </summary>
	public static readonly int[] Rooks =
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        5, 10, 10, 10, 10, 10, 10, 5,
        -5, 0, 0, 0, 0, 0, 0, -5,
        -5, 0, 0, 0, 0, 0, 0, -5,
        -5, 0, 0, 0, 0, 0, 0, -5,
        -5, 0, 0, 0, 0, 0, 0, -5,
        -5, 0, 0, 0, 0, 0, 0, -5,
        0, 0, 0, 5, 5, 0, 0, 0
    };

	/// <summary>
	///     This is the queen position table
	///     Queens can access the their most moves at the center of the board so higher values are seen in the center
	/// </summary>
	public static readonly int[] Queens =
    {
        -20, -10, -10, -5, -5, -10, -10, -20,
        -10, 0, 0, 0, 0, 0, 0, -10,
        -10, 0, 5, 5, 5, 5, 0, -10,
        -5, 0, 5, 5, 5, 5, 0, -5,
        0, 0, 5, 5, 5, 5, 0, -5,
        -10, 5, 5, 5, 5, 5, 0, -10,
        -10, 0, 5, 0, 0, 0, 0, -10,
        -20, -10, -10, -5, -5, -10, -10, -20
    };

	/// <summary>
	///     This is the king middle game position table
	///     Kings in the middle game need to be protected so higher values are seen towards the bottom
	/// </summary>
	public static readonly int[] KingMiddle =
    {
        -30, -40, -40, -50, -50, -40, -40, -30,
        -30, -40, -40, -50, -50, -40, -40, -30,
        -30, -40, -40, -50, -50, -40, -40, -30,
        -30, -40, -40, -50, -50, -40, -40, -30,
        -20, -30, -30, -40, -40, -30, -30, -20,
        -10, -20, -20, -20, -20, -20, -20, -10,
        20, 20, 0, 0, 0, 0, 20, 20,
        20, 30, 10, 0, 0, 10, 30, 20
    };

	/// <summary>
	///     This is the king end game position table
	///     Kings in the end game are more valuable supporting pieces so higher values are seen towards the center
	///     Negative values are seen on the edges and corners because kings can be checkmated here easily
	/// </summary>
	public static readonly int[] KingEnd =
    {
        -50, -40, -30, -20, -20, -30, -40, -50,
        -30, -20, -10, 0, 0, -10, -20, -30,
        -30, -10, 20, 30, 30, 20, -10, -30,
        -30, -10, 30, 40, 40, 30, -10, -30,
        -30, -10, 30, 40, 40, 30, -10, -30,
        -30, -10, 20, 30, 30, 20, -10, -30,
        -30, -30, 0, 0, 0, 0, -30, -30,
        -50, -30, -30, -30, -30, -30, -30, -50
    };

	/// <summary>
	///     Reads a position table and outputs an integer value based on the index calculated.
	/// </summary>
	/// <param name="table">The table to read.</param>
	/// <param name="tileCoordinate">The tile coordinate of the piece.</param>
	/// <param name="isWhite">Used in calculation for determining index.</param>
	/// <returns></returns>
	public static int Read(int[] table, int tileCoordinate, bool isWhite)
    {
        if (!isWhite) return table[tileCoordinate];
        var file = BoardUtilities.FileIndex(tileCoordinate);
        var rank = BoardUtilities.RankIndex(tileCoordinate);

        rank = 7 - rank;
        tileCoordinate = rank * 8 + file;
        // Black doesn't need converting so it can just use the tile coordinate passed in
        return table[tileCoordinate];
    }
}