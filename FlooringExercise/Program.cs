using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlooringExercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<string> file = null;

            #region GetFile
            while (file == null)
            {
                string fileName;

                try
                {
                    fileName = GetFileName(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();

                    return;
                }

                try
                {
                    file = File.ReadAllLines(fileName).ToList();
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
            } 
            #endregion

            Console.WriteLine("Calculating...");
            List<Tile> tiles;

            try
            {
                tiles = GetTiles(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();

                return;
            }

            List<Tile> tilesHighToLow = tiles.OrderByDescending(t => t.GetHeight() * t.GetWidth()).ToList();
            List<Tile> tilesLowToHigh = tiles.OrderBy(t => t.GetHeight() * t.GetWidth()).ToList();
            
            var bestMap = GetBestMap(new List<List<Tile>> { tilesLowToHigh, tilesHighToLow });

            PrintMap(bestMap.Map);
            PrintUsedTiles(bestMap.UsedTiles);
            PrintFreeAreas(bestMap.Map.FreeTiles);

            Console.WriteLine("\nProcess finished, please type any key to exit");
            Console.ReadKey();
        }

        private static BestMap GetBestMap(IEnumerable<List<Tile>> tilesLists)
        {
            TileMap bestMap = null;
            Dictionary<string, int> bestUsedTiles = null;

            foreach (List<Tile> tilesList in tilesLists)
            {
                var map = GetMap(tilesList, out Dictionary<string, int> usedTiles);

                if (bestMap == null)
                {
                    bestMap = map;
                    bestUsedTiles = usedTiles;
                }
                else if (map.FreeTiles < bestMap.FreeTiles)
                {
                    bestMap = map;
                    bestUsedTiles = usedTiles;
                }
            }

            return new BestMap
            {
                Map = bestMap,
                UsedTiles = bestUsedTiles
            };
        }

        private static TileMap GetMap(List<Tile> tiles, out Dictionary<string, int> usedTiles)
        {
            const int roomSize = 100;
            usedTiles = new Dictionary<string, int>();
            var map = HandleTiles.TryToGetMap(tiles, new TileMap(roomSize, roomSize));

            foreach (var tileSize in map.UsedTiles.Select(usedTile => $"{usedTile.GetWidth()} x {usedTile.GetHeight()}"))
            {
                if (usedTiles.ContainsKey(tileSize))
                {
                    usedTiles[tileSize]++;
                }
                else
                {
                    usedTiles.Add(tileSize, 1);
                }
            }

            return map;
        }

        private static void PrintFreeAreas(int bestMapFreeTiles)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nAreas without tiles: {bestMapFreeTiles}");
            Console.ResetColor();
        }

        private static void PrintUsedTiles(Dictionary<string, int> usedTiles)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nUsed Tiles:");
            Console.ResetColor();
            var sum = 0;
            var countTiles = 0;
            foreach (var (tileSize, count) in usedTiles)
            {
                var splited = tileSize.Split("x");
                sum = sum + int.Parse(splited[0].Trim()) * int.Parse(splited[1].Trim()) * count;
                countTiles = countTiles + count;
                Console.WriteLine($" {count}x of {tileSize}");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nArea covered: {sum}");
            Console.WriteLine($"\nUsed tiles: {countTiles}");
            Console.ResetColor();
        }

        private static List<Tile> GetTiles(IEnumerable<string> file)
        {
            var tiles = new List<Tile>();

            foreach (string[] temp in file.Select(line => line.Split(',')))
            {
                var parsedHeight = int.TryParse(temp[0], out var height);
                var parsedWidth = int.TryParse(temp[1], out var width);

                if (!parsedHeight || !parsedWidth)
                {
                    throw new FormatException("One of tiles input is not in the correct format, please check the file");
                }

                var tile = new Tile(width / 10, height / 10);
                tiles.Add(tile);
            }

            return tiles;
        }

        private static string GetFileName(IEnumerable<string> args)
        {
            string fileName = null;

            foreach (var arg in args)
            {
                if (arg.ToLower().StartsWith("-filename="))
                {
                    fileName = arg.Split('=')[1];
                }
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("The system could not get the file, please type the filename contains the tiles:");
                return Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(fileName, "Filename argument cannot be null, please check argument -'fileName'");
            }

            return fileName;
        }

        private static void PrintMap(TileMap tileMap)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nBest Map:");
            Console.ResetColor();

            Console.WriteLine("*: Area with tile placed");
            Console.WriteLine("o: Area w/o tile placed");

            int top = Console.CursorTop;

            for (var i = 0; i < tileMap.GetMapHeight(); i++)
            {
                for (var j = 0; j < tileMap.GetMapWidth(); j++)
                {
                    if (tileMap.Map[i, j])
                    {
                        Console.SetCursorPosition(j + 1, i + top + 1);
                        Console.WriteLine("*");

                        continue;
                    }

                    Console.SetCursorPosition(j + 1, i + top + 1);
                    Console.WriteLine("o");
                }
            }
        }
    }
}
