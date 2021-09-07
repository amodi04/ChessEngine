using System.Linq;
using Engine.Types;
using NUnit.Framework;

namespace Chess.Engine.Tests
{
    /// <summary>
    /// Board testing class
    /// </summary>
    [TestFixture]
    public class BoardTest
    {
        /// <summary>
        /// Tests the initial board setup
        /// </summary>
        [Test]
        public void TestInitialBoard()
        {
            var board = Board.CreateStandardBoard();
            Assert.AreEqual(board.CurrentPlayer.Moves.Count, 20);
            Assert.AreEqual(board.CurrentPlayer.GetOpponent().Moves.Count, 20);
            Assert.False(board.CurrentPlayer.IsInCheck());
            Assert.False(board.CurrentPlayer.IsInCheckmate());
            Assert.False(board.CurrentPlayer.IsInStalemate());
            Assert.AreEqual(board.CurrentPlayer, board.WhitePlayer);
            Assert.AreEqual(board.CurrentPlayer.GetOpponent(), board.BlackPlayer);
        }
    }
}