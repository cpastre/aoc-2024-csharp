using System.Text.RegularExpressions;

namespace Day03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string liveInput = File.ReadAllText(@"input.txt");

            string[] liveInput = File.ReadAllLines(@"input.txt");
            string[] testInput = File.ReadAllLines(@"input - sample.txt");

            var loadToMulStrings = (string[] input) => input.Select(i =>
                Regex.Matches(i, @"mul\(([0-9]{1,3}),([0-9]{1,3})\)")
                .Cast<Match>().Select(s => new Mul()
                {
                    First = int.Parse(s.Groups[1].Value),
                    Second = int.Parse(s.Groups[2].Value)
                }).ToArray());
            var a = loadToMulStrings(testInput);
            var ba = loadToMulStrings(liveInput);

            var SumProd = (IEnumerable<Mul[]> mulArrays) =>
                mulArrays.Sum(muls => muls.Sum(m => m.First * m.Second));

            var testsum = SumProd(loadToMulStrings(testInput));
            var livesum = SumProd(loadToMulStrings(liveInput));

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of safe reports is: {SumProd(loadToMulStrings(liveInput))}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {SumProd(loadToMulStrings(testInput))}.");



        }

        record Mul
        {
            public int First;
            public int Second;
        }
    }
}
