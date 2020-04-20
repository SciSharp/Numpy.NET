using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeMinion.Core.Attributes;
using CodeMinion.Core.Helpers;
using CodeMinion.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SliceAndDice;

namespace CodeMinion.Core
{
    public class CodeGenerator
    {
        public CodeGenerator()
        {
            LoadTemplates();
        }

        public string CopyrightNotice { get; set; }
        public List<StaticApi> StaticApis { get; set; } = new List<StaticApi>();
        public List<DynamicApi> DynamicApis { get; set; } = new List<DynamicApi>();
        public List<ApiClass> ApiClasses { get; set; } = new List<ApiClass>();
        public bool PrintModelJson { get; set; } = false;
        public string NameSpace { get; set; } = "Numpy";
        public string StaticModuleName { get; set; } = "np";
        public string PythonModuleName { get; set; } = "numpy";
        public List<Action<CodeWriter>> InitializationGenerators { get; set; } = new List<Action<CodeWriter>>();

        public bool UsePythonIncluded { get; set; } = true;
        public HashSet<string> Usings { get; set; } = new HashSet<string>()
        {
            @"using System;",
            @"using System.Collections;",
            @"using System.Collections.Generic;",
            @"using System.IO;",
            @"using System.Linq;",
            @"using System.Runtime.InteropServices;",
            @"using System.Text;",
            @"using Python.Runtime;",
        };
        public string StaticApiFilesPath { get; set; }
        public string DynamicApiFilesPath { get; set; }
        public string ModelsPath { get; set; }
        public string TestFilesPath { get; set; }
        public List<TestFile> TestFiles { get; set; } = new List<TestFile>();
        protected Dictionary<string, FunctionBodyTemplate> _templates;
        protected virtual void LoadTemplates()
        {
            _templates = Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.GetCustomAttribute<TemplateAttribute>() != null)
                .Select(x => (FunctionBodyTemplate)Activator.CreateInstance(x)).ToDictionary(x =>
                    x.GetType().GetCustomAttribute<TemplateAttribute>().ApiFunction);
        }

        // generate an entire API function declaration
        protected virtual void GenerateApiFunction(Declaration decl, CodeWriter s, bool prefix = false, bool @static=false)
        {
            if (decl.ManualOverride)
                return;
            //if (decl.Name=="cholesky")
            //    Debugger.Break();
            decl.Sanitize();
            if (decl.CommentOut)
                s.Out("/*");
            var class_names = (decl.GeneratedClassName ?? decl.ClassName ?? "no_name").Split('.');
            int levels = class_names.Length - 1;
            if (levels > 0)
            {
                foreach (var name in class_names.Skip(1))
                {
                    s.Out($"public static partial class {EscapeName(name)} {{");
                    s.Indent();
                }
            }
            GenerateDocString(decl, s);
            var retval = GenerateReturnType(decl);
            switch (decl)
            {
                case Function func:
                    var arguments = GenerateArguments(func);
                    //var passed_args = GeneratePassedArgs(func);
                    var generics = func.Generics == null ? "" : $"<{string.Join(",", func.Generics)}>";
                    var prefix_str = "";
                    if (prefix && levels > 0)
                        prefix_str = string.Join("_", class_names.Skip(1)) + "_";
                    s.Out($"public {(@static ? "static ":"")}{retval} {EscapeName(prefix_str + decl.Name)}{func.SharpOnlyPostfix}{generics}({arguments})");
                    s.Block(() =>
                    {
                        GenerateFunctionBody(func, s, prefix_str);
                    });
                    break;
                case Property prop:                    
                    s.Out($"public  {(@static ? "static " : "")}{prop.Type} {EscapeName(prop.Name)}");
                    s.Block(() =>
                    {
                        s.Out("get", () =>
                        {
                            GeneratePropertyGetter(prop, s);
                        });
                        s.Out("set", () =>
                        {
                            GeneratePropertySetter(prop, s);
                        });
                    });
                    break;
            }
            if (levels > 0)
            {
                foreach (var name in class_names.Skip(1))
                {
                    s.Outdent();
                    s.Out("}");
                }
            }
            if (decl.CommentOut)
                s.Out("*/");
            if (PrintModelJson)
            {
                s.Out("// the declaration model:");
                s.Out("/*");
                s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                s.Out("*/");
            }
            s.Break();
        }

