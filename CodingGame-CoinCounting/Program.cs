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
        public static void Main(string[] args)
        {
            IConsole entry = new StreamConsole(string.Join('\n', args).Replace('.', ' '));
            //IConsole Console= new ConsoleFacade();
            var valueToReach = int.Parse(entry.ReadLine());
            var N = int.Parse(entry.ReadLine());
            var nbOfCoins = entry.ReadLine().Split(' ').Select(x => int.Parse(x)).ToArray();
            var values = new int[N];
            var splitted = entry.ReadLine().Split(' ');
            for (int i = 0; i < N; i++)
            {
                values[i] = int.Parse(splitted[i]);
            }
            System.Console.WriteLine(new StackProcesser().Process(valueToReach, N, nbOfCoins, values));
        }
        public interface IProcesser
        {
            public int Process(int valueToReach, int N, int[] nbOfCoins, int[] values);
        }
        public class StackProcesser : IProcesser
        {
            public int Process(int valueToReach, int N, int[] nbOfCoins, int[] values)
            {
                var coupled = nbOfCoins.Select((n, i) => (n, values[i])).ToList();
                coupled.Sort((kv1, kv2) => -kv1.n.CompareTo(kv2.n));
                var stack = new Stack<(int n, int v)>(coupled);
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
        public class ArrayProcessor : IProcesser
        {
            public int Process(int valueToReach, int N, int[] nbOfCoins, int[] values)
            {
                var coupled = nbOfCoins.Select((n, i) => (n, values[i])).ToList();
                coupled.Sort((kv1, kv2) => -kv1.n.CompareTo(kv2.n));
                var array = coupled.ToArray();
                int sum = 0;
                int result = 0;
                int skipped = 0;
                for (; sum < valueToReach; result++)
                {
                    var couple = array[skipped];
                    if (couple.n > 0)
                    {
                        couple.n--;
                        sum += couple.Item2;
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
