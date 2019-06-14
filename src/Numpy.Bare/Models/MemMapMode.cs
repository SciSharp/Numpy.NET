using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Numpy.Models
{
    public class MemMapMode : PythonObject
    {
        public MemMapMode(PyObject pyobject) : base(pyobject)
        {
        }

    }
}
