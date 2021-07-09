using System;

namespace Engine.BoardRepresentation.TileRepresentation
{
    public static class TileTypeExtension
    {
        public static bool IsOccupied(this TileType tileType)
        {
            return tileType switch
            {
                TileType.Empty => false,
                TileType.Occupied => true,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null)
            };
        }
    }
}