        //protected virtual void GenerateStaticApiRedirection(StaticApi api, Declaration decl, CodeWriter s)
        //{
        //    if (decl.DebuggerBreak)
        //        Debugger.Break();
        //    decl.Sanitize();
        //    if (decl.CommentOut)
        //        s.Out("/*");
        //    var class_names = (decl.GeneratedClassName ?? decl.ClassName ?? "no_name").Split('.');
        //    int levels = class_names.Length - 1;
        //    if (levels > 0)
        //    {
        //        foreach (var name in class_names.Skip(1))
        //        {
        //            s.Out($"public static partial class {EscapeName(name)} {{");
        //            s.Indent();
        //        }
        //    }
        //    GenerateDocString(decl, s);
        //    var retval = GenerateReturnType(decl);
        //    switch (decl)
        //    {
        //        case Function func:
        //            var arguments = GenerateArguments(func);
        //            var passed_args = GeneratePassedArgs(func);
        //            var generics = func.Generics == null ? "" : $"<{string.Join(",", func.Generics)}>";
        //            s.Out($"public static {retval} {EscapeName(decl.Name)}{func.SharpOnlyPostfix}{generics}({arguments})");
        //            var prefix = "";
        //            if (levels > 0)
        //                prefix = string.Join("_", class_names.Skip(1)) + "_";
        //            s.Indent(() => s.Out(
        //                $"=> {api.ImplName}.Instance.{EscapeName(prefix + decl.Name)}({passed_args});"));
        //            break;
        //        case Property prop:
        //            s.Out($"public static {retval} {EscapeName(decl.Name)}");
        //            s.Indent(() => s.Out(
        //                $"=> {api.ImplName}.Instance.{EscapeName(decl.Name)};"));
        //            break;
        //    }
        //    if (levels > 0)
        //    {
        //        foreach (var name in class_names.Skip(1))
        //        {
        //            s.Outdent();
        //            s.Out("}");
        //        }
        //    }
        //    if (decl.CommentOut)
        //        s.Out("*/");
        //    s.Break();
        //}

        // generate the argument list between the parentheses of a generated API function
        protected virtual string GenerateArguments(Function decl)
        {
            var s = new StringBuilder();
            int i = 0;
            foreach (var arg in decl.Arguments)
            {
                if (arg.Type == "string")
                    arg.IsValueType = false;
                // TODO modifier (if any)
                // parameter type
                CheckArgument(arg);
                s.Append(arg.Type);
                if (arg.IsNullable && arg.IsValueType)
                    s.Append("?");
                s.Append(" ");
                // parameter name
                s.Append(EscapeName(arg.Name));
                if (!string.IsNullOrWhiteSpace(arg.DefaultValue))
                    s.Append($" = {MapDefaultValue(arg)}");
                else if (arg.IsNullable)
                    s.Append($" = null");
                i++;
                if (i < decl.Arguments.Count)
                    s.Append(", ");
            }
            return s.ToString();
        }

        private string GeneratePassedArgs(Function decl)
        {
            var s = new StringBuilder();
            int i = 0;
            foreach (var arg in decl.Arguments)
            {
                // TODO modifier (if any)
                var argname = EscapeName(arg.Name);
                if (arg.IsNamedArg)
                    s.Append($"{argname}:");
                s.Append(argname);
                i++;
                if (i < decl.Arguments.Count)
                    s.Append(", ");
            }
            return s.ToString();
        }

        // maps None to null, etc
        protected virtual string MapDefaultValue(Argument arg)
        {
            switch (arg.DefaultValue)
            {
                case "None": return "null";
                case "True": return "true";
                case "False": return "false";
            }
            return arg.DefaultValue;
        }

        // list of c# keywords that are not allowed as variable names or parameter names
        protected readonly HashSet<string> _disallowed_names = new HashSet<string>()
        {
            "abstract", "as", "base", "bool", "break",
            "byte", "case", "catch", "char", "checked",
            "class",   "const",   "continue", "decimal", "default",
            "delegate",    "do", "double",  "else", "enum",
            "event",   "explicit", "extern", "false", "finally",
            "fixed", "float",   "for", "foreach", "goto",
            "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace",
            "new", "null", "object", "operator",    "out",
            "override", "params",  "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed",
            "short", "sizeof", "stackalloc", "static", "string",
            "struct",  "switch", "this",    "throw", "true",
            "try", "typeof", "uint",    "ulong",   "unchecked",
            "unsafe", "ushort", "using", "var", "virtual",
            "void", "volatile", "while",
            "add", "alias",   "async", "await", "dynamic",
            "get", "global",  "nameof",  "partial", "remove",
            "set", "value", "when",    "where", "yield",
            "ascending", "by",  "descending", "equals",  "from",
            "group",   "in", "into", "join",    "let",
            "on",  "orderby", "select",  "where"
        };

