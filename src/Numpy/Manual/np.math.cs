using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        ///	Return the gradient of an N-dimensional array.<br></br>
        ///	
        ///	The gradient is computed using second order accurate central differences
        ///	in the interior points and either first or second order accurate one-sides
        ///	(forward or backwards) differences at the boundaries.<br></br>
        ///	
        ///	The returned gradient hence has the same shape as the input array.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Assuming that  (i.e.,  has at least 3 continuous
        ///	derivatives) and let  be a non-homogeneous stepsize, we
        ///	minimize the “consistency error”  between the true gradient
        ///	and its estimate from a linear combination of the neighboring grid-points:
        ///	
        ///	By substituting  and 
        ///	with their Taylor series expansion, this translates into solving
        ///	the following the linear system:
        ///	
        ///	The resulting approximation of  is the following:
        ///	
        ///	It is worth noting that if 
        ///	(i.e., data are evenly spaced)
        ///	we find the standard second order approximation:
        ///	
        ///	With a similar procedure the forward/backward approximations used for
        ///	boundaries can be derived.<br></br>
        ///	
        ///	References
        /// </summary>
        /// <param name="f">
        ///	An N-dimensional array containing samples of a scalar function.
        /// </param>
        /// <param name="varargs">
        ///	Spacing between f values.<br></br>
        ///	Default unitary spacing for all dimensions.<br></br>
        ///	
        ///	Spacing can be specified using:
        ///	
        ///	If axis is given, the number of varargs must equal the number of axes.<br></br>
        ///	
        ///	Default: 1.
        /// </param>
        /// <param name="edge_order">
        ///	Gradient is calculated using N-th order accurate differences
        ///	at the boundaries.<br></br>
        ///	Default: 1.
        /// </param>
        /// <param name="axis">
        ///	Gradient is calculated only along the given axis or axes
        ///	The default (axis = None) is to calculate the gradient for all the axes
        ///	of the input array.<br></br>
        ///	axis may be negative, in which case it counts from
        ///	the last to the first axis.
        /// </param>
        /// <returns>
        ///	A set of ndarrays (or a single ndarray if there is only one dimension)
        ///	corresponding to the derivatives of f with respect to each dimension.<br></br>
        ///	
        ///	Each derivative has the same shape as f.
        /// </returns>
        public static NDarray gradient(NDarray f, int? edge_order = null, Axis axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;

            var pyargs = new PyObject[] { f.PyObject };
            var kwargs = new PyDict();
            if (edge_order != null) kwargs["edge_order"] = ToPython(edge_order);
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("gradient", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	Return the gradient of an N-dimensional array.<br></br>
        ///	
        ///	The gradient is computed using second order accurate central differences
        ///	in the interior points and either first or second order accurate one-sides
        ///	(forward or backwards) differences at the boundaries.<br></br>
        ///	
        ///	The returned gradient hence has the same shape as the input array.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Assuming that  (i.e.,  has at least 3 continuous
        ///	derivatives) and let  be a non-homogeneous stepsize, we
        ///	minimize the “consistency error”  between the true gradient
        ///	and its estimate from a linear combination of the neighboring grid-points:
        ///	
        ///	By substituting  and 
        ///	with their Taylor series expansion, this translates into solving
        ///	the following the linear system:
        ///	
        ///	The resulting approximation of  is the following:
        ///	
        ///	It is worth noting that if 
        ///	(i.e., data are evenly spaced)
        ///	we find the standard second order approximation:
        ///	
        ///	With a similar procedure the forward/backward approximations used for
        ///	boundaries can be derived.<br></br>
        ///	
        ///	References
        /// </summary>
        /// <param name="f">
        ///	An N-dimensional array containing samples of a scalar function.
        /// </param>
        /// <param name="varargs">
        ///	Spacing between f values.<br></br>
        ///	Default unitary spacing for all dimensions.<br></br>
        ///	
        ///	Spacing can be specified using:
        ///	
        ///	If axis is given, the number of varargs must equal the number of axes.<br></br>
        ///	
        ///	Default: 1.
        /// </param>
        /// <param name="edge_order">
        ///	Gradient is calculated using N-th order accurate differences
        ///	at the boundaries.<br></br>
        ///	Default: 1.
        /// </param>
        /// <param name="axis">
        ///	Gradient is calculated only along the given axis or axes
        ///	The default (axis = None) is to calculate the gradient for all the axes
        ///	of the input array.<br></br>
        ///	axis may be negative, in which case it counts from
        ///	the last to the first axis.
        /// </param>
        /// <returns>
        ///	A set of ndarrays (or a single ndarray if there is only one dimension)
        ///	corresponding to the derivatives of f with respect to each dimension.<br></br>
        ///	
        ///	Each derivative has the same shape as f.
        /// </returns>
        public static NDarray gradient(NDarray f, List<double> varargs, int? edge_order = null, Axis axis = null)
        {
            //auto-generated code, do not change
            var __self__ = self;

            var pyargs = new PyObject[] { f.PyObject }.Concat(varargs.Select(x => new PyFloat(x))).ToArray();
            var kwargs = new PyDict();
            if (edge_order != null) kwargs["edge_order"] = ToPython(edge_order);
            if (axis != null) kwargs["axis"] = ToPython(axis);
            dynamic py = __self__.InvokeMethod("gradient", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	One-dimensional linear interpolation.<br></br>
        ///	
        ///	Returns the one-dimensional piecewise linear interpolant to a function
        ///	with given discrete data points (xp, fp), evaluated at x.<br></br>
        ///	
        ///	Notes
        ///	
        ///	Does not check that the x-coordinate sequence xp is increasing.<br></br>
        ///	
        ///	If xp is not increasing, the results are nonsense.<br></br>
        ///	
        ///	A simple check for increasing is:
        /// </summary>
        /// <param name="x">
        ///	The x-coordinates at which to evaluate the interpolated values.
        /// </param>
        /// <param name="xp">
        ///	The x-coordinates of the data points, must be increasing if argument
        ///	period is not specified.<br></br>
        ///	Otherwise, xp is internally sorted after
        ///	normalizing the periodic boundaries with xp = xp % period.
        /// </param>
        /// <param name="fp">
        ///	The y-coordinates of the data points, same length as xp.
        /// </param>
        /// <param name="left">
        ///	Value to return for x &lt; xp[0], default is fp[0].
        /// </param>
        /// <param name="right">
        ///	Value to return for x &gt; xp[-1], default is fp[-1].
        /// </param>
        /// <param name="period">
        ///	A period for the x-coordinates.<br></br>
        ///	This parameter allows the proper
        ///	interpolation of angular x-coordinates.<br></br>
        ///	Parameters left and right
        ///	are ignored if period is specified.
        /// </param>
        /// <returns>
        ///	The interpolated values, same shape as x.
        /// </returns>
        public static NDarray interp(this NDarray x, IReadOnlyCollection<float> xp, IReadOnlyCollection<float> fp, float? left = null, float? right = null, float? period = null)
        {
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                x,
                xp,
                fp,
            });
            var kwargs = new PyDict();
            if (left != null) kwargs["left"] = ToPython(left);
            if (right != null) kwargs["right"] = ToPython(right);
            if (period != null) kwargs["period"] = ToPython(period);
            dynamic py = __self__.InvokeMethod("interp", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        public static float interp(float x, IReadOnlyCollection<float> xp, IReadOnlyCollection<float> fp, float? left = null, float? right = null, float? period = null)
        {
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                x,
                xp,
                fp,
            });
            var kwargs = new PyDict();
            if (left != null) kwargs["left"] = ToPython(left);
            if (right != null) kwargs["right"] = ToPython(right);
            if (period != null) kwargs["period"] = ToPython(period);
            dynamic py = __self__.InvokeMethod("interp", pyargs, kwargs);
            return ToCsharp<float>(py);
        }

        public static NDarray interp(this NDarray x, IReadOnlyCollection<float> xp, Complex[] fp, Complex? left = null, Complex? right = null, float? period = null)
        {
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                x,
                xp,
                np.array(fp),
            });
            var kwargs = new PyDict();
            if (left != null) kwargs["left"] = ToPython(left);
            if (right != null) kwargs["right"] = ToPython(right);
            if (period != null) kwargs["period"] = ToPython(period);
            dynamic py = __self__.InvokeMethod("interp", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }
    }
}
