using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numpy;
using Numpy.Models;
using Python.Runtime;
using Assert = NUnit.Framework.Assert;

namespace Numpy.UnitTest
{
    [TestClass]
    public class NumpyTest
    {
        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            PythonEngine.BeginAllowThreads();
        }

        [TestMethod]
        public void empty()
        {
            // initialize an array with random integers
            var a = np.empty(new Shape(2, 3), np.int32);
            Console.WriteLine(a.repr);
            Assert.IsNotNull(a.ToString());
            // this should print out the exact integers of the array
            foreach (var x in a.GetData<int>())
                Console.WriteLine(x);
        }

        [TestMethod]
        public unsafe void create_from_pointer_without_copying()
        {
            IntPtr pointer = IntPtr.Zero;
            try {
                var dtype = np.int32;
                const int length = 1024;
                pointer = Marshal.AllocHGlobal(length);
                var ptr = (int*)pointer;
                // fill the buffer with index numbers
                for (int i = 0; i < length / sizeof(int); i++)
                    ptr[i] = i;
                var a = new NDarray(pointer, length, dtype);
                Console.WriteLine(a.ToString());
                var b = np.arange(length / sizeof(int));
                Console.WriteLine(b);
                var truth1 = b.Equals(a);
                var truth2 = a.Equals(b);
                Assert.AreEqual(b, a);
            }
            finally {
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        [TestMethod]
        public void efficient_array_copy()
        {
            var a = np.empty(new Shape(2, 3), np.int32);
            Console.WriteLine(a.repr);
            Assert.IsNotNull(a.ToString());
            long ptr = a.PyObject.ctypes.data;
            Console.WriteLine("ptr: " + ptr);
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            Marshal.Copy(array, 0, new IntPtr(ptr), array.Length);
            Console.WriteLine(a.ToString());
        }

        [TestMethod]
        public void array()
        {
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var a = np.array(array);
            Console.WriteLine(a.repr);
            Assert.AreEqual(array, a.GetData());
        }

        [TestMethod]
        public void ndarray_shape()
        {
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var a = np.array(array);
            Assert.AreEqual(new Shape(6), a.shape);
            Assert.AreEqual(new Shape(100), np.arange(100).shape);
        }

        [TestMethod]
        public void ndarray_strides()
        {
            Assert.AreEqual(new int[] { 4 }, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).strides);
            Assert.AreEqual(new int[] { 8 }, np.arange(10, dtype: np.longlong).strides);
        }

        [TestMethod]
        public void ndarray_ndim()
        {
            Assert.AreEqual(1, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).ndim);
            Assert.AreEqual(1, np.arange(10, dtype: np.longlong).ndim);
        }

