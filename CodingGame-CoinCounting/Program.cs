namespace CoinCounting
{

    public class Program
    {
        public static void Main()
        {
            var valueToReach = int.Parse(Console.ReadLine());
            var N = int.Parse(Console.ReadLine());
            var nbOfCoins = Console.ReadLine().Split(' ').Select(x => int.Parse(x)).ToArray();
            var values = new int[N];
            var splitted = Console.ReadLine().Split(' ');
            for (int i = 0; i < N; i++)
            {
                values[i] = int.Parse(splitted[i]);
            }
            Console.WriteLine(Process(valueToReach, N, nbOfCoins, values));
        }
        public static int Process(int valueToReach, int N, int[] nbOfCoins, int[] values)
        {
            return (valueToReach / values[0]) + 1;
        }
    }
}
