

using System.Reflection.Metadata.Ecma335;

namespace Day06
{
    internal class Program
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

        public static Location MoveLocation(Location location, Direction direction, int distance)
        {
            return new Location
            {
                i = direction.iOperation(location.i, distance),
                j = direction.jOperation(location.j, distance)
            };
        }

        public static Direction OpposingDirection(Direction direction)
        {
            return Directions.Where(d => d.Value.cardinality == NormalizeCardinality(direction.cardinality + Directions.Count / 4 * 2)).First().Value;
        }

        public static Direction TurnDirection(Direction fromDirection, int degrees)
        {
            return Directions.Single(d => d.Value.cardinality
                                          == NormalizeCardinality(fromDirection.cardinality + (degrees + 360) / 45)).Value; //+360 to normalize negative degrees (that are less than 360)
        }

        private static int NormalizeCardinality(int cardinality) => cardinality % Directions.Count;

        public static double Distance(Location location1, Location location2)
        {
            return Math.Sqrt(Math.Pow(location2.i - location1.i, 2) + Math.Pow(location2.j - location1.j, 2));
        }

        static void Main(string[] args)
        {
            string[] liveInput = File.ReadAllLines("input.txt");
            string[] testInput = File.ReadAllLines("input - Sample.txt");

            ProcessPart1(testInput);

        }

        private static void ProcessPart1(string[] input)
        {
            var grid = BuildGrid(input);
            Location initialLocation = FindInitialLocation(grid);
            Direction initialDirection = Directions[Compass.N];
            var nextBlock = NextBlock(grid, initialLocation, initialDirection);
            var nextStartPoint = MoveLocation(nextBlock.blockLocation, OpposingDirection(initialDirection), 1);
            var nextDirection = TurnDirection(initialDirection, -90);
            var distance = Distance(initialLocation, nextStartPoint);

            var totalDistance = 1 + DoPatrol(BuildGrid(input), FindInitialLocation(grid), Directions[Compass.N]);   //1 for initial position

        }

        private static double DoPatrol(char[][] grid, Location startLocation, Direction startDirection)
        {
            double DoPatrolIter(Location location, Direction direction)
            {
                var nextBlock = () => NextBlock(grid, location, direction);
                var newStartLocation = () => MoveLocation(nextBlock().blockLocation, OpposingDirection(direction), 1);
                if (!NextBlock(grid, location, direction).isBlock)
                {
                    //leaves boundary, wrap it up
                    return Distance(location, nextBlock().blockLocation);
                }
                else
                {
                    return Distance(location, newStartLocation()) +
                           DoPatrolIter(newStartLocation(), TurnDirection(direction, -90));
                }
            }
            return DoPatrolIter(startLocation, startDirection);
        }

        private static (bool isBlock, Location blockLocation) NextBlock(char[][] grid, Location fromLocation, Direction fromDirection)
        {
            Location DoStepIter(Location location)
            {
                if (grid[location.i][location.j] == '#' || IsBoundaryInDirection(grid, location, fromDirection))
                {
                    return location;
                }
                else
                {
                    return DoStepIter(MoveLocation(location, fromDirection, 1));
                }
            }
            Location terminus() => DoStepIter(fromLocation);

            return GetValueAt(grid, terminus()) switch
            {
                '#' => (true, terminus()),
                _ => (false, terminus())
            };
        }

        private static bool IsBoundaryInDirection(char[][] grid, Location location, Direction fromDirection)
        {
            return (fromDirection.compass == Compass.E && location.j == grid[location.i].Length - 1) ||
                (fromDirection.compass == Compass.N && location.i == 0) ||
                (fromDirection.compass == Compass.W && location.j == 0) ||
                (fromDirection.compass == Compass.S && location.i == grid.Length - 1);
        }

        private static Location FindInitialLocation(char[][] grid)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] == '^')
                    {
                        return new Location { i = i, j = j };
                    }
                }
            }
            throw new Exception("Initial ^ location not found.");
        }

        private static char[][] BuildGrid(string[] liveInput)
        {
            char[][] grid = new char[liveInput.Length][];
            for (int i = 0; i < liveInput.Length; i++)
            {
                grid[i] = liveInput[i].ToCharArray();
            }
            return grid;
        }

        private static char GetValueAt(char[][] grid, Location location)
        {
            return grid[location.i][location.j];
        }
    }
}
