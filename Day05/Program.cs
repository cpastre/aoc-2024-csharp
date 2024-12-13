
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

            var OrderLines = (string[] input) => input
                .Where(l => l.Contains("|"))
                .Select(l => new OrderLine { EarlyPage = int.Parse(l.Split("|")[0]), LaterPage = int.Parse(l.Split("|")[1]) });
            var UpdatePages = (string[] input) => input
                .Where(l => l.Contains(","))
                .Select(l => l.Split(",").Select(p => int.Parse(p)));

            ProcessPart1(OrderLines(inputTest), UpdatePages(inputTest));
        }

        private static void ProcessPart1(IEnumerable<OrderLine> orderLines, IEnumerable<IEnumerable<int>> updatePages)
        {
            foreach (var pageSet in updatePages)
            {
                foreach (var page in pageSet)
                {

                }
            }
        }
    }
}
