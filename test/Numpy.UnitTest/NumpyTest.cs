using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numpy;
using Numpy.Models;
using Assert = NUnit.Framework.Assert;

namespace Numpy.UnitTest
{
    [TestClass]
    public class NumpyTest
    {
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
            try
            {
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
            finally
            {
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
            Console.WriteLine((a / 2).repr);
            Assert.AreEqual(new[] { 1, 2, 8 }, (a / 2).GetData<double>());
            Assert.AreEqual(new[] { 1, 2, 8 }, (a / b).GetData<double>());
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
            var keep_idx = np.array(new[] {2, 6, 7, 8, 9, 13});
            bboxes = bboxes[keep_idx];
            Assert.AreEqual("(6, 4)", bboxes.shape.ToString());

            //>>> np.where(keep_idx > 4)[0]
            //array([1, 2, 3, 4, 5], dtype = int64)
            var x = np.where(keep_idx > 4)[0];
            Assert.AreEqual("array([1, 2, 3, 4, 5], dtype=int64)", x.repr);
        }

        // TODO:  https://docs.scipy.org/doc/numpy/user/basics.indexing.html?highlight=slice#structural-indexing-tools
        // TODO:  https://docs.scipy.org/doc/numpy/user/basics.indexing.html?highlight=slice#assigning-values-to-indexed-arrays
        // TODO:  https://docs.scipy.org/doc/numpy/user/basics.indexing.html?highlight=slice#dealing-with-variable-numbers-of-indices-within-programs
    }
}
