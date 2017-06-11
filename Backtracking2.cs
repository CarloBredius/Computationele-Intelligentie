using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static int recursiveSteps = 0;
        /* Takes a partially filled-in grid and attempts to assign values to
                all unassigned locations in such a way to meet the requirements
                for Sudoku solution (non-duplication across rows, columns, and boxes) */
        static bool SolveSudoku(List<int>[,] grid, int N, int boxSize)
        {
            // keep track of the amount of recursive steps
            recursiveSteps++;

            int row = 0, col = 0;
            // find smallest domain that doesn't contain 1 value
            int smallest = 1;
            int l = N;
            for (int x = 0; x < N; x++)
                for (int y = 0; y < N; y++)
                {
                    l = grid[x, y].Count;
                    if (l < smallest && l > 1 || l == 2)
                    {
                        smallest = l;
                        row = y;
                        col = x;
                    }
                }
            // If there is none, we are done
            if (smallest == 1)
                return true; // success!
            List<int>[,] copy = copyGrid(grid, N);
            //loop through this domain
            foreach (int num in grid[row, col])
            {
                if (!updateDomains(grid, N, boxSize, row, col, num))
                {
                    grid = copyGrid(copy, N);
                    continue;
                }
                // return, if success, yay!
                if (SolveSudoku(grid, N, boxSize))
                    return true;
                // failure, unmake & try again
                grid = copyGrid(copy, N);
                //get back to the old copy
            }
            return false; // this triggers backtracking
        }

        static List<int>[,] copyGrid(List<int>[,] grid, int N)
        {
            List<int>[,] result = new List<int>[N, N];
            for (int x = 0; x < N; x++)
                for (int y = 0; y < N; y++)
                {
                    result[x, y] = new List<int>(grid[x, y]);
                }
            return result;
        }
        static bool updateDomains(List<int>[,] grid, int N, int boxSize, int row, int col, int num)
        {
            int[,] boolGrid = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                boolGrid[row, i] = 1;
                boolGrid[i, col] = 1;
            }
            for (int i = 0; i < boxSize; i++)
            {
                for (int j = 0; j < boxSize; j++)
                {
                    boolGrid[i + row - row % boxSize, j + col - col % boxSize] = 1;
                }
            }

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (boolGrid[i, j] == 1)
                    {
                        if (i == row && j == col)
                            grid[i, j] = new List<int> { num };
                        else
                            grid[i, j].Remove(num);
                        if (grid[i, j].Count() == 0) return false;
                    }
                }
            }
            return true;
        }
        static void printGrid(int[,] grid, int N)
        {
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    Console.Write(grid[row, col].ToString() + ' ');
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
                //puzzle[i] = new int[N];
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

        static List<int>[,] gridToDomainGrid(int[,] grid, int N, int boxSize)
        {
            List<int>[,] domainGrid = new List<int>[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    domainGrid[i, j] = Enumerable.Range(1, N).ToList();
                }
            }
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        updateDomains(domainGrid, N, boxSize, i, j, grid[i, j]);
                    }
                }
            }
            return domainGrid;
        }
        static int[,] domainGridToGrid(List<int>[,] domainGrid, int N, int boxSize)
        {
            int[,] grid = new int[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    grid[i, j] = domainGrid[i, j].First();
                }
            }
            return grid;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("enter a NxN sudoku with N lines of N numbers with white space between them");

            int[,] grid = readGrid();

            int N = (int)Math.Sqrt(grid.Length);
            int boxSize = Convert.ToInt32(Math.Sqrt(N));

            // Keep track of an NxN array of lists, containing the numbers possible at that position
            List<int>[,] domainGrid;
            domainGrid = gridToDomainGrid(grid, N, boxSize);



            // Keep track of time
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (SolveSudoku(domainGrid, N, boxSize) == true)
            {
                printGrid(domainGridToGrid(domainGrid, N, boxSize), N);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Elapsed time: " + elapsedMs / 1000 + '.' + (elapsedMs - (elapsedMs / 1000)) + " seconds");
                Console.WriteLine("Recursive steps: " + recursiveSteps);
                Console.ReadLine();
            }
            else
                Console.WriteLine("No solution exists");
        }
    }
}

