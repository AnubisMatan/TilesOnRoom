using System.Collections.Generic;

namespace FlooringExercise
{
    internal class HandleTiles
    {
        public static TileMap TryToGetMap(List<Tile> tiles, TileMap currentMap)
        {
            var normalSide = true;
            var rotatedSide = true;
            var mapWidth = currentMap.GetMapWidth();
            var mapHeight = currentMap.GetMapHeight();
            
            for (var i = 0; i < mapHeight; i++)
            {
                for (var j = 0; j < mapWidth; j++)
                {
                    if (currentMap.Map[i, j])
                    {
                        continue;
                    }
                    
                    foreach (var tile in tiles)
                    {
                        var getWidth = tile.GetWidth();
                        var getHeight = tile.GetHeight();

                        if ((j + getWidth > mapWidth ||
                             i + getHeight > mapHeight) &&
                            (j + getHeight > mapWidth ||
                             i + getWidth > mapHeight))
                        {
                            normalSide = false;
                            rotatedSide = false;
                        }

                        //normalSide
                        if (j + getWidth > mapWidth ||
                            i + getHeight > mapHeight)
                        {
                            normalSide = false;
                        }

                        //rotatedSide
                        if (j + getHeight > mapWidth ||
                            i + getWidth > mapHeight)
                        {
                            rotatedSide = false;
                        }

                        var rotatedTile = new Tile(getWidth, getHeight);
                        
                        if (normalSide && rotatedSide)
                        {
                            if (getWidth >= getHeight)
                            {
                                if (FindPossiblePlacement(currentMap, tile, i, j))
                                {
                                    currentMap.SetTile(tile, j, i);
                                    tiles.Remove(tile);

                                    break;
                                }
                            }
                            else
                            {
                                if (FindPossiblePlacement(currentMap, rotatedTile, i, j))
                                {
                                    currentMap.SetTile(rotatedTile, j, i);
                                    tiles.Remove(tile);

                                    break;
                                }
                            }
                        }

                        if (normalSide)
                        {
                            if (FindPossiblePlacement(currentMap, tile, i, j))
                            {
                                currentMap.SetTile(tile, j, i);
                                tiles.Remove(tile);
                                rotatedSide = true;

                                break;
                            }
                        }

                        if (rotatedSide)
                        {
                            if (FindPossiblePlacement(currentMap, rotatedTile, i, j))
                            {
                                currentMap.SetTile(rotatedTile, j, i);
                                tiles.Remove(tile);
                                normalSide = true;

                                break;
                            }
                        }

                        normalSide = true;
                        rotatedSide = true;
                    }
                }
            }

            return currentMap;
        }

        private static bool FindPossiblePlacement(TileMap currentMap, Tile currentTile, int i, int j)
        {
            for (var k = i; k < currentTile.GetHeight() + i; k++)
            {
                for (var l = j; l < currentTile.GetWidth() + j; l++)
                {
                    if (currentMap.Map[k, l])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
