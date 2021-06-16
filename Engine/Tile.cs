using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Engine
{
    public class Tile
    {
        // Backing fields
        private int _tileCoordinate;
        private Piece _piece;
        
        // Properties
        public int TileCoordinate => _tileCoordinate;
        public Piece Piece => IsOccupied() ? _piece : null;
        
        private readonly TileType _tileType;

        // Put all tiles into a readonly dictionary. This is so that there are no repeating indexes and it is immutable.
        // 0-63 tiles
        private static readonly ReadOnlyDictionary<int, Tile> EMPTY_TILES = InitialiseEmptyTiles();
        
        // Initialises the board of 64 tiles
        private static ReadOnlyDictionary<int, Tile> InitialiseEmptyTiles()
        {
            Dictionary<int, Tile> emptyTileDictionary = new Dictionary<int, Tile>();
            for (int i = 0; i < 64; i++)
            {
                emptyTileDictionary[i] = new Tile(i);
            }
            
            // Immutable copy of dictionary returned
            return new ReadOnlyDictionary<int, Tile>(emptyTileDictionary);
        }
        
        // Empty tile constructor
        private Tile(int tileCoordinate)
        {
            _tileCoordinate = tileCoordinate;
            _tileType = TileType.EMPTY;
        }
        
        // Occupied tile constructor
        private Tile(int tileCoordinate, Piece piece)
        {
            _tileCoordinate = tileCoordinate;
            _tileType = TileType.OCCUPIED;
            _piece = piece;
        }

        public bool IsOccupied()
        {
            return _tileType.IsOccupied();
        }
        
        // Public interface for creating tiles. Keeps immutability
        public static Tile CreateTile(int tileCoordinate, Piece piece)
        {
            return piece != null ? new Tile(tileCoordinate, piece) : EMPTY_TILES[tileCoordinate];
        }
    }
}