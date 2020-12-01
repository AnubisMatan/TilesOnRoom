using System.Collections.Generic;
using System.Linq;

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

        public Rectangle LastPlacedTile { get; set; }

        public List<Tile> UsedTiles { get; set; }

        public TileMap(int height, int width) //size of room
        {
            _width = width;
            _height = height;
            FreeTiles = width * height;
            Map = new bool[height, width];
            UsedTiles = new List<Tile>();

            var upperLeft = new Location { X = 0, Y = 0 };
            var upperRight = new Location { X = 0, Y = 0 };
            var bottomLeft = new Location { X = 0, Y = 0 };
            var bottomRight = new Location { X = 0, Y = 0 };

            LastPlacedTile = new Rectangle(upperLeft, upperRight, bottomLeft, bottomRight);
        }

        public void SetTile(Tile tile, int x, int y)
        {
            var tileWidth = tile.GetWidth();
            var tileHeight = tile.GetHeight();
            var upperLeft = new Location { X = x, Y = y };
            var upperRight = new Location { X = x + tileWidth, Y = y };
            var bottomLeft = new Location { X = x, Y = y + tileHeight };
            var bottomRight = new Location { X = x + tileWidth, Y = y + tileHeight };

            LastPlacedTile = new Rectangle(upperLeft, upperRight, bottomLeft, bottomRight);
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

        public TileMap Clone()
        {
            var height = GetMapHeight();
            var width = GetMapWidth();
            var newMap = new bool[height, width];
            int i;
            var j = 0;
            var odd = 0;
            var remain = false;
            var tempWidth = width;

            if (width % 2 != 0)
            {
                odd = tempWidth;
                tempWidth--;

                remain = true;
            }
            
            for (i = 0; i < height; i ++)
            {
                for (j = 0; j < tempWidth; j += 2)
                {
                    if (Map[i, j])
                    {
                        newMap[i, j] = true;
                    }

                    if (Map[i, j + 1])
                    {
                        newMap[i, j + 1] = true;
                    }
                }
            }

            if (remain)
            {
                for (; i < height; i++)
                {
                    for (; j < odd; j ++)
                    {
                        if (Map[i, j])
                        {
                            newMap[i, j] = true;
                        }
                    }
                }
            }

            return new TileMap(height, width)
            {
                Map = newMap,
                UsedTiles = UsedTiles.ToList(),
                FreeTiles = FreeTiles,
                LastPlacedTile = new Rectangle(LastPlacedTile.GetUpperLeft(), LastPlacedTile.GetUpperRight(), LastPlacedTile.GetBottomLeft(), LastPlacedTile.GetBottomRight())
            };
        }
    }
}
