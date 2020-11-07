using CodeMinion.Core.Helpers;

namespace CodeMinion.ApiGenerator.NumPy
{
    public static class SpecialGenerators
    {

        public static void InitNumpyGenerator(CodeWriter s)
        {
            s.Out("#if PYTHON_INCLUDED");
            s.Out("Installer.InstallWheel(typeof(np).Assembly, \"numpy-1.16.3-cp37-cp37m-win_amd64.whl\").Wait();");
            s.Out("#endif");
        }

        public static void ArrayToNDarrayConversion(CodeWriter s)
        {
            s.Out("case Array a:");
            s.Out("if (typeof(T)==typeof(NDarray)) return (T)(object)ConvertArrayToNDarray(a);");
            s.Out("break;");
        }

        public static void ConvertArrayToNDarray(CodeWriter s)
        {
            s.Out("private static NDarray ConvertArrayToNDarray(Array a)", () =>
            {
                s.Out("switch(a)", () =>
                {
                    s.Out("case bool[] arr: return np.array(arr);");
                    s.Out("case int[] arr: return np.array(arr);");
                    s.Out("case float[] arr: return np.array(arr);");
                    s.Out("case double[] arr: return np.array(arr);");
                    s.Out("case int[,] arr: return np.array(arr.Cast<int>().ToArray()).reshape(arr.GetLength(0), arr.GetLength(1));");
                    s.Out("case float[,] arr: return np.array(arr.Cast<float>().ToArray()).reshape(arr.GetLength(0), arr.GetLength(1));");
                    s.Out("case double[,] arr: return np.array(arr.Cast<double>().ToArray()).reshape(arr.GetLength(0), arr.GetLength(1));");
                    s.Out("case bool[,] arr: return np.array(arr.Cast<bool>().ToArray()).reshape(arr.GetLength(0), arr.GetLength(1));");
                    s.Out("default: throw new NotImplementedException($\"Type {a.GetType()} not supported yet in ConvertArrayToNDarray.\");");
                });
            });
        }

        public static void ConvertDict(CodeWriter s)
        {
            s.Out("private static PyDict ToDict(Dictionary<string, NDarray> d)", () =>
            {
                s.Out("var dict = new PyDict();");
                s.Out("foreach (var pair in d)");
                s.Out("    dict[new PyString(pair.Key)] = pair.Value.self;");
                s.Out("return dict;");
            });
        }
    }
}