        // escape a varibale name if it violates C# syntax
        protected virtual string EscapeName(string name)
        {
            if (_disallowed_names.Contains(name))
                return "@" + name;
            return name;
        }

        // generates the return type declaration of a generated API function declaration
        protected virtual string GenerateReturnType(Declaration decl)
        {
            if (decl.Returns == null || decl.Returns.Count == 0)
                return "void";
            else if (decl.Returns.Count == 1)
            {
                CheckArgument(decl.Returns[0]);
                return decl.Returns[0].Type;
            }
            else
            {
                return "(" + string.Join(", ", decl.Returns.Select(x => x.Type).ToArray()) + ")";
            }
        }

        // argument type consistency checks
        protected virtual void CheckArgument(Argument arg)
        {
            switch (arg.Type)
            {
                // basic types
                case "bool":
                case "int":
                case "long":
                case "double":
                case "float":
                    arg.IsValueType = true;
                    break;
                case "object":
                case "string":
                    arg.IsValueType = false;
                    break;
                    // sequence types
            }
        }

        /// <summary>
        /// Generate the xml doc string from the description(s)
        /// </summary>
        /// <param name="decl"></param>
        /// <param name="s"></param>
        protected virtual void GenerateDocString(Declaration decl, CodeWriter s)
        {
            if (string.IsNullOrWhiteSpace(decl.Description))
                return;
            s.Out("/// <summary>");
            var docstring = ProcessDocString(decl.Description);
            foreach (var line in Regex.Split(docstring, @"\r?\n"))
                s.Out("///\t" + line);
            s.Out("/// </summary>");
            if (decl is Function)
            {
                var func = decl as Function;
                foreach (var arg in func.Arguments)
                {
                    if (string.IsNullOrWhiteSpace(arg.Description))
                        continue;
                    s.Out($"/// <param name=\"{arg.Name}\">"); // note: docstring doesn't want parameters escaped with "@"
                    docstring = ProcessDocString(arg.Description);
                    foreach (var line in Regex.Split(docstring, @"\r?\n"))
                        s.Out("///\t" + line.TrimStart());
                    s.Out("/// </param>");
                }
            }
            if (decl.Returns.All(rv => string.IsNullOrWhiteSpace(rv.Description)))
                return;
            s.Out("/// <returns>");
            if (decl.Returns.Count == 1)
                foreach (var line in Regex.Split(ProcessDocString(decl.Returns[0].Description), @"\r?\n"))
                    s.Out("///\t" + line);
            else
            {
                s.Out("/// A tuple of:");
                foreach (var rv in decl.Returns)
                {
                    s.Out("/// " + rv.Name);
                    foreach (var line in Regex.Split(rv.Description, @"\r?\n"))
                        s.Out("///\t" + line);
                }
            }
            s.Out("/// </returns>");
        }

        protected virtual void GenerateDocString(ApiClass decl, CodeWriter s)
        {
            if (string.IsNullOrWhiteSpace(decl.DocString))
                return;
            var docstring= ProcessDocString(decl.DocString);
            s.Out("/// <summary>");
            foreach (var line in Regex.Split(docstring, @"\r?\n"))
                s.Out("///\t" + line);
            s.Out("/// </summary>");
        }

        protected string ProcessDocString(string docstring)
        {
            if (string.IsNullOrWhiteSpace(docstring))
                return docstring;
            // insert linebreak after each sentence
            var text= Regex.Replace(docstring, @"(?:(?<=\w|\)|\])(?<!\d)|(?<=\s\d))\.(?=(?:\s|$))", ".<br></br>\n", RegexOptions.Multiline);
            text = Regex.Replace(text, @"(\r?\n){2,}", "\n\n");
            text = Regex.Replace(text.Trim(), "<br></br>$", "");
            return text;
        }

