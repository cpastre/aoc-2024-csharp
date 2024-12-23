
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Day07
{
    internal class Program
    {
        private record EquationSource
        {
            public required int Sum;
            public required int[] Numbers;
        }

        static void Main()
        {
            var loadInputFile = (string filename) => new ReadOnlyCollection<string>(File.ReadAllLines(filename));
            var testInput = loadInputFile("input - Sample.txt");
            var liveInput = loadInputFile("input.txt");

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine("Test Values");
            ProcessPart1(testInput);
            //Console.WriteLine("Live Values");
            //ProcessPart1(liveInput);
        }

        private static void ProcessPart1(ReadOnlyCollection<string> input)
        {
            var loadEquationSource = () => input.Select(e =>
                new EquationSource
                {
                    Sum = int.Parse(e.Substring(0, e.IndexOf(':'))),
                    Numbers = Regex.Matches(e.Substring(e.IndexOf(':') + 1), @"\b(\w+)")
                                .Select(n => int.Parse(n.Value)).ToArray()
                });
            var t1 = loadEquationSource();
        }
    }
}
