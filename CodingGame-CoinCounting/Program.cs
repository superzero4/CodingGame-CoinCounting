using System.Text;

namespace CoinCounting
{

    public interface IConsole
    {
        string ReadLine();
    }
    public class ConsoleFacade : IConsole
    {
        public string ReadLine() => Console.ReadLine();
    }
    public class StreamConsole : IConsole
    {
        TextReader _sr;
        public string ReadLine()
        {
            return _sr.ReadLine();
        }
        public StreamConsole(string args)
        {
            _sr = new StringReader(args);
        }
    }

    public class Program
    {
        private const bool _generateTestCases = true;
        private const int maxVal = 10;
        private const int maxCount = 100;
        private const char separator = ' ';

        public static void Main(string[] args)
        {
            if (_generateTestCases)
            {
                StringBuilder sb = new StringBuilder();
                foreach (int N in new int[] { 1, 2, 3, 4, 5 })
                {
                    int maxTot = 0;
                    for (int i = maxVal; i >= maxVal - N; i--)
                    {
                        maxTot += i;
                    }
                    maxTot *= maxCount;
                    maxTot /= 10;
                    var rand = new Random();
                    sb.Append(rand.Next(maxTot)).Append(separator);
                    sb.Append(N).Append(separator);
                    sb.Append(string.Join('.', Enumerable.Range(0, N).Select(n => rand.Next(maxCount)))).Append(separator);
                    sb.Append(string.Join('.', Enumerable.Range(0, N).Select(n => rand.Next(maxVal))));
                    var arg = sb.ToString();
                    Solve(arg.Split(separator));
                    Console.WriteLine(sb.ToString().Replace(separator, '\n').Replace('.', ' ') + "\n ^^^^^^ args");
                    sb.Clear();
                }
            }
            else
                Solve(args);
        }

        private static void Solve(string[] args)
        {
            IConsole entry = new StreamConsole(string.Join('\n', args).Replace('.', separator));
            //IConsole Console= new ConsoleFacade();
            var valueToReach = int.Parse(entry.ReadLine());
            var N = int.Parse(entry.ReadLine());
            var nbOfCoins = entry.ReadLine().Split(separator).Select(x => int.Parse(x)).ToArray();
            var values = new int[N];
            var splitted = entry.ReadLine().Split(separator);
            for (int i = 0; i < N; i++)
            {
                values[i] = int.Parse(splitted[i]);
            }
            System.Console.WriteLine(new Processers.StackProcesser().Process(valueToReach, N, nbOfCoins, values));
        }
        public class Sorts : IComparer<(int, int)>
        {
            //static Func<(int, int,int)> comparison = (kv1, kv2) => -kv1.Item2.CompareTo(kv2.Item2);
            public static IEnumerable<(int, int)> Sort(IEnumerable<(int, int)> enumerable)
            {
                var tmp = enumerable.ToList();
                tmp.Sort(new Sorts());
                return tmp;
            }
            public static (int n, int v)[] CreateSortedCouples(int[] nbOfCoins, int[] values)
            {
                var couples = new (int, int)[nbOfCoins.Length];
                for (int i = 0; i < nbOfCoins.Length; i++)
                {
                    couples[i] = (nbOfCoins[i], values[i]);
                }
                Array.Sort(couples, new Sorts());
                return couples;
            }
            public static IEnumerable<(int n, int v)> ZipThenOrder(int[] nbOfCoins, int[] values)
            {
                return nbOfCoins.Zip(values, (n, v) => (n, v)).OrderBy(selector);
            }
            public static Func<(int, int), int> selector => (kv) => kv.Item2;
            public int Compare((int, int) kv1, (int, int) kv2) => -selector(kv1).CompareTo(selector(kv2));
        }
        public class Processers
        {
            public interface IProcesser
            {
                public int Process(int valueToReach, int N, int[] nbOfCoins, int[] values);
            }
            public interface IProcesser<T> : IProcesser where T : IEnumerable<(int n, int v)>
            {
                public T CreateStructure(int[] nbOfCoins, int[] values);
            }

            public abstract class BaseProcesser<T> : IProcesser<T> where T : IEnumerable<(int n, int v)>
            {
                public abstract T CreateStructure(int[] nbOfCoins, int[] values);

                public int Process(int valueToReach, int N, int[] nbOfCoins, int[] values)
                {
                    return Process(valueToReach, CreateStructure(nbOfCoins, values));
                }
                public abstract int Process(int valueToReach, T structure);
            }
            public class StackProcesser : BaseProcesser<Stack<(int n, int v)>>
            {
                public override Stack<(int n, int v)> CreateStructure(int[] nbOfCoins, int[] values)
                {
                    var stack = new Stack<(int, int)>();
                    // Iterate over the arrays in reverse order, and push the tuples onto the stack
                    // in the order of the sorted second values.
                    for (int i = nbOfCoins.Length - 1; i >= 0; i--)
                    {
                        int secondValue = values[i];
                        var tuple = (nbOfCoins[i], secondValue);
                        if (stack.Count == 0 || secondValue <= stack.Peek().Item2)
                            stack.Push(tuple);
                        else
                        {
                            while (stack.Count > 0 && secondValue > stack.Peek().Item2)
                                stack.Pop();
                            stack.Push(tuple);
                        }
                    }
                    //less optimized would be 
                    //stack = new Stack<(int, int)>(Program.Sorts.ZipThenOrder(nbOfCoins, values));

                    return stack;
                }

                public override int Process(int valueToReach, Stack<(int n, int v)> stack)
                {
                    int sum = 0;
                    int result = 0;
                    for (; sum < valueToReach; result++)
                    {
                        var couple = stack.Pop();
                        if (couple.n > 0)
                        {
                            couple.n--;
                            sum += couple.v;
                            if (couple.n > 0)
                                stack.Push(couple);
                            else
                                Console.Error.WriteLine("Out of coins for : " + couple);
                        }
                    }
                    return result;
                }
            }
            public class ArrayProcessor : BaseProcesser<(int n, int v)[]>
            {
                public override (int, int)[] CreateStructure(int[] nbOfCoins, int[] values) => Program.Sorts.CreateSortedCouples(nbOfCoins, values);

                public override int Process(int valueToReach, (int n, int v)[] array)
                {
                    int sum = 0;
                    int result = 0;
                    int skipped = 0;
                    for (var couple = array[skipped]; sum < valueToReach; result++)
                    {
                        if (couple.n > 0)
                        {
                            couple.n--;
                            sum += couple.v;
                        }
                        else
                        {
                            skipped++;
                            //We didn't pick a coin because we skipped it
                            result--;
                            Console.Error.WriteLine("Out of coins for : " + couple);
                        }
                    }
                    return result;
                }
            }
        }
    }
}
