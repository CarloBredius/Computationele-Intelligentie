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
                    // Make fixed numbers red
                    if (grid[row, col].Fixed)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(grid[row, col].Number.ToString() + ' ');
                        Console.ResetColor();
                    }
                    else
                        Console.Write(grid[row, col].Number.ToString() + ' ');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
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

        static int randomWalk(Pair[,] grid, int box, int boxSize, int N, Random r)
        {
            int row1;
            int col1;
            do
            {
                int pos1 = r.Next(N);
                int boxRow = box / boxSize;
                int boxCol = box % boxSize;
                row1 = boxRow * boxSize + pos1 / boxSize;
                col1 = boxCol * boxSize + pos1 % boxSize;
            }
            while (grid[row1, col1].Fixed == true);

            int row2;
            int col2;
            do
            {
                int pos2 = r.Next(N);
                int boxRow = box / boxSize;
                int boxCol = box % boxSize;
                row2 = boxRow * boxSize + pos2 / boxSize;
                col2 = boxCol * boxSize + pos2 % boxSize;
            }
            while (row2 == row1 && col2 == col1 || grid[row2, col2].Fixed == true);

            //we have a swappeble match, calculate the heuristic Change of this swap:
            int heuristicChange = swapHeuristic(grid, N, row1, col1, row2, col2);

            //swap them                
            int two = grid[row2, col2].Number;
            grid[row2, col2].Number = grid[row1, col1].Number;
            grid[row1, col1].Number = two;
            //update heuristic value
            return heuristicChange;
        }
        /*function that perfoms a swap in the given box and alters the heuristicValue
        if its the best heuristic swap in the box and if the total heuristic value of 
        the grid lowers.Returns whether there was a swap or not.*/
        static int hillClimb(Pair[,] grid, int box, int boxSize, int N)
        {
            int boxRow = box / boxSize;
            int boxCol = box % boxSize;
            //store the best swap and its improvement. 
            //(bestSwap contains the row and col of both positions, bestImprovement is the heuristic improvement of the swap (<=0)) 
            int[] bestSwap = new int[4];
            int bestImprovement = 0;
            //iterate through the box
            for (int i = 0; i < N; i++)
            {
                //get the absolute positions in the grid of the current cell
                int row1 = boxRow * boxSize + i / boxSize;
                int col1 = boxCol * boxSize + i % boxSize;
                //check if this cell is not fixed
                if (grid[row1, col1].Fixed == false)
                {
                    //iterate through the remaining cells in the box
                    for (int j = i + 1; j < N; j++)
                    {
                        int row2 = boxRow * boxSize + j / boxSize;
                        int col2 = boxCol * boxSize + j % boxSize;
                        //check if this cell is not fixed
                        if (grid[row2, col2].Fixed == false)
                        {
                            //we have a swappeble match, calculate the heuristicImprovement of this swap:
                            int heuristicImprovement = swapHeuristic(grid, N, row1, col1, row2, col2);
                            //if its the best swap so far, store the rate of improvement and the cells of the swap
                            if (heuristicImprovement <= bestImprovement)
                            {
                                bestImprovement = heuristicImprovement;
                                bestSwap = new int[] { row1, col1, row2, col2 };
                            }
                        }
                    }
                }
            }
            //we have checked every possible swap in the box. Now we can perform the best swap, 
            //and change the heuristic value
            int rowOne = bestSwap[0];
            int colOne = bestSwap[1];
            int rowTwo = bestSwap[2];
            int colTwo = bestSwap[3];

            int two = grid[rowTwo, colTwo].Number;
            grid[rowTwo, colTwo].Number = grid[rowOne, colOne].Number;
            grid[rowOne, colOne].Number = two;
            return bestImprovement;
        }
        static int swapHeuristic(Pair[,] grid, int N, int row1, int col1, int row2, int col2)
        {
            int result = 0;
            //iterate 2 times, to calculate the heuristic value before and after the swap
            for (int j = -1; j < 2; j += 2)
            {
                //in the second iteration, make the swap before calculating heuristic value
                if (j == 1)
                {
                    int two2 = grid[row2, col2].Number;
                    grid[row2, col2].Number = grid[row1, col1].Number;
                    grid[row1, col1].Number = two2;
                }
                //calculate heuristic value of the two rows and two columns that are involved in the swap
                List<int> row1Missing = Enumerable.Range(1, N).ToList();
                List<int> col1Missing = Enumerable.Range(1, N).ToList();
                List<int> row2Missing = Enumerable.Range(1, N).ToList();
                List<int> col2Missing = Enumerable.Range(1, N).ToList();

                for (int i = 0; i < N; i++)
                {
                    row1Missing.Remove(grid[row1, i].Number);
                    col1Missing.Remove(grid[i, col1].Number);
                    row2Missing.Remove(grid[row2, i].Number);
                    col2Missing.Remove(grid[i, col2].Number);
                }
                //the result is negative if the number of missing numbers decreases, which is good, 
                //       and is positive if the number of missing numbers increases, which is bad.
                //(remember that j is -1 before the swap and 1 after the swap)
                result += j * (row1Missing.Count() + col1Missing.Count() + row2Missing.Count() + col2Missing.Count());
            }
            //undo the swap and return the heuristic improvement of the swap.
            int two = grid[row2, col2].Number;
            grid[row2, col2].Number = grid[row1, col1].Number;
            grid[row1, col1].Number = two;
            return result;
        }
        static void Main(string[] args)
        {
            List<int> heuristicsData = new List<int>();
            int maxTimeOnPlateau = 50;
            int S = 1;
            int s = 0;
            bool performRandomWalk = false;

            int[,] grid = readGrid();
            int N = (int)Math.Sqrt(grid.Length);
            Pair[,] filledGrid = fillGrid(grid);
            printGrid(filledGrid);

            // maak een lijst van lengte sqrt(N) met random volgorde hoe we door de blokken gaan.
            int boxSize = (int)Math.Sqrt(Math.Sqrt(grid.Length));
            int heuristicValue = HeuristicValue(filledGrid, N);
            heuristicsData.Add(heuristicValue);
            int timesOnPlateau = 0;
            Random r = new Random();
            List<int> listPlateauSizes = new List<int>();
            List<int> steps = new List<int>();
            for (int i = 5; i < 30; i++)
            {

                maxTimeOnPlateau = i;
                heuristicsData.Clear();
                for (int j = 0; j < 30; j++)
                {
                    filledGrid = fillGrid(grid);
                    while (HeuristicValue(filledGrid, N) != 0)
                    {
                        int box = r.Next(N);
                        //Console.WriteLine(box);
                        //if improvement possible perform the swap and add the improvement of the swap to the heuristicValue
                        //remember that the heuristValue shoud get lower, so ChangeInBox() returns an int <=0.
                        if (timesOnPlateau > maxTimeOnPlateau)
                        {
                            performRandomWalk = true;
                            timesOnPlateau = 0;
                            s = 0;
                        }
                        if (s == S)
                        {
                            performRandomWalk = false;
                            s = 0;
                        }
                        if (performRandomWalk == true)
                        {
                            s++;
                            heuristicValue += randomWalk(filledGrid, box, boxSize, N, r);
                            heuristicsData.Add(heuristicValue);
                            //printGrid(filledGrid);
                            // Console.WriteLine(heuristicValue);
                        }
                        else
                        {
                            int heuristicChange = hillClimb(filledGrid, box, boxSize, N);
                            heuristicValue += heuristicChange;
                            heuristicsData.Add(heuristicValue);
                            //printGrid(filledGrid);
                            //Console.WriteLine("heuristic Value:");
                            //Console.WriteLine(heuristicValue);
                            if (heuristicChange < 0)
                            {
                                listPlateauSizes.Add(timesOnPlateau);
                                timesOnPlateau = 0;
                            }
                            else
                            {
                                timesOnPlateau++;
                            }
                        }
                    }
                    //solved!
                    Console.WriteLine(i);
                    printGrid(filledGrid);

                    string csv = String.Join(",", heuristicsData.Select(x => x.ToString()).ToArray());
                    string csv20 = String.Join(",", listPlateauSizes.Select(x => x.ToString()).ToArray());

                }
                steps.Add(heuristicsData.Count);
            }
            string csv3 = String.Join(",", steps.Select(x => x.ToString()).ToArray());

            string csv2 = String.Join(",", listPlateauSizes.Select(x => x.ToString()).ToArray());
            Console.ReadLine();
        }
        struct Pair
        {
            public int Number;
            public bool Fixed;
        };
    }
}

