// Copyright (c) 2019 by Meinrad Recheis and the SciSharp Team
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;
using Numpy.Models;
using Python.Included;

namespace Numpy
{
    public static partial class ctypes
    {

        public static PyObject self => _lazy_self.Value;

        private static Lazy<PyObject> _lazy_self = new Lazy<PyObject>(() =>
        {
            var x=np.self; // <-- make sure np initializes the python engine
            var mod = Py.Import("ctypes");
            return mod;
        });

        public static dynamic dynamic_self => self;
        private static bool IsInitialized => self != null;

        public static void Dispose()
        {
            self?.Dispose();
        }

        
    }
}


