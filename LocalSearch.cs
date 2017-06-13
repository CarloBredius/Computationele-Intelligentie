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
        static Pair[,] fillGrid(int[,] grid)
        {
            int N = (int)Math.Sqrt(grid.Length);
            Pair[,] filledGrid = new Pair[N,N];
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
                                if (taboe[i, j].Contains(counter))
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
        static void Main(string[] args)
        {
            int[,] grid = readGrid();
            Pair[,] filledGrid = fillGrid(grid);
            printGrid(filledGrid);
            Console.ReadLine();
        }
        struct Pair
        {
            public int Number;
            public bool Fixed;
        };
    }
}

    