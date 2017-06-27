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
        /* recursive function that solves the sudoku using forward checking with the most constrained
         * variables heuristic. The sudoku is descripted as an 2D-array of lists of the domains. 
         * If a list also contains a zero, besides the domain, it is unassigned, without a zero it is assigned.*/
        static bool SolveSudoku(List<int>[,] grid, int N, int boxSize)
        {
            // keep track of the amount of recursive steps
            recursiveSteps++;

            int row = -1, col = -1;
            // find smallest domain of the positions not filled in jet (so 0 is still in domain)
            int smallest = N + 1;
            int l;
            for (int x = 0; x < N; x++)
                for (int y = 0; y < N; y++)
                {
                    if (grid[y, x].First() == 0)//we look only for unassigned positions
                    {
                        l = grid[y, x].Count();
                        if (l < smallest)
                        {
                            smallest = l;
                            row = y;
                            col = x;
                        }
                    }

                }
            // If there is none, we are done
            if (row == -1)
            {
                printGrid(domainGridToGrid(grid, N, boxSize), N);
                return true; // success!


            }
            //we make a copy of the grid in order to fall back to a previous state if a node in the 
            //search tree can't lead to a solution
            List<int>[,] copy = copyGrid(grid, N);
            //loop through the smallest domain 
            foreach (int num in grid[row, col])
            {
                if (num == 0) continue;//the zero is not part of the domain
                //checks if this particular number is possible according to the forward checking algorithm
                if (!updateDomains(grid, N, boxSize, row, col, num))
                {
                    //if not, fall back to the copy and try the next number in the domain
                    grid = copyGrid(copy, N);
                    continue;
                }
                // checks if the new state of the sudoku can lead to a solution
                if (SolveSudoku(grid, N, boxSize))
                    return true;//succes!

                //no succes, fall back to the copy and try next number in the domain
                grid = copyGrid(copy, N);
            }
            return false; // none of the numbers in the domain could lead to a solution, 
                          //return false to the previous recursive step
        }
        //function to make a independent copy of the grid
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
        //checks if a number can be assigned according to forward checking
        static bool updateDomains(List<int>[,] grid, int N, int boxSize, int row, int col, int num)
        {
            //create a bool grid with 1 in the row, col or box of the position
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
            //loops through the entire grid
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    //if boolgrid is 1 this possition might need an altered domain
                    if (boolGrid[i, j] == 1)
                    {
                        //if it is the assigned position, the domain collapses to the given number
                        if (i == row && j == col)
                            grid[i, j] = new List<int> { num };
                        else
                        {
                            //if it is an unassigned position, remove the number from this domain. If after this the domain
                            //only contains 0, (count()==1), something went wrong and false is returned
                            if (grid[i, j].First() == 0)
                            {
                                grid[i, j].Remove(num);
                                if (grid[i, j].Count() == 1)
                                {
                                    return false;
                                }
                            }

                        }
                    }
                }
            }
            return true;
        }
        //function to print a simple 2D integer arrray
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
        //function to read the grid from the console
        static int[,] readGrid()
        {
            int N;
            string line = Console.ReadLine();
            if (line.Last() == ' ') line.Remove(line.Length);
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

        //function to change a integer 2D array into a 2D array with lists of the domains
        static List<int>[,] gridToDomainGrid(int[,] grid, int N, int boxSize)
        {
            List<int>[,] domainGrid = new List<int>[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    domainGrid[i, j] = Enumerable.Range(0, N + 1).ToList();
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
        //function to get back to the simple grid to be able to print it
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
            Console.WriteLine("enter a NxN sudoku with N lines of N numbers with white space between them and with 0's representing unassigned positions");

            int[,] grid = readGrid();

            int N = (int)Math.Sqrt(grid.Length);
            int boxSize = Convert.ToInt32(Math.Sqrt(N));

            // Keep track of an NxN array of lists, containing the numbers possible at that position
            List<int>[,] domainGrid;
            domainGrid = gridToDomainGrid(grid, N, boxSize);



            // Keep track of time
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //solve the sudoku
            if (SolveSudoku(domainGrid, N, boxSize) == true)
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Elapsed time: " + elapsedMs + " milliseconds");
                Console.WriteLine("Recursive steps: " + recursiveSteps);
                Console.ReadLine();
            }
            else
                Console.WriteLine("No solution exists");
        }
    }
}

