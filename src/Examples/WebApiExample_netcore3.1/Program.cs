using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Numpy;
using Python.Runtime;

namespace WebApiExample_netcore3._1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // this call initializes numpy. it is necessary to do that before PythonEngine.BeginAllowThreads()
            np.arange(1);
            PythonEngine.BeginAllowThreads(); // <--- this is very important for a web server since all requests are on different threads

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
