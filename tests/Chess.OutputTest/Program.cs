using System;
using Engine.Types;
using Engine.Util;

namespace OutputTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            foreach (string s in BoardUtilities.GenerateAlgebraicNotation())
            {
                Console.WriteLine(s);
            }
        }
    }
}