
using System.Buffers;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Day07
{
    public class Program
    {
        private record EquationSource
        {
            public required long Sum;
            public required long[] Numbers;
        }

        public record Operation
        {
            public required string Name;
            public required Func<long, long, long> Op;
            public required int Identity;
        }

        public static Operation plusOp = new() { Name = "+", Op = static (long in1, long in2) => in1 += in2, Identity = 0 };
        public static Operation multOp = new() { Name = "*", Op = static (long in1, long in2) => in1 *= in2, Identity = 1 };
        public static Operation squishOp = new() { Name = "||", Op = static (long in1, long in2) => long.Parse(in1.ToString() + in2.ToString()), Identity = 0 };


        public static void Main()
        {
            var loadInputFile = (string filename) => new ReadOnlyCollection<string>(File.ReadAllLines(filename));
            var testInput = loadInputFile("input - Sample.txt");
            var liveInput = loadInputFile("input.txt");
            DateTime startTime;

            startTime = DateTime.Now;
            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine("Test Values");
            ProcessPart1(testInput);
            Console.WriteLine("Live Values");
            ProcessPart1(liveInput);
            Console.WriteLine($"Process Time: {(DateTime.Now - startTime).TotalMilliseconds}");

            startTime = DateTime.Now;
            Console.WriteLine("--- PART TWO ---\n");
            Console.WriteLine("Test Values");
            ProcessPart2(testInput);
            Console.WriteLine("Live Values");
            ProcessPart2(liveInput);
            Console.WriteLine($"Process Time: {(DateTime.Now - startTime).TotalMilliseconds}");

            startTime = DateTime.Now;
            Console.WriteLine(@"--- PART ""THREE"" ---\n");
            Console.WriteLine("Test Values");
            ProcessPart3(testInput);
            Console.WriteLine("Live Values");
            ProcessPart3(liveInput);
            Console.WriteLine($"Process Time: {(DateTime.Now - startTime).TotalMilliseconds}");
        }

        private static void ProcessPart1(ReadOnlyCollection<string> input)
        {
            var loadEquationSource = () => input.Select(e =>
                new EquationSource
                {
                    Sum = long.Parse(e.Substring(0, e.IndexOf(':'))),
                    Numbers = Regex.Matches(e.Substring(e.IndexOf(':') + 1), @"\b(\w+)")
                                .Select(n => long.Parse(n.Value)).ToArray()
                });
            //var t1 = loadEquationSource();
            //var t2 = GeneratePermutations([plusOp, multOp], t1.First().Numbers.Count() - 1);
            //var t3 = PerformOperationPermutations(t2, t1.First().Numbers);
            var identifyMatching = () => loadEquationSource()
                .SelectMany(es => PerformOperationPermutations(GeneratePermutations([plusOp, multOp], es.Numbers.Length - 1), es.Numbers)
                                  .Distinct().Where(r => r == es.Sum));
            Console.WriteLine($"Matching total: {identifyMatching().Sum()}");

        }

        private static void ProcessPart2(ReadOnlyCollection<string> input)
        {
            var loadEquationSource = () => input.AsParallel().Select(e =>
                new EquationSource
                {
                    Sum = long.Parse(e.Substring(0, e.IndexOf(':'))),
                    Numbers = Regex.Matches(e.Substring(e.IndexOf(':') + 1), @"\b(\w+)")
                                .Select(n => long.Parse(n.Value)).ToArray()
                });
            //var t1 = loadEquationSource();
            //var t2 = GeneratePermutations([plusOp, multOp], t1.First().Numbers.Count() - 1);
            //var t3 = PerformOperationPermutations(t2, t1.First().Numbers);
            var identifyMatching = () => loadEquationSource().AsParallel()
                .SelectMany(es => PerformOperationPermutations(GeneratePermutations([plusOp, multOp, squishOp], es.Numbers.Length - 1), es.Numbers)
                                  .Distinct().Where(r => r == es.Sum));
            Console.WriteLine($"Matching total: {identifyMatching().Sum()}");

        }

        //bonus "part 3" solution - processes all space permutations and then runs through rather than simply apply || as a new operation to be performed in sequence
        private static void ProcessPart3(ReadOnlyCollection<string> input)
        {
            var intToMask = (int i) => Convert.ToString(i, 2);
            var permMasks = (int spaceCount) => Enumerable.Range(0, (int)Math.Pow(2, spaceCount))
                                                          .Select(i => intToMask(i).PadLeft(spaceCount, '0').Replace('0','@').Replace('1','#'));
            var preprocessEqSource = () => input.AsParallel().Select(e =>
            {
                string sumChunk() => e.Substring(0, e.IndexOf(':') + 2);
                string numsChunk() => e.Substring(e.IndexOf(':') + 2);
                string[] numStrings() => Regex.Matches(e.Substring(e.IndexOf(':') + 1), @"\b(\w+)")
                                .Select(n => n.Value).ToArray();

                return permMasks(numsChunk().Count(c => c == ' '))
                    .Select(m => sumChunk() + m.Zip(numStrings()[0..^1])
                    .Select(s => s.Second + s.First)
                    .Aggregate((s1, s2) => s1 + s2)
                    .Replace("@",@"").Replace('#',' ') + numStrings().Last());
            }).SelectMany(s => s).ToArray();

            //var t = preprocessEqSource();
            var loadEquationSource = (string[] input) => input.Select(e =>
               new EquationSource
               {
                   Sum = long.Parse(e.Substring(0, e.IndexOf(':'))),
                   Numbers = Regex.Matches(e.Substring(e.IndexOf(':') + 1), @"\b(\w+)")
                               .Select(n => long.Parse(n.Value)).ToArray()
               });
            //var t1 = loadEquationSource(preprocessEqSource());

            var identifyMatching = () => loadEquationSource(preprocessEqSource()).AsParallel()
                    .SelectMany(es => PerformOperationPermutations(GeneratePermutations([plusOp, multOp], es.Numbers.Length - 1), es.Numbers)
                    .Distinct().Where(r => r == es.Sum));
            //var t2 = loadEquationSource(preprocessEqSource())
            //        .SelectMany(es => PerformOperationPermutations(GeneratePermutations([plusOp, multOp], es.Numbers.Length - 1), es.Numbers)
            //            .Select(s => (es.Sum, es.Numbers, s)));

            //foreach (var _t in t2)
            //{
            //    Console.WriteLine($"{_t.Sum}: {String.Join(' ', _t.Numbers)} = {_t.s}\t\t{_t.Sum == _t.s}");
            //}

            Console.WriteLine($"Matching total: {identifyMatching().Sum()}");

        }

        public static IEnumerable<Operation[]> GeneratePermutations(Operation[] ops, int count)
        {
            IEnumerable<Operation[]> GenPermIter(IEnumerable<Operation[]> perms, int iterCount)
            {
                if (iterCount == 0)
                {
                    return [];
                }
                else if (iterCount == 1)
                {
                    return perms;
                }
                else
                {
                    var iterOnOps = (Operation[] _ops, Operation[] _perm) =>
                        _ops.Select<Operation, Operation[]>(o => [.. _perm, o]);
                    var iterOnPerms = () => perms.SelectMany(p => iterOnOps(ops, p));

                    return GenPermIter(iterOnPerms(), --iterCount);
                }
            }
            return GenPermIter(ops.Select<Operation, Operation[]>(o => [o]), count);
        }

        public static long[] PerformOperationPermutations(IEnumerable<Operation[]> perms, long[] Numbers)
        {
            long PerformOpIter(Operation[] ops, long[] nums, long cumResult)
            {
                if (ops.Length == 0)
                {
                    return cumResult;
                }
                else
                {
                    return PerformOpIter(ops[1..], nums[1..], ops[0].Op(cumResult, nums[0]));
                }
            }

            if (perms.Count() == 0)
            {
                return Numbers;
            }
            else
            {
                return perms.Select(p => PerformOpIter([plusOp, .. p], Numbers, 0)).ToArray();
            }
        }
    }
}
