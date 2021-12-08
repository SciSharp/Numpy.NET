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
        ///	Insert values along the given axis before the given indices.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Note that for higher dimensional inserts obj=0 behaves very different
        ///	from obj=[0] just like arr[:,0,:] = values is different from
        ///	arr[:,[0],:] = values.
        /// </summary>
        /// <param name="arr">
        ///	Input array.
        /// </param>
        /// <param name="obj">
        ///	Object that defines the index or indices before which values is
        ///	inserted.<br></br>
        ///	
        ///	Support for multiple insertions when obj is a single scalar or a
        ///	sequence with one element (similar to calling insert multiple
        ///	times).
        /// </param>
        /// <param name="values">
        ///	Values to insert into arr.<br></br>
        ///	If the type of values is different
        ///	from that of arr, values is converted to the type of arr.<br></br>
        ///	
        ///	values should be shaped so that arr[...,obj,...] = values
        ///	is legal.
        /// </param>
        /// <param name="axis">
        ///	Axis along which to insert values.<br></br>
        ///	If axis is None then arr
        ///	is flattened first.
        /// </param>
        /// <returns>
        ///	A copy of arr with values inserted.<br></br>
        ///	  Note that insert
        ///	does not occur in-place: a new array is returned.<br></br>
        ///	 If
        ///	axis is None, out is a flattened array.
        /// </returns>
        public static NDarray insert(NDarray arr, int obj, NDarray values = null, int? axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                arr,
                ToPython(obj),
            });
            var kwargs = new PyDict();
            if (values != null) kwargs["values"] = ToPython(values);
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("insert", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	Insert values along the given axis before the given indices.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Note that for higher dimensional inserts obj=0 behaves very different
        ///	from obj=[0] just like arr[:,0,:] = values is different from
        ///	arr[:,[0],:] = values.
        /// </summary>
        /// <param name="arr">
        ///	Input array.
        /// </param>
        /// <param name="obj">
        ///	Object that defines the index or indices before which values is
        ///	inserted.<br></br>
        ///	
        ///	Support for multiple insertions when obj is a single scalar or a
        ///	sequence with one element (similar to calling insert multiple
        ///	times).
        /// </param>
        /// <param name="values">
        ///	Values to insert into arr.<br></br>
        ///	If the type of values is different
        ///	from that of arr, values is converted to the type of arr.<br></br>
        ///	
        ///	values should be shaped so that arr[...,obj,...] = values
        ///	is legal.
        /// </param>
        /// <param name="axis">
        ///	Axis along which to insert values.<br></br>
        ///	If axis is None then arr
        ///	is flattened first.
        /// </param>
        /// <returns>
        ///	A copy of arr with values inserted.<br></br>
        ///	  Note that insert
        ///	does not occur in-place: a new array is returned.<br></br>
        ///	 If
        ///	axis is None, out is a flattened array.
        /// </returns>
        public static NDarray insert(NDarray arr, NDarray obj, NDarray values = null, int? axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                arr,
                obj,
            });
            var kwargs = new PyDict();
            if (values != null) kwargs["values"] = ToPython(values);
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("insert", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	Insert values along the given axis before the given indices.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Note that for higher dimensional inserts obj=0 behaves very different
        ///	from obj=[0] just like arr[:,0,:] = values is different from
        ///	arr[:,[0],:] = values.
        /// </summary>
        /// <param name="arr">
        ///	Input array.
        /// </param>
        /// <param name="obj">
        ///	Object that defines the index or indices before which values is
        ///	inserted.<br></br>
        ///	
        ///	Support for multiple insertions when obj is a single scalar or a
        ///	sequence with one element (similar to calling insert multiple
        ///	times).
        /// </param>
        /// <param name="values">
        ///	Values to insert into arr.<br></br>
        ///	If the type of values is different
        ///	from that of arr, values is converted to the type of arr.<br></br>
        ///	
        ///	values should be shaped so that arr[...,obj,...] = values
        ///	is legal.
        /// </param>
        /// <param name="axis">
        ///	Axis along which to insert values.<br></br>
        ///	If axis is None then arr
        ///	is flattened first.
        /// </param>
        /// <returns>
        ///	A copy of arr with values inserted.<br></br>
        ///	  Note that insert
        ///	does not occur in-place: a new array is returned.<br></br>
        ///	 If
        ///	axis is None, out is a flattened array.
        /// </returns>
        public static NDarray insert(NDarray arr, Slice obj, NDarray values = null, int? axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                arr,
                obj.ToPython(),
            });
            var kwargs = new PyDict();
            if (values != null) kwargs["values"] = ToPython(values);
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("insert", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	Insert values along the given axis before the given indices.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Note that for higher dimensional inserts obj=0 behaves very different
        ///	from obj=[0] just like arr[:,0,:] = values is different from
        ///	arr[:,[0],:] = values.
        /// </summary>
        /// <param name="arr">
        ///	Input array.
        /// </param>
        /// <param name="obj">
        ///	Object that defines the index or indices before which values is
        ///	inserted.<br></br>
        ///	
        ///	Support for multiple insertions when obj is a single scalar or a
        ///	sequence with one element (similar to calling insert multiple
        ///	times).
        /// </param>
        /// <param name="values">
        ///	Values to insert into arr.<br></br>
        ///	If the type of values is different
        ///	from that of arr, values is converted to the type of arr.<br></br>
        ///	
        ///	values should be shaped so that arr[...,obj,...] = values
        ///	is legal.
        /// </param>
        /// <param name="axis">
        ///	Axis along which to insert values.<br></br>
        ///	If axis is None then arr
        ///	is flattened first.
        /// </param>
        /// <returns>
        ///	A copy of arr with values inserted.<br></br>
        ///	  Note that insert
        ///	does not occur in-place: a new array is returned.<br></br>
        ///	 If
        ///	axis is None, out is a flattened array.
        /// </returns>
        public static NDarray insert<T>(NDarray arr, int obj, T values, int? axis = null) where T : struct
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                arr,
                ToPython(obj),
            });
            var kwargs = new PyDict();
            kwargs["values"] = ToPython(values);
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("insert", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

    }
}
