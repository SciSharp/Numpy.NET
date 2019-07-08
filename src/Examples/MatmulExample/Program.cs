using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Numpy;
using Python.Runtime;

namespace MatmulExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Efficient matrix multiplication with NumPy:");
            // before starting the measurement, let us call numpy once to get the setup checks done. 
            np.arange(1);
            var stopwatch = Stopwatch.StartNew();

            var a1 = np.arange(60000).reshape(300, 200);
            var a2 = np.arange(80000).reshape(200, 400);

            var result = np.matmul(a1, a2);
            stopwatch.Stop();

            Console.WriteLine($"execution time with NumPy: {stopwatch.Elapsed.TotalMilliseconds}ms\n");
            Console.WriteLine("Result:\n" + result.repr);


            Console.WriteLine("executing on bg thread");

            var a = np.arange(1000);
            var b = np.arange(1000);

            // https://github.com/pythonnet/pythonnet/issues/109
            PythonEngine.BeginAllowThreads();

            Task.Run(() =>
            {
                using (Py.GIL())
                {
                    np.matmul(a, b);
                    Console.WriteLine("matmul on bg thread is done");
                }
            }).Wait();
            Console.WriteLine("Press key");

            Console.ReadKey();
        }
    }
}
