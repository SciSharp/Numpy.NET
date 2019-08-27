using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Python.Runtime;

namespace Numpy
{

    public partial class Dtype : PythonObject
    {
        public Dtype(PyObject pyobj) : base(pyobj)
        {
        }

        public Dtype(Dtype t) : base((PyObject)t.PyObject)
        {
        }

    }


    public static class DtypeExtensions
    {
        public static Dtype GetDtype<T>(this T obj)
        {
            return ToDtype(typeof(T));
        }

        public static Dtype GetDtype<T>(this T[] obj)
        {
            return ToDtype(typeof(T));
        }

        public static Dtype GetDtype(this object obj)
        {
            switch (obj as dynamic)
            {
                case bool o: return np.bool8;
                case sbyte o: return np.int8;
                case byte o: return np.uint8;
                case short o: return np.int16;
                case ushort o: return np.uint16;
                case int o: return np.int32;
                case uint o: return np.uint32;
                case long o: return np.int64;
                case ulong o: return np.uint64;
                case float o: return np.float32;
                case double o: return np.float64;
                case string o: return np.unicode_;
                case char o: return np.unicode_;
                case bool[] o: return np.bool8;
                case byte[] o: return np.@byte;
                case short[] o: return np.int16;
                case int[] o: return np.int32;
                case long[] o: return np.int64;
                case float[] o: return np.float32;
                case double[] o: return np.float64;
                case string[] o: return np.unicode_;
                case char[] o: return np.unicode_;
                case bool[,] o: return np.bool8;
                case byte[,] o: return np.uint8;
                case short[,] o: return np.int16;
                case int[,] o: return np.int32;
                case long[,] o: return np.int64;
                case float[,] o: return np.float32;
                case double[,] o: return np.float64;
                case string[,] o: return np.unicode_;
                case char[,] o: return np.unicode_;
                case bool[,,] o: return np.bool8;
                case byte[,,] o: return np.uint8;
                case short[,,] o: return np.int16;
                case int[,,] o: return np.int32;
                case long[,,] o: return np.int64;
                case float[,,] o: return np.float32;
                case double[,,] o: return np.float64;
                case string[,,] o: return np.unicode_;
                case char[,,] o: return np.unicode_;
                default: throw new ArgumentException($"Can not cast {obj.GetType()} to any dtype: ");
            }
        }

        public static Dtype ToDtype(this Type t)
        {
            object instance = Activator.CreateInstance(t);
            return GetDtype(instance);
        }
    }
}
