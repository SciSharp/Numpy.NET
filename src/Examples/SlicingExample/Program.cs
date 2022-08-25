using System;
using Numpy;

namespace SlicingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = np.arange(20).reshape(4,5);
            Console.WriteLine(a);
            var b = a["2:4"];
            Console.WriteLine("\n sliced with 2:4");
            Console.WriteLine(b );
        }
    }
}
