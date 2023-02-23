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
            System.Console.WriteLine(Process(valueToReach, N, nbOfCoins, values));
        }
        public static int Process(int valueToReach, int N, int[] nbOfCoins, int[] values)
        {
            return (valueToReach / values[0]) + 1;
        }
    }
}
