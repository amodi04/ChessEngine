using System.Collections.Generic;
using Engine.Pieces;

namespace Engine.Board
{
    public class Tile
    {
        public int TileCoordinate { get; }
        private Piece _piece;
        public Piece Piece => IsOccupied() ? _piece : null;
        
        private readonly TileType _tileType;

        // Put all tiles into a dictionary. This is so that there are no repeating indexes.
        // 0-63 tiles
        private static readonly Dictionary<int, Tile> EMPTY_TILES = InitialiseEmptyTiles();
        
        // Initialises the board of 64 tiles
        private static Dictionary<int, Tile> InitialiseEmptyTiles()
        {
            Dictionary<int, Tile> emptyTileDictionary = new Dictionary<int, Tile>();
            for (int i = 0; i < 64; i++)
            {
                emptyTileDictionary[i] = new Tile(i);
            }
            
            return emptyTileDictionary;
        }
        
        // Empty tile constructor
        private Tile(int tileCoordinate)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.EMPTY;
        }
        
        // Occupied tile constructor
        private Tile(int tileCoordinate, Piece piece)
        {
            TileCoordinate = tileCoordinate;
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