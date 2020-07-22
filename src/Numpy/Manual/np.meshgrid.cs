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
        ///	Return coordinate matrices from coordinate vectors.<br></br>
        ///	
        ///	Make N-D coordinate arrays for vectorized evaluations of
        ///	N-D scalar/vector fields over N-D grids, given
        ///	one-dimensional coordinate arrays x1, x2,…, xn.<br></br>
        ///	
        ///	Notes
        ///	
        ///	This function supports both indexing conventions through the indexing
        ///	keyword argument.<br></br>
        ///	  Giving the string ‘ij’ returns a meshgrid with
        ///	matrix indexing, while ‘xy’ returns a meshgrid with Cartesian indexing.<br></br>
        ///	
        ///	In the 2-D case with inputs of length M and N, the outputs are of shape
        ///	(N, M) for ‘xy’ indexing and (M, N) for ‘ij’ indexing.<br></br>
        ///	  In the 3-D case
        ///	with inputs of length M, N and P, outputs are of shape (N, M, P) for
        ///	‘xy’ indexing and (M, N, P) for ‘ij’ indexing.<br></br>
        ///	  The difference is
        ///	illustrated by the following code snippet:
        ///	
        ///	In the 1-D and 0-D case, the indexing and sparse keywords have no effect.
        /// </summary>
        /// <param name="xi">
        ///	1-D arrays representing the coordinates of a grid.
        /// </param>
        /// <param name="indexing">
        ///	Cartesian (‘xy’, default) or matrix (‘ij’) indexing of output.<br></br>
        ///	
        ///	See Notes for more details.
        /// </param>
        /// <param name="sparse">
        ///	If True a sparse grid is returned in order to conserve memory.<br></br>
        ///	
        ///	Default is False.
        /// </param>
        /// <param name="copy">
        ///	If False, a view into the original arrays are returned in order to
        ///	conserve memory.<br></br>
        ///	Default is True.<br></br>
        ///	Please note that
        ///	sparse=False, copy=False will likely return non-contiguous
        ///	arrays.<br></br>
        ///	Furthermore, more than one element of a broadcast array
        ///	may refer to a single memory location.<br></br>
        ///	If you need to write to the
        ///	arrays, make copies first.
        /// </param>
        /// <returns>
        ///	For vectors x1, x2,…, ‘xn’ with lengths Ni=len(xi) ,
        ///	return (N1, N2, N3,...Nn) shaped arrays if indexing=’ij’
        ///	or (N2, N1, N3,...Nn) shaped arrays if indexing=’xy’
        ///	with the elements of xi repeated to fill the matrix along
        ///	the first dimension for x1, the second for x2 and so on.
        /// </returns>
        public static NDarray[] meshgrid(NDarray[] xi, string indexing = "xy", bool? sparse = null, bool? copy = null)
        {
            var __self__ = self;
            var pyargs = ToTuple(xi);
            var kwargs = new PyDict();
            if (indexing != "xy") kwargs["indexing"] = ToPython(indexing);
            if (sparse != null) kwargs["sparse"] = ToPython(sparse);
            if (copy != null) kwargs["copy"] = ToPython(copy);
            dynamic py = __self__.InvokeMethod("meshgrid", pyargs, kwargs);
            return ToCsharp<NDarray[]>(py);
        }

        /// <summary>
        ///	Return coordinate matrices from coordinate vectors.<br></br>
        ///	
        ///	Make N-D coordinate arrays for vectorized evaluations of
        ///	N-D scalar/vector fields over N-D grids, given
        ///	one-dimensional coordinate arrays x1, x2,…, xn.<br></br>
        ///	
        ///	Notes
        ///	
        ///	This function supports both indexing conventions through the indexing
        ///	keyword argument.<br></br>
        ///	  Giving the string ‘ij’ returns a meshgrid with
        ///	matrix indexing, while ‘xy’ returns a meshgrid with Cartesian indexing.<br></br>
        ///	
        ///	In the 2-D case with inputs of length M and N, the outputs are of shape
        ///	(N, M) for ‘xy’ indexing and (M, N) for ‘ij’ indexing.<br></br>
        ///	  In the 3-D case
        ///	with inputs of length M, N and P, outputs are of shape (N, M, P) for
        ///	‘xy’ indexing and (M, N, P) for ‘ij’ indexing.<br></br>
        ///	  The difference is
        ///	illustrated by the following code snippet:
        ///	
        ///	In the 1-D and 0-D case, the indexing and sparse keywords have no effect.
        /// </summary>
        /// <param name="xi">
        ///	1-D arrays representing the coordinates of a grid.
        /// </param>
        /// <returns>
        ///	For vectors x1, x2,…, ‘xn’ with lengths Ni=len(xi) ,
        ///	return (N1, N2, N3,...Nn) shaped arrays if indexing=’ij’
        ///	or (N2, N1, N3,...Nn) shaped arrays if indexing=’xy’
        ///	with the elements of xi repeated to fill the matrix along
        ///	the first dimension for x1, the second for x2 and so on.
        /// </returns>
        public static NDarray[] meshgrid(params NDarray[] xi)
        {
            var __self__ = self;
            var pyargs = ToTuple(xi);
            var kwargs = new PyDict();
            dynamic py = __self__.InvokeMethod("meshgrid", pyargs, kwargs);
            return ToCsharp<NDarray[]>(py);
        }

    }
}
