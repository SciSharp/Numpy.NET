using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CodeMinion.Core;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;
using CodeMinion.Parser;
using HtmlAgilityPack;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator.NumPy
{
    // Routines: [x] means generated
    // ====================
    // [x] Array creation routines 
    // [x] Array manipulation routines
    // [x] Binary operations
    // [x] String operations
    // [x] Datetime Support Functions
    // [x] Data type routines
    // [x] Optionally Scipy-accelerated routines(numpy.dual)
    // [ ] Floating point error handling
    // [x] Discrete Fourier Transform(numpy.fft)
    // [x] Financial functions
    // [ ] Functional programming
    // [x] Indexing routines
    // [x] Input and output
    // [x] Linear algebra(numpy.linalg)
    // [x] Logic functions
    // [ ] Masked array operations
    // [x] Mathematical functions
    // [ ] Matrix library(numpy.matlib)
    // [ ] Miscellaneous routines
    // [x] Padding Arrays
    // [ ] Polynomials
    // [x] Random sampling(numpy.random)
    // [x] Set routines
    // [x] Sorting, searching, and counting
    // [x] Statistics
    // [x] Window functions


    public class ApiGenerator : ICodeGenerator
    {
        private CodeGenerator _generator;
        public ApiGenerator()
        {
            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            var test_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\test\\";

            _generator = new CodeGenerator
            {
                CopyrightNotice = "Copyright (c) 2020 by Meinrad Recheis (Member of SciSharp)",
                NameSpace = "Numpy",
                StaticModuleName = "np",
                PythonModuleName="numpy",
                TestFilesPath = Path.Combine(test_dir, "Numpy.UnitTest"),
                Usings = { "using Numpy.Models;" },
                ToPythonConversions = {
                    "case Axis o: return o.Axes==null ? null : ToTuple(o.Axes);",
                    "case Shape o: return ToTuple(o.Dimensions);",
                    "case Slice o: return o.ToPython();",
                    "case PythonObject o: return o.PyObject;",
                    "case Dictionary<string, NDarray> o: return ToDict(o);",
                },
                ToCsharpConversions =
                {
                    "case \"Dtype\": return (T)(object)new Dtype(pyobj);",
                    "case \"NDarray\": return (T)(object)new NDarray(pyobj);",
                    "case \"NDarray`1\":",
                    "switch (typeof(T).GenericTypeArguments[0].Name)",
                    "{",
                    "   case \"Byte\": return (T)(object)new NDarray<byte>(pyobj);",
                    "   case \"Short\": return (T)(object)new NDarray<short>(pyobj);",
                    "   case \"Boolean\": return (T)(object)new NDarray<bool>(pyobj);",
                    "   case \"Int32\": return (T)(object)new NDarray<int>(pyobj);",
                    "   case \"Int64\": return (T)(object)new NDarray<long>(pyobj); ",
                    "   case \"Single\": return (T)(object)new NDarray<float>(pyobj); ",
                    "   case \"Double\": return (T)(object)new NDarray<double>(pyobj); ",
                    "   default: throw new NotImplementedException($\"Type NDarray<{typeof(T).GenericTypeArguments[0].Name}> missing. Add it to 'ToCsharpConversions'\");",
                    "}",
                    "break;",
                    "case \"NDarray[]\":",
                    "   var po = pyobj as PyObject;",
                    "   var len = po.Length();",
                    "   var rv = new NDarray[len];",
                    "   for (int i = 0; i < len; i++)",
                    "       rv[i] = ToCsharp<NDarray>(po[i]);",
                    "   return (T) (object) rv;",
                    "case \"Matrix\": return (T)(object)new Matrix(pyobj);",
                },
                SpecialConversionGenerators = { SpecialGenerators.ConvertArrayToNDarray, SpecialGenerators.ConvertDict },
                SharpToSharpConversions =
                {
                    SpecialGenerators.ArrayToNDarrayConversion,
                },
                InitializationGenerators = { SpecialGenerators.InitNumpyGenerator },
            };
        }

        // use this to avoid duplicates
        HashSet<string> parsed_api_functions = new HashSet<string>();
        private DynamicApi ndarray_api;

        public string Generate()
        {
            var dir = Directory.GetCurrentDirectory();
            var src_dir = dir.Substring(0, dir.LastIndexOf("\\src\\")) + "\\src\\";
            // ----------------------------------------------------
            // array creation
            // ----------------------------------------------------
            var array_creation_api = new StaticApi()
            {
                PartialName = "array_creation", // name-part of the partial class file
                StaticName = "np", // name of the static API class
                ImplName = "NumPy", // name of the singleton that implements the static API behind the scenes
                PythonModule = "numpy", // name of the Python module that the static api wraps 
            };
            _generator.StaticApis.Add(array_creation_api);
            ParseNumpyApi(array_creation_api, "routines.array-creation.html");
            // ----------------------------------------------------
            // ndarray
            // ----------------------------------------------------
            ndarray_api = new DynamicApi()
            {
                ClassName = "NDarray",
            };
            _generator.DynamicApis.Add(ndarray_api);
            ParseNdarrayApi(ndarray_api);
            // ----------------------------------------------------
            // array manipulation
            // ----------------------------------------------------
            var array_manipulation_api = new StaticApi() { PartialName = "array_manipulation", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(array_manipulation_api);
            ParseNumpyApi(array_manipulation_api, "routines.array-manipulation.html");
            // ----------------------------------------------------
            // dtype
            // ----------------------------------------------------
            var dtype_api = new StaticApi() { PartialName = "dtype", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(dtype_api);
            ParseDtypeApi(dtype_api);
            // ----------------------------------------------------
            // bitwise api
            // ----------------------------------------------------
            var bitwise_api = new StaticApi() { PartialName = "bitwise", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(bitwise_api);
            ParseNumpyApi(bitwise_api, "routines.bitwise.html");
            // ----------------------------------------------------
            // String operations
            // ----------------------------------------------------
            var string_api = new StaticApi() { PartialName = "string", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(string_api);
            ParseNumpyApi(string_api, "routines.char.html");
            // ----------------------------------------------------
            // Datetime Support Functions
            // ----------------------------------------------------
            var datetime_api = new StaticApi() { PartialName = "datetime", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(datetime_api);
            ParseNumpyApi(datetime_api, "routines.datetime.html");
            // ----------------------------------------------------
            // Data type routines
            // ----------------------------------------------------
            var dtype_routines_api = new StaticApi() { PartialName = "dtype.routines", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(dtype_routines_api);
            ParseNumpyApi(dtype_routines_api, "routines.dtype.html");
            // ----------------------------------------------------
            // Optionally Scipy-accelerated routines (linalg, fft, ...)
            // ----------------------------------------------------
            var linalg_fft_api = new StaticApi() { PartialName = "linalg_fft", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(linalg_fft_api);
            ParseNumpyApi(linalg_fft_api, "routines.dual.html");
            // ----------------------------------------------------
            // Discrete Fourier Transform (numpy.fft)
            // ----------------------------------------------------
            var fft_api = new StaticApi() { PartialName = "fft", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(fft_api);
            ParseNumpyApi(fft_api, "routines.fft.html");
            // ----------------------------------------------------
            // Financial functions
            // ----------------------------------------------------
            var financial_api = new StaticApi() { PartialName = "financial", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(financial_api);
            ParseNumpyApi(financial_api, "routines.financial.html");
            // ----------------------------------------------------
            // Indexing routines
            // ----------------------------------------------------
            var indexing_api = new StaticApi() { PartialName = "indexing", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(indexing_api);
            ParseNumpyApi(indexing_api, "routines.indexing.html");
            // ----------------------------------------------------
            // Input and output
            // ----------------------------------------------------
            var io_api = new StaticApi() { PartialName = "io", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(io_api);
            ParseNumpyApi(io_api, "routines.io.html");
            // ----------------------------------------------------
            // Linear Algebra
            // ----------------------------------------------------
            var linalg_api = new StaticApi() { PartialName = "linalg", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(linalg_api);
            ParseNumpyApi(linalg_api, "routines.linalg.html");
            // ----------------------------------------------------
            // Logic functions
            // ----------------------------------------------------
            var logic_api = new StaticApi() { PartialName = "logic", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(logic_api);
            ParseNumpyApi(logic_api, "routines.logic.html");
            // ----------------------------------------------------
            // Mathematical functions
            // ----------------------------------------------------
            var math_api = new StaticApi() { PartialName = "math", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(math_api);
            ParseNumpyApi(math_api, "routines.math.html");
            // ----------------------------------------------------
            // Padding Arrays
            // ----------------------------------------------------
            var padding_api = new StaticApi() { PartialName = "padding", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(padding_api);
            ParseNumpyApi(padding_api, "routines.padding.html");
            // ----------------------------------------------------
            // Random sampling
            // ----------------------------------------------------
            var random_api = new StaticApi() { PartialName = "random", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(random_api);
            ParseNumpyApi(random_api, "routines.random.html");
            // ----------------------------------------------------
            // Set routines
            // ----------------------------------------------------
            var set_api = new StaticApi() { PartialName = "set", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(set_api);
            ParseNumpyApi(set_api, "routines.set.html");
            // ----------------------------------------------------
            // Sorting, searching, and counting
            // ----------------------------------------------------
            var sorting_api = new StaticApi() { PartialName = "sorting", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(sorting_api);
            ParseNumpyApi(sorting_api, "routines.sort.html");
            // ----------------------------------------------------
            // Statistics
            // ----------------------------------------------------
            var staticstics_api = new StaticApi() { PartialName = "staticstics", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(staticstics_api);
            ParseNumpyApi(staticstics_api, "routines.statistics.html");
            // ----------------------------------------------------
            // Window functions
            // ----------------------------------------------------
            var window_api = new StaticApi() { PartialName = "window", StaticName = "np", ImplName = "NumPy", PythonModule = "numpy", };
            _generator.StaticApis.Add(window_api);
            ParseNumpyApi(window_api, "routines.window.html");

            // ----------------------------------------------------
            // generate Numpy
            // it is based on Python.Included and packs the Numpy wheel
            // ----------------------------------------------------
            ndarray_api.OutputPath = Path.Combine(src_dir, "Numpy/Models");
            _generator.ModelsPath = Path.Combine(src_dir, "Numpy/Models");
            _generator.StaticApiFilesPath = Path.Combine(src_dir, "Numpy");
            _generator.DynamicApiFilesPath = Path.Combine(src_dir, "Numpy");
            _generator.Generate();
            ApiStatistics();
            Console.WriteLine($"Number of generated functions: {parsed_api_functions.Count} / {all_api_functions.Count}");

            Thread.Sleep(2000);

            return "DONE";
        }

        private void ParseNdarrayApi(DynamicApi api)
        {
            var docs = LoadDocs("arrays.ndarray.html");
            foreach (var html_doc in docs)
            {
                var doc = html_doc.Doc;
                // declaration
                var h1 = doc.DocumentNode.Descendants("h1").FirstOrDefault();
                if (h1 == null)
                    continue;
                var dl = doc.DocumentNode.Descendants("dl").FirstOrDefault();
                if (dl == null || dl.Attributes["class"]?.Value != "method") continue;
                var class_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descclassname").InnerText;
                var func_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descname").InnerText;
                // do not generate the following:
                switch (func_name)
                {
                    case "sort":
                    case "partition":
                    case "transpose":
                        continue;
                }
                var decl = new Function() { Name = func_name, ClassName = class_name.TrimEnd('.') };
                // function description
                var dd = dl.Descendants("dd").FirstOrDefault();
                decl.Description = ParseDescription(dd);
                var table = doc.DocumentNode.Descendants("table")
                    .FirstOrDefault(x => x.Attributes["class"]?.Value == "docutils field-list");
                if (table == null)
                    continue;
                // arguments
                ParseArguments(html_doc, table, decl);

                // return type(s)
                ParseReturnTypes(html_doc, table, decl);

                PostProcess(decl);
                // if necessary create overloads
                foreach (var d in InferOverloads(decl))
                {
                    PostProcessOverloads(d);
                    api.Declarations.Add(d);
                }
            }
        }

        private void PostProcessOverloads(Function function)
        {
            foreach (var arg in function.Arguments)
            {
                if (arg.Name == "axis")
                {
                    if (arg.Type!="int[]")
                        continue;
                    if (arg.DefaultValue == null || arg.DefaultValue == "null")
                        arg.Type = "Axis";
                }
            }
        }

        private int _function_count = 0;

        private void ParseDtypeApi(StaticApi api)
        {
            var doc = GetHtml("arrays.scalars.html");
            foreach (var tr in doc.Doc.DocumentNode.Descendants("tr"))
            {
                if (tr.Descendants("td").Count() != 3)
                    continue;
                var span = tr.Descendants("span").FirstOrDefault();
                if (span == null)
                    continue;
                var td = tr.Descendants("td").Skip(1).FirstOrDefault();
                _function_count++;
                api.Declarations.Add(new Property() { Name = span.InnerText, Description = td?.InnerText, Type = "Dtype", HasSetter = false });
            }
        }

        private void ParseNumpyApi(StaticApi api, string link)
        {
            var docs = LoadDocs(link);
            var testfile = new TestFile() { Name = $"{api.ImplName}_{api.PartialName}" };
            _generator.TestFiles.Add(testfile);
            foreach (var html_doc in docs)
            {
                var doc = html_doc.Doc;
                // declaration
                var h1 = doc.DocumentNode.Descendants("h1").FirstOrDefault();
                if (h1 == null)
                    continue;
                var dl = doc.DocumentNode.Descendants("dl").FirstOrDefault();
                //if (dl == null || dl.Attributes["class"]?.Value != "function") continue;
                var class_name = doc.DocumentNode.Descendants("code").FirstOrDefault(x => x.Attributes["class"]?.Value == "descclassname")?.InnerText;
                if (class_name == null)
                    continue;
                var func_name = doc.DocumentNode.Descendants("code")
                    .First(x => x.Attributes["class"]?.Value == "descname").InnerText;
                if (parsed_api_functions.Contains(class_name + "." + func_name))
                    continue;
                parsed_api_functions.Add(class_name + "." + func_name);
                var decl = new Function() { Tag = link, Name = func_name, ClassName = class_name.TrimEnd('.') };
                // function description
                var dd = dl.Descendants("dd").FirstOrDefault();
                decl.Description = ParseDescription(dd);
                var table = doc.DocumentNode.Descendants("table")
                    .FirstOrDefault(x => x.Attributes["class"]?.Value == "docutils field-list");
                if (table == null)
                    continue;
                //if (decl.Name == "copyto")
                //    Debugger.Break();
                // arguments
                ParseArguments(html_doc, table, decl);

                // return type(s)
                ParseReturnTypes(html_doc, table, decl);

                PostProcess(decl);
                if (!decl.CommentOut)
                    _function_count++;

                // if necessary create overloads
                foreach (var d in InferOverloads(decl))
                {
                    PostProcessOverloads(d);
                    api.Declarations.Add(d);
                    // if this is an ndarray member, add it to the dynamic api also
                    if (ndarray_api != null && d.Arguments.FirstOrDefault()?.Type == "NDarray" && class_name == "numpy.")
                    {
                        switch (decl.Name)
                        {
                            // do not add to NDArray instance methods
                            case "copyto":
                            case "transpose":
                            case "amax":
                            case "amin":
                            case "real":
                            case "imag":
                                continue;
                        }
                        var dc = d.Clone<Function>();
                        dc.Arguments.RemoveAt(0);
                        //dc.ForwardToStaticImpl = "NumPy.Instance";
                        ndarray_api.Declarations.Add(dc);
                    }
                }
                // see if there are any examples which we can convert to test cases
                var testcase = ParseTests(doc, decl);
                if (testcase != null)
                    testfile.TestCases.Add(testcase);
            }
        }

        private TestCase ParseTests(HtmlDocument doc, Function decl)
        {
            int i = 0;
            var dd = doc.DocumentNode.Descendants("dd").FirstOrDefault();
            if (dd == null)
                return null;
            var iter = dd.ChildNodes.SkipWhile(x =>
                  !(x.Name == "p" && x.Attributes["class"]?.Value == "rubric" && x.InnerText == "Examples"));
            var nodes = iter.Skip(1).ToArray();
            if (nodes.Length == 0)
                return null;
            var testcase = new TestCase() { Name = $"{decl.Name}Test" };
            foreach (var element in nodes)
            {
                if (element.Name == "p")
                {
                    var text = HtmlEntity.DeEntitize(element.InnerText ?? "").Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                        testcase.TestParts.Add(new Comment() { Text = text });
                    continue;
                }
                var pre = element.Descendants("pre").FirstOrDefault();
                if (pre == null)
                {
                    //Debugger.Break();
                    continue;
                }
                var part = new ExampleCode() { Text = HtmlEntity.DeEntitize(pre.InnerText) };
                //&gt; &gt; &gt; np.eye(2, dtype = int)
                //array([[1, 0],
                //       [0, 1]])
                //&gt;&gt;&gt; np.eye(3, k=1)
                //array([[ 0.,  1.,  0.],
                //       [ 0.,  0.,  1.],
                //       [ 0.,  0.,  0.]])
                var lines = new Queue<string>(Regex.Split(part.Text.Trim(), @"\r?\n"));
                foreach (var line in lines)
                {
                    if (line.StartsWith(">>>"))
                    {
                        var cmd = line.Replace(">>>", "");
                        if (cmd.Contains("np."))
                            cmd = cmd.Replace('[', '{').Replace(']', '}');
                        part.Lines.Add(new CodeLine() { Text = { cmd }, Type = "cmd" });
                        continue;
                    }
                    if (line.StartsWith("#"))
                    {
                        part.Lines.Add(new CodeLine() { Text = { line.Replace("#", "//") }, Type = "comment" });
                        continue;
                    }
                    if (part.Lines.Count == 0 || part.Lines.Last().Type != "output")
                        part.Lines.Add(new CodeLine() { Text = { line }, Type = "output" });
                    else
                        part.Lines.Last().Text.Add(line);
                }
                testcase.TestParts.Add(part);
            }
            if (testcase.TestParts.Count == 0)
                return null;
            return testcase;
        }

        private string ParseDescription(HtmlNode dd)
        {
            if (dd == null)
                return null;
            var desc = string.Join("\r\n\r\n", dd.ChildNodes.Where(n => n.Name == "p").Select(p => p.InnerText).TakeWhile(s => !s.StartsWith("Examples")));
            return desc;
        }

        private void ParseArguments(HtmlDoc html_doc, HtmlNode table, Function decl)
        {
            var tr = table.Descendants("tr").FirstOrDefault();
            if (tr == null)
                return;
            foreach (var dt in tr.Descendants("dt"))
            {
                var arg = new Argument() { Tag = decl.Tag };
                var strong = dt.Descendants("strong").FirstOrDefault();
                if (strong == null)
                    continue;
                arg.Name = strong.InnerText;
                if (arg.Name.ToLower() == "none")
                    continue; // there are no arguments fro this method
                string type_description = null;
                if (arg.Name.Contains(":"))
                {
                    var tuple = arg.Name.Split(":");
                    arg.Name = tuple[0].Trim();
                    type_description = tuple[1].Trim();
                }
                else
                {
                    type_description = dt.Descendants("span")
                        .FirstOrDefault(span => span.Attributes["class"]?.Value == "classifier")?.InnerText;
                }
                if (type_description == null)
                    type_description = "_NoValue";
                var type = type_description.Split(",").FirstOrDefault();
                arg.Type = InferType(type, arg);
                if (type_description.Contains("optional"))
                {
                    arg.IsNamedArg = true;
                    arg.IsNullable = true;
                }
                if (type_description.Contains("default:"))
                    arg.DefaultValue = InferDefaultValue(type_description.Split(",")
                        .First(x => x.Contains("default: ")).Replace("default: ", ""));
                var dd = dt.NextSibling?.NextSibling;
                arg.Description = ParseDescription(dd);
                arg.Position = decl.Arguments.Count;
                decl.Arguments.Add(arg);
            }
            ParseDefaultValues(html_doc, decl);
            foreach (var arg in decl.Arguments)
                PostProcess(arg);
        }

        private void ParseDefaultValues(HtmlDoc htmlDoc, Function decl)
        {
            var dl = htmlDoc.Doc.DocumentNode.Descendants("dl").FirstOrDefault(x => x.Attributes["class"]?.Value == "function");
            if (dl == null)
                return;
            foreach (var em in dl.Descendants("em"))
            {
                var tokens = em.InnerText.Split("=");
                if (tokens.Length >= 2)
                {
                    var (attr_name, default_value) = (tokens[0], tokens[1]);
                    var attr = decl.Arguments.FirstOrDefault(x => x.Name == attr_name);
                    if (attr == null)
                    {
                        Console.WriteLine("ParseDefaultValues: Attr '{attr_name}' not found");
                        continue;
                    }
                    attr.DefaultValue = InferDefaultValue(default_value);
                }
            }
        }

        private void PostProcess(Argument arg)
        {
            switch (arg.Name)
            {
                case "order":
                    arg.DefaultValue = null;
                    break;
                case "axes":
                    {
                        arg.Type = "int[]";
                        arg.DefaultValue = "null";
                        return;
                    }
                case "where":
                case "out":
                    {
                        arg.IsNullable = true;
                        arg.IsNamedArg = true;
                        arg.DefaultValue = null;
                        break;
                    }
                case "requirements":
                case "comments":
                    arg.DefaultValue = "null";
                    break;
            }
            if (arg.Name.StartsWith("*"))
                arg.Name = arg.Name.TrimStart('*');
            switch (arg.Type)
            {
                //case "int[]":
                //case "Hashtable":
                //    arg.IsValueType = false;
                //    break;
                case "Dtype":
                    arg.IsValueType = false;
                    if (!string.IsNullOrWhiteSpace(arg.DefaultValue))
                        arg.DefaultValue = "null";
                    break;
            }
        }

        private void PostProcess(Function decl)
        {
            if (decl.Arguments.Any(a => a.Type == "buffer_like"))
                decl.CommentOut = true;
            // iterable object            
            if (decl.Arguments.Any(a => a.Type.Contains("<T>")))
            {
                decl.Generics = new string[] { "T" };
                if (decl.Returns[0].Type == "NDarray") // TODO: this feels like a hack. make it more robust if necessary
                    decl.Returns[0].Type = "NDarray<T>";
            }
            if (decl.Returns.Any(a => a.Type == "T" || a.Type.Contains("<T>")))
            {
                decl.Generics = new string[] { "T" };
            }
            // split combined arguments: NDarray x, y => NDarray x, NDarray y
            for (int i = decl.Arguments.Count - 1; i >= 0; i--)
            {
                var arg = decl.Arguments[i];
                if (arg.Type == "_NoValue")
                {
                    decl.Arguments.RemoveAt(i);
                    continue;
                }
                if (arg.Name.Contains(","))
                {
                    var names = arg.Name.Split(',').Select(x => x.Trim()).ToArray();
                    arg.Name = names[0];
                    decl.Arguments.Insert(i + 1, decl.Arguments[i].Clone());
                    decl.Arguments[i].Name = names[1];
                    continue;
                }
            }
            switch (decl.Name)
            {
                case "fft":
                case "random":
                    decl.SharpOnlyPostfix = "_";
                    break;
                case "array":
                    if (decl.ClassName == "numpy")
                        decl.ManualOverride = true; // do not generate an implementation
                    break;
                case "itemset":
                case "tostring":
                case "tobytes":
                case "view":
                case "resize":
                    decl.ManualOverride = true; // do not generate an implementation
                    break;
                case "arange":
                    decl.Arguments[0].IsNullable = false;
                    decl.Arguments[0].DefaultValue = "0";
                    decl.Arguments[2].DefaultValue = "1";
                    decl.Arguments[2].IsNullable = false;
                    decl.Arguments[3].IsNullable = false;
                    decl.Arguments[3].IsNamedArg = true;
                    break;
                case "logspace":
                case "geomspace":
                    decl.Arguments.First(a => a.Type == "Dtype").IsNullable = false;
                    decl.Arguments.First(a => a.Type == "Dtype").DefaultValue = "null";
                    decl.Arguments.First(a => a.Type == "Dtype").IsNamedArg = true;
                    break;
                case "copy":
                    if (decl.Returns.Count==0)
                        decl.Returns.Add(new Argument(){Type = "NDarray"});
                    break;
                case "mat":
                case "bmat":
                case "block":
                case "interp":
                case "einsum_path":
                case "cond":
                case "get_state":
                case "set_state":
                case "genfromtxt":
                case "array2string":
                case "tolist":
                case "format_float_positional":
                case "format_float_scientific":
                case "set_printoptions":
                case "set_string_function":
                case "ravel_multi_index":
                case "nditer":
                case "nested_iters":
                case "result_type":
                case "issubclass_":
                case "find_common_type":
                case "busdaycalendar":
                case "is_busday":
                case "busday_count":
                case "busday_offset":
                    decl.CommentOut = true;
                    break;
                case "require":
                case "tensordot":
                    if (decl.Returns.Count == 0)
                        decl.Returns.Add(new Argument() { Type = "NDarray", Name = "array", IsReturnValue = true });
                    break;
                case "isfortran":
                    if (decl.Returns.Count == 0)
                        decl.Returns.Add(new Argument() { Type = "bool", Name = "retval", IsReturnValue = true });
                    break;
                case "matrix_rank":
                    if (decl.Returns.Count == 0)
                        decl.Returns.Add(new Argument() { Type = "int", Name = "retval", IsReturnValue = true });
                    break;
                case "correlate":
                    decl.Arguments.Remove(decl.Arguments.FirstOrDefault(x => x.Name == "old_behavior"));
                    break;
                case "einsum":
                    var optimize = decl.Arguments.First(x => x.Name == "optimize");
                    optimize.Type = "object";
                    optimize.DefaultValue = "null";
                    optimize.DefaultIfNull = "false";
                    break;
                case "rot90":
                    var axes = decl.Arguments.First(x => x.Name == "axes");
                    axes.DefaultValue = "null";
                    axes.DefaultIfNull = "new int[] {0, 1}";
                    break;
                case "insert":
                    var obj = decl.Arguments.First(x => x.Name == "obj");
                    obj.DefaultValue = "0";
                    var values = decl.Arguments.First(x => x.Name == "values");
                    values.DefaultValue = "null";
                    break;
                case "trapz":
                    var dx = decl.Arguments.First(x => x.Name == "dx");
                    dx.Type = "float";
                    break;
                case "lstsq":
                    {
                        var rcond = decl.Arguments.First(x => x.Name == "rcond");
                        rcond.DefaultValue = "null";
                        break;
                    }
                case "pinv":
                    {
                        var rcond = decl.Arguments.First(x => x.Name == "rcond");
                        rcond.Type = "float";
                        break;
                    }
                case "histogram":
                case "histogram2d":
                case "histogramdd":
                case "histogram_bin_edges":
                    {
                        var bins = decl.Arguments.First(x => x.Name == "bins");
                        bins.DefaultValue = "null";
                        break;
                    }
                case "exponential":
                    decl.Arguments[0].DefaultValue = "null";
                    break;
                case "gamma":
                    decl.Arguments.First(x => x.Name == "scale").DefaultValue = "null";
                    break;
                case "gumbel":
                case "laplace":
                case "logistic":
                case "lognormal":
                case "normal":
                case "poisson":
                case "rayleigh":
                case "uniform":
                    decl.Arguments.ForEach(x =>
                    {
                        x.DefaultValue = "null";
                        x.IsNamedArg = true;
                    });
                    break;
                case "RandomState":
                    {
                        decl.Arguments[0].Type = "int";
                        decl.Arguments[0].DefaultValue = "null";
                        decl.Arguments[0].IsNullable = true;
                        decl.Arguments[0].IsNamedArg = true;
                    }
                    break;
                case "fftfreq":
                case "rfftfreq":
                    decl.Arguments[1].Type = "float";
                    break;
                case "load":
                    decl.Arguments.First(x => x.Name == "mmap_mode").Type = "MemMapMode";
                    // allow_pickle was changed in Numpy version 1.16.3: Made default False in response to CVE-2019-6446.
                    decl.Arguments.First(x => x.Name == "allow_pickle").DefaultValue = "false";
                    break;
                case "save":
                case "savez":
                case "savez_compressed":
                    //    decl.Arguments.First(x => x.Name == "args").Type = "NDarray[]";
                    //    decl.Arguments.First(x => x.Name == "kwds").Type = "Dictionary<string, NDarray>";
                    decl.ManualOverride = true; // do not generate an implementation
                    break;
                case "savetxt":
                    decl.Arguments.First(x => x.Name == "fmt").DefaultValue = "null";
                    decl.Arguments.First(x => x.Name == "encoding").Type = "string";
                    break;
                case "mask_indices":
                    decl.Arguments.First(x => x.Name == "k").Type = "int";
                    break;
                case "select":
                    decl.Arguments.First(x => x.Name == "default").Type = "object";
                    decl.Arguments.First(x => x.Name == "default").DefaultValue = "null";
                    break;
                case "pv":
                case "pmt":
                case "ppmt":
                case "ipmt":
                case "nper":
                    decl.Arguments.First(x => x.Name == "fv").DefaultValue = "null";
                    var when = decl.Arguments.FirstOrDefault(x => x.Name == "when");
                    if (when != null)
                        when.IsNamedArg = true;
                    break;
                case "format_parser":
                    decl.Arguments.First(x => x.Name == "titles").Type = "string[]";
                    break;
                case "mintypecode":
                    decl.Arguments.First(x => x.Name == "typeset").DefaultValue = "null";
                    break;
                case "rand":
                case "randn":
                    decl.Arguments.Clear();
                    decl.Arguments.Add(new Argument(){ Name = "shape", Type = "int[]"});
                    decl.ManualOverride = true;
                    break;
                case "take_along_axis":
                    decl.Arguments[2].IsNullable = true;
                    decl.Returns.Clear();
                    decl.Returns.Add(new Argument() { Type = "NDarray", Name = "array", IsReturnValue = true });
                    break;
                case "column_stack":
                    decl.ManualOverride = true;
                    break;
                case "meshgrid":
                    decl.ReturnType = "NDarray[]";
                    decl.Arguments[0].Type = "NDarray[]";
                    decl.Arguments[0].Name = "xi";
                    decl.Arguments.RemoveAt(1);
                    decl.Arguments[1].DefaultValue = "\"xy\"";
                    break;
                case "mgrid":
                    decl.ReturnType = "NDarray[]";
                    break;
                case "ogrid":
                    decl.ReturnType = "NDarray[]";
                    decl.Arguments.Clear();
                    break;
                case "fromfile":
                    decl.ReturnType = "NDarray";
                    break;
            }
        }

        private void ParseReturnTypes(HtmlDoc html_doc, HtmlNode table, Declaration decl)
        {
            var tr = table.Descendants("tr").FirstOrDefault(x => x.InnerText.StartsWith("Returns:"));
            if (tr == null)
                return;
            foreach (var dt in tr.Descendants("dt"))
            {
                var arg = new Argument() { IsReturnValue = true };
                var strong = dt.Descendants("strong").FirstOrDefault();
                if (strong != null)
                    arg.Name = strong.InnerText;
                if (arg.Name == null || arg.Name.ToLower() == "none")
                    continue;
                var type_description = dt.Descendants("span")
                    .FirstOrDefault(span => span.Attributes["class"]?.Value == "classifier")?.InnerText;
                if (type_description == null)
                    continue;
                var type = type_description.Split(",").FirstOrDefault();
                arg.Type = InferType(type, arg);
                var dd = dt.NextSibling?.NextSibling;
                arg.Description = ParseDescription(dd);
                decl.Returns.Add(arg);
            }
        }

        private IEnumerable<Function> InferOverloads(Function decl)
        {
            switch (decl.Name)
            {
                case "norm":
                case "asscalar":
                case "normal":
                case "meshgrid":
                    // don't generate at all
                    yield break;
                case "all":
                case "any":
                    {
                        decl.Arguments[0].Type = "NDarray";
                        decl.Returns[0].Type = "NDarray<bool>";
                        decl.Arguments.FirstOrDefault(x => x.Name == "axis").IsNullable = false; // make axis mandatory
                        yield return decl;
                        var clone = decl.Clone<Function>();
                        clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "axis"));
                        clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "out"));
                        clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "keepdims"));
                        clone.Returns[0].Type = "bool";
                        yield return clone;
                        yield break;
                    }
                case "count_nonzero":
                    {
                        decl.Arguments[0].Type = "NDarray";
                        decl.Returns[0].Type = "NDarray<int>";
                        decl.Arguments.FirstOrDefault(x => x.Name == "axis").IsNullable = false; // make axis mandatory
                        yield return decl;
                        var clone = decl.Clone<Function>();
                        clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "axis"));
                        clone.Returns[0].Type = "int";
                        yield return clone;
                        yield break;
                    }
                case "sort":
                    decl.Arguments.FirstOrDefault(x => x.Name == "axis").DefaultValue = "-1";
                    break;
                case "percentile":
                case "nanpercentile":
                case "quantile":
                case "nanquantile":
                case "median":
                case "average":
                case "mean":
                case "std":
                case "var":
                case "nanmedian":
                case "nanmean":
                case "nanstd":
                case "nanvar":
                    {
                        decl.Arguments[0].Type = "NDarray";
                        decl.Returns[0].Type = "NDarray<double>";
                        var interpol = decl.Arguments.FirstOrDefault(x => x.Name == "interpolation");
                        if (interpol != null)
                        {
                            interpol.DefaultValue = "\"linear\"";
                            interpol.IsNamedArg = true;
                        }
                        var weights = decl.Arguments.FirstOrDefault(x => x.Name == "weights");
                        if (weights != null)
                        {
                            weights.Type = "NDarray";
                            weights.IsNamedArg = true;
                            weights.IsNullable = true;
                        }
                        //decl.Generics=new string[]{"T"};
                        decl.Arguments.FirstOrDefault(x => x.Name == "axis").IsNullable = false; // make axis mandatory
                        yield return decl;
                        var clone = decl.Clone<Function>();
                        clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "axis"));
                        //clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "out"));
                        clone.Arguments.Remove(clone.Arguments.FirstOrDefault(x => x.Name == "keepdims"));
                        clone.Returns[0].Type = "double";
                        yield return clone;
                        yield break;
                    }
                    break;
                case "histogram":
                case "histogram2d":
                case "histogramdd":
                case "histogram_bin_edges":
                    {
                        decl.Arguments[0].Type = "NDarray";
                        if (decl.Returns.Count > 1)
                        {
                            decl.Returns[1].Type = "NDarray";
                            decl.Generics = null;
                        }
                        var bins = decl.Arguments.FirstOrDefault(x => x.Name == "bins");
                        if (bins != null)
                        {
                            bins.Type = "int";
                            bins.IsNamedArg = true;
                        }
                        var range = decl.Arguments.FirstOrDefault(x => x.Name == "range");
                        if (range != null)
                        {
                            range.Type = "(float, float)";
                            range.IsNamedArg = true;
                            range.IsValueType = true;
                            range.IsNullable = true;
                        }
                        var weights = decl.Arguments.FirstOrDefault(x => x.Name == "weights");
                        if (weights != null)
                        {
                            weights.Type = "NDarray";
                            weights.IsNamedArg = true;
                            weights.IsNullable = true;
                        }
                        var y = decl.Arguments.FirstOrDefault(x => x.Name == "y");
                        if (y != null)
                            y.Type = "NDarray";
                        yield return decl;
                        var clone1 = decl.Clone<Function>();
                        clone1.Arguments.First(x => x.Name == "bins").Type = "NDarray";
                        yield return clone1;
                        var clone2 = decl.Clone<Function>();
                        clone2.Arguments.First(x => x.Name == "bins").Type = "List<string>";
                        yield return clone2;
                        yield break;
                    }
                    break;
                case "choice":
                case "permutation":
                case "binomial":
                    {
                        if (!decl.Arguments[0].Type.StartsWith("NDarray"))
                            decl.Arguments[0].Type = "NDarray";
                        yield return decl;
                        var clone = decl.Clone<Function>();
                        clone.Arguments[0].Type = "int";
                        yield return clone;
                        yield break;
                    }
                    break;
                case "seed":
                case "RandomState":
                    {
                        decl.Arguments[0].Type = "int";
                        decl.Arguments[0].DefaultValue = "null";
                        decl.Arguments[0].IsNullable = true;
                        yield return decl;
                        var clone = decl.Clone<Function>();
                        clone.Arguments[0].Type = "NDarray";
                        yield return clone;
                        yield break;
                    }
                    break;
                case "unique":
                    {
                        decl["ar"].Type = "NDarray";
                        decl.Arguments.ForEach(a =>
                        {
                            if (a.Name != "axis")
                            {
                                a.IsNullable = false;
                                a.DefaultValue = null;
                            }
                        });
                        decl.Returns.RemoveAt(3);
                        decl.Returns.RemoveAt(2);
                        decl.Returns.RemoveAt(1);
                        // without return_index, return_inverse and return_counts we return just an NDarray
                        yield return decl.Clone(f =>
                        {
                            f.Arguments.RemoveAt(3);
                            f.Arguments.RemoveAt(2);
                            f.Arguments.RemoveAt(1);
                        });
                        // if all parameters are specified we return NDarray[]
                        decl.Returns[0].Type = "NDarray[]";
                        yield return decl;
                    }
                    yield break;
                case "linspace":
                    {
                        decl["retstep"].Ignore = true;
                        decl["start"].Type = "NDarray";
                        decl["stop"].Type = "NDarray";
                        decl["num"].DefaultValue="50";
                        decl["endpoint"].DefaultValue="true";
                        //decl["retstep"].MakeMandatory();
                        yield return decl;
                        yield return decl.Clone(f =>
                        {
                            f.Returns.RemoveAt(1);
                            f["start"].Type = "double";
                            f["stop"].Type = "double";
                        });
                    }
                    yield break;
                case "rand":
                case "randn":
                    yield return decl;
                    yield return decl.Clone(f =>
                    {
                        f.Arguments.Clear();
                        f.ManualOverride = false;
                        f.Returns[0].Type = "float";
                    });
                    yield break;
                case "where":
                {
                    decl["condition"].Type = "NDarray";
                    decl["x"].Type = "NDarray";
                    decl["y"].Type = "NDarray";
                    yield return decl;
                    yield return decl.Clone(f =>
                    {
                        f.Arguments.RemoveAt(2);
                        f.Arguments.RemoveAt(1);
                        f.ReturnType = "NDarray[]";
                    });
                }
                    yield break;
                case "transpose":
                    if (decl.Arguments[0].Type == "array_like")
                    {
                        decl.Arguments[0].Type = "NDarray";
                        yield return decl;
                        yield return decl.Clone(f => { f.Arguments[0].Type = "NDarray[]"; });
                    }
                    else
                        yield return decl;
                    yield break;
                //case "meshgrid":
                //    yield return decl;
                //    yield return decl.Clone(f =>
                //    {
                //        f.Arguments[0].Type = "params NDarray[]";
                //        f.Arguments.RemoveAt(1);
                //        f.Arguments.RemoveAt(1);
                //        f.Arguments.RemoveAt(1);
                //    });
                //    yield break;
            }

            // without args we don't need to consider possible overloads
            if (decl.Arguments.Count == 0)
            {
                yield return decl;
                yield break;
            }
            if (decl.Name == "arange")
            {
                foreach (var d in ExpandArange(decl))
                    yield return d;
                yield break;
            }
            if (decl.Name == "bmat")
            {
                decl.Arguments[0].Type = "string";
                yield return decl;
                var clone_decl = decl.Clone<Function>();
                clone_decl.Arguments[0].Type = "T[]";
                clone_decl.Arguments[0].ConvertToSharpType = "NDarray";
                clone_decl.Generics = new[] { "T" };
                clone_decl.Returns[0].Type = "Matrix<T>";
                yield return clone_decl;
                yield break;
            }
            HashSet<Function> overloads = new HashSet<Function>() { decl };
            int i = -1;
            foreach (var arg in decl.Arguments)
            {
                i++;
                // array_like
                if (arg.Type == "array_like")
                {
                    arg.Type = "NDarray";
                    if (arg.Tag != "routines.array-creation.html")
                        continue;
                    switch (decl.Name)
                    {
                        case "insert":
                        case "append":
                        case "resize":
                        case "flip":
                        case "flipud":
                        case "fliplr":
                        case "squeeze":
                        case "expand_dims":
                        case "broadcast_to":
                        case "transpose":
                        case "swapaxes":
                        case "ravel":
                        case "reshape":
                        case "copyto":
                            if (i == 0)
                                continue;
                            break;
                        case "logspace":
                        case "linspace":
                        case "geomspace":
                        case "tile":
                        case "delete":
                        case "repeat":
                        case "roll":
                        case "rot90":
                            continue;
                    }
                    foreach (var overload in overloads.ToArray())
                    {
                        foreach (var type in "T[] T[,]".Split())
                        {
                            var clone = overload.Clone<Function>();
                            clone.Arguments[i].Type = type;
                            clone.Generics = new string[] { "T" };
                            clone.Arguments[i].ConvertToSharpType = "NDarray";
                            if (clone.Returns.FirstOrDefault()?.Type == "NDarray"
                            ) // TODO: this feels like a hack. make it more robust if necessary
                                clone.Returns[0].Type = "NDarray<T>";
                            overloads.Add(clone);
                        }
                    }
                }
                // array_like of bool
                else if (arg.Type == "array_like of bool")
                {
                    foreach (var overload in overloads.ToArray())
                    {
                        overload.Arguments[i].Type = "NDarray";
                        var clone = overload.Clone<Function>();
                        clone.Arguments[i].Type = "bool[]";
                        clone.Arguments[i].ConvertToSharpType = "NDarray";
                        overloads.Add(clone);
                    }
                }
                // number
                else if (arg.Type == "number")
                {
                    foreach (var overload in overloads.ToArray())
                    {
                        overload.Arguments[i].Type = "float";
                        foreach (var type in "byte short int long double".Split())
                        {
                            var clone = overload.Clone<Function>();
                            clone.Arguments[i].Type = type;
                            overloads.Add(clone);
                        }
                    }
                }
            }
            foreach (var overload in overloads)
                yield return overload;
        }

        // special treatment for np.arange which is a "monster"
        private IEnumerable<Function> ExpandArange(Function decl)
        {
            // numpy.arange([start, ]stop, [step, ]dtype=None)
            var dtype = decl.Arguments.Last();
            dtype.IsNullable = true;
            dtype.IsNamedArg = true;
            if (decl.Arguments.Any(a => a.Type == "number"))
            {
                foreach (var type in "byte short int long float double".Split())
                {
                    // start, stop
                    var clone_decl = decl.Clone<Function>();
                    clone_decl.Arguments.ForEach(a =>
                    {
                        if (a.Type == "number")
                            a.Type = type;
                    });
                    clone_decl.Arguments[0].IsNamedArg = false;
                    clone_decl.Arguments[0].IsNullable = false;
                    clone_decl.Arguments[0].DefaultValue = null;
                    yield return clone_decl;
                    // [start=0] <-- remove start from arg list
                    clone_decl = clone_decl.Clone<Function>(); // <---- clone from the clone, as it has the correct type
                    clone_decl.Arguments.RemoveAt(0);
                    yield return clone_decl;
                }
                yield break;
            }
        }

        private string InferDefaultValue(string default_value)
        {
            switch (default_value)
            {
                case "None":
                case "&lt;class 'float'&gt;":
                case "&lt;class 'numpy.float64'&gt;":
                case "&lt;no value&gt;":
                    return null;
                case "True": return "true";
                case "False": return "false";
                case "'C'": return "\"C\"";
            }
            if (default_value.StartsWith("'"))
                return $"\"{default_value.Trim('\'')}\"";
            if (Regex.IsMatch(default_value, @"[+-]?(\d+\.\d+)|(\d+(\.\d+)?e[+-]\d+)"))
                return default_value + "f";
            return default_value;
        }

        private string InferType(string type, Argument arg)
        {
            switch (arg.Name)
            {
                case "shape": return "Shape";
                case "newshape": return "Shape";
                case "new_shape": return "Shape";
                case "dtype": return "Dtype";
                case "order": return "string";
                case "slice": return "Slice";
                case "strides": return "int[]";
                case "arys1, arys2, …":
                    arg.Name = "arys";
                    return "params NDarray[]";
                case "`*args`":
                    arg.Name = "args";
                    break;
                case "a1, a2, …":
                    arg.Name = "arys";
                    break;
                case "norm":
                    if (type == "{None")
                        return "string";
                    break;
                case "axis":
                    if (type == "{int")
                        return "int[]";
                    break;
                case "edge_order": return "int";
            }
            switch (type)
            {
                // Dtype
                case "dtype":
                case "data-type":
                case "dtype or dtype specifier":
                case "data type code":
                case "integer type":
                case "dtype_like":
                    return "Dtype";
                case "matrix": return "Matrix";
                // NDarray
                case "array":
                case "ndarray":
                case "np.ndarray":
                case "2-D array":
                case "1-D array or sequence":
                case "(…":
                case "(…) array_like":
                case "{(…":
                case "{ (…":
                case "(M":
                case "(N":
                case "(k":
                case "{(M":
                case "{(N":
                case "{(1":
                case "(min(M":
                case "list of scalar or array":
                case "scalar or array_like or None":
                case "scalar or array_like":
                case "float or ndarray":
                case "(…) array_like of float":
                case "complex ndarray":
                case "1-D array_like":
                case "scalar or ndarray":
                case "broadcast object":
                case "array_like or scalar":
                case "single item or ndarray":
                case "1-D array-like":
                case "2-D array_like":
                case "array_like of rank N":
                case "{sequence":
                case "1D or 2D array_like":
                case "1-D sequence":
                case "scalar or array_like of shape(M":
                case "array_like of values":
                case "array-like":
                    return "NDarray";
                // NDarray<int>
                case "array of ints searchsorted(1-D array_like":
                case "array of ints":
                case "array of integer type":
                case "array_like of integer type":
                case "int or ndarray of ints":
                case "int or array_like of ints":
                case "int array":
                    return "NDarray<int>";
                // NDarray<float>
                case "array_like of float":
                case "array of dtype float":
                case "float or ndarray of floats":
                case "float or array_like of floats":
                case "float or array_like of float":
                case "sequence of floats":
                    return "NDarray<float>";
                // NDarray<bool>
                case "bool (scalar) or boolean ndarray":
                case "bool or ndarray of bool":
                case "1-D array of bools":
                case "array of bool":
                    return "NDarray<bool>";
                // NDarray<bool>[]
                case "list of bool ndarrays":
                    return "NDarray<bool>[]";
                case "scalar":
                    if (!arg.IsReturnValue)
                        return "ValueType";
                    else
                        return "T";
                    break;
                // string
                case "{ ‘warn’":
                case "file":
                case "str":
                case "string or list":
                case "file or str":
                case "str or function":
                case "file-like object":
                case "str or file":
                case "filename or file handle":
                case "str or regexp":
                case "str or None":
                case "file-like":
                case "{{‘begin’":
                case "str or array_like of bool":
                case "str or unicode":
                case "{str":
                case "str of length 256":
                    return "string";
                // string[]
                case "str or sequence of str":
                case "str or list of str":
                case "array of str or unicode-like":
                case "str or sequence of strs":
                case "sequence of str":
                case "str or list":
                case "str or list/tuple of str":
                case "list of str or array_like":
                case "array_like of datetime64":
                case "array_like of datetime64[D]":
                case "array_like of str or unicode":
                case "array-like of str or unicode":
                    return "string[]";
                // Delegate
                case "callable":
                case "function":
                    return "Delegate";
                case "any": return "object";
                case "iterable object": return "IEnumerable<T>";
                case "dict": return "Hashtable";
                // int[]
                case "int or tuple":
                case "int or sequence":
                case "int or sequence of int":
                case "int or sequence of ints":
                case "sequence of ints":
                case "int or array of ints":
                case "int or tuple of ints":
                case "None or int or tuple of ints":
                case "int or 1-D array":
                case "sequence or int":
                case "array_like of ints":
                case "ints":
                    return "int[]";
                case "boolean": return "bool";
                case "integer":
                    return "int";
                case "int or None":
                    arg.IsNullable = true;
                    return "int";
                case "Standard Python scalar object": return "T";
                case "Arguments (variable number and type)": return "params int[]";
                case "list": return "List<T>";
                // NDarray[]
                case "list of arrays":
                case "array_likes":
                case "sequence of arrays":
                case "sequence of ndarrays":
                case "sequence of array_like":
                case "sequence of 1-D or 2-D arrays.":
                case "list of ndarrays":
                case "tuple":
                case "list of array_like":
                case "tuple of ndarrays":
                case "tuple of ndarray":
                case "1-D sequences":
                case "tuple of arrays.":
                case "tuple of arrays":
                case "array_like (Ni…":
                case "array_like (Nj…)":
                    return "NDarray[]";
                case "slice": return "Slice";
                // double
                case "Number":
                    return "double";
                // object
                case "scalar dtype or object":
                    return "object";
            }
            if (arg.IsReturnValue)
            {
                switch (type)
                {
                    case "array": return "NDarray";
                    case "array_like": return "NDarray";
                }
            }
            if (type.StartsWith("ndarray"))
                return "NDarray";
            if (type.StartsWith("{‘"))
                return "string";
            return type;
        }

        string BaseUrl = "https://docs.scipy.org/doc/numpy-1.16.1/reference/";

        public List<HtmlDoc> LoadDocs(string overview_url)
        {
            var docs = new List<HtmlDoc>();
            var doc = GetHtml(overview_url);
            LoadDocsFromOverviewPage(doc.Doc, docs);
            return docs;
        }

        private void LoadDocsFromOverviewPage(HtmlDocument doc, List<HtmlDoc> docs)
        {
            var nodes = doc.DocumentNode.Descendants("a")
                .Where(x => x.Attributes["class"]?.Value == "reference internal")
                .ToList();
            foreach (var node in nodes)
            {
                var relative_link = node.Attributes["href"].Value;
                if (!relative_link.StartsWith("generated"))
                    continue;
                var uri = relative_link.Split("#").First();
                docs.Add(GetHtml(uri));
            }
        }

        HtmlDoc GetHtml(string relative_url)
        {
            Console.WriteLine("Loading: " + relative_url);
            var doc = new HtmlDoc();
            doc.Filename = relative_url.Replace("/", "_");
            if (File.Exists(doc.Filename))
            {
                doc.Doc = new HtmlDocument();
                doc.Doc.Load(doc.Filename);
                doc.Text = doc.Doc.Text;
                return doc;
            }
            var web = new HtmlWeb();
            doc.Doc = web.Load(BaseUrl + relative_url);
            doc.Text = doc.Doc.Text;
            File.WriteAllText(doc.Filename, doc.Text);
            return doc;
        }

        public void ApiStatistics()
        {
            var doc = GetNumpyReference();
            var li = doc.Doc.DocumentNode.DescendantsOfClass("li", "toctree-l1").FirstOrDefault(x => x.InnerText.StartsWith("NumPy Reference"));
            var stack = new Stack<string>();
            CollectApiFunctions(li, stack);
        }

        HashSet<string> all_api_functions = new HashSet<string>();

        private void CollectApiFunctions(HtmlNode li, Stack<string> stack)
        {
            var a = li.ChildNodes.FirstOrDefault(x => x.Name == "a");
            var ul = li.ChildNodes.FirstOrDefault(x => x.Name == "ul");
            stack.Push(a.InnerText);
            if (a.InnerText.StartsWith("numpy"))
                all_api_functions.Add(a.InnerText);
            if (ul != null)
                foreach (var sub_li in ul.ChildNodes.Where(x => x.Name == "li"))
                    CollectApiFunctions(sub_li, stack);
            stack.Pop();
        }

        public HtmlDoc GetNumpyReference()
        {
            var url = "https://docs.scipy.org/doc/numpy/contents.html";
            Console.WriteLine("Loading: " + url);
            var doc = new HtmlDoc();
            doc.Filename = "contents.html";
            if (File.Exists(doc.Filename))
            {
                doc.Doc = new HtmlDocument();
                doc.Doc.Load(doc.Filename);
                doc.Text = doc.Doc.Text;
                return doc;
            }
            var web = new HtmlWeb();
            doc.Doc = web.Load(url);
            doc.Text = doc.Doc.Text;
            File.WriteAllText(doc.Filename, doc.Text);
            return doc;
        }
    }
}

