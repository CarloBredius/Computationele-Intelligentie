using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTracking
{
    class Program
    {
        static int N;
        static void Main(string[] args)
        {
            string line = Console.ReadLine();
            string[] seperated = line.Split(' ');
            N = seperated.Count(); // een sudoku van N x N
            int[,] puzzle = new int[N, N];
            int number;
            List <int> NotExpandNumbers = new List<int>();

            int maxSteps = N * N; // maximaal aantal stappen is van een N x N puzzel is een diepte van N x N

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
            int[,] root = puzzle;

            NotExpandNumbers = CheckNoUse(puzzle, 1, 0, NotExpandNumbers); // find numbers to not check for
            // apply backtracking

            // Write solution
            for (int x = 0; x < N; x++)
            {
                for (int y = 0; y < N; y++)
                {
                    Console.Write(puzzle[x, y].ToString() + ' ');
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }

        public void BackTrack(int[,] puzzle)
        {
            if (Empty(puzzle)) // check empty
                return;
            else
            {
                Solve(puzzle);
            }
        }
        public void Solve(int[,] puzzle)
        {
            if (full(puzzle)) // check full
                return;
            else
            {

            }
        }

        public static List<int> CheckNoUse(int[,] puzzle, int x, int y, List<int> numbers)
        {
            CheckLine(puzzle, x, numbers);
            CheckColumn(puzzle, y, numbers);
            CheckBlock(puzzle, x, y, numbers);
            return numbers;
        }
        public static void CheckLine(int[,] puzzle, int x, List<int> numbers)
        {
            for (int i = 0; i < N; i++)
            {
                if (puzzle[x, i] != 0)
                {
                    numbers.Add(puzzle[x, i]);
                }
            }
        }
        public static void CheckColumn(int[,] puzzle, int y, List<int> numbers)
        {
            for (int i = 0; i < N; i++)
            {
                if (puzzle[i, y] != 0)
                {
                    numbers.Add(puzzle[i, y]);
                }
            }
        }
        public static void CheckBlock(int[,] puzzle, int x, int y, List<int> numbers)
        {
            int firstThird = ((int)Math.Sqrt(puzzle.Length) - 1) / 3;
            int secondThird = ((int)Math.Sqrt(puzzle.Length) - 1) * 2 / 3;
            int lastThird = puzzle.Length - 1;

            if (0 <= x && x <= firstThird)   // first third of row
            {
                if (0 <= y && y <= firstThird)   // first third of column, block 1
                {
                    for (int i = 0; i <= firstThird; i++)
                    {
                        for (int j = 0; j <= firstThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
                else if (3 <= y && y <= 5)  // second third of collumn, block 2
                {
                    for (int i = firstThird; i <= secondThird; i++)
                    {
                        for (int j = 0; j <= firstThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
                else // block 3
                {
                    for (int i = secondThird; i <= lastThird; i++)
                    {
                        for (int j = 0; j <= firstThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
            }
            else if (3 <= x && x <= 5) // second third of row
            {
                if (0 <= y && y <= 2)   // first third of column, block 4
                {
                    for (int i = 0; i <= firstThird; i++)
                    {
                        for (int j = firstThird; j <= secondThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
                else if (3 <= y && y <= 5)  // second third of collumn, block 5
                {
                    for (int i = firstThird; i <= secondThird; i++)
                    {
                        for (int j = firstThird; j <= secondThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
                else // block 6
                {
                    for (int i = secondThird; i <= lastThird; i++)
                    {
                        for (int j = firstThird; j <= secondThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
            }
            else
            {
                if (0 <= y && y <= 2)   // first third of column, block 7
                {
                    for (int i = 0; i <= firstThird; i++)
                    {
                        for (int j = secondThird; j <= lastThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
                else if (3 <= y && y <= 5)  // second third of collumn, block 8
                {
                    for (int i = firstThird; i <= secondThird; i++)
                    {
                        for (int j = secondThird; j <= lastThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
                else // block 9
                {
                    for (int i = secondThird; i <= lastThird; i++)
                    {
                        for (int j = secondThird; j <= lastThird; j++)
                        {
                            if (puzzle[i, j] != 0)
                            {
                                numbers.Add(puzzle[i, j]);
                            }
                        }
                    }
                }
            }
        }
        public bool Empty(int[,] puzzle)
        {
            foreach (int number in puzzle)
            {
                if (number != 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool full(int[,] puzzle)
        {
            foreach (int number in puzzle)
            {
                if (number == 0)
                    return false;
            }
            return true;
        }
    }
}
