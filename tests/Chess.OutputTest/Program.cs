using System;
using Engine.BoardRepresentation;

namespace OutputTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Board board = Board.CreateStandardBoard();
            
            Console.WriteLine(board);
        }
    }
}