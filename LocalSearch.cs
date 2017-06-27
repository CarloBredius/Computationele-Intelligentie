using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        //function to print a sudoku
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
        //function to read a sudoku from the console
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
        //function to calculate the heuristic value of a sudoku. The more errors the higher.
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
        //function to fill a sudoku with only consistent boxes.
        static Pair[,] fillGrid(int[,] grid)
        {
            int N = (int)Math.Sqrt(grid.Length);
            Pair[,] filledGrid = new Pair[N, N];
            int boxSize = (int)Math.Sqrt(Math.Sqrt(grid.Length));
            //double array with lists for every box create a tabu list with numbers that are allready taken
            List<int>[,] taboe = new List<int>[boxSize, boxSize];
            //loop once to check which numbers are allowed and put them in tabu
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
            //loop again to fill the sudoku
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
        //perform a random swap in a given box and return the change in heuristic value 
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
        /*function that perfoms the best heuristic swap in the given box 
        and returns the change in heuristic value. If the best swap highers 
        the heuristic value there will be no swap*/
        static int hillClimb(Pair[,] grid, int box, int boxSize, int N, Random r)
        {
            int boxRow = box / boxSize;
            int boxCol = box % boxSize;
            //store the best swap and its improvement. 
            //(bestSwap contains the row and col of both positions, bestImprovement is the heuristic improvement of the swap (<=0)) 
            List<int[]> bestSwap = new List<int[]>();
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
                                if (heuristicImprovement < bestImprovement)
                                {
                                    bestSwap.Clear();
                                }
                                bestImprovement = heuristicImprovement;
                                bestSwap.Add(new int[] { row1, col1, row2, col2 });

                            }
                        }
                    }
                }
            }
            //we have checked every possible swap in the box. Now we can perform the best swap, 
            //and change the heuristic value
            if (bestSwap.Count > 0)
            {
                int swap = r.Next(bestSwap.Count);
                int rowOne = bestSwap[swap][0];
                int colOne = bestSwap[swap][1];
                int rowTwo = bestSwap[swap][2];
                int colTwo = bestSwap[swap][3];

                int two = grid[rowTwo, colTwo].Number;
                grid[rowTwo, colTwo].Number = grid[rowOne, colOne].Number;
                grid[rowOne, colOne].Number = two;
                return bestImprovement;
            }
            return 0;
        }
        //returns the heuristic change of a given swap between two cells in the sudoku
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

        static List<int> BoxOrder(int N)
        {
            List<int> boxOrderList = Enumerable.Range(0, N).ToList();
            List<int> randomizedList = new List<int>();
            Random r = new Random();
            while (boxOrderList.Count > 0)
            {
                int randomIndex = r.Next(0, boxOrderList.Count);
                randomizedList.Add(boxOrderList[randomIndex]);
                boxOrderList.RemoveAt(randomIndex);
            }
            return randomizedList; //return the new random list
        }

        static void Main(string[] args)
        {
            //research data
            List<int> heuristicsData = new List<int>();
            string[] csv3 = new string[10];
            List<int> listPlateauSizes = new List<int>();

            //timing variables
            Stopwatch stopWatch = new Stopwatch();
            List<long> TimerTotal = new List<long>();
            List<long> TimerS = new List<long>();
            long elapsedTime;

            // tracing Swaps
            List<long> SwapListTotal = new List<long>();
            List<long> SwapListS = new List<long>();
            long Swaps;

            //starting values
            bool performRandomWalk = false;
            int Smin = 1;
            int Smax = 7;
            int maxTime = 90000; // 1.5 minute
            bool tooLong = false; // bool om te checken of de tijdslimiet is vertreken

            // array to keep track of average swaps and average time per s
            long[,] DataPerS = new long[2, Smax];

            //get the sudoku from the console
            int[,] grid = readGrid();
            int N = (int)Math.Sqrt(grid.Length);
            Pair[,] filledGrid = fillGrid(grid);
            printGrid(filledGrid);

            // maak een lijst van lengte sqrt(N) met random volgorde hoe we door de blokken gaan.
            int boxSize = (int)Math.Sqrt(Math.Sqrt(grid.Length));
            //create a list of unswappable boxes. 
            //(boxes with zero or one free numbers can't have swaps. 
            //They are blacklisted here)
            List<int> forbiddenBoxes = new List<int>();
            for (int boxRow = 0; boxRow < boxSize; boxRow++)
                for (int boxCol = 0; boxCol < boxSize; boxCol++)
                {
                    int nFixed = 0;
                    for (int row = boxRow * boxSize; row < (boxRow * boxSize) + boxSize; row++)
                        for (int col = boxCol * boxSize; col < (boxCol * boxSize) + boxSize; col++)
                        {
                            if (filledGrid[row, col].Fixed == true)
                            {
                                nFixed++;
                            }
                        }
                    if (nFixed > N - 2) { forbiddenBoxes.Add(boxRow * boxSize + boxCol); }
                }

            Random r = new Random();

            //loop through S values
            for (int S = Smin; S <= Smax; S++)
            {
                //every S value has different range of relevant plateauTimes (i):
                List<int> steps = new List<int>();
                int iMax = 9;
                int iMin = 0;
                switch (S)
                {
                    case 1:
                        iMin = 8;
                        iMax = 13;
                        break;
                    case 2:
                        iMin = 8;
                        iMax = 13;
                        break;
                    case 3:
                        iMin = 16;
                        iMax = 21;
                        break;
                    case 4:
                        iMin = 18;
                        iMax = 23;
                        break;
                    case 5:
                        iMin = 19;
                        iMax = 24;
                        break;
                    case 6:
                        iMin = 23;
                        iMax = 28;
                        break;
                    case 7:
                        iMin = 31;
                        iMax = 36;
                        break;
                    default:
                        Console.WriteLine("S not in range of 1 - 7");
                        break;
                }

                //loop through the plateauTimes:
                for (int i = iMin; i < iMax; i++)
                {
                    // what S and what maxPlateau the algorithm is on
                    Console.WriteLine("S = " + S + ", maxPlateau = " + i);

                    //research value:
                    heuristicsData.Clear();

                    int maxTimeOnPlateau = i;
                    //solve 10 times for smoother results:
                    for (int j = 0; j < 30; j++)
                    {
                        //initalize time and amount of swaps
                        Swaps = 0;
                        elapsedTime = 0;

                        stopWatch.Reset();
                        stopWatch.Start();

                        //starting values:
                        int timesOnPlateau = 0;
                        int s = 0;

                        //reset the sudoku to beginning state:
                        filledGrid = fillGrid(grid);

                        //calculate the heuristicValue at the start, 
                        //from now on only heuristic changes have to be found to update this value.
                        int heuristicValue = HeuristicValue(filledGrid, N);
                        heuristicsData.Add(heuristicValue);
                        //finally solving a sudoku:
                        while (HeuristicValue(filledGrid, N) != 0)
                        {
                            // check time limit
                            if (stopWatch.ElapsedMilliseconds > maxTime)
                            {
                                tooLong = true;
                                break;
                            }
                            // increment swaps
                            Swaps++;

                            //choose a random box order:
                            List<int> randomOrder = BoxOrder(N);
                            foreach (var box in randomOrder)
                            {

                                if (forbiddenBoxes.Contains(box)) { continue; };
                                //now determine if there will be a hillclimb or random walk:

                                //start random walking when:
                                if (timesOnPlateau > maxTimeOnPlateau)
                                {
                                    performRandomWalk = true;
                                    timesOnPlateau = 0;
                                    s = 0;
                                }
                                //stop random walking when:
                                if (s == S)
                                {
                                    performRandomWalk = false;
                                    s = 0;
                                }
                                //random walk:
                                if (performRandomWalk == true)
                                {
                                    s++;
                                    heuristicValue += randomWalk(filledGrid, box, boxSize, N, r);
                                    heuristicsData.Add(heuristicValue);
                                    //printGrid(filledGrid);
                                    // Console.WriteLine(heuristicValue);
                                }
                                //hill climb:
                                else
                                {
                                    //if improvement possible perform the swap and add the improvement of the swap to the heuristicValue
                                    //remember that the heuristValue shoud get lower or stays equal, so ChangeInBox() returns an int <=0.
                                    int heuristicChange = hillClimb(filledGrid, box, boxSize, N, r);
                                    heuristicValue += heuristicChange;
                                    heuristicsData.Add(heuristicValue);
                                    //if improvement, the timesOnPlateau value is reset. 
                                    if (heuristicChange < 0)
                                    {
                                        timesOnPlateau = 0;

                                        //research value update:
                                        listPlateauSizes.Add(timesOnPlateau);
                                        break;
                                    }
                                    else
                                    {
                                        //research value update:
                                        timesOnPlateau++;
                                    }
                                }
                            }
                        }
                        //solved, or time limit
                        stopWatch.Stop();
                        elapsedTime = stopWatch.ElapsedMilliseconds;
                        TimerS.Add(elapsedTime);
                        TimerTotal.Add(elapsedTime);

                        SwapListS.Add(Swaps);
                        SwapListTotal.Add(Swaps);
                        //printGrid(filledGrid);
                        if (tooLong)
                        {
                            Console.WriteLine(j + ": took too long");
                        }
                        else
                        {
                            Console.WriteLine(j + ": time: " + elapsedTime + "\t swaps: " + Swaps);
                        }
                    }
                    Console.WriteLine();
                    steps.Add(heuristicsData.Count);
                }
                //calculate average swaps in S
                long SwapsInS = 0;
                foreach (long stepsS in SwapListS)
                {
                    SwapsInS += stepsS;
                }
                long averageSwaps = (SwapsInS / SwapListS.Count);

                //calculate average time in S
                long timeInS = 0;
                foreach (long timeS in TimerS)
                {
                    timeInS += timeS;
                }
                long averageTimeInS = (timeInS / TimerS.Count);
                Console.WriteLine();
                Console.WriteLine("Average swaps in S = " + S + " is " + averageSwaps + " steps");
                Console.WriteLine("Average time in S = " + S + " is " + averageTimeInS + " ms");
                Console.WriteLine();

                // store average data of this S;
                DataPerS[0, S - 1] = averageSwaps;
                DataPerS[1, S - 1] = averageTimeInS;

                //clear lists for the next S
                TimerS.Clear();
                SwapListS.Clear();

                //research update:
                csv3[S] = String.Join(",", steps.Select(x => x.ToString()).ToArray());
            }
            //calculate total swaps
            long totalSwaps = 0;
            foreach (long Swap in SwapListTotal)
            {
                totalSwaps += Swap;
            }

            //calculate total time
            long totalTime = 0;
            foreach (long time in TimerTotal)
            {
                totalTime += time;
            }
            Console.WriteLine("Averages per S: ");
            for (int i = 0; i < DataPerS.Length / 2; i++)
            {
                Console.WriteLine("S = " + (i + 1) + ", Steps = " + DataPerS[0, i] + ", time = " + DataPerS[1, i]);
            }
            Console.WriteLine();
            Console.WriteLine("Averages total:");
            Console.WriteLine("time: " + (totalTime / TimerTotal.Count));
            Console.WriteLine("steps: " + (totalSwaps / SwapListTotal.Count));

            Console.ReadLine();
        }
        struct Pair
        {
            public int Number;
            public bool Fixed;
        };
    }
}

