
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace Day07
{
    internal class Program
    {
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
            
        }
    }
}
