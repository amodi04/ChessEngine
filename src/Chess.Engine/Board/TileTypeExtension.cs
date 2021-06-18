using System;

namespace Engine.Board
{
    public static class TileTypeExtension
    {
        public static bool IsOccupied(this TileType tileType)
        {
            return tileType switch
            {
                TileType.EMPTY => false,
                TileType.OCCUPIED => true,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null)
            };
        }
    }
}