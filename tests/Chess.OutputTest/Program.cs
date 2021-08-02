using System;
using Engine.BoardRepresentation;

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