using System.Collections.Generic;
using System.Linq;

namespace FlooringExercise
{
    internal class HandleTiles
    {
        public static TileMap PlaceTiles(List<Tile> tiles, TileMap currentMap)
        {
            if (tiles.Count == 0)
            {
                return currentMap; //no more tiles to place
            }

            var currentTileToPlace = tiles[0];
            tiles.Remove(tiles[0]);
            Location[] possiblePlacements = FindPossiblePlacements(currentMap, currentTileToPlace);

            if (possiblePlacements.Length == 0)
            {
                return PlaceTiles(tiles, currentMap); //no where to place this tile, try to place the remaining ones
            }

            var allTilesMap = new List<TileMap>();

            foreach (var location in possiblePlacements)
            {
                var tileMap = currentMap.Clone();
                tileMap.SetTile(currentTileToPlace, location.X, location.Y);
                var placeTiles = PlaceTiles(tiles.ToList(), tileMap);

                allTilesMap.Add(placeTiles);

                if (placeTiles.FreeTiles <= 0)
                {
                    return placeTiles;
                }
            }

            return allTilesMap
                .OrderBy(tileMap => tileMap.FreeTiles)
                .First();
        }

        private static Location[] FindPossiblePlacements(TileMap currentMap, Tile currentTile)
        {
            var locations = new List<Location>();

            var roundsRight = currentMap.LastPlacedTile.GetBottomRight().Y - currentMap.LastPlacedTile.GetUpperRight().Y;
            var roundsBottom = currentMap.LastPlacedTile.GetBottomRight().X - currentMap.LastPlacedTile.GetBottomLeft().X;
            var roundsLeft = currentMap.LastPlacedTile.GetBottomLeft().Y - currentMap.LastPlacedTile.GetUpperLeft().Y;
            var roundsTop = currentMap.LastPlacedTile.GetUpperRight().X - currentMap.LastPlacedTile.GetUpperLeft().X;
            var flagStatus = true;

            var tileHeight = currentTile.GetHeight();
            var tileWidth = currentTile.GetWidth();
            var getUpperRightX = currentMap.LastPlacedTile.GetUpperRight().X;
            var getUpperRightY = currentMap.LastPlacedTile.GetUpperRight().Y;
            var getUpperLeftX = currentMap.LastPlacedTile.GetUpperLeft().X;
            var getUpperLeftY = currentMap.LastPlacedTile.GetUpperLeft().Y;
            var getBottomLeftX = currentMap.LastPlacedTile.GetBottomLeft().X;
            var getBottomLeftY = currentMap.LastPlacedTile.GetBottomLeft().Y;

            flagStatus = RunFirstCycle(currentMap, roundsRight, roundsBottom, tileHeight, tileWidth, getUpperRightY, getUpperRightX, flagStatus, locations);
            flagStatus = GetRightOptions(currentMap, getUpperRightX, tileWidth, roundsRight, getUpperRightY, tileHeight, flagStatus, locations);
            flagStatus = GetBottomOptions(currentMap, getBottomLeftY, tileHeight, roundsBottom, getBottomLeftX, tileWidth, flagStatus, locations);
            flagStatus = GetLeftOptions(currentMap, getUpperLeftX, tileWidth, roundsLeft, getUpperLeftY, tileHeight, flagStatus, locations);
            GetTopOptions(currentMap, getUpperLeftY, tileHeight, roundsTop, getUpperLeftX, tileWidth, flagStatus, locations);

            return locations.ToArray();
        }

        private static bool GetLeftOptions(TileMap currentMap, int getUpperLeftX, int tileWidth, int roundsLeft, int getUpperLeftY, int tileHeight, bool flagStatus, List<Location> locations)
        {
            if (getUpperLeftX <= tileWidth)
            {
                return flagStatus;
            }

            for (var k = 0; k < roundsLeft; k++)
            {
                if (getUpperLeftY + k + tileHeight >= currentMap.GetMapHeight())
                {
                    continue;
                }

                for (var i = 0; i < tileHeight; i++)
                {
                    if (flagStatus == false)
                    {
                        break;
                    }

                    for (var j = 0; j < tileWidth; j++)
                    {
                        if (!currentMap.Map[getUpperLeftY + i + k, getUpperLeftX - j - 1])
                        {
                            continue;
                        }

                        flagStatus = false;
                        break;
                    }
                }

                if (flagStatus)
                {
                    locations.Add(new Location
                    {
                        X = getUpperLeftX - tileWidth,
                        Y = getUpperLeftY + k
                    });
                }

                flagStatus = true;
            }

            return flagStatus;
        }

