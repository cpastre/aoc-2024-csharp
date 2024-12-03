using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines(@"input.txt");
        string[] testLines = File.ReadAllLines(@"input - sample.txt");

        var match = static (string _line, int matchNo) => Regex.Matches(_line, @"\b(\w+)")[matchNo].Value;
        var genLeftList = (string[] _lines) => _lines.Select(l => int.Parse(match(l, 0)));
        var genRightList = (string[] _lines) => _lines.Select(l => int.Parse(match(l, 1)));
        var finalList = static (IEnumerable<int> _left, IEnumerable<int> _right) => 
            _left.Order().Zip(_right.Order()).Select(p => new Pair() { Left = p.First, Right = p.Second });
        var calculateDistance = static (IEnumerable<Pair> pairs) => pairs.Sum(p => Math.Abs(p.Left - p.Right));

        Console.WriteLine("--- PART ONE ---\n");
        Console.WriteLine($"The total distance is: {calculateDistance(finalList(genLeftList(lines), genRightList(lines)))}.");
        Console.WriteLine();
        Console.WriteLine($"Test distance: {calculateDistance(finalList(genLeftList(testLines), genRightList(testLines)))}.");

        var SimilarityScore = (int leftNumber, IEnumerable<int> rightList) => rightList.Where(i => i == leftNumber).Sum();
        var TotalSimilarityScore = (IEnumerable<int> _left, IEnumerable<int> _right) => _left.Sum(i => SimilarityScore(i, _right));

        Console.WriteLine("--- PART TWO ---\n");
        Console.WriteLine($"The total similary score is: {TotalSimilarityScore(genLeftList(lines), genRightList(lines))}.");
        Console.WriteLine();
        Console.WriteLine($"Test score: {TotalSimilarityScore(genLeftList(testLines), genRightList(testLines))}.");

    }
}

record Pair
{
    public int Left;
    public int Right;
}