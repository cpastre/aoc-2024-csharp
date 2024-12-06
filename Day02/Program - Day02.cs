using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var liveInput = File.ReadAllLines(@"input.txt");
            var testInput = File.ReadAllLines(@"input - sample.txt");

            var LoadToArrays = (string[] input) => input.Select(i =>
                Regex.Matches(i, @"\b(\w+)").Cast<Match>().Select(s => int.Parse(s.Value)).ToArray());
            var OrderlyArrays = (IEnumerable<int[]> loadedArray) => loadedArray.Where(iList =>
                AscendingYes(iList) || DescendingYes(iList));
            var SafeArrays = (IEnumerable<int[]> orderlyArrays) => orderlyArrays
                .Where(a => SafeYes(a));
            var SafeArraysDamper = (IEnumerable<int[]> orderlyArrays) => orderlyArrays
                .Where(a => SafeYesDamper(a, 1));

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of safe reports is: {SafeArrays(LoadToArrays(liveInput)).Count()}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {SafeArrays(LoadToArrays(testInput)).Count()}.");

            Console.WriteLine("--- PART TWO ---\n");
            Console.WriteLine($"The number of safe reports is: {SafeArraysDamper(LoadToArrays(liveInput)).Count()}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {SafeArraysDamper(LoadToArrays(testInput)).Count()}.");

            var a = SafeArraysDamper(LoadToArrays(liveInput));
            var b = SafeArrays(OrderlyArrays(LoadToArrays(liveInput)));
            var ArrayMatch = (int[] a, int[] b) =>
                (a.Length == b.Length)
                && Enumerable.Range(0, a.Length - 1).All(i => a[i] == b[i]);
            //var testA = ArrayMatch([1, 2, 3, 4], [1, 2, 3, 4]);
            //var testB = ArrayMatch([1, 2, 3], [1, 2, 3, 4]);
            //var testC = ArrayMatch([0, 2, 3], [1, 2, 3]);
            var Except = (IEnumerable<int[]> a, IEnumerable<int[]> b) =>
                a.Where(amember => !b.Any(bmember => ArrayMatch(amember, bmember)));
            var c = Except(a, b);
            //c.ToList().ForEach(array => Console.WriteLine(string.Join(", ", array)));
            var d = SafeYesDamper([1, 3, 4, 7, 10, 13, 16, 20], 1);
            var d2 = SafeYesDamper([1, 3, 4, 7, 10, 13, 21, 15], 1);
            var d3 = SafeYesDamper([61, 62, 65, 67, 70], 1);
            var d4 = SafeYesDamper([1, 2, 3, 4, 5], 1);
            var d5 = SafeYesDamper([1, 2, 3], 1);
            var e = LoadToArrays(liveInput).Select(a => new ArrayScore { 
                Array = a, IsSafePart1 = SafeYes(a), IsSafePart2 = SafeYesDamper(a, 1),
                DescendingYes = DescendingYes(a), AscendingYes = AscendingYes(a) });
            var safep1 = e.Count(a => a.IsSafePart1);
            var safep2 = e.Count(a => a.IsSafePart2);
            e.Where(a => a.IsSafePart1 && !a.IsSafePart2).ToList().ForEach(a => Console.WriteLine(string.Join(", ", a.Array)));
        }

        record ArrayScore
        {
            public int[] Array;
            public bool IsSafePart1;
            public bool IsSafePart2;
            public bool AscendingYes;
            public bool DescendingYes;
        }

        static bool AscendingYes(int[] ints)
        {
            if(ints.Length <= 1)
            {
                //got here, so must be ascending
                return true;
            }
            else
            {
                if (ints[0] < ints[1])
                {
                    return AscendingYes(ints.TakeLast(ints.Length - 1).ToArray());
                }
                else
                {
                    return false;
                }
            }
        }

        static bool DescendingYes(int[] ints)
        {
            if (ints.Length <= 1)
            {
                //got here, so must be descending
                return true;
            }
            else
            {
                if (ints[0] > ints[1])
                {
                    return DescendingYes(ints.TakeLast(ints.Length - 1).ToArray());
                }
                else
                {
                    return false;
                }
            }
        }

        static IEnumerable<int[]> OrderlyArrays(IEnumerable<int[]> loadedArray)
        {
            return loadedArray.Where(iList =>
                AscendingYes(iList) || DescendingYes(iList));
        }

        static bool SafeYes(int[] ints)
        {
            return SafeYesTolerance(ints, 3);
        }

        static bool SafeYesTolerance(int[] ints, int maxLevel, int damper = 0, int errors = 0)
        {
            if (!AscendingYes(ints) && !DescendingYes(ints))
            { 
                return false;
            }
            else
            {
                if (ints.Length <= 1)
                {
                    return true;
                }
                else
                {
                    var Overload = (int[] ints) => Math.Abs(ints[0] - ints[1]) > maxLevel;
                    if (!Overload(ints))
                    {
                        return SafeYesTolerance(
                            ints.TakeLast(ints.Length - 1).ToArray(),
                            maxLevel,
                            damper,
                            errors);
                    }
                    else
                    {
                        if (errors < damper)
                        {
                            return SafeYesTolerance(
                            ints.TakeLast(ints.Length - 1).ToArray(),
                            maxLevel,
                            damper,
                            errors++);
                        }
                        else
                        {
                            return false;
                        }
                    }    
                }
            }
        }

        static bool SafeYesDamper(int[] ints, int damper = 0)
        {
            var Permutations = DampedPermutations(ints, damper);

            return Permutations.Where(a => SafeYes(a)).Any();
        }

        static IEnumerable<int[]> DampedPermutations(int[] ints, int damper = 0)
        {
            var Permutations = Enumerable.Range(0, ints.Length + 1 - damper).Select(i =>
                ints.Take(i).Concat(ints.TakeLast(ints.Length - i - damper)).ToArray());
            
            return Permutations.Append(ints);
        }
    }
}
