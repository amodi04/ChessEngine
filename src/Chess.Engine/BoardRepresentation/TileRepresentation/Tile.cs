using System;
using System.Collections.Generic;
using Engine.Enums;
using Engine.Extensions;
using Engine.Pieces;
using Engine.Util;

namespace Engine.BoardRepresentation.TileRepresentation
{
    /// <summary>
    ///     This class contains tile data which will be used for determining relative positions in a game.
    /// </summary>
    public class Tile
    {
        // Member fields
        private static readonly IDictionary<int, Tile> EmptyTiles = InitialiseEmptyTiles();
        private readonly Piece _piece;
        private readonly TileType _tileType;

        /// <summary>
        ///     Empty tile constructor to allow the creation of unoccupied tiles.
        ///     Private-only access so that immutability is retained.
        /// </summary>
        /// <param name="tileCoordinate">The position of the tile in a grid of 64 tiles.</param>
        private Tile(int tileCoordinate)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.Empty;
        }

        /// <summary>
        ///     Occupied tile constructor to allow the creation of occupied tiles.
        ///     Private-only access so that immutability is retained.
        /// </summary>
        /// <param name="tileCoordinate">The position of the tile in a grid of 64 tiles.</param>
        /// <param name="piece">The piece object that occupies the tile.</param>
        private Tile(int tileCoordinate, Piece piece)
        {
            TileCoordinate = tileCoordinate;
            _tileType = TileType.Occupied;
            _piece = piece;
        }

        public int TileCoordinate { get; }
        public Piece Piece => IsOccupied() ? _piece : null;

        /// <summary>
        ///     Gets the string representation of the tile object.
        /// </summary>
        /// <returns>"-" if tile is empty. If occupied, the string representation of the piece is returned.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Called when an unknown tile state is passed in.</exception>
        public override string ToString()
        {
            return _tileType switch
            {
                TileType.Empty => "-",
                TileType.Occupied => Piece.ToString(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        ///     Loops from 0 to 64 to generate 64 empty tiles.
        /// </summary>
        /// <returns>An IDictionary containing references to the 64 empty tiles.</returns>
        private static IDictionary<int, Tile> InitialiseEmptyTiles()
        {
            var emptyTileDictionary = new Dictionary<int, Tile>();

            // Loop through 64 times and create empty tiles.
            for (var i = 0; i < BoardUtilities.NumTiles; i++) emptyTileDictionary[i] = new Tile(i);

            return emptyTileDictionary;
        }

        /// <summary>
        ///     Gets the occupancy status of the tile.
        /// </summary>
        /// <returns>Occupancy status of tile.</returns>
        public bool IsOccupied()
        {
            return _tileType.IsOccupied();
        }

        /// <summary>
        ///     Public interface for creating tiles. Tile is created based on piece passed in.
        /// </summary>
        /// <param name="tileCoordinate">The position of the tile in a grid of 64 tiles.</param>
        /// <param name="piece">The piece object that occupies the tile.</param>
        /// <returns>
        ///     An occupied tile if the piece object exists.
        ///     Otherwise the reference to the tile at the passed in tile coordinate is returned.
        /// </returns>
        public static Tile CreateTile(int tileCoordinate, Piece piece)
        {
            // Create an occupied tile if their is a piece.
            // otherwise return the lookup reference in the empty tiles dictionary.
            return piece != null ? new Tile(tileCoordinate, piece) : EmptyTiles[tileCoordinate];
        }
    }
}