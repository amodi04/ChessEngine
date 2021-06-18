using System.Linq;

namespace Engine.Board
{
    public static class BoardUtilities
    {
        public const uint NumTiles = 64;
        public const uint NumTilesPerRank = 8;
        public const uint NumTilesPerFile = 8;

        public static readonly int[] FirstRank = CreateRank(0);
        public static readonly int[] SecondRank = CreateRank(8);
        public static readonly int[] ThirdRank = CreateRank(16);
        public static readonly int[] FourthRank = CreateRank(24);
        public static readonly int[] FifthRank = CreateRank(32);
        public static readonly int[] SixthRank = CreateRank(40);
        public static readonly int[] SeventhRank = CreateRank(48);
        public static readonly int[] EighthRank = CreateRank(56);

        public static readonly int[] FirstFile = CreateFile(0);
        public static readonly int[] SecondFile = CreateFile(1);
        public static readonly int[] ThirdFile = CreateFile(2);
        public static readonly int[] FourthFile = CreateFile(3);
        public static readonly int[] FifthFile = CreateFile(4);
        public static readonly int[] SixthFile = CreateFile(5);
        public static readonly int[] SeventhFile = CreateFile(6);
        public static readonly int[] EighthFile = CreateFile(7);

        private static int[] CreateRank(int startingTileNumber)
        {
            return Enumerable.Range(startingTileNumber , 8).ToArray();
        }

        private static int[] CreateFile(int startingFileNumber)
        {
            return Enumerable.Range(0, 8).Select(n => 8*n + startingFileNumber ).ToArray();
        }
        
        public static bool IsValidTileCoordinate(int tileCoordinate)
        {
            return tileCoordinate < NumTiles;
        }
    }
}