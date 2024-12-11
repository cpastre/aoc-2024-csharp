namespace Day05
{
    internal class Program
    {
        public record OrderLine
        {
            public required int EarlyPage;
            public required int LaterPage;
        }

        static void Main(string[] args)
        {
            string[] inputLive = File.ReadAllLines(@"input.txt");
            string[] inputTest = File.ReadAllLines(@"input - sample.txt");

            var orderLines = inputLive
                .Where(l => l.Contains("|"))
                .Select(l => new OrderLine { EarlyPage = int.Parse(l.Split("|")[0]), LaterPage = int.Parse(l.Split("|")[1]) });
            var updatePages = inputLive
                .Where(l => l.Contains(","))
                .Select(l => l.Split(",").Select(p => int.Parse(p)));
        }
    }
}
