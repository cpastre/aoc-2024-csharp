using System.Runtime.CompilerServices;

namespace Day06
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

        public record Step
        {
            public required Location l1;
            public required Location l2;
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

        public static async Task Main(string[] args)
        {
            string[] liveInput = File.ReadAllLines("input.txt");
            string[] testInput = File.ReadAllLines("input - Sample.txt");

            Console.WriteLine("--- PART ONE ---\n");
            Console.WriteLine("Test Values");
            ProcessPart1(testInput);
            Console.WriteLine("Live Values");
            ProcessPart1(liveInput);

            Console.WriteLine("--- PART TWO ---\n");
            Console.WriteLine("Test Values");
            await ProcessPart2(testInput);
            Console.WriteLine("Live Values");
            await ProcessPart2(liveInput);
        }

        private static async void ProcessPart1(string[] input)
        {
            Console.WriteLine($"Total Positions visited: {(await PatrolLocationsCovered(BuildGrid(input), FindInitialLocation(BuildGrid(input)), Directions[Compass.N]))
                .locations.Count()}");
            Console.WriteLine($"Unique Positions visited: {(await PatrolLocationsCovered(BuildGrid(input), FindInitialLocation(BuildGrid(input)), Directions[Compass.N]))
                .locations.Distinct().Count()}");
        }

        private static async Task ProcessPart2(string[] input)
        {
            var inputWithNewBlock = BuildGrid(input);
            List<Location> locs = [];
            for (int i = 0; i < inputWithNewBlock.Length; i++)
            {
                for (int j = 0; j < inputWithNewBlock[i].Length; j++)
                {
                    char initialValue = inputWithNewBlock[i][j];
                    if (initialValue != '^' && initialValue != '#')
                    {
                        inputWithNewBlock[i][j] = '#';
                        //Console.WriteLine($"Replacing: ({ i}, { j})");
                        foreach (char[] s in inputWithNewBlock)
                        {
                            //Console.WriteLine(s);
                        }
                        var plc = () => PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
                        if ((await plc()).isLoop)
                        {
                            locs.Add(new Location { i = i, j = j });
                            Console.WriteLine($"({i}, {j})");
                        }
                        inputWithNewBlock[i][j] = initialValue;
                    }
                }
            }
            //foreach (var l in locs)
            //    Console.WriteLine($"({l.i}, {l.j})");
            Console.WriteLine($"Count: {locs.Count()}");
        }

        public static async Task<(IEnumerable<Location> locations, bool isLoop)> PatrolLocationsCovered(char[][] grid, Location startLocation,
            Direction startDirection)
        {
            (IEnumerable<Location>, bool isLoop) DoPatrolIter(IEnumerable<Location> pathSoFar, Direction direction)
            {
                var nextBlockPath = () => NextBlockPath(grid, pathSoFar.Last(), direction);
                var nextBlock = () => nextBlockPath().path.Last();

                if (!nextBlockPath().isBlock)
                {
                    //leaves boundary, wrap it up (including the final block, which isn't actually a block, just last before boundary)
                    return ([.. pathSoFar, .. nextBlockPath().path], false);
                }
                else
                {
                    var nextBlockPathExStart = () => nextBlockPath().path.Skip(1);
                    var nextBlockPathExStartExBlock = () => nextBlockPathExStart().Take(nextBlockPathExStart().Count() - 1);

                    //check to see if this new path end is already on the existing path and if we were going in the same direction when we started out - maybe we're in a loop
                    if (IsLoop([..pathSoFar, ..nextBlockPathExStartExBlock()]))
                    {
                        //yes, we've been here before - let's return the complete loop with the end position == start position
                        return ([.. pathSoFar,
                                .. nextBlockPath().path.TakeWhile(l => l != startLocation),
                                startLocation], true);
                    }
                    else
                    {
                        IEnumerable<Location> newPath() => [.. pathSoFar, .. nextBlockPathExStart().Take(nextBlockPathExStart().Count() - 1)];
                        var newPathStartLocation = () => newPath().Last();
                        return DoPatrolIter(newPath(), TurnDirection(direction, -90));
                    }
                }
            }
            return DoPatrolIter(new List<Location>() { startLocation }, startDirection);
        }

        private static double PatrolDistance(char[][] grid, Location startLocation, Direction startDirection)
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

        private static (bool isBlock, Location blockLocation)
            NextBlock(char[][] grid, Location fromLocation, Direction fromDirection)
        {
            return (NextBlockPath(grid, fromLocation, fromDirection).isBlock,
                    NextBlockPath(grid, fromLocation, fromDirection).blockLocation);
        }

        private static (bool isBlock, Location blockLocation, IEnumerable<Location> path)
            NextBlockPath(char[][] grid, Location fromLocation, Direction fromDirection)
        {
            IEnumerable<Location> DoStepIter(IEnumerable<Location> pathSoFar)
            {
                if (grid[pathSoFar.Last().i][pathSoFar.Last().j] == '#' || IsBoundaryInDirection(grid, pathSoFar.Last(), fromDirection))
                {
                    return pathSoFar;
                }
                else
                {
                    return DoStepIter([.. pathSoFar, MoveLocation(pathSoFar.Last(), fromDirection, 1)]);
                }
            }
            IEnumerable<Location> returnPath = DoStepIter([fromLocation]);
            Location terminus() => returnPath.Last();

            return GetValueAt(grid, terminus()) switch
            {
                '#' => (true, terminus(), returnPath),
                _ => (false, terminus(), returnPath)
            };
        }

        private static bool IsBoundaryInDirection(char[][] grid, Location location, Direction fromDirection)
        {
            return (fromDirection.compass == Compass.E && location.j == grid[location.i].Length - 1) ||
                (fromDirection.compass == Compass.N && location.i == 0) ||
                (fromDirection.compass == Compass.W && location.j == 0) ||
                (fromDirection.compass == Compass.S && location.i == grid.Length - 1);
        }

        public static Location FindInitialLocation(char[][] grid)
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

        public static char[][] BuildGrid(string[] liveInput)
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

        public static IEnumerable<Step> ZipSteps(IEnumerable<Location> locations)
        {
            if (locations.Count() <= 1)
            {
                throw new ArgumentException("Can't zip a list of locations fewer than 2.");
            }
            return [//new Step { l1 = null, l2 = locations.First()},
                    ..locations.Take(locations.Count()-1).Zip(locations.Skip(1)).Select(i => new Step { l1 = i.First, l2 = i.Second} ),
                    //new Step { l1 = locations.Last(), l2 = null }
                    ];
        }

        public static IEnumerable<Step> TrimDuplicateSteps(IEnumerable<Step> steps)
        {
            if (steps.Count() == 0 
                || !steps.Take(steps.Count() - 1).Contains(steps.Last()))
            {
                return steps;
            }
            else
            {
                return TrimDuplicateSteps(steps.Take(steps.Count() - 1));
            }
        }

        public static IEnumerable<Location> UnzipSteps(IEnumerable<Step> steps)
        {
            if (steps.Count() == 0)
            {
                return [];
            }
            else
            {
                return [.. steps.Select(s => s.l1), steps.Last().l2];
            }
        }

        public static bool IsLoop(IEnumerable<Location> path)
        {
            var trimmed = () => UnzipSteps(TrimDuplicateSteps(ZipSteps(path)));
            return path.Count() > 1 && path.Count() != trimmed().Count();   //can't be a loop if it's less than 2 locations
        }
    }
}
