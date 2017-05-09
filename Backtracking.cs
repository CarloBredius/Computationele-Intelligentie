using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            string line = Console.ReadLine();
            string[] seperated = line.Split(' ');
            int N = seperated.Count(); // een sudoku van N x N
            int[,] puzzle = new int[N, N];
            int number;
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

        public void CheckNoUse(int[,] puzzle, int x, int y, int[] numbers)
        {
            CheckLine(puzzle, x, y, numbers);
            CheckColumn(puzzle, x, y, numbers);
            CheckBlock(puzzle, x, y, numbers);
        }
        public void CheckLine(int[,] puzzle, int x, int y, int[] numbers)
        {

        }
        public void CheckColumn(int[,] puzzle, int x, int y, int[] numbers)
        {

        }
        public void CheckBlock(int[,] puzzle, int x, int y, int[] numbers)
        {

        }
        public bool Empty(int[,] puzzle)
        {
            foreach(int number in puzzle)
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
