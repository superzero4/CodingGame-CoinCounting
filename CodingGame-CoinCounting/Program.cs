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
        private const bool _generateTestCases = false;
        private const int maxValuesAndCount = 1000;
        private const int maxTot = 1000;
        private const char separator = ' ';
        private const string separatorTest = ", ";

        public static void Main(string[] args)
        {
            if (_generateTestCases)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sbUnitTest = new StringBuilder();
                foreach (int N in new int[] { 100, 1000/*1, 2, 3, 4, 5*/ })
                {
                    int maxReachableTot = 0;
                    for (int i = maxValuesAndCount; i >= maxValuesAndCount - N; i--)
                    {
                        maxReachableTot += i;
                    }
                    maxReachableTot *= maxValuesAndCount;
                    maxReachableTot = (int)MathF.Min(maxReachableTot, maxTot);
                    //maxTot /= 10;
                    var rand = new Random();
                    int valueToReach = rand.Next(maxReachableTot);
                    sb.Append(valueToReach).Append(separator);
                    sbUnitTest.Append(valueToReach).Append(separatorTest);
                    sb.Append(N).Append(separator);
                    sbUnitTest.Append(N).Append(separatorTest);
                    //To list is mandatory because rand.Next is called when the iterator comes through and we want fixed list
                    var counts = Enumerable.Range(0, N).Select(n => rand.Next(maxValuesAndCount)).ToList();
                    sb.Append(string.Join('.', counts)).Append(separator);
                    TextArray(sbUnitTest, counts).Append(separatorTest);

                    var values = Enumerable.Range(0, N).Select(n => rand.Next(maxValuesAndCount)).ToList();
                    sb.Append(string.Join('.', values));
                    TextArray(sbUnitTest, values).Append(")]");

                    var arg = sb.ToString();
                    Solve(arg.Split(separator));
                    Console.WriteLine('\n' + sb.ToString().Replace(separator, '\n').Replace('.', ' ') + "\n ^^^^^^ CodingGameTest \\ C# test vvvvvvv");
                    Console.Write("[TestCase(");
                    Solve(arg.Split(separator));
                    Console.WriteLine(separatorTest + sbUnitTest.ToString());
                    sb.Clear();
                    sbUnitTest.Clear();
                }
            }
            else
                Solve(args);

            StringBuilder TextArray(StringBuilder sb, IEnumerable<int> values)
            {
                return sb.Append(" new int[] { ").Append(string.Join(", ", values)).Append(" }");
            }
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
            System.Console.Write(new Processers.MinArrayProcessor().Process(valueToReach, N, nbOfCoins, values));
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
            public static (int n, int v)[] ZipArrays(int[] nbOfCoins, int[] values)
            {
                var couples = new (int, int)[nbOfCoins.Length];
                for (int i = 0; i < nbOfCoins.Length; i++)
                {
                    couples[i] = (nbOfCoins[i], values[i]);
                }
                return couples;
            }
            public static (int n, int v)[] CreateSortedCouples(int[] nbOfCoins, int[] values)
            {
                var couples =ZipArrays(nbOfCoins, values);
                Array.Sort(couples, new Sorts());
                return couples;
            }
            public static IEnumerable<(int n, int v)> ZipThenOrder(int[] nbOfCoins, int[] values)
            {
                return nbOfCoins.Zip(values, (n, v) => (n, v)).OrderBy(selector);
            }
            public static Func<(int n, int v), int> selector => (kv) => kv.v;
            public int Compare((int, int) kv1, (int, int) kv2) => selector(kv1).CompareTo(selector(kv2));
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
                    //If total ponderated sum if < goal, we return -1 meaning we can't get the value
                    if (nbOfCoins.Zip(values, (n, v) => n * v).Sum() < valueToReach)
                        return -1;
                    return Process(valueToReach, CreateStructure(nbOfCoins, values));
                }
                public abstract int Process(int valueToReach, T structure);
            }
            public class StackProcesser : BaseProcesser<Stack<(int n, int v)>>
            {
                public override Stack<(int n, int v)> CreateStructure(int[] nbOfCoins, int[] values)
                {
                    var stack = new Stack<(int, int)>();
                    for (int i =  0; i < nbOfCoins.Length; i++)
                    {
                        int secondValue = values[i];
                        var tuple = (nbOfCoins[i], secondValue);
                        if (stack.Count == 0 || secondValue <= stack.Peek().Item2)
                            stack.Push(tuple);
                        else
                        {
                            var toReorder = new List<(int, int)>();
                            while (stack.Count > 0 && secondValue > stack.Peek().Item2)
                                toReorder.Add(stack.Pop());
                            stack.Push(tuple);
                            foreach (var item in toReorder.AsEnumerable().Reverse())
                                stack.Push(item);
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
                        /*                        if (stack.Count == 0)
                                                    return -1;*/
                        var couple = stack.Pop();
                        if (couple.n > 0)
                        {
                            couple.n--;
                            sum += couple.v;
                            if (couple.n > 0)
                                stack.Push(couple);
                            /*else
                                Console.Error.WriteLine("Out of coins for : " + couple);*/
                        }
                    }
                    return result;
                }
            }
            public class SortedArrayProcessor : BaseProcesser<(int n, int v)[]>
            {
                public override (int, int)[] CreateStructure(int[] nbOfCoins, int[] values) => Program.Sorts.CreateSortedCouples(nbOfCoins, values);

                public override int Process(int valueToReach, (int n, int v)[] array)
                {
                    int sum = 0;
                    int result = 0;
                    int skipped = 0;
                    for (; sum < valueToReach; result++)
                    {
                        if (array[skipped].n > 0)
                        {
                            array[skipped].n--;
                            sum += array[skipped].v;
                            if (array[skipped].n == 0)
                                skipped++;
                        }
                    }
                    return result;
                }
            }
            public class MinArrayProcessor : BaseProcesser<(int n, int v)[]>
            {
                public override (int n, int v)[] CreateStructure(int[] nbOfCoins, int[] values) => Sorts.ZipArrays(nbOfCoins, values);

                public override int Process(int valueToReach, (int n, int v)[] array)
                {
                    int sum = 0;
                    int result = 0;
                    int index = 0;
                    while (sum < valueToReach)
                    {
                        int min = Program.maxValuesAndCount * 2;
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].n > 0 && array[i].v < min)
                            {
                                min = array[i].v;
                                index = i;
                            }
                        }
                        var couple = array[index];
                        //(index,var couple) = array.Where(nv => nv.n > 0).Select((nv,i)=>(i,nv)).MinBy(inv => inv.nv.v);
                        for (; couple.n > 0 && sum < valueToReach; result++, couple.n--)
                        {
                            sum += couple.v;
                            //Update modified value
                        }
                        array[index] = couple;
                    }
                    return result;
                }
            }
        }
    }
}

