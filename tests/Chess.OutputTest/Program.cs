using System;
using Engine.BoardRepresentation;

namespace OutputTest
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (int i in BoardUtilities.FirstRank)
            {
                Console.WriteLine(i.ToString());
            }
            Console.WriteLine("\n");
            foreach (int i in BoardUtilities.FirstFile)
            {
                Console.WriteLine(i.ToString());
            }
        }
    }
}