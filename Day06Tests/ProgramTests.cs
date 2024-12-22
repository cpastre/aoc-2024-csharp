using Microsoft.VisualStudio.TestTools.UnitTesting;
using Day06;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Day06.Program;

namespace Day06.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void PatrolLocationsCoveredTestPart1()
        {
            string[] input = File.ReadAllLines("input - Sample.txt");

            //Part1
            Assert.AreEqual(46, PatrolLocationsCovered(BuildGrid(input), FindInitialLocation(BuildGrid(input)), Directions[Compass.N])
                .locations.Count());
            Assert.AreEqual(41, PatrolLocationsCovered(BuildGrid(input), FindInitialLocation(BuildGrid(input)), Directions[Compass.N])
                .locations.Distinct().Count());
        }

        [TestMethod()]
        public void PatrolLocationsCoveredTestPart2()
        {
            string[] input = File.ReadAllLines("input - Sample.txt");

            //Part2
            var inputWithNewBlock = BuildGrid(input);
            inputWithNewBlock[6][3] = '#';
            var snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreEqual(snake.locations.First(), snake.locations.Last());

            inputWithNewBlock = BuildGrid(input);
            inputWithNewBlock[7][6] = '#';
            snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreEqual(snake.locations.First(), snake.locations.Last());

            inputWithNewBlock = BuildGrid(input);
            inputWithNewBlock[7][7] = '#';
            snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreEqual(snake.locations.First(), snake.locations.Last());

            inputWithNewBlock = BuildGrid(input);
            inputWithNewBlock[8][1] = '#';
            snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreEqual(snake.locations.First(), snake.locations.Last());

            inputWithNewBlock = BuildGrid(input);
            inputWithNewBlock[8][3] = '#';
            snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreEqual(snake.locations.First(), snake.locations.Last());

            inputWithNewBlock = BuildGrid(input);
            inputWithNewBlock[9][7] = '#';
            snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreEqual(snake.locations.First(), snake.locations.Last());

            inputWithNewBlock = BuildGrid(input);
            snake = PatrolLocationsCovered(inputWithNewBlock, FindInitialLocation(BuildGrid(input)), Directions[Compass.N]);
            Assert.AreNotEqual(snake.locations.First(), snake.locations.Last());
        }

        [TestMethod()]
        public void ZipStepsTest()
        {
            IEnumerable<Location> locs = [
                        new Location { i = 5, j = 2 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 },
                        new Location { i = 2, j = 2 },
                        new Location { i = 2, j = 3 },
                        new Location { i = 3, j = 3 },
                        new Location { i = 4, j = 3 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 }
            ];

            Assert.AreEqual(new Step { l1 = new Location { i = 5, j = 2 }, l2 = new Location { i = 4, j = 2 } },
                            ZipSteps(locs).First());
            Assert.AreEqual(new Step { l1 = new Location { i = 4, j = 2 }, l2 = new Location { i = 3, j = 2 } },
                            ZipSteps(locs).Last());
        }

        [TestMethod()]
        public void TrimDuplicateStepsTest()
        {
            IEnumerable<Location> locs = [
                        new Location { i = 5, j = 2 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 },
                        new Location { i = 2, j = 2 },
                        new Location { i = 2, j = 3 },
                        new Location { i = 3, j = 3 },
                        new Location { i = 4, j = 3 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 }
            ];

            IEnumerable<Location> locsTrimmed = [
                        new Location { i = 5, j = 2 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 },
                        new Location { i = 2, j = 2 },
                        new Location { i = 2, j = 3 },
                        new Location { i = 3, j = 3 },
                        new Location { i = 4, j = 3 },
                        new Location { i = 4, j = 2 }
                        ];


            Assert.AreEqual(ZipSteps(locsTrimmed).Last(), TrimDuplicateSteps(ZipSteps(locs)).Last());
        }

        [TestMethod()]
        public void UnzipStepsTest()
        {
            IEnumerable<Location> locs = [
            new Location { i = 5, j = 2 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 },
                        new Location { i = 2, j = 2 },
                        new Location { i = 2, j = 3 },
                        new Location { i = 3, j = 3 },
                        new Location { i = 4, j = 3 },
                        new Location { i = 4, j = 2 },
                        new Location { i = 3, j = 2 }
];
            Assert.AreEqual(locs.Last(), UnzipSteps(ZipSteps(locs)).Last());
        }
    }
}