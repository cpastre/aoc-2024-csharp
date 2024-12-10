using Microsoft.VisualStudio.TestTools.UnitTesting;
using Day04;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Day04.Program;
using System.Net.Http.Headers;

namespace Day04.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void MoveLocationTest()
        {
            Location initialLocation = new Location { i = 3, j = 4 };

            Location newLocation = MoveLocation(initialLocation, Directions[Compass.NW], 1);
            Assert.AreEqual(newLocation, new() { i = 2, j = 3 });

            newLocation = MoveLocation(initialLocation, Directions[Compass.S], 2);
            Assert.AreEqual(newLocation, new() { i = 5, j = 4 });

            newLocation = MoveLocation(initialLocation, Directions[Compass.NE], 1);
            Assert.AreEqual(newLocation, new() { i = 2, j = 5 });
        }

        [TestMethod()]
        public void CountFoundStringsTest()
        {
            var liveInput = File.ReadAllLines(@"input.txt");
            var testInput = File.ReadAllLines(@"input - sample.txt");

            var countXmases = (string[] input) => CountFoundStrings(input, "XMAS");

            //part 1
            Assert.AreEqual(countXmases(liveInput), 2390);
            Assert.AreEqual(countXmases(testInput), 18);
        }

        [TestMethod()]
        public void PerpendicularDirectionsTest()
        {
            var result = PerpendicularDirections(Directions[Compass.E]);
            Assert.IsTrue((result.perp1.compass == Compass.N || result.perp1.compass == Compass.S) &&
                (result.perp2.compass == Compass.N || result.perp2.compass == Compass.S));

            result = PerpendicularDirections(Directions[Compass.SW]);
            Assert.IsTrue((result.perp1.compass == Compass.NW || result.perp1.compass == Compass.SE) &&
                (result.perp2.compass == Compass.NW || result.perp2.compass == Compass.SE));

            result = PerpendicularDirections(Directions[Compass.S]);
            Assert.IsTrue((result.perp1.compass == Compass.W || result.perp1.compass == Compass.E) &&
                (result.perp2.compass == Compass.W || result.perp2.compass == Compass.E));
        }

        [TestMethod()]
        public void ArePerpendicularTest()
        {
            Assert.IsTrue(ArePerpendicular(Directions[Compass.W], Directions[Compass.N]));
            Assert.IsTrue(ArePerpendicular(Directions[Compass.SE], Directions[Compass.NE]));
            Assert.IsFalse(ArePerpendicular(Directions[Compass.W], Directions[Compass.SW]));
            Assert.IsFalse(ArePerpendicular(Directions[Compass.SW], Directions[Compass.SW]));
        }

        [TestMethod()]
        public void GetSubBlockTest()
        {
            string[] testblock = ["01234", "56789", "ABCDE", "FGHIJ", "KLMNO"];
            Assert.ThrowsException<ArgumentException>(() => GetSubBlock(testblock, new Location { i = 2, j = 2 }, 2, 3));
            Assert.ThrowsException<ArgumentException>(() => GetSubBlock(testblock, new Location { i = 3, j = 3 }, 5, 6));
            Assert.ThrowsException<ArgumentException>(() => GetSubBlock(testblock, new Location { i = 4, j = 3 }, 3, 3));
            Assert.ThrowsException<ArgumentException>(() => GetSubBlock(testblock, new Location { i = 2, j = 3 }, 5, 5));

            string[] testSubBlock1 = ["678", "BCD", "GHI"];
            foreach (int row in Enumerable.Range(0, 3))
                Assert.AreEqual(testSubBlock1[row], GetSubBlock(testblock, new Location { i = 2, j = 2 }, 3, 3)[row]);

            string[] testSubBlock2 = ["01234", "56789", "ABCDE", "FGHIJ", "KLMNO"];
            foreach (int row in Enumerable.Range(0, 5))
                Assert.AreEqual(testSubBlock2[row], GetSubBlock(testblock, new Location { i = 2, j = 2 }, 5, 5)[row]);

            string[] testSubBlock3 = ["CDE", "HIJ", "MNO"];
            foreach (int row in Enumerable.Range(0, 3))
                Assert.AreEqual(testSubBlock3[row], GetSubBlock(testblock, new Location { i = 3, j = 3 }, 3, 3)[row]);

            string[] testSubBlock4 = ["1"];
            foreach (int row in Enumerable.Range(0, 1))
                Assert.AreEqual(testSubBlock4[row], GetSubBlock(testblock, new Location { i = 0, j = 1 }, 1, 1)[row]);

            Assert.AreNotEqual("9", GetSubBlock(testblock, new Location { i = 0, j = 1 }, 1, 1)[0]);
        }

        [TestMethod()]
        public void OpposingDirectionTest()
        {
            Assert.AreEqual(Directions[Compass.E], OpposingDirection(Directions[Compass.W]));
            Assert.AreEqual(Directions[Compass.SE], OpposingDirection(Directions[Compass.NW]));
            Assert.AreEqual(Directions[Compass.S], OpposingDirection(Directions[Compass.N]));
        }

        [TestMethod()]
        public void IsMasTest()
        {
            string[] testBlock = [".M.S.",
                                  "..A..",
                                  ".M.S.",
                                  "..A.A",
                                  ".M.S.M"];

            Assert.IsTrue(IsMas(testBlock, Directions[Compass.SE], new Location { i = 0, j = 1 }));
            Assert.IsFalse(IsMas(testBlock, Directions[Compass.E], new Location { i = 0, j = 1 }));
        }

        [TestMethod()]
        public void IsMasFromDirectionTest()
        {
            string[] testBlock = [".M.S.",
                                  "..A..",
                                  ".M.S.",
                                  "..A.A",
                                  ".M.S.M"];

            Assert.IsTrue(IsMasFromDirection(testBlock, Directions[Compass.NW], new Location { i = 1, j = 2 }));
            Assert.IsTrue(IsMasFromDirection(testBlock, Directions[Compass.SW], new Location { i = 1, j = 2 }));
            Assert.IsFalse(IsMasFromDirection(testBlock, Directions[Compass.SE], new Location { i = 1, j = 2 }));
        }

        [TestMethod()]
        public void BlocksMatchTest()
        {
            string[] testBlock1 = [".M.S.",
                                  "..A..",
                                  ".M.S.",
                                  "..A.A",
                                  ".M.S.M"];

            string[] testBlock2 = [".M.S.",
                                  "..A..",
                                  ".X.S.",
                                  "..A.A",
                                  ".M.S.M"];

            //same as 1
            string[] testBlock3 = [".M.S.",
                                  "..A..",
                                  ".M.S.",
                                  "..A.A",
                                  ".M.S.M"];

            Assert.IsTrue(BlocksMatch(testBlock1, testBlock3));
            Assert.IsFalse(BlocksMatch(testBlock1, testBlock2));
        }
    }
}