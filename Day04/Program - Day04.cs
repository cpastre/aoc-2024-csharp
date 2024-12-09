namespace Day04
{
    public class Program
    {
        public record Direction
        {
            public Compass compass;
            public required Func<int, int, int> iOperation;
            public required Func<int, int, int> jOperation;
            public required int cardinality;
        }

        public record Location
        {
            public required int i;
            public required int j;
        }

        public record Found
        {
            public required Location location;
            public required Direction direction;
        }

        public enum Compass { E, NE, N, NW, W, SW, S, SE };

        public static readonly Func<int, int, int> plusOp = (int i1, int i2) => i1 + i2;
        public static readonly Func<int, int, int> minusOp = (int i1, int i2) => i1 - i2;
        public static readonly Func<int, int, int> zeroOp = (int i1, int i2) => i1;
        public static readonly Dictionary<Compass, Direction> Directions = new()
        {
            [Compass.E] = new Direction { compass = Compass.E, iOperation = zeroOp, jOperation = plusOp, cardinality = 0 },  //E
            [Compass.NE] = new Direction { compass = Compass.NE, iOperation = minusOp, jOperation = plusOp, cardinality = 1 },  //NE
            [Compass.N] = new Direction { compass = Compass.N, iOperation = minusOp, jOperation = zeroOp, cardinality = 2 },  //N
            [Compass.NW] = new Direction { compass = Compass.NW, iOperation = minusOp, jOperation = minusOp, cardinality = 3 },  //NW
            [Compass.W] = new Direction { compass = Compass.W, iOperation = zeroOp, jOperation = minusOp, cardinality = 4 },  //W
            [Compass.SW] = new Direction { compass = Compass.SW, iOperation = plusOp, jOperation = minusOp, cardinality = 5 },  //SW
            [Compass.S] = new Direction { compass = Compass.S, iOperation = plusOp, jOperation = zeroOp, cardinality = 6 },  //S
            [Compass.SE] = new Direction { compass = Compass.SE, iOperation = plusOp, jOperation = plusOp, cardinality = 7 }  //SE
        };

        static void Main(string[] args)
        {
            var liveInput = File.ReadAllLines(@"input.txt");
            var testInput = File.ReadAllLines(@"input - sample.txt");

            var countXmases = (string[] input) => CountFoundStrings(input, "XMAS");

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine($"The number of found XMASes is: {countXmases(liveInput)}.");
            Console.WriteLine();
            Console.WriteLine($"Test reports: {countXmases(testInput)}.");

            //Part 2

        }

        public static int CountFoundStrings(string[] input, string findString)
        {
            List<Found> founds = [];
            //each row
            for (int i = 0; i < input.Length; i++)
            {
                //each column
                for (int j = 0; j < input[i].Length; j++)
                {
                    //each direction
                    foreach (var direction in Directions)
                    {
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

        private static string crawlDirection(string[] input, Direction aimDirection, Location startLocation, int length)
        {
            string crawlDirectionIter(Location iterLocation, int iterLength)
            {
                if (iterLength == 0 ||
                    iterLocation.i < 0 || iterLocation.j < 0
                    || iterLocation.i > input.Length - 1 || iterLocation.j > input[iterLocation.i].Length - 1)
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

        public static Location MoveLocation(Location location, Direction direction, int distance)
        {
            return new Location { i = direction.iOperation(location.i, distance), 
                                  j = direction.jOperation(location.j, distance) };
        }

        public static (Direction perp1, Direction perp2) PerpendicularDirections(Direction direction)
        {
            var NormalizeCardinality = (int cardinality) => cardinality % Directions.Count;
            return (Directions.Where(d => d.Value.cardinality == NormalizeCardinality(direction.cardinality + Directions.Count/4*1)).First().Value,
                Directions.Where(d => d.Value.cardinality == NormalizeCardinality(direction.cardinality + Directions.Count/4*3)).First().Value);
        }

        public static bool ArePerpendicular(Direction direction1, Direction direction2) =>
            direction2 == PerpendicularDirections(direction1).perp1 ||
            direction2 == PerpendicularDirections(direction1).perp2;

        public static string[] GetSubBlock(string[] block, Location centerLocation, int iHeight, int jWidth)
        {
            if (iHeight % 2 == 0 || jWidth % 2 == 0)
            {
                throw new ArgumentException("iHeight and jWidth must be odd numbers.");
            }

            int iBufferRequired = (iHeight - 1) / 2;
            int jBufferRequired = (jWidth - 1) / 2;
            if (centerLocation.i - iBufferRequired < 0 
                || centerLocation.i + iBufferRequired >= block[0].Length
                || centerLocation.j - jBufferRequired < 0
                || centerLocation.j + jBufferRequired >= block.Length)
            {
                throw new ArgumentException("Subblock dimensions too large for initial block");
            }

            return null;
        }
    }
}
