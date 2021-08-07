using System;
using Engine.Types;

namespace OutputTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var board = Board.CreateStandardBoard();

            Console.WriteLine(board);
        }
    }
}