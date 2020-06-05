using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Numpy;
using Numpy.Models;
using Python.Runtime;

namespace Numpy
{
    /// <summary>
    /// Manual type conversions
    /// </summary>
    public static partial class np
    {

        /// <summary>
        ///	Stack 1-D arrays as columns into a 2-D array.<br></br>
        ///	
        ///	Take a sequence of 1-D arrays and stack them as columns
        ///	to make a single 2-D array.<br></br>
        ///	 2-D arrays are stacked as-is,
        ///	just like with hstack.<br></br>
        ///	  1-D arrays are turned into 2-D columns
        ///	first.
        /// </summary>
        /// <param name="tup">
        ///	Arrays to stack.<br></br>
        ///	All of them must have the same first dimension.
        /// </param>
        /// <returns>
        ///	The array formed by stacking the given arrays.
        /// </returns>
        public static NDarray column_stack(params NDarray[] tup)
        {
            //auto-generated code, do not change
            var __self__ = self;
            PyTuple pyargs;
            if (tup.Length == 1)
            {
                pyargs = ToTuple(new object[] { tup[0], });
            }
            else
            {
                pyargs = ToTuple(new object[] { tup, });
            }
            var kwargs = new PyDict();
            dynamic py = __self__.InvokeMethod("column_stack", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
    }
}
