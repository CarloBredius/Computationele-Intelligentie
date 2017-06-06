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

            int row, col;
            //find smallest domain that doesn't contain 1 value
            int smallest = N;
            int l = N;
            for (int x = 0; x < N; x++)
                for (int y = 0; y < N; y++)
                {
                    l = grid[x, y].Count;
                    if (l < smallest && l > 1)
                    {
                        smallest = l;
                        row = y;
                        col = x;
                    }
                }
            // If there is none, we are done
            return true; // success!
                         //else get the position of this domain and the domain

            //loop through domain
            foreach (int num in grid[row, col])
            {
                List<int>[,] copy;
                copy = grid;
                // check if a domain collapses to zero 
                if (updateDomains(grid, N, boxSize, row, col, num))
                {
                    // return, if success, yay!
                    if (SolveSudoku(grid, N, boxSize))
                        return true;
                    // failure, unmake & try again
                    grid = copy;
                    //get back to the old copy

                }

            }
            return false; // this triggers backtracking
        }


        /* Returns a boolean which indicates whether any assigned entry
         in the specified row matches the given number. */
        static bool notAllowedInRow(List<int>[,] grid, int N, int row, int num)
        {
            for (int col = 0; col < N; col++)
            {
                if (grid[row, col] == new List<int> { num })
                    return true;
                grid[row, col].Remove(num);
            }
            return false;
        }
        /* Returns a boolean which indicates whether any assigned entry
           in the specified column matches the given number. */
        static bool notAllowedInCol(List<int>[,] grid, int N, int col, int num)
        {
            for (int row = 0; row < N; row++)
            {
                if (grid[row, col] == new List<int> { num })
                    return true;
                grid[row, col].Remove(num);
            }
            return false;
        }

        /* Returns a boolean which indicates whether any assigned entry
           within the specified 3x3 box matches the given number. */
        static bool notAllowedInBox(List<int>[,] grid, int boxSize, int boxStartRow, int boxStartCol, int num)
        {
            for (int row = 0; row < boxSize; row++)
                for (int col = 0; col < boxSize; col++)
                {
                    if (grid[row + boxStartRow, col + boxStartCol] == new List<int> { num })
                        return true;
                    grid[row + boxStartRow, col + boxStartCol].Remove(num);
                }
            return false;
        }

        static bool updateDomains(List<int>[,] grid, int N, int boxSize, int row, int col, int num)
        {
            /* Check if 'num' is not already placed in current row,
        current column and current NxN box */
            return !notAllowedInRow(grid, N, row, num) &&
                   !notAllowedInCol(grid, N, col, num) &&
                   !notAllowedInBox(grid, boxSize, row - row % boxSize, col - col % boxSize, num);
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
            List<int> domain = Enumerable.Range(1, N).ToList();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    domainGrid[i, j] = domain;
                }
            }
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (grid[i, j] != 0)
                    {
                        domainGrid[i, j] = new List<int> { grid[i, j] };
                        updateDomains(domainGrid, N, boxSize, i, j, grid[i, j]);
                    }

                }
            }
            return domainGrid;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("enter a NxN sudoku with N lines of N numbers with white space between them");

            int[,] grid = readGrid();

            int N = (int)Math.Sqrt(grid.Length);
            int boxSize = Convert.ToInt32(Math.Sqrt(N));

            // Keep track of an NxN array of lists, containing the numbers possible at that position
            List<int>[,] domainGrid = new List<int>[N, N];
            domainGrid = gridToDomainGrid(grid, N, boxSize);



            // Keep track of time
            var watch = System.Diagnostics.Stopwatch.StartNew();

            if (SolveSudoku(domainGrid, N, boxSize) == true)
            {
                printGrid(grid, N);
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

