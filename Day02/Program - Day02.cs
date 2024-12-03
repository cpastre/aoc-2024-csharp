using System.Text.RegularExpressions;

namespace Day02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var liveInput = File.ReadAllLines(@"input.txt");
            var testInput = File.ReadAllLines(@"input - sample.txt");

            var loadToArrays = (string[] input) => input.Select(i =>
                Regex.Matches(i, @"\b(\w+)").Cast<Match>().Select(s => int.Parse(s.Value)).ToArray());
            var orderlyArrays = (IEnumerable<int[]> loadedArray) => loadedArray.Where(iList =>
                AscendingYes(iList) || DescendingYes(iList));
            var safeArrays = (IEnumerable<int[]> orderlyArrays) => orderlyArrays
                .Where(a => SafeYes(a));
            var safeArraysDamper = (IEnumerable<int[]> orderlyArrays) => orderlyArrays
                .Where(a => SafeYesDamper(a, 1));

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of safe reports is: {safeArrays(orderlyArrays(loadToArrays(liveInput))).Count()}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {safeArrays(orderlyArrays(loadToArrays(testInput))).Count()}.");

            Console.WriteLine("--- PART TWO ---\n");
            Console.WriteLine($"The number of safe reports is: {safeArraysDamper(orderlyArrays(loadToArrays(liveInput))).Count()}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {safeArraysDamper(orderlyArrays(loadToArrays(testInput))).Count()}.");

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

        static bool SafeYes(int[] ints)
        {
            return SafeYesWithErrors(ints, 3);
        }

        static bool SafeYesWithErrors(int[] ints, int maxLevel, int damper = 0, int errors = 0)
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
                    return SafeYesWithErrors(
                        ints.TakeLast(ints.Length - 1).ToArray(),
                        maxLevel,
                        damper,
                        errors);
                }
                else
                {
                    if (errors < damper)
                    {
                        return SafeYesWithErrors(
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

        static bool SafeYesDamper(int[] ints, int damper = 0)
        {
            var Permutations = Enumerable.Range(1, ints.Length - 1 - damper).Select(i =>
                ints.Take(i).Concat(ints.TakeLast(ints.Length - i - damper)).ToArray());
            return Permutations.Select(a => SafeYes(a)).Any();
        }
    }
}