        private static void GetTopOptions(TileMap currentMap, int getUpperLeftY, int tileHeight, int roundsTop, int getUpperLeftX, int tileWidth, bool flagStatus, List<Location> locations)
        {
            if (getUpperLeftY < tileHeight)
            {
                return;
            }

            for (var k = 0; k < roundsTop; k++)
            {
                if (getUpperLeftX + k + tileWidth + 1 >= currentMap.GetMapWidth())
                {
                    continue;
                }

                for (var i = 0; i < tileHeight; i++)
                {
                    if (flagStatus == false)
                    {
                        break;
                    }

                    for (var j = 0; j < tileWidth; j++)
                    {
                        if (!currentMap.Map[getUpperLeftY - i - 1, getUpperLeftX + j + k])
                        {
                            continue;
                        }

                        flagStatus = false;
                        break;
                    }
                }

                if (flagStatus)
                {
                    locations.Add(new Location
                    {
                        X = getUpperLeftX + k,
                        Y = getUpperLeftY - tileHeight
                    });
                }

                flagStatus = true;
            }
        }

        private static bool GetBottomOptions(TileMap currentMap, int getBottomLeftY, int tileHeight, int roundsBottom, int getBottomLeftX, int tileWidth, bool flagStatus, List<Location> locations)
        {
            if (getBottomLeftY + tileHeight > currentMap.GetMapHeight())
            {
                return flagStatus;
            }

            for (var k = 0; k < roundsBottom; k++)
            {
                if (getBottomLeftX + k + tileWidth >= currentMap.GetMapWidth())
                {
                    continue;
                }

                for (var i = 0; i < tileHeight; i++)
                {
                    if (flagStatus == false)
                    {
                        break;
                    }

                    for (var j = 0; j < tileWidth; j++)
                    {
                        if (!currentMap.Map[getBottomLeftY + i, getBottomLeftX + j + k])
                        {
                            continue;
                        }

                        flagStatus = false;
                        break;
                    }
                }

                if (flagStatus)
                {
                    locations.Add(new Location
                    {
                        X = getBottomLeftX + k,
                        Y = getBottomLeftY
                    });
                }

                flagStatus = true;
            }

            return flagStatus;
        }

        private static bool GetRightOptions(TileMap currentMap, int getUpperRightX, int tileWidth, int roundsRight, int getUpperRightY, int tileHeight, bool flagStatus, List<Location> locations)
        {
            if (getUpperRightX + tileWidth > currentMap.GetMapWidth())
            {
                return flagStatus;
            }

            for (var k = 0; k < roundsRight; k++)
            {
                if (getUpperRightY + k + tileHeight > currentMap.GetMapHeight())
                {
                    continue;
                }

                for (var i = 0; i < tileHeight; i++)
                {
                    if (flagStatus == false)
                    {
                        break;
                    }

                    for (var j = 0; j < tileWidth; j++)
                    {
                        if (!currentMap.Map[getUpperRightY + i + k, getUpperRightX + j])
                        {
                            continue;
                        }

                        flagStatus = false;
                        break;
                    }
                }

                if (flagStatus)
                {
                    locations.Add(new Location
                    {
                        X = getUpperRightX,
                        Y = getUpperRightY + k
                    });
                }

                flagStatus = true;
            }

            return flagStatus;
        }

        private static bool RunFirstCycle(TileMap currentMap, int roundsRight, int roundsBottom, int tileHeight, int tileWidth, int getUpperRightY, int getUpperRightX, bool flagStatus, List<Location> locations)
        {
            if (roundsRight != 0 || roundsBottom != 0)
            {
                return flagStatus;
            }

            for (var i = 0; i < tileHeight; i++)
            {
                for (var j = 0; j < tileWidth; j++)
                {
                    if (currentMap.Map[getUpperRightY + i, getUpperRightX + j])
                    {
                        flagStatus = false;
                    }
                }
            }

            if (flagStatus)
            {
                locations.Add(new Location
                {
                    X = getUpperRightX,
                    Y = getUpperRightY
                });
            }

            return true;
        }
    }
}
