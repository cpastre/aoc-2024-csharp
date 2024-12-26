using System.Collections.ObjectModel;

namespace Day08
{
    internal class Program
    {
        public record Location
        {
            public required int i;
            public required int j;
        }
        public record Node
        {
            public required char Frequency;
            public required Location Location;
        }

        static void Main(string[] args)
        {
            var loadInputFile = (string filename) => new ReadOnlyCollection<string>(File.ReadAllLines(filename));
            var testInput = loadInputFile("input - Sample.txt");
            var liveInput = loadInputFile("input.txt");

            DateTime startTime;

            startTime = DateTime.Now;
            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine("Test Values");
            ProcessPart1(testInput);
            //Console.WriteLine("Live Values");
            //ProcessPart1(liveInput);
            Console.WriteLine($"Process Time: {(DateTime.Now - startTime).TotalMilliseconds}");

        }

        private static void ProcessPart1(ReadOnlyCollection<string> input)
        {
            static char[][] initialMap(IEnumerable<string> input) => input.Select(r => r.ToCharArray()).ToArray();

            static Node[] nodes(char[][] map) =>
                Enumerable.Range(0, map.Length)
                .SelectMany(i =>
                    Enumerable.Range(0, map[i].Length)
                    .Select(j => new Node
                    {
                        Frequency = map[i][j],
                        Location = new Location { i = i, j = j }
                    }))
                .Where(n => n.Frequency != '.')
                .ToArray();

            var t1 = nodes(initialMap(input)); 
        }
    }
}
