using System.Collections.Generic;

namespace FlooringExercise
{
    public class TileMap
    {
        private readonly int _width;
        private readonly int _height;

        public int GetMapWidth() => _width;

        public int GetMapHeight() => _height;

        public bool[,] Map { get; set; }

        public int FreeTiles { get; set; }

        public List<Tile> UsedTiles { get; set; }

        public TileMap(int height, int width) //size of room
        {
            _width = width;
            _height = height;
            FreeTiles = width * height;
            Map = new bool[height, width];
            UsedTiles = new List<Tile>();
        }

        public void SetTile(Tile tile, int x, int y)
        {
            var tileWidth = tile.GetWidth();
            var tileHeight = tile.GetHeight();

            UsedTiles.Add(tile);

            for (var i = y; i < tileHeight + y; i++)
            {
                for (var j = x; j < tileWidth + x; j++)
                {
                    Map[i, j] = true;
                    FreeTiles -= 1;
                }
            }
        }
    }
}
