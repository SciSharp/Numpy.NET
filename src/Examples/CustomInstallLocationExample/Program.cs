using System;
using System.IO;
using Numpy;

namespace CustomInstallLocationExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // ================================================
            // This example demonstrates how to install Python and Numpy from the assembly's resources
            // (build action 'Embedded resource') into a custom location (here the local execution directory ".")
            // and then use Numpy.Bare with that installation.
            // ================================================

            // set the installation source to be the embedded python zip from our resources
            Python.Deployment.Installer.Source = new Python.Deployment.Installer.EmbeddedResourceInstallationSource()
            {
                Assembly = typeof(Program).Assembly,
                ResourceName = "python-3.7.3-embed-amd64.zip",
            };

            // install in local directory. if you don't set it will install in local app data of your user account
            Python.Deployment.Installer.InstallPath = Path.GetFullPath(".");

            // see what the installer is doing
            Python.Deployment.Installer.LogMessage += Console.WriteLine;

            // install from the given source
            Python.Deployment.Installer.SetupPython(force: false).Wait();

            Python.Deployment.Installer.InstallWheel(typeof(Program).Assembly,
                "numpy-1.16.3-cp37-cp37m-win_amd64.whl").Wait();

            // if the installation is local, you don't even need to set the path
            //Environment.SetEnvironmentVariable("PATH", Path.GetFullPath(@"./python-3.7.3-embed-amd64"), EnvironmentVariableTarget.Process);

            // Now use Numpy.Bare
            var a = np.arange(10);
            Console.WriteLine("a: "+ a.repr);
            var b = np.arange(10)["::-1"];
            Console.WriteLine("b: " + b.repr);
            var a_x_b = np.matmul(a, b);
            Console.WriteLine("a x b: " + a_x_b.repr);
        }
    }
}
