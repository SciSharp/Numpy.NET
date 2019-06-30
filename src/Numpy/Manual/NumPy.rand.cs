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

    public partial class NumPy
    {

        /// <summary>
        ///	Random values in a given shape.<br></br>
        ///	
        ///	Create an array of the given shape and populate it with
        ///	random samples from a uniform distribution
        ///	over [0, 1).<br></br>
        ///	
        ///	Notes
        ///	
        ///	This is a convenience function.<br></br>
        ///	 If you want an interface that
        ///	takes a shape-tuple as the first argument, refer to
        ///	np.random.random_sample .
        /// </summary>
        /// <returns>
        ///	Random values.
        /// </returns>
        public NDarray random_rand(params int[] shape)
        {
            //auto-generated code, do not change
            var random = self.GetAttr("random");
            var __self__ = random;
            var pyargs = ToTuple(shape);
            var kwargs = new PyDict();
            dynamic py = __self__.InvokeMethod("rand", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

        /// <summary>
        ///	Return a sample (or samples) from the “standard normal” distribution.<br></br>
        ///	
        ///	If positive, int_like or int-convertible arguments are provided,
        ///	randn generates an array of shape (d0, d1, ..., dn), filled
        ///	with random floats sampled from a univariate “normal” (Gaussian)
        ///	distribution of mean 0 and variance 1 (if any of the  are
        ///	floats, they are first converted to integers by truncation).<br></br>
        ///	 A single
        ///	float randomly sampled from the distribution is returned if no
        ///	argument is provided.<br></br>
        ///	
        ///	This is a convenience function.<br></br>
        ///	  If you want an interface that takes a
        ///	tuple as the first argument, use numpy.random.standard_normal instead.<br></br>
        ///	
        ///	Notes
        ///	
        ///	For random samples from , use:
        ///	
        ///	sigma * np.random.randn(...) + mu
        /// </summary>
        /// <returns>
        ///	A (d0, d1, ..., dn)-shaped array of floating-point samples from
        ///	the standard normal distribution, or a single such float if
        ///	no parameters were supplied.
        /// </returns>
        public NDarray random_randn(params int[] shape)
        {
            //auto-generated code, do not change
            var random = self.GetAttr("random");
            var __self__ = random;
            var pyargs = ToTuple(shape);
            var kwargs = new PyDict();
            dynamic py = __self__.InvokeMethod("randn", pyargs, kwargs);
            return ToCsharp<NDarray>(py);
        }

    }
}
