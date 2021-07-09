﻿using System.Collections.Generic;
using Engine.Pieces;

namespace Engine.BoardRepresentation.TileRepresentation
{
    public class Tile
    {
        // Put all tiles into a dictionary. This is so that there are no repeating indexes.
        // 0-63 tiles
        private static readonly IDictionary<int, Tile> EmptyTiles = InitialiseEmptyTiles();

        private readonly TileType _tileType;
        private readonly Piece _piece;
        
        public int TileCoordinate { get; }
        public Piece Piece => IsOccupied() ? _piece : null;

        // Empty tile constructor
        private Tile(int tileCoordinate)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.Empty;
        }

        // Occupied tile constructor
        private Tile(int tileCoordinate, Piece piece)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.Occupied;
            _piece = piece;
        }

        // Initialises the board of 64 tiles
        private static IDictionary<int, Tile> InitialiseEmptyTiles()
        {
            var emptyTileDictionary = new Dictionary<int, Tile>();
            for (var i = 0; i < 64; i++) emptyTileDictionary[i] = new Tile(i);

            return emptyTileDictionary;
        }

        public bool IsOccupied()
        {
            return _tileType.IsOccupied();
        }

        // Public interface for creating tiles. Keeps immutability
        public static Tile CreateTile(int tileCoordinate, Piece piece)
        {
            return piece != null ? new Tile(tileCoordinate, piece) : EmptyTiles[tileCoordinate];
        }
    }
}