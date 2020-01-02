using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Numpy
{
    public partial class PythonObject : IDisposable
    {
        protected PyObject self; // can not be made readonly because of NDarray(IntPtr ... )
        public dynamic PyObject => self;

        public IntPtr Handle => self.Handle;

        public PythonObject(PyObject pyobject)
        {
            this.self = pyobject;
        }

        public PythonObject(PythonObject t)
        {
            this.self = t.PyObject;
        }

        protected PythonObject() { } // required for some constructors

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            switch (obj)
            {
                case PythonObject other:
                    return self.Equals(other.self);
                case PyObject other:
                    return self.Equals(other);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return self.GetHashCode();
        }

        public override string ToString()
        {
            return self.ToString();
        }

        public void Dispose()
        {
            self?.Dispose();
        }

        public static PythonObject Create<T>(string python_class)
        {
           throw new NotImplementedException();
        }
    }
}
