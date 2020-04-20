using System;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            ICodeGenerator generator = new NumPy.ApiGenerator();
            var result = generator.Generate();

            Console.WriteLine(result);
            //Console.ReadKey();
        }
    }
}
