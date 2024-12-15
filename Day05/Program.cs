
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
                .Select(l => new OrderLine { EarlyPage = int.Parse(l.Split("|")[0]), LaterPage = int.Parse(l.Split("|")[1]) })
                .ToArray();
            var UpdatePages = (string[] input) => input
                .Where(l => l.Contains(","))
                .Select(l => l.Split(",").Select(p => int.Parse(p)).ToArray())
                .ToArray();

            Console.WriteLine("--- PART ONE ---\n");

            Console.WriteLine("Test Values");
            ProcessPart1(OrderLines(inputTest), UpdatePages(inputTest));
            Console.WriteLine("Live Values");
            ProcessPart1(OrderLines(inputLive), UpdatePages(inputLive));
        }

        private static void ProcessPart1(OrderLine[] orderLines, int[][] updatePages)
        {
            List<int[]> lines = [];
            foreach (var pageSet in updatePages)
            {
                if (CheckInSequence(orderLines, pageSet))
                {
                    lines.Add(pageSet);
                }
                Console.WriteLine($" - {string.Join(", ", pageSet)}: {CheckInSequence(orderLines, pageSet)}");
            }
            var MiddlePageNumber = (int[] pages) => pages[(pages.Length - 1) / 2];
            var MiddlePageTotal = lines.Sum(MiddlePageNumber);
            Console.WriteLine($"Total middle pages: {MiddlePageTotal}");
        }

        private static bool CheckInSequence(OrderLine[] orderLines, int[] pages)
        {
            bool CheckInSequenceIter(int[] _pageSet)
            {
                if (_pageSet.Length == 0)
                {
                    return true;
                }
                else
                {
                    var PageIsAfter = (int earlyPage, int laterPage) =>
                        orderLines.Where(l => l.EarlyPage == earlyPage && l.LaterPage == laterPage).Any();
                    var remainingPages = _pageSet.Skip(1);
                    var afterPages = orderLines.Where(p => p.EarlyPage == _pageSet[0]);

                    if (!_pageSet.Skip(1).Where(p => !PageIsAfter(_pageSet[0], p)).Any())
                    {
                        return CheckInSequenceIter(_pageSet.Skip(1).ToArray());
                    }
                    else
                    {
                        return false;
                    }
                }
            };
            return CheckInSequenceIter(pages);
        }
    }
}
