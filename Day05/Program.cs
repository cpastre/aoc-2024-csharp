

using static Day05.Program;

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

            Console.WriteLine();
            Console.WriteLine("--- PART TWO ---\n");

            Console.WriteLine("Test Values");
            ProcessPart2(OrderLines(inputTest), UpdatePages(inputTest));
            Console.WriteLine("Live Values");
            ProcessPart2(OrderLines(inputLive), UpdatePages(inputLive));

        }

        private static void ProcessPart1(OrderLine[] orderLines, int[][] pageSets)
        {
            List<int[]> inOrderPageSets = pageSets.Where(ps => CheckInSequence(orderLines, ps)).ToList();
            var MiddlePageNumber = (int[] pages) => pages[(pages.Length - 1) / 2];
            var MiddlePageTotal = inOrderPageSets.Sum(MiddlePageNumber);
            Console.WriteLine($"Total middle pages: {MiddlePageTotal}");
        }

        private static void ProcessPart2(OrderLine[] orderLines, int[][] pageSets)
        {
            List<int[]> outOfOrderPageSets = pageSets.Where(ps => !CheckInSequence(orderLines, ps)).ToList();

            int[] PatchPageInIter(int pageNumber, int[] head, int[] tail)
            {
                if (head.Length == 0)
                {
                    return [.. (new int[] { pageNumber }), .. tail];
                }
                if(PageIsAfter(orderLines, head.Last(), pageNumber))
                {
                    return head.Concat([pageNumber]).Concat(tail).ToArray();
                }
                else
                {
                    return PatchPageInIter(pageNumber, head[0..^1], (new int[] { head.Last() }).Concat(tail).ToArray());
                }
            }
            var PatchPageIn = (int pageNumber, int[] pageSet) => PatchPageInIter(pageNumber, pageSet, new int[] { });

            int[] ReorderPagesIter(int[] pageSet, int[] resultPageSet)
            {
                if (pageSet.Length == 0)
                {
                    return resultPageSet;
                }
                else
                {
                    return ReorderPagesIter(pageSet.TakeLast(pageSet.Length - 1).ToArray(),
                                            PatchPageIn(pageSet[0], resultPageSet));
                }
            }
            var ReorderPages = (int[] pageSet) => ReorderPagesIter(pageSet, []);
            
            int[] result = [];
            foreach(var page in outOfOrderPageSets[2])
            {
                result = PatchPageIn(page, result);
            }

            var MiddlePageNumber = (int[] pages) => pages[(pages.Length - 1) / 2];
            var MiddlePageTotal = (int[][] pageSets) => pageSets.Sum(s => MiddlePageNumber(s));
            Console.WriteLine($"Total middle pages: " +
                $"{MiddlePageTotal(outOfOrderPageSets.Select(s => ReorderPages(s)).ToArray())}");
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
                    var remainingPages = _pageSet.Skip(1);
                    var afterPages = orderLines.Where(p => p.EarlyPage == _pageSet[0]);

                    if (!_pageSet.Skip(1).Where(p => !PageIsAfter(orderLines, _pageSet[0], p)).Any())
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

        private static bool PageIsAfter(OrderLine[] orderLines, int earlyPage, int laterPage) =>
            orderLines.Where(l => l.EarlyPage == earlyPage && l.LaterPage == laterPage).Any();

    }
}
