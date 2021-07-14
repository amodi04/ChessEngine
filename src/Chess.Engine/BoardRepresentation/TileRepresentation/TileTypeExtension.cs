using System;

namespace Engine.BoardRepresentation.TileRepresentation
{
    /// <summary>
    /// This extension class contains methods for dealing with tile state.
    /// </summary>
    public static class TileTypeExtension
    {
        /// <summary>
        /// Gets the occupancy status which is either true or false. 
        /// </summary>
        /// <param name="tileType">The current tile state.</param>
        /// <returns>A boolean which is decided based on the tile state. True if tile is occupied.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Handles the case when an undefined TileType is passed in</exception>
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