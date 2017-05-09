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
    }
}
