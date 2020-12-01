using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;

namespace FlooringExercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<string> file = null;

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

            List<Tile> tiles;
            Console.WriteLine("Calculating...");

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

            tiles = tiles.OrderByDescending(t => t.GetHeight() * t.GetWidth()).ToList();
            GetBestMap(tiles);

            Console.WriteLine("\nProcess finished, please type any key to exit");
            Console.ReadKey();
        }

        private static void GetBestMap(List<Tile> tiles)
        {
            const int roomSize = 25;
            var usedTiles = new Dictionary<string, int>();
            var bestMap = HandleTiles.PlaceTiles(tiles, new TileMap(roomSize, roomSize));

            foreach (var tileSize in bestMap.UsedTiles.Select(usedTile => $"{usedTile.GetWidth()} x {usedTile.GetHeight()}"))
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

            PrintMap(bestMap);
            PrintUsedTiles(usedTiles);
            PrintFreeAreas(bestMap.FreeTiles);
        }

        private static void PrintFreeAreas(in int bestMapFreeTiles)
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

            foreach (var (tileSize, count) in usedTiles)
            {
                Console.WriteLine($" {count}x of {tileSize}");
            }
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
                        Console.SetCursorPosition(j +  1, i + top + 1);
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
