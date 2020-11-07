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
        ///	Save an array to a binary file in NumPy .npy format.<br></br>
        ///	
        ///	Notes
        ///	
        ///	For a description of the .npy format, see numpy.lib.format.
        /// </summary>
        /// <param name="file">
        ///	File or filename to which the data is saved.<br></br>
        ///	If file is a file-object,
        ///	then the filename is unchanged.<br></br>
        ///	If file is a string or Path, a .npy
        ///	extension will be appended to the file name if it does not already
        ///	have one.
        /// </param>
        /// <param name="arr">
        ///	Array data to be saved.
        /// </param>
        /// <param name="allow_pickle">
        ///	Allow saving object arrays using Python pickles.<br></br>
        ///	Reasons for disallowing
        ///	pickles include security (loading pickled data can execute arbitrary
        ///	code) and portability (pickled objects may not be loadable on different
        ///	Python installations, for example if the stored objects require libraries
        ///	that are not available, and not all pickled data is compatible between
        ///	Python 2 and Python 3).<br></br>
        ///	
        ///	Default: True
        /// </param>
        /// <param name="fix_imports">
        ///	Only useful in forcing objects in object arrays on Python 3 to be
        ///	pickled in a Python 2 compatible way.<br></br>
        ///	If fix_imports is True, pickle
        ///	will try to map the new Python 3 names to the old module names used in
        ///	Python 2, so that the pickle data stream is readable with Python 2.
        /// </param>
        public static void save(string file, NDarray arr, bool? allow_pickle = true, bool? fix_imports = true)
        {
            //auto-generated code, do not change
            var __self__ = self;
            var pyargs = ToTuple(new object[]
            {
                file,
                arr,
            });
            var kwargs = new PyDict();
            if (allow_pickle != true) kwargs["allow_pickle"] = ToPython(allow_pickle);
            if (fix_imports != true) kwargs["fix_imports"] = ToPython(fix_imports);
            dynamic py = __self__.InvokeMethod("save", pyargs, kwargs);
        }

        /// <summary>
        ///	Save several arrays into a single file in uncompressed .npz format.<br></br>
        ///	
        ///	If arguments are passed in with no keywords, the corresponding variable
        ///	names, in the .npz file, are ‘arr_0’, ‘arr_1’, etc.<br></br>
        ///	 If keyword
        ///	arguments are given, the corresponding variable names, in the .npz
        ///	file will match the keyword names.<br></br>
        ///	
        ///	Notes
        ///	
        ///	The .npz file format is a zipped archive of files named after the
        ///	variables they contain.<br></br>
        ///	  The archive is not compressed and each file
        ///	in the archive contains one variable in .npy format.<br></br>
        ///	 For a
        ///	description of the .npy format, see numpy.lib.format.<br></br>
        ///	
        ///	When opening the saved .npz file with load a NpzFile object is
        ///	returned.<br></br>
        ///	 This is a dictionary-like object which can be queried for
        ///	its list of arrays (with the .files attribute), and for the arrays
        ///	themselves.
        /// </summary>
        /// <param name="file">
        ///	Either the file name (string) or an open file (file-like object)
        ///	where the data will be saved.<br></br>
        ///	If file is a string or a Path, the
        ///	.npz extension will be appended to the file name if it is not
        ///	already there.
        /// </param>
        /// <param name="args">
        ///	Arrays to save to the file.<br></br>
        ///	Since it is not possible for Python to
        ///	know the names of the arrays outside savez, the arrays will be saved
        ///	with names “arr_0”, “arr_1”, and so on.<br></br>
        ///	These arguments can be any
        ///	expression.
        /// </param>
        /// <param name="kwds">
        ///	Arrays to save to the file.<br></br>
        ///	Arrays will be saved in the file with the
        ///	keyword names.
        /// </param>
        public static void savez(string file, NDarray[] args = null, Dictionary<string, NDarray> kwds = null)
        {
            var __self__ = self;
            var pyargs = ToTuple(new object[] { file, }.Concat(args ?? new NDarray[0]).ToArray());
            var kwargs = new PyDict();
            if (kwds != null)
            {
                foreach(var pair in kwds)
                    kwargs[pair.Key] = ToPython(pair.Value);
            }
            dynamic py = __self__.InvokeMethod("savez", pyargs, kwargs);
        }

        /// <summary>
        ///	Save several arrays into a single file in compressed .npz format.<br></br>
        ///	
        ///	If keyword arguments are given, then filenames are taken from the keywords.<br></br>
        ///	
        ///	If arguments are passed in with no keywords, then stored file names are
        ///	arr_0, arr_1, etc.<br></br>
        ///	
        ///	Notes
        ///	
        ///	The .npz file format is a zipped archive of files named after the
        ///	variables they contain.<br></br>
        ///	  The archive is compressed with
        ///	zipfile.ZIP_DEFLATED and each file in the archive contains one variable
        ///	in .npy format.<br></br>
        ///	 For a description of the .npy format, see 
        ///	numpy.lib.format.<br></br>
        ///	
        ///	When opening the saved .npz file with load a NpzFile object is
        ///	returned.<br></br>
        ///	 This is a dictionary-like object which can be queried for
        ///	its list of arrays (with the .files attribute), and for the arrays
        ///	themselves.
        /// </summary>
        /// <param name="file">
        ///	Either the file name (string) or an open file (file-like object)
        ///	where the data will be saved.<br></br>
        ///	If file is a string or a Path, the
        ///	.npz extension will be appended to the file name if it is not
        ///	already there.
        /// </param>
        /// <param name="args">
        ///	Arrays to save to the file.<br></br>
        ///	Since it is not possible for Python to
        ///	know the names of the arrays outside savez, the arrays will be saved
        ///	with names “arr_0”, “arr_1”, and so on.<br></br>
        ///	These arguments can be any
        ///	expression.
        /// </param>
        /// <param name="kwds">
        ///	Arrays to save to the file.<br></br>
        ///	Arrays will be saved in the file with the
        ///	keyword names.
        /// </param>
        public static void savez_compressed(string file, NDarray[] args = null, Dictionary<string, NDarray> kwds = null)
        {
            var __self__ = self;
            var pyargs = ToTuple(new object[] { file, }.Concat(args ?? new NDarray[0]).ToArray());
            var kwargs = new PyDict();
            if (kwds != null)
            {
                foreach (var pair in kwds)
                    kwargs[pair.Key] = ToPython(pair.Value);
            }
            dynamic py = __self__.InvokeMethod("savez_compressed", pyargs, kwargs);
        }

    }
}
