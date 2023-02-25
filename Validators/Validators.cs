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

        [TestCase(new int[] { 1,1,1},new int[] { 2,5,3})]
        [TestCase(new int[] { 1,2,3},new int[] { 5,2,4})]
        [TestCase(new int[] { 1,2,3,5,3},new int[] { 3,6,5,2,4})]
        public void TestStackGeneration(int[] nbOfCoins, int[] values)
        {
            var obtained = new Program.Processers.StackProcesser().CreateStructure(nbOfCoins, values);
            //We reverse zipped array even thou it's sorted correctly because we're in a LIFO struct when creating stack to compare so elements need to be inserted from enumerable in reverse order
            Assert.AreEqual(new System.Collections.Generic.Stack<(int, int)>(Program.Sorts.ZipThenOrder(nbOfCoins, values).Reverse()),obtained);
        }
        public void Test<T>(int result, int valueToReach, int N, int[] nbOfCoins, int[] values) where T : Program.Processers.IProcesser, new()
        {
            var obtained = new T().Process(valueToReach, N, nbOfCoins, values);
            //Assert.AreEqual(consoleOut.ToString(), "Hello, World!");
            Assert.AreEqual(result, obtained);
        }
        [TestCase(16, 14, 4, new int[] { 2, 3, 4, 5 }, new int[] { 4, 5, 2, 8 })]
        [TestCase(9, 14, 3, new int[] { 4, 3, 4, 5 }, new int[] { 4, 5, 2, 8 })]
        [TestCase(3, 14, 2, new int[] { 2, 2 }, new int[] { 6, 5 })]
        [TestCase(3, 9, 1, new int[] { 3 }, new int[] { 4 })]
        public void TestStack(int result, int valueToReach, int N, int[] nbOfCoins, int[] values) => Test<Program.Processers.StackProcesser>(result, valueToReach, N, nbOfCoins, values);
        [TestCase(16, 14, 4, new int[] { 2, 3, 4, 5 }, new int[] { 4, 5, 2, 8 })]
        [TestCase(9, 14, 3, new int[] { 4, 3, 4, 5 }, new int[] { 4, 5, 2, 8 })]
        [TestCase(3, 14, 2, new int[] { 2, 2 }, new int[] { 6, 5 })]
        [TestCase(3, 9, 1, new int[] { 3 }, new int[] { 4 })]
        public void TestArray(int result, int valueToReach, int N, int[] nbOfCoins, int[] values) => Test<Program.Processers.ArrayProcessor>(result, valueToReach, N, nbOfCoins, values);
    }
}