        [TestMethod]
        public void ndarray_size()
        {
            Assert.AreEqual(6, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).size);
            Assert.AreEqual(10, np.arange(10, dtype: np.longlong).size);
        }

        [TestMethod]
        public void ndarray_len()
        {
            Assert.AreEqual(6, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).len);
            Assert.AreEqual(10, np.arange(10, dtype: np.longlong).len);
        }

        [TestMethod]
        public void ndarray_itemsize()
        {
            Assert.AreEqual(4, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).itemsize);
            Assert.AreEqual(8, np.arange(10, dtype: np.longlong).itemsize);
        }

        [TestMethod]
        public void ndarray_nbytes()
        {
            Assert.AreEqual(24, np.array(new int[] { 1, 2, 3, 4, 5, 6 }).nbytes);
            Assert.AreEqual(80, np.arange(10, dtype: np.longlong).nbytes);
        }

        [TestMethod]
        public void ndarray_base()
        {
            var a = np.array(new int[] { 1, 2, 3, 4, 5, 6 });
            var b = a.reshape(new Shape(2, 3));
            Assert.AreEqual(null, a.@base);
            Assert.AreEqual(a, b.@base);
        }

        [TestMethod]
        public void ndarray_dtype()
        {
            Assert.AreEqual(np.int32, np.array(new int[] { 1, 2, 3, 4, 5, 6 }, dtype: np.int32).dtype);
            Assert.AreEqual(np.longlong, np.arange(10, dtype: np.longlong).dtype);
            Assert.AreEqual(np.float32, np.arange(10, dtype: np.float32).dtype);
            Assert.AreEqual(np.@double, np.arange(10, dtype: np.float64).dtype);
        }

        [TestMethod]
        public void ndarray_multidim_source_array()
        {
            var a = np.array(new float[,] { { 1f, 2f }, { 3f, 4f }, { 3f, 4f } });
            Console.WriteLine(a.repr);
            Assert.AreEqual(new Shape(3, 2), a.shape);
            Assert.AreEqual(np.float32, a.dtype);
        }

        [TestMethod]
        public void ndarray_T()
        {
            var x = np.array(new float[,] { { 1f, 2f }, { 3f, 4f } });
            Assert.AreEqual("[[1. 2.]\n [3. 4.]]", x.ToString());
            var t = x.T;
            Console.WriteLine(t.repr);
            Assert.AreEqual("[[1. 3.]\n [2. 4.]]", t.ToString());
            // getting data of transposed array returns transposed array!
            Assert.AreEqual(new[] { 1f, 3f, 2f, 4f }, t.GetData<float>());
        }

        [TestMethod]
        public void ndarray_flatten()
        {
            var x = np.array(new float[,] { { 1f, 2f }, { 3f, 4f } });
            Assert.AreEqual("[1. 2. 3. 4.]", x.flatten().ToString());
            var t = x.T;
            Assert.AreEqual("[1. 3. 2. 4.]", t.flatten().ToString());
            Assert.AreEqual(new[] { 1f, 3f, 2f, 4f }, t.flatten().GetData<float>());
        }

        [TestMethod]
        public void ndarray_reshape()
        {
            var a = np.array(new int[] { 1, 2, 3, 4, 5, 6 });
            var b = a.reshape(new Shape(2, 3));
            Assert.AreEqual("[[1 2 3]\n [4 5 6]]", b.str);
            Assert.AreEqual(new Shape(2, 3), b.shape);
            Assert.AreEqual(null, a.@base);
            Assert.AreEqual(a, b.@base);
        }

        [TestMethod]
        public void ndarray_indexing()
        {
            // using string indices
            var x = np.arange(10);
            Assert.AreEqual("2", x["2"].str);
            Assert.AreEqual("8", x["-2"].str);
            Assert.AreEqual("[2 3 4 5 6 7]", x["2:-2"].str);
            var y = x.reshape(new Shape(2, 5));
            Assert.AreEqual("8", y["1,3"].str);
            Assert.AreEqual("9", y["1,-1"].str);
            Assert.AreEqual("array([0, 1, 2, 3, 4])", y["0"].repr);
            Assert.AreEqual("2", y["0"]["2"].str);
        }

        [TestMethod]
        public void ndarray_indexing1()
        {
            // using int indices
            var x = np.arange(10);
            Assert.AreEqual("2", x[2].str);
            Assert.AreEqual("8", x[-2].str);
            var y = x.reshape(new Shape(2, 5));
            Assert.AreEqual("8", y[1, 3].str);
            Assert.AreEqual("9", y[1, -1].str);
            Assert.AreEqual("array([0, 1, 2, 3, 4])", y[0].repr);
            Assert.AreEqual("2", y[0][2].str);
        }

        [TestMethod]
        public void ndarray_indexing2()
        {
            var x = np.arange(10, 1, -1);
            Assert.AreEqual("array([10,  9,  8,  7,  6,  5,  4,  3,  2])", x.repr);
            Assert.AreEqual("array([7, 7, 9, 2])", x[np.array(new[] { 3, 3, 1, 8 })].repr);
            Assert.AreEqual("array([7, 7, 4, 2])", x[np.array(new[] { 3, 3, -3, 8 })].repr);
            Assert.AreEqual("array([[9, 9],\n       [8, 7]])", x[np.array(new int[,] { { 1, 1 }, { 2, 3 } })].repr);
        }

        [TestMethod]
        public void ndarray_indexing3()
        {
            var y = np.arange(35).reshape(5, 7);
            Assert.AreEqual("array([ 0, 15, 30])", y[np.array(0, 2, 4), np.array(0, 1, 2)].repr);
            Assert.AreEqual("array([ 1, 15, 29])", y[np.array(0, 2, 4), 1].repr);
            Assert.AreEqual(
                "array([[ 0,  1,  2,  3,  4,  5,  6],\n       [14, 15, 16, 17, 18, 19, 20],\n       [28, 29, 30, 31, 32, 33, 34]])",
                y[np.array(0, 2, 4)].repr);
        }

        [TestMethod]
        public void ndarray_indexing_setter1()
        {
            // using int indices
            var x = np.arange(10);
            Assert.AreEqual("2", x[2].str);
            x[2] = (NDarray)22;
            Assert.AreEqual("22", x[2].str);
            Assert.AreEqual("8", x[-2].str);
            x[-2] = (NDarray)88;
            Assert.AreEqual("88", x[-2].str);
            var y = x.reshape(new Shape(2, 5));
            Assert.AreEqual("88", y[1, 3].str);
            y[1, 3] = (NDarray)888;
            Assert.AreEqual("888", y[1, 3].str);
            Assert.AreEqual("array([ 0,  1, 22,  3,  4])", y[0].repr);
            Assert.AreEqual("22", y[0][2].str);
            y[0][2] = (NDarray)222;
            Assert.AreEqual("222", y[0][2].str);
        }

        [TestMethod]
        public void ndarray_indexing_setter2()
        {
            // using string indices
            var x = np.arange(10);
            Assert.AreEqual("2", x[2].str);
            x["2"] = (NDarray)22;
            Assert.AreEqual("22", x[2].str);
            Assert.AreEqual("8", x[-2].str);
            x["-2"] = (NDarray)88;
            Assert.AreEqual("88", x[-2].str);
            var y = x.reshape(new Shape(2, 5));
            Assert.AreEqual("88", y[1, 3].str);
            y["1, 3"] = (NDarray)888;
            Assert.AreEqual("888", y[1, 3].str);
            Assert.AreEqual("array([ 0,  1, 22,  3,  4])", y[0].repr);
            Assert.AreEqual("22", y[0][2].str);
            y["0"]["2"] = (NDarray)222;
            Assert.AreEqual("222", y[0][2].str);
        }

        [TestMethod]
        public void ndarray_indexing_setter3()
        {
            var a = np.array(new int[] { 1, 2, 3, 4, 5, 6 }).reshape(new Shape(2, 3));
            Assert.AreEqual("[[1 2 3]\n [4 5 6]]", a.str);
            a[":", 1] = a[":", 1] * 2;
            Assert.AreEqual("[[ 1  4  3]\n [ 4 10  6]]", a.str);
        }

        [TestMethod]
        public void ndarray_indexing_setter4()
        {
            var x = np.arange(10, 1, -1);
            Assert.AreEqual("array([10,  9,  8,  7,  6,  5,  4,  3,  2])", x.repr);
            Assert.AreEqual("array([7, 7, 9, 2])", x[np.array(new[] { 3, 3, 1, 8 })].repr);
            x[np.array(new[] { 3, 3, 1, 8 })] = np.arange(4);
            Assert.AreEqual("array([10,  2,  8,  1,  6,  5,  4,  3,  3])", x.repr);
        }

        [TestMethod]
        public void ndarray_slice()
        {
            var x = np.arange(10);
            Assert.AreEqual("array([2, 3, 4])", x["2:5"].repr);
            Assert.AreEqual("array([0, 1, 2])", x[":-7"].repr);
            Assert.AreEqual("array([1, 3, 5])", x["1:7:2"].repr);
            var y = np.arange(35).reshape(new Shape(5, 7));
            Assert.AreEqual("array([[ 7, 10, 13],\n       [21, 24, 27]])", y["1:5:2,::3"].repr);
        }

        [TestMethod]
        public void ndarray_slice1()
        {
            var y = np.arange(35).reshape(5, 7);
            var b = y > 20;
            Assert.AreEqual(
                "array([[ 1,  2],\n" +
                "       [15, 16],\n" +
                "       [29, 30]])",
                y[np.array(0, 2, 4), "1:3"].repr);
            Assert.AreEqual("array([[22, 23],\n       [29, 30]])", y[b[":", 5], "1:3"].repr);
        }

        [TestMethod]
        public void ndarray_masking()
        {
            var y = np.arange(35).reshape(5, 7);
            var b = y > 20;
            Assert.AreEqual("array([21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34])", y[b].repr);
            // use a 1-D boolean whose first dim agrees with the first dim of y
            Assert.AreEqual("array([False, False, False,  True,  True])", b[":", 5].repr);
            Assert.AreEqual("array([[21, 22, 23, 24, 25, 26, 27],\n       [28, 29, 30, 31, 32, 33, 34]])", y[b[":", 5]].repr);
        }

        [TestMethod]
        public void ndarray_masking1()
        {
            var x = np.arange(30).reshape(2, 3, 5);
            Assert.AreEqual(
                "array([[[ 0,  1,  2,  3,  4],\n" +
                "        [ 5,  6,  7,  8,  9],\n" +
                "        [10, 11, 12, 13, 14]],\n\n" +
                "       [[15, 16, 17, 18, 19],\n" +
                "        [20, 21, 22, 23, 24],\n" +
                "        [25, 26, 27, 28, 29]]])",
                 x.repr);
            var b = np.array(new[,] { { true, true, false }, { false, true, true } });
            Assert.AreEqual(
                "array([[ 0,  1,  2,  3,  4],\n" +
                "       [ 5,  6,  7,  8,  9],\n" +
                "       [20, 21, 22, 23, 24],\n" +
                "       [25, 26, 27, 28, 29]])",
            x[b].repr);
        }

        [TestMethod]
        public void ndarray_comparison_operators()
        {
            var a = np.array(1, 2, 3);
            // comparison with a scalar
            Assert.AreEqual(new[] { true, false, false }, (a < 2).GetData());
            Assert.AreEqual(new[] { true, true, false }, (a <= 2).GetData());
            Assert.AreEqual(new[] { false, false, true }, (a > 2).GetData());
            Assert.AreEqual(new[] { false, true, true }, (a >= 2).GetData());
            Assert.AreEqual(new[] { false, true, false }, (a.equals(2)).GetData());
            Assert.AreEqual(new[] { true, false, true }, (a.not_equals(2)).GetData());
            // comparison with an array
            var b = (np.ones(new Shape(3), np.int32) * 2);
            Assert.AreEqual(new[] { true, false, false }, (a < b).GetData());
            Assert.AreEqual(new[] { true, true, false }, (a <= b).GetData());
            Assert.AreEqual(new[] { false, false, true }, (a > b).GetData());
            Assert.AreEqual(new[] { false, true, true }, (a >= b).GetData());
            Assert.AreEqual(new[] { false, true, false }, (a.equals(b)).GetData());
            Assert.AreEqual(new[] { true, false, true }, (a.not_equals(b)).GetData());
        }

        [TestMethod]
        public void ndarray_unary_operators()
        {
            // unary operations
            var a = np.array(1, 2, 3);
            Assert.AreEqual(new[] { -1, -2, -3 }, (-a).GetData<int>());
            Assert.AreEqual(new[] { 1, 2, 3 }, (+a).GetData<int>());
            // todo: test operator ~
        }

        [TestMethod]
        public void ndarray_arithmetic_operators()
        {
            // arithmetic operators
            var a = np.array(1, 2, 3);
            var b = (np.ones(new Shape(3), np.int32) * 2);
            Assert.AreEqual(new[] { 11, 12, 13 }, (a + 10).GetData<int>());
            Assert.AreEqual(new[] { 3, 4, 5 }, (a + b).GetData<int>());
            Assert.AreEqual(new[] { -9, -8, -7 }, (a - 10).GetData<int>());
            Assert.AreEqual(new[] { -1, 0, 1 }, (a - b).GetData<int>());
            Assert.AreEqual(new[] { 10, 20, 30 }, (a * 10).GetData<int>());
            Assert.AreEqual(new[] { 2, 4, 6 }, (a * b).GetData<int>());
            a = np.array(2, 4, 16);
            Assert.AreEqual(new[] { 1, 2, 8 }, (a / 2).GetData<double>());
            Assert.AreEqual(new[] { 1, 2, 8 }, (a / b).GetData<double>());
            Assert.AreEqual(new[] { 4, 2, .5 }, (8 / a).GetData<double>());
            Assert.AreEqual(new[] { 4, 2, -10 }, (6 - a).GetData<int>());
        }

        [TestMethod]
        public void ndarray_arithmetic_inplace_operators()
        {
            var a = np.array(1, 2, 3);
            var b = (np.ones(new Shape(3), np.int32) * 2);
            a.iadd(10);
            Assert.AreEqual(new[] { 11, 12, 13 }, a.GetData<int>());
            a.isub(10);
            Assert.AreEqual(new[] { 1, 2, 3 }, a.GetData<int>());
            a.iadd(b);
            Assert.AreEqual(new[] { 3, 4, 5 }, a.GetData<int>());
            a.isub(b);
            Assert.AreEqual(new[] { 1, 2, 3 }, a.GetData<int>());
        }

        [TestMethod]
        public void ndarray_value_div_ndarray()
        {
            // division operator
            var a = np.array(1.0, 2.0, 3.0);
            Assert.AreEqual(new[] { 0.5, 1.0, 1.5 }, (a / 2.0).GetData<double>());
            Assert.AreEqual(new[] { 6.0, 3.0, 2.0 }, (6.0 / a).GetData<double>());
            // minus operator
            Assert.AreEqual(new[] { -1.0, 0.0, 1.0 }, (a - 2.0).GetData<double>());
            Assert.AreEqual(new[] { 1.0, 0.0, -1.0 }, (2.0 - a).GetData<double>());
        }

        [TestMethod]
        public void np_where()
        {
            //>>> import numpy as np
            //>>> a = [1, 2, 3, 4, 0, 0, 1, 2]
            //>>> a = np.array(a)
            //>>> b = np.where(a == 0)
            //>>> b[0]
            //array([4, 5], dtype = int64)
            var a = np.array(new[] { 1, 2, 3, 4, 0, 0, 1, 2 });
            var b = np.where(a.equals(0));
            Assert.AreEqual("array([4, 5], dtype=int64)", b[0].repr);
        }

        [TestMethod]
        public void GetData_noncontiguous()
        {
            int[,] X = new int[3, 3];
            X[0, 0] = -1;

            NDarray npX = np.array(X, dtype: np.int32); // control
            NDarray npY = np.array(X, dtype: np.int32); // test

            Console.WriteLine("Control:");
            Console.WriteLine(npX);

            Console.WriteLine("Test:");
            Console.WriteLine(npY);

            // flip on the row axis
            npY = np.flip(npY, new int[] { 0 });
            Console.WriteLine("Test flipped on axis 0:");
            Console.WriteLine(npY);

            // get their data
            int[] cX = npX.GetData<int>();
            int[] cY = npY.GetData<int>();

            Console.WriteLine("Control extracted back to C#:\n" + string.Join(" ", cX));
            Assert.AreEqual("-1 0 0 0 0 0 0 0 0", string.Join(" ", cX));
            Console.WriteLine("Test extracted back to C#:\n" + string.Join(" ", cY));
            Assert.AreEqual("0 0 0 0 0 0 -1 0 0", string.Join(" ", cY));
        }

        [TestMethod]
        public void CopyDataInAndOutExample()
        {
            var a = np.array(new[] { 2, 4, 9, 25 });
            Console.WriteLine("a: " + a.repr);
            // a: array([ 2,  4,  9, 25])
            var roots = np.sqrt(a);
            Console.WriteLine(roots.repr);
            // array([1.41421356, 2.        , 3.        , 5.        ])
            Assert.AreEqual("array([1.41421356, 2.        , 3.        , 5.        ])", roots.repr);
            Console.WriteLine(string.Join(", ", roots.GetData<int>()));
            // 1719614413, 1073127582, 0, 1073741824
            Console.WriteLine("roots.dtype: " + roots.dtype);
            // roots.dtype: float64
            Console.WriteLine(string.Join(", ", roots.GetData<double>()));
            // 1.4142135623731, 2, 3, 5
            Assert.AreEqual(new double[] { 1.41, 2, 3, 5 }, roots.GetData<double>().Select(x => Math.Round(x, 2)).ToArray());
        }

        [TestMethod]
        public void QuestionByPiyushspss()
        {
            // np.column_stack(np.where(mat > 0))

            //>>> a = np.array([1, 0, 0, 1, 2, 3, 0, 1]).reshape(2, 4)
            //         >>> a
            //array([[1, 0, 0, 1],
            //       [2, 3, 0, 1]])
            //>>> np.column_stack(a)
            //array([[1, 2],
            //       [0, 3],
            //       [0, 0],
            //       [1, 1]])
            //>>> np.where(a > 0)
            //(array([0, 0, 1, 1, 1], dtype = int64), array([0, 3, 0, 1, 3], dtype = int64))
            //>>> np.column_stack(np.where(a > 0))
            //array([[0, 0],
            //       [0, 3],
            //       [1, 0],
            //       [1, 1],
            //       [1, 3]], dtype = int64)
            //>>>
            var a = np.array(new[] { 1, 0, 0, 1, 2, 3, 0, 1 }).reshape(2, 4);
            var expected =
                "array([[1, 2],\n" +
                "       [0, 3],\n" +
                "       [0, 0],\n" +
                "       [1, 1]])";
            Assert.AreEqual(expected, np.column_stack(a).repr);
            // note: this was a bug, now you don't get a python tuple back, but an array of NDarrays instead so the following line just doesn't compile any more
            //Assert.AreEqual("(array([0, 0, 1, 1, 1], dtype=int64), array([0, 3, 0, 1, 3], dtype=int64))", np.where(a > 0).repr);
            expected =
                "array([[0, 0],\n" +
                "       [0, 3],\n" +
                "       [1, 0],\n" +
                "       [1, 1],\n" +
                "       [1, 3]], dtype=int64)";
            Assert.AreEqual(expected, np.column_stack(np.where(a > 0)).repr);
        }

        [TestMethod]
        public void QuestionByGurelsoycaner()
        {
            //>>> import numpy as np
            //>>> P1 = np.array([1, 2, 3, 4])
            //>>> P2 = np.array([4, 3, 2, 1])
            //>>> ex = (P2 - P1) / (np.linalg.norm(P2 - P1))
            //>>> ex
            //array([0.67082039, 0.2236068, -0.2236068, -0.67082039])
            var P1 = np.array(new[] { 1, 2, 3, 4 });
            var P2 = np.array(new[] { 4, 3, 2, 1 });
            var ex = (P2 - P1) / (np.linalg.norm(P2 - P1));
            Assert.AreEqual("array([ 0.67082039,  0.2236068 , -0.2236068 , -0.67082039])", ex.repr);
        }

        [TestMethod]
        public void QuestionBySimonBuehler()
        {
            //import numpy as np
            //bboxes = np.empty([999, 4])
            //keep_idx = np.array([2, 6, 7, 8, 9, 13])
            //bboxes = bboxes[keep_idx]
            //>>> bboxes.shape
            //(6, 4)
            var bboxes = np.empty(new Shape(999, 4));
            var keep_idx = np.array(new[] { 2, 6, 7, 8, 9, 13 });
            bboxes = bboxes[keep_idx];
            Assert.AreEqual("(6, 4)", bboxes.shape.ToString());

            //>>> np.where(keep_idx > 4)[0]
            //array([1, 2, 3, 4, 5], dtype = int64)
            var x = np.where(keep_idx > 4)[0];
            Assert.AreEqual("array([1, 2, 3, 4, 5], dtype=int64)", x.repr);
        }

        [TestMethod]
        public void StringArray()
        {
            //>>> a = numpy.array(['apples', 'foobar', 'cowboy'])
            //>>> a[2] = 'bananas'
            //>>> a
            //array(['apples', 'foobar', 'banana'], 
            //      dtype = '|S6')
            var a = np.array(new string[] { "apples", "foobar", "cowboy" });
            Assert.AreEqual("array(['apples', 'foobar', 'cowboy'], dtype='<U6')", a.repr);
            // todo: a[2]="banana";
            a.self.SetItem(new PyInt(2), new PyString("banana"));
            Assert.AreEqual("array(['apples', 'foobar', 'banana'], dtype='<U6')", a.repr);

            //>>> a = numpy.array(['apples', 'foobar', 'cowboy'], dtype = object)
            //>>> a
            //array([apples, foobar, cowboy], dtype = object)
            //>>> a[2] = 'bananas'
            //>>> a
            //array([apples, foobar, bananas], dtype = object)
            a = np.array(new string[] { "apples", "foobar", "cowboy" }, dtype: np.object_);
            Assert.AreEqual("array(['apples', 'foobar', 'cowboy'], dtype=object)", a.repr);
            // todo: a[2]="banana";
            a.self.SetItem(new PyInt(2), new PyString("banana"));
            Assert.AreEqual("array(['apples', 'foobar', 'banana'], dtype=object)", a.repr);
        }

        [TestMethod]
        public void ComplexNumbers()
        {
            //>>> a = np.array([1+2j, 3+4j, 5+6j])
            //>>> a.imag
            //array([2.,  4.,  6.])
            var a = np.array(new Complex[] { new Complex(1, 2), new Complex(3, 4), new Complex(5, 6), });
            Assert.AreEqual("array([1., 3., 5.])", a.real.repr);
            Assert.AreEqual("array([2., 4., 6.])", a.imag.repr);
            //>>> np.imag(a)
            //array([2., 4., 6.])
            //>>> np.real(a)
            //array([1., 3., 5.])
            Assert.AreEqual("array([1., 3., 5.])", np.real(a).repr);
            Assert.AreEqual("array([2., 4., 6.])", np.imag(a).repr);
            //>>> a.imag = np.array([8, 10, 12])
            //>>> a
            //array([1. +8.j,  3.+10.j,  5.+12.j])
            a.imag = np.array(new[] { 8, 10, 12 });
            Assert.AreEqual("array([1. +8.j, 3.+10.j, 5.+12.j])", a.repr);
            //>>> np.imag(1 + 1j)
            //1.0
            Assert.AreEqual(1.0, np.imag(new Complex(1, 1)).asscalar<double>());

            // getting the complex numbers out again
            var c = a.GetData<Complex>();
            Assert.IsTrue(Enumerable.SequenceEqual(new Complex[] { new Complex(1, 8), new Complex(3, 10), new Complex(5, 12), }, c));

            // accessing scalar values
            var b = new NDarray<Complex>(a);
            Assert.AreEqual(new Complex(1, 8), b[0].asscalar<Complex>());
        }

        [TestMethod]
        public void IssueByXlient()
        {
            var points = new Point[] { new Point(0, 0), new Point(17, 4), new Point(2, 22), new Point(10, 7), };
            int[,] Pts = new int[,]
            {
                {points[0].X, points[0].Y },
                {points[1].X, points[1].Y },
                {points[2].X, points[2].Y } ,
                {points[3].X, points[3].Y }
            };

            // exception here / deadlock
            Dtype dtype = Pts.GetDtype();

            NDarray pts = np.array(Pts);
            NDarray rectangle = np.zeros(new Shape(4, 2), dtype);

            NDarray sum = np.sum(pts, 1);
            NDarray differnce = np.diff(pts, axis: 1);

            rectangle[0] = pts[sum.argmin()];
            rectangle[2] = pts[sum.argmax()];
            rectangle[1] = pts[differnce.argmin()];
            rectangle[3] = pts[differnce.argmax()];
        }

        [TestMethod]
        public void IssueByVolgaone()
        {
            var n = np.array(new float[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
            var row0 = n[0]; //extract 1st row
            Assert.AreEqual("array([1., 2., 3.], dtype=float32)", row0.repr);
            var row0Data = row0.GetData<float>(); //this is correct - {1,2,3} 
            Assert.AreEqual("1,2,3", string.Join(",", row0Data));
            var col1 = n[":,1"]; //extract 1st column - NDarray is [2 5 8] as expected
            Assert.AreEqual("array([2., 5., 8.], dtype=float32)", col1.repr);
            var col1Data = col1.GetData();//this is wrong - {2,3,4}
            Assert.AreEqual("2,5,8", string.Join(",", col1Data));
        }

        [TestMethod]
        public void IssueByNbustins()
        {
            int[,,,] iarr = new int[3, 25, 25, 3];
            NDarray nd = np.array(iarr);
            Assert.AreEqual(new Shape(3, 25, 25, 3), nd.shape);
        }

        [TestMethod]
        public void IssueByBanyc1()
        {
            //a = np.array([[1, 2, 3], [4, 5, 6]])
            //b = np.array([1, 2])
            //np.savez('/tmp/123.npz', a = a, b = b)
            //data = np.load('/tmp/123.npz')
            //data['a']
            //array([[1, 2, 3],
            //[4, 5, 6]])
            //data['b']
            //array([1, 2])
            //data.close()
            var a = np.array(new[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = np.array(new[] { 1, 2 });
            string tempFile = Path.GetTempFileName() + ".npz";
            Console.WriteLine(tempFile);
            np.savez(tempFile, kwds: new Dictionary<string, NDarray>() { ["a"] = a, ["b"] = b });
            var data = np.load(tempFile, allow_pickle: true);
            var a1 = new NDarray(data.self["a"]);
            Console.WriteLine(a1.repr);
            var b1 = new NDarray(data.self["b"]);
            Console.WriteLine(b1.repr);
            Assert.AreEqual("array([[1, 2, 3],\n       [4, 5, 6]])", a1.repr);
            Assert.AreEqual(@"array([1, 2])", b1.repr);
        }

        [TestMethod]
        public void IssueByBanyc2()
        {
            var a = np.array(new[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            var b = np.array(new[] { 1, 2 });
            string tempFile = Path.GetTempFileName() + ".npz";
            Console.WriteLine(tempFile);
            np.savez(tempFile, new[] { a, b });
            var data = np.load(tempFile, allow_pickle: true);
            var a1 = new NDarray(data.self["arr_0"]);
            Console.WriteLine(a1.repr);
            var b1 = new NDarray(data.self["arr_1"]);
            Console.WriteLine(b1.repr);
            Assert.AreEqual("array([[1, 2, 3],\n       [4, 5, 6]])", a1.repr);
            Assert.AreEqual(@"array([1, 2])", b1.repr);
        }

        [TestMethod]
        public void IssueByBanyc3()
        {
            //>>> a = np.ones((1, 2, 3, 4))
            //>>> a
            //array([[[[1., 1., 1., 1.],
            //         [1., 1., 1., 1.],
            //         [1., 1., 1., 1.]],

            //        [[1., 1., 1., 1.],
            //         [1., 1., 1., 1.],
            //         [1., 1., 1., 1.]]]])
            //>>> c = np.transpose(a, (0, 2, 3, 1))
            //>>> c
            //array([[[[1., 1.],
            //         [1., 1.],
            //         [1., 1.],
            //         [1., 1.]],

            //        [[1., 1.],
            //         [1., 1.],
            //         [1., 1.],
            //         [1., 1.]],

            //        [[1., 1.],
            //         [1., 1.],
            //         [1., 1.],
            //         [1., 1.]]]])
            //>>> b = a.transpose((0, 2, 3, 1))
            //>>> b
            //array([[[[1., 1.],
            //         [1., 1.],
            //         [1., 1.],
            //         [1., 1.]],

            //        [[1., 1.],
            //         [1., 1.],
            //         [1., 1.],
            //         [1., 1.]],

            //        [[1., 1.],
            //         [1., 1.],
            //         [1., 1.],
            //         [1., 1.]]]])
            //>>>
            NDarray a = np.ones(1, 2, 3, 4);
            NDarray c = np.transpose(a, new int[] { 0, 2, 3, 1 });

            string s = "array([[[[1., 1.],\n         [1., 1.],\n         [1., 1.],\n         [1., 1.]],\n\n        [[1., 1.],\n         [1., 1.],\n         [1., 1.],\n         [1., 1.]],\n\n        [[1., 1.],\n         [1., 1.],\n         [1., 1.],\n         [1., 1.]]]])";
            Assert.AreEqual(s, c.repr);
            NDarray b = a.transpose(0, 2, 3, 1);
            Assert.AreEqual(s, b.repr);
        }

        [TestMethod]
        public void IssueBybeanels01()
        {
            //sample = [np.array([[1., 2., 3.]]),np.array([[4., 5., 6.]]),np.array([[7., 8., 9.]])]
            //for test in sample:
            //    n = np.argmax(test[0])
            //    print(n)
            //# expected: 
            //# 2
            //# 2
            //# 2
            var result = new List<int>();
            var nc = np.array(new[] { np.array(new[] { 1, 2, 3 }), np.array(new[] { 4, 5, 6 }), np.array(new[] { 7, 8, 9 }) });
            for (int i = 0; i < nc.len; i++) {
                var n = np.argmax(nc[i]).asscalar<int>();
                result.Add(n);
            }
            Assert.AreEqual("2, 2, 2", string.Join(", ", result));
        }

        [TestMethod]
        public void IssueBybeanels01a()
        {
            //sample = [np.array([[1., 2., 3.]]),np.array([[4., 5., 6.]]),np.array([[7., 8., 9.]])]
            //for test in sample:
            //    n = np.argmax(test[0])
            //    print(n)
            //# expected: 
            //# 2
            //# 2
            //# 2
            var result = new List<int>();
            var nc = np.array(new[] { np.array(new[] { 1, 2, 3 }), np.array(new[] { 4, 5, 6 }), np.array(new[] { 7, 8, 9 }) });
            for (int i = 0; i < nc.len; i++) {
                var n = np.argmax(nc[i]).asscalar<int>();
                result.Add(n);
            }
            Assert.AreEqual("2, 2, 2", string.Join(", ", result));
        }

        [TestMethod]
        public void IssueByMatteo_0()
        {
            //>>> x = np.array([0, 1, 2, 3])
            //>>> y = np.array([-1, 0.2, 0.9, 2.1])
            //>>> A = np.vstack([x, np.ones(len(x))]).T
            //>>> A
            //array([[0., 1.],
            //       [1., 1.],
            //       [2., 1.],
            //       [3., 1.]])
            //>>> np.linalg.lstsq(A, y, rcond = None)
            //(array([1.  , -0.95]), array([0.05]), 2, array([4.10003045, 1.09075677]))
            var x = np.array(new[] { 0, 1, 2, 3 });
            var y = np.array(new[] { -1, 0.2, 0.9, 2.1 });
            var A = np.vstack(x, np.ones(x.len)).T;
            Assert.AreEqual("array([[0., 1.],\n       [1., 1.],\n       [2., 1.],\n       [3., 1.]])", A.repr);
            var tuple = np.linalg.lstsq(A, y, null);
            Assert.AreEqual("array([ 1.  , -0.95])", tuple.Item1.repr);
            Assert.AreEqual("array([0.05])", tuple.Item2.repr);
            Assert.AreEqual(2, tuple.Item3);
            Assert.AreEqual("array([4.10003045, 1.09075677])", tuple.Item4.repr);
        }

        [TestMethod]
        public void IssueByDecemberDream()
        {
            //a = np.array([1, 2, -2, -4, 0])
            //np.roots(a)
            //# returns array([ 1.41421356, -2., -1.41421356, 0.])            
            var a = np.array(new[] { 1, 2, -2, -4, 0 });
            var b = np.roots(a);
            Assert.AreEqual("array([ 1.41421356, -2.        , -1.41421356,  0.        ])", b.repr);
        }

        [TestMethod]
        public void IssueByDecemberDream2()
        {
            NDarray test = np.array(new int[,] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 9, 10, 11 } });
            NDarray rows = np.array(new int[,] { { 0, 0 }, { 3, 3 } });
            NDarray cols = np.array(new int[,] { { 0, 2 }, { 0, 2 } });

            var b = test[rows, cols];
            // should return
            // [[0, 2],
            //  [9, 11]]
            Assert.AreEqual("array([[ 0,  2],\n       [ 9, 11]])", b.repr);
        }

        [TestMethod]
        public void IssueByAmpangboy()
        {
            var arr = np.array(1.0);
            var result = np.insert(arr, 0, 1.0);
            Assert.AreEqual("array([1., 1.])", result.repr);
        }

        [TestMethod]
        public void IssueByBigpo()
        {
            var a = np.random.randn(3, 3);
            var tmp = np.linalg.qr(a);
        }

        [TestMethod]
        public void IssueByAllenP()
        {
            //>>> dx = 4.0
            //>>> dy = 5.0
            //>>> zX =[[1, 2, 3],[4,5,6],[8,9,0]]
            //>>> np.gradient(zX, dx, dy)
            //[array([[0.75, 0.75, 0.75],
            //       [0.875, 0.875, -0.375],
            //       [1.   , 1.   , -1.5]]), array([[0.2, 0.2, 0.2],
            //       [ 0.2,  0.2,  0.2],
            //       [ 0.2, -0.8, -1.8]])]
            var zX = new NDarray(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 8, 9, 0 } });
            var result = np.gradient(zX, new List<double> { 4.0, 5.0 });
            var expected = @"[array([[ 0.75 ,  0.75 ,  0.75 ],
       [ 0.875,  0.875, -0.375],
       [ 1.   ,  1.   , -1.5  ]]), array([[ 0.2,  0.2,  0.2],
       [ 0.2,  0.2,  0.2],
       [ 0.2, -0.8, -1.8]])]".Replace("\r", "");
            Assert.AreEqual(expected, result.repr);
        }

        [TestMethod]
        public void PrimitiveConversion()
        {
            dynamic np = Numpy.np.dynamic_self;
            Assert.AreEqual(3, (new PyInt(3)).As<int>());
            Assert.AreEqual(1_000_000_000_000_000, new PyInt(1_000_000_000_000_000).As<long>());
            Console.WriteLine(((dynamic)new PyInt(1_000_000_000_000_000)).__class__); // => <class 'int'>
            Console.WriteLine(np.int64(1_000_000_000_000_000).__class__); // => <class 'numpy.int64'>
            Assert.AreEqual(3, (np.int32(3).item() as PyObject).As<int>());
            Assert.AreEqual(1_000_000_000_000_000, (np.int64(1_000_000_000_000_000).item() as PyObject).As<long>());
        }

        [TestMethod]
        public void IssueByMegawattFs()
        {
            var arr = np.array(new int[] { 1, 2, 3, 4, 5 });
            var slice0 = new Slice(2, 4);
            var arr4 = arr[slice0];
            Assert.AreEqual("array([3, 4])", arr4.repr);
            var slice1 = new Slice(2, -1);
            var arr5 = arr[slice1];
            Assert.AreEqual("array([3, 4])", arr5.repr);
            var arr1 = arr["2:4"];
            Assert.AreEqual("array([3, 4])", arr1.repr);
            var arr2 = arr[":4"];
            Assert.AreEqual("array([1, 2, 3, 4])", arr2.repr);
            var arr3 = arr[":-1"];
            Assert.AreEqual("array([1, 2, 3, 4])", arr3.repr);
        }

        [TestMethod]
        public async Task IssueByMrCOrrupted()
        {
            Dictionary<string, NDarray> arrays = new Dictionary<string, NDarray>();
            arrays["a"] = np.arange(6).reshape(2, 3);
            arrays["b"] = np.arange(3);

            var filename = Path.Combine(Path.GetTempPath(), "test.npz");
            np.savez_compressed(filename, null, arrays);
            var archive = np.load(filename);
            Console.WriteLine(archive.repr);
            var a = new NDarray( archive.PyObject["a"]);
            var b = new NDarray( archive.PyObject["b"]);
            Console.WriteLine(a.repr);
            Console.WriteLine(b.repr);
            Assert.AreEqual("array([[0, 1, 2],\n       [3, 4, 5]])", a.repr);
            Assert.AreEqual(@"array([0, 1, 2])", b.repr);
        }

        [TestMethod]
        public async Task AsscalarRemovedInNumpyV1_23()
        {
            Assert.AreEqual(143, new NDarray<int>(new int[]{143}).asscalar<int>());
            Assert.AreEqual(143d, new NDarray<double>(new [] { 143d }).asscalar<double>());
            Assert.AreEqual(143d, new NDarray<double>(new[] { 143d }).item());
        }

        [TestMethod]
        public async Task IssueByMaLichtenegger()
        {
            // byte array als uint32 array
            var bytes = new byte[] { 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0 };
            var uints =np.zeros(new Shape(3), np.uint32);
            Console.WriteLine(uints.repr);
            var ctypes = uints.PyObject.ctypes;
            long ptr = ctypes.data;
            Marshal.Copy(bytes, 0, new IntPtr(ptr), bytes.Length);
            Console.WriteLine(uints.repr);
            Assert.AreEqual("array([1, 2, 3], dtype=uint32)", uints.repr);
            // byte array als float64 array
            bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0 };
            var doubles = np.zeros(new Shape(2), np.float64);
            Console.WriteLine(doubles.repr);
            ctypes = doubles.PyObject.ctypes;
            ptr = ctypes.data;
            Marshal.Copy(bytes, 0, new IntPtr(ptr), bytes.Length);
            Console.WriteLine(doubles.repr);
            Assert.IsTrue(doubles[0].asscalar<double>() != 0);
            Assert.IsTrue(doubles[1].asscalar<double>() == 0);
        }

        [TestMethod]
        public async Task IssueByMartinDevans()
        {
            //>>> x = np.arange(9)
            //>>> np.split(x, 3)
            //[array([0, 1, 2]), array([3, 4, 5]), array([6, 7, 8])]
            var x = np.arange(9);
            var b = np.split(x, 3).repr();
            var a = "(array([0, 1, 2]), array([3, 4, 5]), array([6, 7, 8]))";
            Assert.AreEqual(a, b);
            //>>> x = np.arange(8.0)
            //>>> np.split(x, [3, 5, 6, 10])
            //[array([0., 1., 2.]),
            //array([3., 4.]),
            //array([5.]),
            //array([6., 7.]),
            //array([], dtype = float64)]
            x = np.arange(8);
            b = np.split(x, new[] { 3, 5, 6, 10 }).repr();
            a = "(array([0, 1, 2]), array([3, 4]), array([5]), array([6, 7]), array([], dtype=int32))";
            Assert.AreEqual(a, b);
        }

        // TODO:  https://docs.scipy.org/doc/numpy/user/basics.indexing.html?highlight=slice#structural-indexing-tools
        // TODO:  https://docs.scipy.org/doc/numpy/user/basics.indexing.html?highlight=slice#assigning-values-to-indexed-arrays
        // TODO:  https://docs.scipy.org/doc/numpy/user/basics.indexing.html?highlight=slice#dealing-with-variable-numbers-of-indices-within-programs
    }
}
