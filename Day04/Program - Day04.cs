namespace Day04
{
    internal class Program
    {
        record Direction
        {
            public Compass compass;
            public required Func<int, int, int> iOperation;
            public required Func<int, int, int> jOperation;
        }

        record Location
        {
            public required int i;
            public required int j;
        }

        record Found
        {
            public required Location location;
            public required Direction direction;
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
                [Compass.E] = new Direction { compass = Compass.E, iOperation = zeroOp, jOperation = plusOp },  //E
                [Compass.NE] = new Direction { compass = Compass.NE, iOperation = minusOp, jOperation = plusOp },  //NE
                [Compass.N] = new Direction { compass = Compass.N, iOperation = minusOp, jOperation = zeroOp },  //N
                [Compass.NW] = new Direction { compass = Compass.NW, iOperation = minusOp, jOperation = minusOp },  //NW
                [Compass.W] = new Direction { compass = Compass.W, iOperation = zeroOp, jOperation = minusOp },  //W
                [Compass.SW] = new Direction { compass = Compass.SW, iOperation = plusOp, jOperation = minusOp },  //SW
                [Compass.S] = new Direction { compass = Compass.S, iOperation = plusOp, jOperation = zeroOp },  //S
                [Compass.SE] = new Direction { compass = Compass.SE, iOperation = plusOp, jOperation = plusOp }  //SE
            };

            var countXmases = (string[] input) => CountFoundStrings(input, Directions, "XMAS");

            //int testCount = countXmases(testInput);

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of found XMASes is: {countXmases(liveInput)}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {countXmases(testInput)}.");
        }

        private static int CountFoundStrings(string[] input, Dictionary<Compass, Direction> directions, string findString)
        {
            List<Found> founds = [];
            //each row
            for (int i = 0; i < input.Length; i++)
            {
                //each column
                for (int j = 0; j < input[i].Length; j++)
                {
                    //each direction
                    foreach (var direction in directions)
                    {
                        string crawlDirection (string[] input, Direction aimDirection, Location startLocation, int length) {
                            string crawlDirectionIter(Location iterLocation, int iterLength)
                            {
                                if (iterLength == 0 ||
                                    iterLocation.i < 0 || iterLocation.j < 0
                                    || iterLocation.i > input.Length - 1 ||  iterLocation.j > input[i].Length - 1)
                                {
                                    return string.Empty;
                                }
                                else
                                {
                                    return input[iterLocation.i][iterLocation.j] 
                                        + crawlDirectionIter(MoveLocation(iterLocation, aimDirection, 1), --iterLength);
                                }
                            }
                            return crawlDirectionIter(startLocation, length);
                        }

                        string found = crawlDirection(input, direction.Value, new Location { i = i, j = j }, findString.Length);

                        if (found == findString)
                        {
                            founds.Add(new Found { location = new Location { i = i, j = j }, direction = direction.Value });
                        }
                    }
                }
            }
            return founds.Count;
        }

        static Location MoveLocation(Location location, Direction direction, int distance)
        {
            return new Location { i = direction.iOperation(location.i, distance), 
                                  j = direction.jOperation(location.j, distance) };
        }
    }
}
