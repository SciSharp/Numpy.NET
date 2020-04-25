using System;
using Numpy;

namespace NetCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var m = np.array(new int[] { 1, 2, 3, 4 }) * 2;
            Console.WriteLine(m.repr);

            Console.WriteLine("");
            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }
}
