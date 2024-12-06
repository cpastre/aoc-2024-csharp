namespace Day04
{
    internal class Program
    {
        record Direction
        {
            public required Func<int, int, int> iOperation;
            public required Func<int, int, int> jOperation;
        }

        record Location
        {
            public required int i;
            public required int j;
        }

        enum Compass { E, NE, N, NW, W, SW, S, SE };
        static void Main(string[] args)
        {
            var liveInput = File.ReadAllLines(@"input.txt");
            var testInput = File.ReadAllLines(@"input - sample.txt");

            Func<int, int, int> plusOp = (int i1, int i2) => i1 + i2;
            Func<int, int, int> minusOp = (int i1, int i2) => i1 - i2;
            Func<int, int, int> zeroOp = (int i1, int i2) => i1;
            Dictionary<Compass, Direction> Directions = new Dictionary<Compass, Direction> {
                [Compass.E] = new Direction { iOperation = zeroOp, jOperation = plusOp },  //E
                [Compass.NE] = new Direction { iOperation = minusOp, jOperation = plusOp },  //NE
                [Compass.N] = new Direction { iOperation = minusOp, jOperation = zeroOp },  //N
                [Compass.NW] = new Direction { iOperation = minusOp, jOperation = minusOp },  //NW
                [Compass.W] = new Direction { iOperation = zeroOp, jOperation = minusOp },  //W
                [Compass.SW] = new Direction { iOperation = minusOp, jOperation = plusOp },  //SW
                [Compass.S] = new Direction { iOperation = minusOp, jOperation = zeroOp },  //S
                [Compass.SE] = new Direction { iOperation = plusOp, jOperation = plusOp }  //SE
                };

            var countXmases = (string[] input) => CountFoundStrings(input, Directions, "XMAS");

            int testCount = countXmases(testInput);

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of found XMASes is: {countXmases(liveInput)}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {countXmases(testInput)}.");

)

            
        }

        private static int CountFoundStrings(string[] input, Dictionary<Compass, Direction> directions, string findString)
        {
            int findCount = 0;
            //each row
            for (int i = 0; i < input.Length; i++)
            {
                //each column
                for (int j = 0; j < input[i].Length; j++)
                {
                    //each direction
                    foreach (var direction in directions)
                    {
                        string crawlDirection (string[] input, Direction direction, Location startLocation, int length) {
                            string crawlDirectionIter(Location location, int length)
                            {
                                if (location.i < 0 || location.j < 0
                                    || location.i > input.Length - 1 ||  location.j > input[i].Length - 1)
                                {
                                    return string.Empty;
                                }
                                else
                                {
                                    return crawlDirectionIter(MoveLocation(location, direction, 1), 1) + input[i][j];
                                }
                            }
                            return crawlDirectionIter(startLocation, 1);
                        }

                        if (crawlDirection(input, direction, new Location { i = i, j = j }, findString.Length)
                            == findString)
                        {
                            findCount++;
                        }
                    }
                }
            }
            return findCount;
        }

        static Location MoveLocation(Location location, Direction direction, int distance)
        {
            return new Location { i = direction.iOperation(location.i, distance), 
                                  j = direction.jOperation(location.j, distance) };
        }
    }
}
