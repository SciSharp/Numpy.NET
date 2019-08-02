using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace ReleaseBot
{
    class Program
    {
        private const string V = "1.9"; // <--- numpy.net version!
        private const string PythonNetVersion = "1";

        private const string ProjectPath = "../../../Numpy";
        private const string ProjectName = "Numpy.csproj";

        private const string ProjectPath2 = "../../../Numpy.Bare";
        private const string ProjectName2 = "Numpy.Bare.csproj";

        private const string Description = "C# bindings for NumPy on {0} - a fundamental library for scientific computing, machine learning and AI. Does require Python {1} with NumPy 1.16 installed!";
        private const string Tags = "Data science, Machine Learning, ML, AI, Scientific Computing, NumPy, Linear Algebra, FFT, SVD, BLAS, Vector, Matrix, Python";

        static void Main(string[] args)
        {
            // ==> Numpy
            ProcessNumpy();

            // ==> Numpy Bare
            var specs = new ReleaseSpec[]
            {
                // linux                
                new ReleaseSpec() { CPythonVersion = "2.7", Platform="Linux",   },
                new ReleaseSpec() { CPythonVersion = "3.5", Platform="Linux",   },
                new ReleaseSpec() { CPythonVersion = "3.6", Platform="Linux",   },
                new ReleaseSpec() { CPythonVersion = "3.7", Platform="Linux",   },
                // mac
                new ReleaseSpec() { CPythonVersion = "2.7", Platform="OSX",  },
                new ReleaseSpec() { CPythonVersion = "3.5", Platform="OSX",  },
                new ReleaseSpec() { CPythonVersion = "3.6", Platform="OSX",  },
                new ReleaseSpec() { CPythonVersion = "3.7", Platform="OSX",  },
                // win
                new ReleaseSpec() { CPythonVersion = "2.7", Platform="Win64",   },
                new ReleaseSpec() { CPythonVersion = "3.5", Platform="Win64",   },
                new ReleaseSpec() { CPythonVersion = "3.6", Platform="Win64",   },
                new ReleaseSpec() { CPythonVersion = "3.7", Platform="Win64",   },

            };
            foreach (var spec in specs)
            {
                spec.Version = $"{spec.CPythonVersion}.{V}";
                spec.PythonNetVersion = $"{spec.CPythonVersion}.{PythonNetVersion}";
                spec.Description = string.Format(Description, spec.Platform, spec.CPythonVersion);
                spec.PackageTags = Tags;
                spec.RelativeProjectPath = ProjectPath2;
                spec.ProjectName = ProjectName2;
                switch (spec.Platform)
                {
                    case "Linux":
                        spec.PackageId = "Numpy.Bare.Mono";
                        spec.PythonNet = "Python.Runtime.Mono";
                        break;
                    case "OSX":
                        spec.PackageId = "Numpy.Bare.OSX";
                        spec.PythonNet = "Python.Runtime.OSX";
                        break;
                    case "Win64":
                        spec.PackageId = "Numpy.Bare";
                        spec.PythonNet = "Python.Runtime.NETStandard";
                        break;
                }
                spec.Process();
            }

            var key = File.ReadAllText("../../nuget.key").Trim();
            foreach (var nuget in Directory.GetFiles(Path.Combine(ProjectPath2, "bin", "Release"), "*.nupkg"))
            {
                Console.WriteLine("Push " + nuget);
                var arg = $"push -Source https://api.nuget.org/v3/index.json -ApiKey {key} {nuget}";
                var p = new Process() { StartInfo = new ProcessStartInfo("nuget.exe", arg) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false } };
                p.OutputDataReceived += (x, data) => Console.WriteLine(data.Data);
                p.ErrorDataReceived += (x, data) => Console.WriteLine("Error: " + data.Data);
                p.Start();
                p.WaitForExit();
                Console.WriteLine("... pushed");
            }
            Thread.Sleep(3000);
        }

        private static void ProcessNumpy()
        {
            var spec = new ReleaseSpec()
            {
                Version = $"3.7.{V}",
                ProjectName = ProjectName,
                RelativeProjectPath = ProjectPath,
                PackageId = "Numpy",
                Description = @"C# bindings for NumPy - a fundamental library for scientific computing, machine learning and AI. Does not require a local Python installation!",
                PackageTags = "Data science, Machine Learning, ML, AI, Scientific Computing, NumPy, Linear Algebra, FFT, SVD, Matrix, Python",
                UsePythonIncluded = true,
            };
            spec.Process();
            // nuget
            var key = File.ReadAllText("../../nuget.key").Trim();
            foreach (var nuget in Directory.GetFiles(Path.Combine(ProjectPath, "bin", "Release"), "*.nupkg"))
            {
                Console.WriteLine("Push " + nuget);
                var arg = $"push -Source https://api.nuget.org/v3/index.json -ApiKey {key} {nuget}";
                var p = new Process() { StartInfo = new ProcessStartInfo("nuget.exe", arg) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false } };
                p.OutputDataReceived += (x, data) => Console.WriteLine(data.Data);
                p.ErrorDataReceived += (x, data) => Console.WriteLine("Error: " + data.Data);
                p.Start();
                p.WaitForExit();
                Console.WriteLine("... pushed");
            }
        }
    }

    public class ReleaseSpec
    {
        /// <summary>
        /// The assembly / nuget package version
        /// </summary>
        public string Version;

        public string CPythonVersion;
        public string Platform;

        /// <summary>
        /// Project description
        /// </summary>
        public string Description;

        /// <summary>
        /// Project description
        /// </summary>
        public string PackageTags;

        /// <summary>
        /// Nuget package id
        /// </summary>
        public string PackageId;

        /// <summary>
        /// PythonNet package name
        /// </summary>
        public string PythonNet;

        /// <summary>
        /// PythonNet Version
        /// </summary>
        public string PythonNetVersion;

        /// <summary>
        /// Name of the csproj file
        /// </summary>
        public string ProjectName;

        /// <summary>
        /// Path to the csproj file, relative to the execution directory of ReleaseBot
        /// </summary>
        public string RelativeProjectPath;

        public string FullProjectPath => Path.Combine(RelativeProjectPath, ProjectName);

        /// <summary>
        /// Uses Python.Included
        /// </summary>
        public bool UsePythonIncluded { get; set; }

        public void Process()
        {
            if (!File.Exists(FullProjectPath))
                throw new InvalidOperationException("Project not found at: " + FullProjectPath);
            // modify csproj
            var doc = new HtmlDocument() { OptionOutputOriginalCase = true, OptionWriteEmptyNodes = true };
            doc.Load(FullProjectPath);
            var group0 = doc.DocumentNode.Descendants("propertygroup").FirstOrDefault();
            SetInnerText(group0.Element("version"), Version);
            Console.WriteLine("Version: " + group0.Element("version").InnerText);
            SetInnerText(group0.Element("description"), Description);
            Console.WriteLine("Description: " + group0.Element("description").InnerText);
            if (!UsePythonIncluded)
            {
                SetInnerText(group0.Element("packageid"), PackageId);
                var group1 = doc.DocumentNode.Descendants("itemgroup").FirstOrDefault(g => g.Element("packagereference") != null);
                var reference = group1.Descendants("packagereference").ToArray()[1];
                reference.Attributes["Include"].Value = PythonNet;
                reference.Attributes["Version"].Value = PythonNetVersion;
            }
            doc.Save(FullProjectPath);
            // now build in release mode
            RestoreNugetDependencies();
            Build();
        }

        private void RestoreNugetDependencies()
        {
            Console.WriteLine("Fetch Nugets " + Description);
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo("dotnet", "restore")
                { WorkingDirectory = Path.GetFullPath(RelativeProjectPath) }
            };
            p.Start();
            p.WaitForExit();
        }

        private void Build()
        {
            Console.WriteLine("Build " + Description);
            var p = new Process()
            {
                StartInfo = new ProcessStartInfo("dotnet", "build -c Release")
                { WorkingDirectory = Path.GetFullPath(RelativeProjectPath) }
            };
            p.Start();
            p.WaitForExit();
        }

        private void SetInnerText(HtmlNode node, string text)
        {
            node.ReplaceChild(HtmlTextNode.CreateNode(text), node.FirstChild);
        }
    }
}
