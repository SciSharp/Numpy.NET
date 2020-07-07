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
    /// Manual overloads
    /// </summary>
    public static partial class np
    {

        /// <summary>
        ///	Return a new array with sub-arrays along an axis deleted.<br></br>
        ///	 For a one
        ///	dimensional array, this returns those entries not returned by
        ///	arr[obj].<br></br>
        ///	
        ///	Notes
        ///	
        ///	Often it is preferable to use a boolean mask.<br></br>
        ///	 For example:
        ///	
        ///	Is equivalent to np.delete(arr, [0,2,4], axis=0), but allows further
        ///	use of mask.
        /// </summary>
        /// <param name="arr">
        ///	Input array.
        /// </param>
        /// <param name="obj">
        ///	Indicate which sub-arrays to remove.
        /// </param>
        /// <param name="axis">
        ///	The axis along which to delete the subarray defined by obj.<br></br>
        ///	
        ///	If axis is None, obj is applied to the flattened array.
        /// </param>
        /// <returns>
        ///	A copy of arr with the elements specified by obj removed.<br></br>
        ///	 Note
        ///	that delete does not occur in-place.<br></br>
        ///	 If axis is None, out is
        ///	a flattened array.
        /// </returns>
        public static NDarray delete(NDarray arr, int obj, int? axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                arr,
                obj,
            });
            var kwargs = new PyDict();
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("delete", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	Return a new array with sub-arrays along an axis deleted.<br></br>
        ///	 For a one
        ///	dimensional array, this returns those entries not returned by
        ///	arr[obj].<br></br>
        ///	
        ///	Notes
        ///	
        ///	Often it is preferable to use a boolean mask.<br></br>
        ///	 For example:
        ///	
        ///	Is equivalent to np.delete(arr, [0,2,4], axis=0), but allows further
        ///	use of mask.
        /// </summary>
        /// <param name="arr">
        ///	Input array.
        /// </param>
        /// <param name="obj">
        ///	Indicate which sub-arrays to remove.
        /// </param>
        /// <param name="axis">
        ///	The axis along which to delete the subarray defined by obj.<br></br>
        ///	
        ///	If axis is None, obj is applied to the flattened array.
        /// </param>
        /// <returns>
        ///	A copy of arr with the elements specified by obj removed.<br></br>
        ///	 Note
        ///	that delete does not occur in-place.<br></br>
        ///	 If axis is None, out is
        ///	a flattened array.
        /// </returns>
        public static NDarray delete(NDarray arr, int[] obj, int? axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                arr,
                obj,
            });
            var kwargs = new PyDict();
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("delete", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
    }
}
