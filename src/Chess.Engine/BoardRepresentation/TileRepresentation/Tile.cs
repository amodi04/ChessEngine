﻿using System.Collections.Generic;
using Engine.Pieces;

namespace Engine.BoardRepresentation.TileRepresentation
{
    /// <summary>
    /// This class contains tile data which will be used for determining relative positions in a game.
    /// </summary>
    public class Tile
    {
        // Member fields
        private static readonly IDictionary<int, Tile> EmptyTiles = InitialiseEmptyTiles();
        private readonly Piece _piece;
        private readonly TileType _tileType;
        public int TileCoordinate { get; }
        public Piece Piece => IsOccupied() ? _piece : null;
        
        /// <summary>
        /// Empty tile constructor to allow the creation of unoccupied tiles.
        /// Private-only access so that immutability is retained.
        /// </summary>
        /// <param name="tileCoordinate">The position of the tile in a grid of 64 tiles.</param>
        private Tile(int tileCoordinate)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.Empty;
        }

        /// <summary>
        /// Occupied tile constructor to allow the creation of occupied tiles.
        /// Private-only access so that immutability is retained.
        /// </summary>
        /// <param name="tileCoordinate">The position of the tile in a grid of 64 tiles.</param>
        /// <param name="piece">The piece object that occupies the tile.</param>
        private Tile(int tileCoordinate, Piece piece)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.Occupied;
            _piece = piece;
        }

        /// <summary>
        /// Loops from 0 to 64 to generate 64 empty tiles.
        /// </summary>
        /// <returns>An IDictionary containing references to the 64 empty tiles.</returns>
        private static IDictionary<int, Tile> InitialiseEmptyTiles()
        {
            var emptyTileDictionary = new Dictionary<int, Tile>();
            for (var i = 0; i < 64; i++) emptyTileDictionary[i] = new Tile(i);

            return emptyTileDictionary;
        }

        /// <summary>
        /// Gets the occupancy status of the tile.
        /// </summary>
        /// <returns>Occupancy status of tile.</returns>
        public bool IsOccupied()
        {
            return _tileType.IsOccupied();
        }

        /// <summary>
        /// Public interface for creating tiles. Tile is created based on piece passed in.
        /// </summary>
        /// <param name="tileCoordinate">The position of the tile in a grid of 64 tiles.</param>
        /// <param name="piece">The piece object that occupies the tile.</param>
        /// <returns>An occupied tile if the piece object exists.
        /// Otherwise the reference to the tile at the passed in tile coordinate is returned.
        /// </returns>
        public static Tile CreateTile(int tileCoordinate, Piece piece)
        {
            return piece != null ? new Tile(tileCoordinate, piece) : EmptyTiles[tileCoordinate];
        }
    }
}