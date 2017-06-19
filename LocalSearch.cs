using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void printGrid(Pair[,] grid)
        {
            int N = (int)Math.Sqrt(grid.Length);
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    Console.Write(grid[row, col].Number.ToString() + ' ');
                }
                Console.WriteLine();
            }
        }
        static int[,] readGrid()
        {
            int N;
            string line = Console.ReadLine();
            string[] seperated = line.Split(' ');
            N = seperated.Length; // een sudoku van N x N
            int[,] puzzle = new int[N, N];
            int number;

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    number = Convert.ToInt32(seperated[j]);
                    puzzle[i, j] = number;
                }
                line = Console.ReadLine();
                seperated = line.Split(' ');
            }
            return puzzle;
        }

        static int HeuristicValue(Pair[,] grid, int N)
        {
            int result = 0;
            for (int i = 0; i < N; i++)
            {
                List<int> rowMissing = Enumerable.Range(1, N).ToList();
                List<int> colMissing = Enumerable.Range(1, N).ToList();
                for (int j = 0; j < N; j++)
                {
                    rowMissing.Remove(grid[i, j].Number);
                    colMissing.Remove(grid[j, i].Number);
                }
                result += rowMissing.Count() + colMissing.Count();
            }
            return result;
        }

        static List<int> BoxOrder(int N)
        {
            List<int> boxOrderList = new List<int>();

            for (int i = 1; i < N + 1; i++)
            {
                boxOrderList.Add(i);
            }

            List<int> randomizedList = new List<int>();
            Random r = new Random();
            int randomIndex = 0;
            while (boxOrderList.Count > 0)
            {
                randomIndex = r.Next(0, boxOrderList.Count);
                randomizedList.Add(boxOrderList[randomIndex]);
                boxOrderList.RemoveAt(randomIndex);
            }
            return randomizedList; //return the new random list
        }

        static Pair[,] fillGrid(int[,] grid)
        {
            int N = (int)Math.Sqrt(grid.Length);
            Pair[,] filledGrid = new Pair[N, N];
            int boxSize = (int)Math.Sqrt(Math.Sqrt(grid.Length));
            List<int>[,] taboe = new List<int>[boxSize, boxSize];

            for (int i = 0; i < boxSize; i++)
                for (int j = 0; j < boxSize; j++)
                {
                    taboe[i, j] = new List<int>();

                    for (int row = i * boxSize; row < (i * boxSize) + boxSize; row++)
                        for (int col = j * boxSize; col < (j * boxSize) + boxSize; col++)
                        {
                            Pair p;
                            p.Number = grid[row, col];
                            if (grid[row, col] != 0)
                            {
                                taboe[i, j].Add(grid[row, col]);
                                p.Fixed = true;
                            }
                            else
                            {
                                p.Fixed = false;
                            }
                            filledGrid[row, col] = p;
                        }
                }

            for (int i = 0; i < boxSize; i++)
                for (int j = 0; j < boxSize; j++)
                {
                    int counter = 1;
                    for (int row = i * boxSize; row < (i * boxSize) + boxSize; row++)
                        for (int col = j * boxSize; col < (j * boxSize) + boxSize; col++)
                        {
                            if (filledGrid[row, col].Fixed == false)
                            {
                                while (taboe[i, j].Contains(counter))
                                {
                                    counter++;
                                }
                                filledGrid[row, col].Number = counter;
                                counter++;
                            }
                        }
                }
            return filledGrid;
        }
        static Pair[,] ChangeInBox(Pair[,] grid, int box, int boxSize)
        {
            int boxRow = box - 1;
            int boxCol = ((box - 1) % boxSize ) * boxSize;

            for (int row = 0; row < boxSize; row++)
            {
                for (int col = 0; col < boxSize; col++)
                {
                    //each number in this specific box
                    Console.Write(grid[boxRow + row, boxCol + col].Number + " ");
                }
                Console.WriteLine();
            }
            return grid;
        }
        static void Main(string[] args)
        {
            int[,] grid = readGrid();
            int N = (int)Math.Sqrt(grid.Length);
            Pair[,] filledGrid = fillGrid(grid);
            printGrid(filledGrid);


            // maak een lijst van lengte sqrt(N) met random volgorde hoe we door de blokken gaan.
            List<int> randomOrder = new List<int>();
            int boxSize = (int)Math.Sqrt(Math.Sqrt(grid.Length));

            while (HeuristicValue(filledGrid, 4) != 0)
            {
                randomOrder = BoxOrder(N); // nieuwe random volgorde in welke box er geswapped word
                foreach (var box in randomOrder)
                {
                    ChangeInBox(filledGrid, box, boxSize);
                }
            }

            Console.WriteLine(HeuristicValue(filledGrid, 4)); // 4 aanpassen naar dynamisch getal
            Console.ReadLine();
        }
        struct Pair
        {
            public int Number;
            public bool Fixed;
        };
    }
}

