using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Numpy;

namespace WebApiExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // this call initializes numpy. it is necessary to do that before PythonEngine.BeginAllowThreads()
            np.arange(1);
            Python.Runtime.PythonEngine.BeginAllowThreads(); // <--- this is very important for a web server since all requests are on different threads
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