        // generates only the body of the API function declaration
        protected virtual void GenerateFunctionBody(Function func, CodeWriter s, string prefix = "")
        {
            s.Out("//auto-generated code, do not change");
            if (_templates.ContainsKey(func.Name))
            {
                // use generator template instead
                _templates[func.Name].GenerateBody(func, s);
                return;
            }

            //if (func.Name=="norm")
            //    Debugger.Break();
            var class_names = (func.ClassName ?? "no_name").Split('.');
            int levels = class_names.Length - 1;
            if (levels < 1)
            {
                s.Out("var __self__=self;");
            }
            else
            {
                var last = "self";
                foreach (var name in class_names.Skip(1))
                {
                    s.Out($"var {EscapeName(name)} = {last}.GetAttr(\"{name}\");");
                    last = name;
                }
                s.Out($"var __self__={last};");
            }
            if (func.Arguments.Any())
            {
                // first generate the positional args
                s.Out($"var pyargs=ToTuple(new object[]");
                s.Block(() =>
                {
                    foreach (var arg in func.Arguments.Where(a => a.IsNamedArg == false))
                    {
                        var name = EscapeName(arg.Name);
                        if (!string.IsNullOrWhiteSpace(arg.ConvertToSharpType))
                            s.Out($"SharpToSharp<{arg.ConvertToSharpType}>({name}),");
                        else
                            s.Out($"{name},");
                    }
                }, "{", "});");
                // then generate the named args
                s.Out($"var kwargs=new PyDict();");
                foreach (var arg in func.Arguments.Where(a => a.IsNamedArg == true))
                {
                    var name = EscapeName(arg.Name);
                    if (!string.IsNullOrWhiteSpace(arg.DefaultValue))
                        s.Out($"if ({name}!={arg.DefaultValue}) kwargs[\"{arg.Name}\"]=ToPython({name});");
                    else if (string.IsNullOrWhiteSpace(arg.DefaultValue))
                    {
                        if (string.IsNullOrWhiteSpace(arg.DefaultIfNull) || arg.DefaultValue == "null")
                            s.Out($"if ({name}!=null) kwargs[\"{arg.Name}\"]=ToPython({name});");
                        else
                            s.Out($"kwargs[\"{arg.Name}\"]=ToPython({name} ?? {arg.DefaultIfNull});");
                    }
                    else //if (arg.IsNullable)
                        s.Out($"if ({name}!=null) kwargs[\"{arg.Name}\"]=ToPython({name});");
                        
                }
                // then call the function
                s.Out($"dynamic py = __self__.InvokeMethod(\"{func.Name}\", pyargs, kwargs);");
            }
            else
            {
                // call function with no arguments
                s.Out($"dynamic py = __self__.InvokeMethod(\"{func.Name}\");");
            }

            if (func.IsConstructor)
            {
                s.Out("self=py as PyObject;");
                return;
            }
            // return the return value if any
            if (func.Returns.Count == 0)
                return;
            if (func.Returns.Count == 1)
                s.Out($"return ToCsharp<{func.Returns[0].Type}>(py);");
            else
            {
                var returns = func.Returns.Select((x, i) => $"ToCsharp<{x.Type}>(py[{i}])").ToArray();
                s.Out($"return ({string.Join(", ", returns)});");
            }
        }

        //private void GenerateForwardingBody(Function member_func, CodeWriter s, string prefix = "")
        //{
        //    var func = member_func.Clone<Function>();
        //    // inserting this at position 0 since this is a forwarding of a member function to a static implementation
        //    func.Arguments.Insert(0, new Argument() { Name = "this", Type = "irrelevant" });
        //    var passed_args = GeneratePassedArgs(func);
        //    s.Out("var @this=this;");
        //    var return_keyword = member_func.Returns.Count > 0 ? "return " : "";
        //    s.Out($"{return_keyword}{func.ForwardToStaticImpl}.{EscapeName(prefix + func.Name)}({passed_args});");
        //}

        private void GeneratePropertyGetter(Property prop, CodeWriter s)
        {
            s.Out($"dynamic py = self.GetAttr(\"{prop.Name}\");");
            if (prop.Returns.Count==1 && prop.Type!=null)
                s.Out($"return ToCsharp<{prop.Type}>(py);");
            else
            {
                throw new NotImplementedException("TODO: Property returns a tuple");
            }
        }

