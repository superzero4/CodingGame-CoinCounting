using CoinCounting;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Linq;
namespace Validators
{
    public class Validators
    {
        [SetUp]
        public void Setup()
        {

        }

        [TestCase(new int[] { 1, 1, 1 }, new int[] { 2, 5, 3 })]
        [TestCase(new int[] { 1, 2, 3 }, new int[] { 5, 2, 4 })]
        [TestCase(new int[] { 1, 2, 3, 5, 3 }, new int[] { 3, 6, 5, 2, 4 })]
        public void TestStackGeneration(int[] nbOfCoins, int[] values)
        {
            var obtained = new Program.Processers.StackProcesser().CreateStructure(nbOfCoins, values);
            //We reverse zipped array even thou it's sorted correctly because we're in a LIFO struct when creating stack to compare so elements need to be inserted from enumerable in reverse order
            Assert.AreEqual(new System.Collections.Generic.Stack<(int, int)>(Program.Sorts.ZipThenOrder(nbOfCoins, values).Reverse()), obtained);
        }
        public void Test<T>(int result, int valueToReach, int N, int[] nbOfCoins, int[] values) where T : Program.Processers.IProcesser, new()
        {
            var obtained = new T().Process(valueToReach, N, nbOfCoins, values);
            //Assert.AreEqual(consoleOut.ToString(), "Hello, World!");
            Assert.AreEqual(result, obtained);
        }
        //Randomly generated
        [TestCase(7, 49, 1, new int[] { 89 }, new int[] { 8 })]
        [TestCase(-1, 188, 1, new int[] { 3 }, new int[] { 8 })]
        [TestCase(22, 154, 2, new int[] { 66, 76 }, new int[] { 8, 7 })]
        [TestCase(-1, 246, 3, new int[] { 12, 7, 26 }, new int[] { 7, 8, 0 })]
        [TestCase(45, 134, 4, new int[] { 54, 94, 44, 44 }, new int[] { 9, 3, 6, 9 })]
        [TestCase(121, 366, 5, new int[] { 94, 90, 71, 23, 82 }, new int[] { 6, 2, 8, 6, 9 })]
        //HandMade
        [TestCase(3, 14, 2, new int[] { 2, 2 }, new int[] { 6, 5 })]
        [TestCase(3, 9, 1, new int[] { 3 }, new int[] { 4 })]
        public void TestStack(int result, int valueToReach, int N, int[] nbOfCoins, int[] values) => Test<Program.Processers.StackProcesser>(result, valueToReach, N, nbOfCoins, values);
        
        //Randomly generated
        [TestCase(7, 49, 1, new int[] { 89 }, new int[] { 8 })]
        [TestCase(-1, 188, 1, new int[] { 3 }, new int[] { 8 })]
        [TestCase(22, 154, 2, new int[] { 66, 76 }, new int[] { 8, 7 })]
        [TestCase(-1, 246, 3, new int[] { 12, 7, 26 }, new int[] { 7, 8, 0 })]
        [TestCase(45, 134, 4, new int[] { 54, 94, 44, 44 }, new int[] { 9, 3, 6, 9 })]
        [TestCase(121, 366, 5, new int[] { 94, 90, 71, 23, 82 }, new int[] { 6, 2, 8, 6, 9 })]
        //HandMade
        [TestCase(3, 14, 2, new int[] { 2, 2 }, new int[] { 6, 5 })]
        [TestCase(3, 9, 1, new int[] { 3 }, new int[] { 4 })]
        public void TestArray(int result, int valueToReach, int N, int[] nbOfCoins, int[] values) => Test<Program.Processers.SortedArrayProcessor>(result, valueToReach, N, nbOfCoins, values);
    }
}