using CoinCounting;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
namespace Validators
{
    public class Validators
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase(16, 14, 4, new int[] { 2, 3, 4, 5 }, new int[] { 4, 5, 2, 8 })]
        [TestCase(9, 14, 3, new int[] { 4, 3, 4, 5 }, new int[] { 4, 5, 2, 8 })]
        [TestCase(3, 14, 2, new int[] { 2,2 }, new int[] { 6,5 })]
        [TestCase(3, 9, 1, new int[] { 3 }, new int[] { 4 })]
        public void Test(int result, int valueToReach, int N, int[] nbOfCoins, int[] values)
        {
            var obtained = Program.Process(valueToReach,N, nbOfCoins, values);
            //Assert.AreEqual(consoleOut.ToString(), "Hello, World!");
            Assert.AreEqual(result, obtained);
        }
    }
}