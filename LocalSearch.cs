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
        static Pair[,] ChangeInBox(Pair[,] grid, int box, int boxSize, int currentHeuristicValue, int N)
        {
            // list with heuristic value, and x, y of number 1 to be swapped with x, y of number 2
            List<Tuple<int, Tuple<int, int>, Tuple<int, int>>> heuristicValues = new List<Tuple<int, Tuple<int, int>, Tuple<int, int>>>();

            int boxRow = ((box - 1) / boxSize) * boxSize ;
            int boxCol = ((box - 1) % boxSize ) * boxSize;
            Pair[,] copyGrid = new Pair[N, N];
            List<int> possibilies = new List<int>();

            // iterate over each number in this specific box
            for (int row = 0; row < boxSize; row++)
            {
                for (int col = 0; col < boxSize; col++)
                {
                    // check if its a number that can be swapped or not
                    if (grid[boxRow + row, boxCol + col].Fixed)
                        continue;
                    else
                    {
                        possibilies.Add(grid[boxRow + row, boxCol + col].Number);
                        ////try every swap with other non fixed numbers
                        //int temp = grid[boxRow + row, boxCol + col].Number;

                        //for (int rowLeftovers = 0; rowLeftovers < boxSize; rowLeftovers++)
                        //    for (int colLeftovers = 0; colLeftovers < boxSize; colLeftovers++)
                        //    {
                        //        // check if other is fixed and if it is not the first (no swap with itself)
                        //        if (!grid[boxRow + row + rowLeftovers, boxCol + col + colLeftovers].Fixed &&
                        //            grid[boxRow + row + rowLeftovers, boxCol + col + colLeftovers].Number != temp)
                        //        {
                        //            // make a copy of the grid
                        //            Array.Copy(grid, 0, copyGrid, 0, grid.Length);
                        //            copyGrid[boxRow + row, boxCol + col].Number = copyGrid[boxRow + row + rowLeftovers, boxCol + col + colLeftovers].Number;
                        //            copyGrid[boxRow + row + rowLeftovers, boxCol + col + colLeftovers].Number = temp; // swap with temp
                        //            // add heuristic value to later check the best, change 4 to dynamic
                        //            heuristicValues.Add(new Tuple<int, int, int>(HeuristicValue(copyGrid, 4), copyGrid[boxRow + row + rowLeftovers, boxCol + col + colLeftovers].Number, copyGrid[boxRow + row, boxCol + col].Number));
                        //            //printGrid(copyGrid);
                        //        }
                        //        else
                        //            continue;
                        //    }
                    }
                }
            }

            // make list with every pair
            List<Tuple<int, int>> combinations = (from item in possibilies
                                from item2 in possibilies
                                where item < item2
                                select new Tuple<int, int>(item, item2)).ToList();

            foreach (Tuple<int, int> pair in combinations) // every combination
            {
                // make copy
                Array.Copy(grid, 0, copyGrid, 0, grid.Length);
                int item1X = 0; // never used as Item1 will always be found before Item2 (Item1 < Item2)
                int item1Y = 0;
                int item2X = 0;
                int item2Y = 0;
                for (int row = 0; row < boxSize; row++)
                {
                    for (int col = 0; col < boxSize; col++)
                    {
                        if(copyGrid[row, col].Number == pair.Item1)
                        {
                            item1X = row;
                            item1Y = col;
                        }
                        if (copyGrid[row, col].Number == pair.Item2)
                        { // swap
                            item2X = row;
                            item2Y = col;
                            int temp = copyGrid[item1X, item1Y].Number;
                            copyGrid[item1X, item1Y].Number = copyGrid[row, col].Number;
                            copyGrid[row, col].Number = temp;
                        }
                    }
                }
                heuristicValues.Add(new Tuple<int, Tuple<int, int>, Tuple<int, int>>(HeuristicValue(copyGrid, 4), new Tuple<int, int>(item1X, item1Y), new Tuple<int, int>(item2X, item2Y))); // change 4 into dynamic number
            }

            if (heuristicValues.Min(c => c.Item1) < currentHeuristicValue)
            {
                int swapX = 0;
                int swapY = 0;

                for (int row = 0; row < boxSize; row++)
                {
                    for (int col = 0; col < boxSize; col++)
                    {
                        if (heuristicValues.Min(c => c.Item2.Item1) == row &&
                            heuristicValues.Min(c => c.Item2.Item2) == col)
                        {
                            swapX = row;
                            swapY = col;
                        }
                        if (heuristicValues.Min(c => c.Item3.Item1) == row &&
                            heuristicValues.Min(c => c.Item3.Item2) == col)
                        {
                            int temp = copyGrid[swapX, swapY].Number;
                            copyGrid[swapX, swapY].Number = copyGrid[row, col].Number;
                            copyGrid[row, col].Number = temp;
                        }
                    }
                }
                return copyGrid;
            }
            else // no swaps in this box will get the algorithm closer to its solution
            {
                return grid;
            }
        }

        private static bool MatchesFileMask(int item1)
        {
            throw new NotImplementedException();
        }

        static void swap (Pair[,] grid, int i, int j)
        {
            int temp = i;
            i = j;
            j = temp;
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
                int currentHeuristicValue = HeuristicValue(filledGrid, 4);
                Console.WriteLine(currentHeuristicValue); // 4 aanpassen naar dynamisch getal
                randomOrder = BoxOrder(N); // nieuwe random volgorde in welke box er geswapped word
                foreach (var box in randomOrder)
                {
                    ChangeInBox(filledGrid, box, boxSize, currentHeuristicValue, N); // change 1 back to box for dynamic
                }
            }

            Console.ReadLine();
        }
        struct Pair
        {
            public int Number;
            public bool Fixed;
        };
    }
}

