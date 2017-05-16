using System;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        // N is used for size of Sudoku grid. Size will be NxN
        int N = 9;
        /* Takes a partially filled-in grid and attempts to assign values to
                all unassigned locations in such a way to meet the requirements
                for Sudoku solution (non-duplication across rows, columns, and boxes) */
        static bool SolveSudoku(int[,] grid, int N, int boxSize)
        {
            int[] position;
            // If there is no unassigned location, we are done
            position = FindUnassignedLocation(grid, N);
            if (position[0] == -1)
                return true; // success!
            int row = position[0];
            int col = position[1];
            // consider digits 1 to N
            for (int num = 1; num <= N; num++)
            {
                // if looks promising
                if (isSafe(grid, N, boxSize, row, col, num))
                {
                    // make tentative assignment
                    grid[row,col] = num;
                    // return, if success, yay!
                    if (SolveSudoku(grid, N, boxSize))
                        return true;
                    // failure, unmake & try again
                    grid[row,col] = 0;
                }

            }
            return false; // this triggers backtracking
        }

        /* Searches the grid to find an entry that is still unassigned. If
         found, the reference parameters row, col will be set the location
         that is unassigned, and true is returned. If no unassigned entries
         remain, false is returned. */
        static int[] FindUnassignedLocation(int[,] grid, int N)
        {

            int[] point = new int[2];
            point[0] = -1;
            point[1] = -1;
            for (int row = 0; row < N; row++)
                for (int col = 0; col < N; col++)
                    if (grid[row,col] == 0)
                    {
                        point[0] = row;
                        point[1] = col;
                        return point;
                    }
            return point;
        }
        /* Returns a boolean which indicates whether any assigned entry
         in the specified row matches the given number. */
        static bool UsedInRow(int[,] grid, int N, int row, int num)
        {
            for (int col = 0; col < N; col++)
                if (grid[row,col] == num)
                    return true;
            return false;
        }
        /* Returns a boolean which indicates whether any assigned entry
           in the specified column matches the given number. */
        static bool UsedInCol(int[,] grid, int N, int col, int num)
        {
            for (int row = 0; row < N; row++)
                if (grid[row,col] == num)
                    return true;
            return false;
        }

        /* Returns a boolean which indicates whether any assigned entry
           within the specified 3x3 box matches the given number. */
        static bool UsedInBox(int[,] grid, int boxSize, int boxStartRow, int boxStartCol, int num)
        {

            for (int row = 0; row < boxSize; row++)
                for (int col = 0; col < boxSize; col++)
                    if (grid[row + boxStartRow,col + boxStartCol] == num)
                        return true;
            return false;
        }

        /* Returns a boolean which indicates whether it will be legal to assign
           num to the given row,col location. */
        static bool isSafe(int[,] grid, int N, int boxSize, int row, int col, int num)
        {
            /* Check if 'num' is not already placed in current row,
        current column and current NxN box */
            return !UsedInRow(grid, N, row, num) &&
                   !UsedInCol(grid, N, col, num) &&
                   !UsedInBox(grid, boxSize, row - row % boxSize, col - col % boxSize, num);
        }
        /* A utility function to print grid  */
        static void printGrid(int[,] grid, int N)
        {
            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    Console.Write(grid[row,col].ToString() + ' ');
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
        static int[,] readGrid()
          {
              int N;
              string line = Console.ReadLine();
              string[] seperated = line.Split(' ');
              N = seperated.Length; // een sudoku van N x N
              int[,] puzzle = new int[N,N];
              int number;

            for (int i = 0; i < N; i++)
            {
                //puzzle[i] = new int[N];
                  for (int j = 0; j < N; j++)
                  {
                      number = Convert.ToInt32(seperated[j]);
                      puzzle[i,j] = number;
                  }
                  line = Console.ReadLine();
                  seperated = line.Split(' ');
              }
              return puzzle;
          }
        static void Main(string[] args)
        {
            int[,] grid = readGrid();

            //int[,] grid = new int[9, 9]{
            //    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            //    { 0, 0, 0, 0, 0, 3, 0, 8, 5 },
            //    { 0, 0, 1, 0, 2, 0, 0, 0, 0 },
            //    { 0, 0, 0, 5, 0, 7, 0, 0, 0 },
            //    { 0, 0, 4, 0, 0, 0, 1, 0, 0 },
            //    { 0, 9, 0, 0, 0, 0, 0, 0, 0 },
            //    { 0, 0, 0, 5, 0, 7, 0, 0, 0 },
            //    { 5, 0, 0, 0, 0, 0, 0, 7, 3 },
            //    { 0, 0, 2, 0, 1, 0, 0, 0, 0 }
            //    };
            int N = (int)Math.Sqrt(grid.Length);
            int boxSize = Convert.ToInt32(Math.Sqrt(N));

            if (SolveSudoku(grid, N, boxSize) == true)
                printGrid(grid, N);
            else
                Console.WriteLine("No solution exists");
        }
    }
}
