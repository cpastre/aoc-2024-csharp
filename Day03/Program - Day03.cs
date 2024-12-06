using System.Text;
using System.Text.RegularExpressions;

namespace Day03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string liveInput = File.ReadAllText(@"input.txt");
            string testInput = File.ReadAllText(@"input - sample.txt");
            string testInputPart2 = File.ReadAllText(@"input - sample - part2.txt");

            var loadToMulStrings = (string input) => 
                Regex.Matches(input, @"mul\(([0-9]{1,3}),([0-9]{1,3})\)", RegexOptions.Singleline)
                .Cast<Match>().Select(s => new Mul()
                {
                    First = int.Parse(s.Groups[1].Value),
                    Second = int.Parse(s.Groups[2].Value)
                }).ToArray();
            var a = loadToMulStrings(testInput);
            var ba = loadToMulStrings(liveInput);

            var SumProd = (Mul[] mulArray) =>
                mulArray.Sum(muls => muls.First * muls.Second);

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of safe reports is: {SumProd(loadToMulStrings(liveInput))}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {SumProd(loadToMulStrings(testInput))}.");

            //Part 2
            var surroundDoDont = (string input) => "do()" + input + "don't()";
            string liveInput2 = surroundDoDont(liveInput);
            string testInput2 = surroundDoDont(testInput);

            var LoadToDoDontChunks = (string input) =>
                Regex.Matches(input, @"(do\(\).*?don't\(\))", RegexOptions.Singleline)
                .Cast<Match>().Select(s => s.Value).ToArray();

            var IterateDoDontChunks = (string[] input) =>
                Enumerable.Range(0, input.Length).Select(chunkNo => loadToMulStrings(input[chunkNo])).SelectMany(m => m).ToArray();

            Console.WriteLine("--- PART TWO ---\n");
            Console.WriteLine($"The number of safe reports is: {SumProd(IterateDoDontChunks(LoadToDoDontChunks(surroundDoDont(liveInput))))}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {SumProd(IterateDoDontChunks(LoadToDoDontChunks(surroundDoDont(testInputPart2))))}.");

         }

        record Mul
        {
            public int First;
            public int Second;
        }
    }
}
