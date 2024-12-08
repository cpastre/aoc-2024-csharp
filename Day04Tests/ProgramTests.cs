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
    }
}