        private void GeneratePropertySetter(Property prop, CodeWriter s)
        {
            s.Out($"self.SetAttr(\"{prop.Name}\", ToPython(value));");
        }

        public virtual void GenerateStaticApi(StaticApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.Out($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public static partial class {api.StaticName}");
                s.Block(() =>
                {
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            if (decl.Ignore)
                                continue;
                            GenerateApiFunction(decl, s, @static:true);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");

                        }
                    }
                    s.Break();
                });
            });
        }

        private void GenerateUsings(CodeWriter s)
        {
            foreach (var @using in Usings)
            {
                s.AppendLine(@using);
            }
            if (UsePythonIncluded)
                s.Out(@"using Python.Included;");
            s.AppendLine();
        }

        public virtual void GenerateApiImpl(StaticApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public partial class {api.ImplName}");
                s.Block(() =>
                {
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            if (decl.ManualOverride || decl.Ignore)
                                continue;
                            GenerateApiFunction(decl, s, prefix: true);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");

                        }
                    }
                });
            });
        }

        public virtual void GenerateDynamicApi(DynamicApi api, CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public partial class {api.ClassName}");
                s.Block(() =>
                {
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            if (decl.ManualOverride || decl.Ignore)
                                continue;
                            GenerateApiFunction(decl, s);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");
                        }
                    }
                });
            });
        }

        public virtual void GenerateClass(ApiClass api, CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                var names = new ArraySlice<string>(api.ClassName.Split('.'));
                var static_classes = names.GetSlice(new Slice(0, names.Length - 1));
                var class_name = names.Last();
                int levels = names.Length - 1;
                if (levels > 0)
                {
                    foreach (var name in static_classes)
                    {
                        s.Out($"public static partial class {EscapeName(name)} {{");
                        s.Indent();
                    }
                }
                GenerateDocString(api, s);
                s.Out($"public partial class {EscapeName(class_name)} : {EscapeName(api.BaseClass)}");
                s.Block(() =>
                {
                    s.Out($"// auto-generated class");
                    s.Break();
                    s.Out($"public {EscapeName(class_name)}(PyObject pyobj) : base(pyobj) {{ }}");
                    s.Break();
                    s.Out($"public {EscapeName(class_name)}({EscapeName(api.BaseClass)} other) : base(other.PyObject as PyObject) {{ }}");
                    s.Break();

                    // additional constructors
                    foreach (var func in api.Constructors)
                    {
                        try
                        {
                            if (func.ManualOverride || func.Ignore)
                                continue;
                            func.Sanitize();
                            func.IsConstructor = true;
                            func.ClassName = string.Join(".", static_classes);
                            func.Name = class_name;
                            var arguments = GenerateArguments(func);
                            //var passed_args = GeneratePassedArgs(func);
                            s.Out($"public {EscapeName(class_name)}({arguments})");
                            s.Block(() =>
                            {
                                GenerateFunctionBody(func, s);
                            });
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating constructor");
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(func).ToString(Formatting.Indented));
                            s.Out("*/");
                        }
                    }
                    // functions
                    s.Break();
                    foreach (var decl in api.Declarations)
                    {
                        try
                        {
                            if (decl.ManualOverride || decl.Ignore)
                                continue;
                            GenerateApiFunction(decl, s);
                        }
                        catch (Exception e)
                        {
                            s.Out("// Error generating delaration: " + decl.Name);
                            s.Out("// Message: " + e.Message);
                            s.Out("/*");
                            s.Out(e.StackTrace);
                            s.Out("----------------------------");
                            s.Out("Declaration JSON:");
                            s.Out(JObject.FromObject(decl).ToString(Formatting.Indented));
                            s.Out("*/");
                        }
                    }
                });
                if (levels > 0)
                {
                    foreach (var name in static_classes)
                    {
                        s.Outdent();
                        s.Out("}");
                    }
                }
                //if (decl.CommentOut)
                //    s.Out("*/");
                s.Break();
            });
        }
        protected void WriteFile(string path, Action<CodeWriter> generate_action)
        {
            var s = new CodeWriter();
            try
            {
                if (!string.IsNullOrWhiteSpace(CopyrightNotice))
                    s.Out("// " + CopyrightNotice);
                s.Out("// Code generated by CodeMinion: https://github.com/SciSharp/CodeMinion");
                s.Break();
                generate_action(s);
            }
            catch (Exception e)
            {
                s.AppendLine("/*");
                s.AppendLine("\r\n --------------- generator exception ---------------------");
                s.AppendLine(e.Message);
                s.AppendLine(e.StackTrace);
                s.AppendLine("*/");
            }

            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, s.ToString());
        }

        public void Generate()
        {
            // generate all static apis that have been configured
            var generated_implementations = new HashSet<string>();
            var conv_file = Path.Combine(StaticApiFilesPath, $"{StaticModuleName}.module.gen.cs");
            WriteFile(conv_file, s => { GenerateStaticModuleHead( s); });

            foreach (var api in StaticApis)
            {
                var outpath = api.OutputPath ?? StaticApiFilesPath;
                if (string.IsNullOrWhiteSpace(outpath))
                    throw new InvalidDataException("either set generators StaticApiFilesPath or static_api's OutputPath");
                // generate static apis
                //if (!generated_implementations.Contains(api.ImplName))
                //{
                //}

                generated_implementations.Add(api.ImplName);
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(outpath, $"{api.StaticName + partial}.gen.cs");
                //var impl_file = Path.Combine(outpath, $"{api.ImplName + partial}.gen.cs");
                WriteFile(api_file, s => { GenerateStaticApi(api, s); });
                //WriteFile(impl_file, s => { GenerateApiImpl(api, s); });
            }
            // PythonObject functions:
            var pyobj_file = Path.Combine(ModelsPath ?? DynamicApiFilesPath, $"PythonObject.gen.cs");
            WriteFile(pyobj_file, s => { GeneratePythonObjectConversions(s); });
            // Dynamic APIs
            foreach (var api in DynamicApis)
            {
                var outpath = api.OutputPath ?? DynamicApiFilesPath;
                // generate dynamic apis
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(outpath, $"{api.ClassName + partial}.gen.cs");
                WriteFile(api_file, s => { GenerateDynamicApi(api, s); });
            }
            // Classes
            foreach (var api in ApiClasses)
            {
                if (api.Ignore)
                    continue;
                var outpath = api.OutputPath ?? DynamicApiFilesPath;
                if (api.SubDir != null)
                    outpath = Path.Combine(outpath, api.SubDir);
                // generate dynamic apis
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(outpath, $"{api.ClassName + partial}.gen.cs");
                WriteFile(api_file, s => { GenerateClass(api, s); });
            }
            // generate missing tests
            GenerateAllTests();
        }

        /// <summary>
        /// Generate tests that probably have to be manually corrected, syntax wise. For that reason
        /// We will not overwrite any existing files!
        /// </summary>
        public void GenerateAllTests()
        {
            foreach (var file in TestFiles)
            {
                if(file.TestCases.Count==0)
                    continue;
                var path = TestFilesPath;
                if (!string.IsNullOrWhiteSpace(file.SubDir))
                    path = Path.Combine(path, file.SubDir);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var test_file = Path.Combine(path, $"{file.Name}.tests.cs");
                if (File.Exists(test_file))
                    continue; // never overwrite already generated files!
                WriteFile(test_file, s => { GenerateTests(file, s); });
            }
        }

        private void GenerateTests(TestFile file, CodeWriter s)
        {
            GenerateUsings(s);
            s.Out("using Microsoft.VisualStudio.TestTools.UnitTesting;");
            s.Out("using Assert = NUnit.Framework.Assert;");
            s.Break();
            s.Out($"namespace {NameSpace}.UnitTest", () =>
            {
                s.Out("[TestClass]");
                s.Out($"public class {file.Name}Test : BaseTestCase", () =>
                {
                    foreach (var testcase in file.TestCases)
                        GenerateTestCase(testcase, s);
                });
            });
        }

        private void GenerateTestCase(TestCase testcase, CodeWriter s)
        {
            s.Break();
            s.Out("[TestMethod]");
            s.Out($"public void {testcase.Name}()", () =>
            {
                var given_var = false;
                var expected_var = false;
                foreach (var part in testcase.TestParts)
                {
                    switch (part)
                    {
                        case Comment c:
                            foreach (var ln in Regex.Split(c.Text, @"\r?\n"))
                                s.Out(@"// " + ln);
                            s.Break();
                            break;
                        case ExampleCode example:
                            var lines = Regex.Split(example.Text, @"\r?\n");
                            foreach (var ln in lines)
                                s.Out(@"// " + ln);
                            s.Break();
                            s.Out("#if TODO");
                            foreach (var line in example.Lines)
                            {
                                if (line.Type == "comment")
                                {
                                    s.Out(line.Text[0]);
                                    continue;
                                }
                                if (line.Type == "cmd")
                                {
                                    var cmd = line.Text[0];
                                    s.Out($"{(given_var ? "" : "var")} given= " + cmd + ";");
                                    given_var = true;
                                    continue;
                                }

                                if (line.Type == "output")
                                {
                                    s.Out($"{(expected_var ? "" : "var")} expected=");
                                    expected_var = true;
                                    s.Indent(() =>
                                    {
                                        int i = 0;
                                        foreach (var output in line.Text)
                                        {
                                            var newline = i < line.Text.Count - 1 ? @"\n" : "";
                                            var delimiter = i < line.Text.Count - 1 ? @" +" : ";";
                                            s.Out($"\"{output}{newline}\"{delimiter}");
                                            i++;
                                        }
                                    });
                                }
                                s.Out("Assert.AreEqual(expected, given.repr);");
                            }
                            s.Out("#endif");
                            break;
                    }
                }
            });
            s.Break();
        }

        private void GenerateStaticModuleHead(CodeWriter s)
        {
            GenerateUsings(s);
            s.AppendLine($"namespace {NameSpace}");
            s.Block(() =>
            {
                s.Out($"public static partial class {StaticModuleName}", () =>
                {
                    s.Break();
                    s.Out("public static PyObject self => _lazy_self.Value;");
                    s.Break();
                    s.Out($"private static Lazy<PyObject> _lazy_self = new Lazy<PyObject>(() => ", () =>
                    {
                        s.Out("try", () =>
                            {
                                s.Out("return InstallAndImport();");
                            });
                        s.Out("catch (Exception)", () =>
                        {
                            s.Out("// retry to fix the installation by forcing a repair, if Python.Included is used.");
                            s.Out("return InstallAndImport(force: true);");
                        });
                        //s.Out("return instance;");
                    });
                    s.Out(");");
                    s.Break();
                    s.Out("private static PyObject InstallAndImport(bool force = false)", () =>
                    {
                        if (UsePythonIncluded)
                        {
                            s.Out("Installer.SetupPython(force).Wait();");
                        }
                        foreach (var generator in InitializationGenerators)
                            generator(s);
                        s.Out("PythonEngine.Initialize();");
                        s.Out($"var mod = Py.Import(\"{PythonModuleName}\");");
                        s.Out("return mod;");
                    });
                    s.Break();
                    s.Out("public static dynamic dynamic_self => self;");
                    s.Out("private static bool IsInitialized => self != null;");
                    s.Break();
                    //s.Out($"private {api.ImplName}() {{ }}");
                    s.Break();
                    s.Out("public static void Dispose()", () =>
                    {
                        s.Out("self?.Dispose();");
                    });
                    s.Break();
                    GenToTuple(s, @static:true);
                    GenToPython(s, @static: true);
                    GenToCsharp(s, @static: true);
                    GenSharpToSharp(s, @static: true);
                    GenSpecialConversions(s, @static: true);
                });
            });
        }

        private void GeneratePythonObjectConversions(CodeWriter s)
        {
            GenerateUsings(s);
            s.Out($"namespace {NameSpace}", () =>
            {
                s.Out($"public partial class PythonObject", () =>
                {
                    s.Break();
                    GenToTuple(s);
                    GenToPython(s);
                    GenToCsharp(s);
                    GenSharpToSharp(s);
                    GenSpecialConversions(s);
                });
            });
        }

        private void GenToTuple(CodeWriter s, bool @static=false)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out($"{(@static?"private static":"public")} PyTuple ToTuple(Array input)", () =>
            {
                s.Out("var array = new PyObject[input.Length];");
                s.Out("for (int i = 0; i < input.Length; i++)", () =>
                {
                    s.Out("array[i]=ToPython(input.GetValue(i));");
                });
                s.Out("return new PyTuple(array);");
            });
        }

        public HashSet<string> ToCsharpConversions { get; set; } = new HashSet<string>();

        private void GenToCsharp(CodeWriter s, bool @static = false)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out($"{(@static?"private static":"public")} T ToCsharp<T>(dynamic pyobj)", () =>
            {
                s.Out("switch (typeof(T).Name)", () =>
                {
                    s.Out("// types from 'ToCsharpConversions'");
                    foreach (var @case in ToCsharpConversions)
                    {
                        s.Out(@case);
                    }
                    s.Out("default:");
                    s.Out("try", () => s.Out("return pyobj.As<T>();"));
                    s.Out("catch (Exception e)", () =>
                    {
                        s.Out(
                            "throw new NotImplementedException($\"conversion from {typeof(T).Name} to {pyobj.__class__} not implemented\", e);");
                        s.Out("return default(T);");
                    });
                });
            });
        }

        public HashSet<string> ToPythonConversions { get; set; } = new HashSet<string>();

        private void GenToPython(CodeWriter s, bool @static=false)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out($"{(@static?"private static":"public")} PyObject ToPython(object obj)", () =>
            {
                s.Out("if (obj == null) return Runtime.GetPyNone();");
                s.Out("switch (obj)", () =>
                {
                    s.Out("// basic types");
                    s.Out("case int o: return new PyInt(o);");
                    s.Out("case long o: return new PyLong(o);");
                    s.Out("case float o: return new PyFloat(o);");
                    s.Out("case double o: return new PyFloat(o);");
                    s.Out("case string o: return new PyString(o);");
                    s.Out("case bool o: return ConverterExtension.ToPython(o);");
                    s.Out("case PyObject o: return o;");
                    s.Out("// sequence types");
                    s.Out("case Array o: return ToTuple(o);");
                    s.Out("// special types from 'ToPythonConversions'");
                    foreach (var @case in ToPythonConversions)
                    {
                        s.Out(@case);
                    }
                    s.Out("default: throw new NotImplementedException($\"Type is not yet supported: { obj.GetType().Name}. Add it to 'ToPythonConversions'\");");
                });
            });
        }

        public List<Action<CodeWriter>> SharpToSharpConversions { get; set; } = new List<Action<CodeWriter>>();

        private void GenSharpToSharp(CodeWriter s, bool @static=false)
        {
            s.Break();
            s.Out("//auto-generated");
            s.Out($"{(@static ? "private static" : "public")} T SharpToSharp<T>(object obj)", () =>
            {
                s.Out("if (obj == null) return default(T);");
                s.Out("switch (obj)", () =>
                {
                    s.Out("// from 'SharpToSharpConversions':");
                    foreach (var gen in SharpToSharpConversions)
                    {
                        gen(s);
                    }
                });
                s.Out("throw new NotImplementedException($\"Type is not yet supported: { obj.GetType().Name}. Add it to 'SharpToSharpConversions'\");");
            });
        }


        public List<Action<CodeWriter>> SpecialConversionGenerators { get; set; } = new List<Action<CodeWriter>>();

        private void GenSpecialConversions(CodeWriter s, bool @static=false)
        {
            foreach (var generator in SpecialConversionGenerators)
            {
                s.Break();
                s.Out("//auto-generated: SpecialConversions");
                generator(s);
            }
        }

        public void GenerateIntermediateJson()
        {
            foreach (var api in StaticApis)
            {
                var outpath = api.OutputPath ?? StaticApiFilesPath;
                if (string.IsNullOrWhiteSpace(outpath))
                    throw new InvalidDataException("either set generators StaticApiFilesPath or static_api's OutputPath");
                // generate static apis
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(outpath, $"{api.StaticName + partial}.gen.json");
                WriteFile(api_file, s => s.AppendLine(JObject.FromObject(api).ToString(Formatting.Indented)));
            }
            foreach (var api in DynamicApis)
            {
                var outpath = api.OutputPath ?? DynamicApiFilesPath;
                // generate dynamic apis
                var partial = (api.PartialName == null ? "" : "." + api.PartialName);
                var api_file = Path.Combine(outpath, $"{api.ClassName + partial}.gen.json");
                WriteFile(api_file, s => s.AppendLine(JObject.FromObject(api).ToString(Formatting.Indented)));
            }
        }
    }
}
