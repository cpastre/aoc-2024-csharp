using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        string[] lines = File.ReadAllLines(@"input.txt");

        List<int> leftlist = [];
        List<int> rightlist = [];

        foreach (var line in lines)
        {
            leftlist.Add(int.Parse(line.Split(" ", StringSplitOptions.TrimEntries)[0]));
            rightlist.Add(int.Parse(line.Split(" ", StringSplitOptions.TrimEntries)[1]));
        }

        leftlist.Sort();
        rightlist.Sort();

        List<Pair> pairs = [];

        for (int i = 0; i < leftlist.Count; i++)
        {
            pairs.Add(new Pair { Left = leftlist[i], Right = rightlist[i] });
        }

        int distance = pairs.Sum(s => Math.Abs(s.Left + s.Right));

        Console.WriteLine($"The total distance is: {distance}.");
    }
}

record Pair
{
    public int Left;
    public int Right;
}