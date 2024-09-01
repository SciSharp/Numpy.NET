﻿using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace Numpy
{
    public partial class NDarray
    {
        // this is needed for arithmetic operations where we need to call the module "operator", i.e. value/ndarray
        public static PyObject operator_module;

        //------------------------------
        // Comparison operators:
        //------------------------------

        // Return self<value.
        public static NDarray<bool> operator <(NDarray a, ValueType obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__lt__", obj.ToPython()));
        }

        // Return self<=value.
        public static NDarray<bool> operator <=(NDarray a, ValueType obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__le__", obj.ToPython()));
        }

        // Return self>value.
        public static NDarray<bool> operator >(NDarray a, ValueType obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__gt__", obj.ToPython()));
        }

        // Return self>=value.
        public static NDarray<bool> operator >=(NDarray a, ValueType obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__ge__", obj.ToPython()));
        }

        // NOTE: overloading == and != with Python's functionality would cause compile errors throughout all of the code
        //// Return self==value.
        //public static NDarray<bool> operator ==(NDarray a, ValueType obj)
        //{
        //    return new NDarray<bool>(a.self.InvokeMethod("__eq__", obj.ToPython()));
        //}

        // NOTE: overloading == and != with Python's functionality would cause compile errors throughout all of the code
        //// Return self==value.
        //public static NDarray<bool> operator !=(NDarray a, ValueType obj)
        //{
        //    return new NDarray<bool>(a.self.InvokeMethod("__ne__", obj.ToPython()));
        //}

        /// <summary>
        /// Returns an array of bool where the elements of the array are == value
        /// </summary>
        public NDarray<bool> equals(ValueType obj)
        {
            return new NDarray<bool>(self.InvokeMethod("__eq__", obj.ToPython()));
        }

        /// <summary>
        /// Returns an array of bool where the elements of the array are != value
        /// </summary>
        public NDarray<bool> not_equals(ValueType obj)
        {
            return new NDarray<bool>(self.InvokeMethod("__ne__", obj.ToPython()));
        }

        // Return element-wise self<array.
        public static NDarray<bool> operator <(NDarray a, NDarray obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__lt__", obj.self));
        }

        // Return element-wise self<=array.
        public static NDarray<bool> operator <=(NDarray a, NDarray obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__le__", obj.self));
        }

        // Return element-wise self>array.
        public static NDarray<bool> operator >(NDarray a, NDarray obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__gt__", obj.self));
        }

        // Return element-wise self>=array.
        public static NDarray<bool> operator >=(NDarray a, NDarray obj)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__ge__", obj.self));
        }

        /// <summary>
        /// Returns an array of bool where the elements of the array are == array element-wise
        /// </summary>
        public NDarray<bool> equals(NDarray obj)
        {
            return new NDarray<bool>(self.InvokeMethod("__eq__", obj.self));
        }

        /// <summary>
        /// Returns an array of bool where the elements of the array are != array element-wise
        /// </summary>
        public NDarray<bool> not_equals(NDarray obj)
        {
            return new NDarray<bool>(self.InvokeMethod("__ne__", obj.self));
        }

        //------------------------------
        // Truth value of an array(bool) :
        //------------------------------

        /// <summary>
        /// Note
        /// Truth-value testing of an array invokes ndarray.__nonzero__, which raises an error if the
        /// number of elements in the array is larger than 1, because the truth value of such arrays is
        /// ambiguous.Use.any() and.all() instead to be clear about what is meant in such cases.
        /// (If the number of elements is 0, the array evaluates to False.)
        /// </summary>
        public static NDarray<bool> nonzero(NDarray a)
        {
            return new NDarray<bool>(a.self.InvokeMethod("__nonzero__"));
        }

        //------------------------------
        // Unary operations:
        //------------------------------

        // Return 	-self
        public static NDarray operator -(NDarray a)
        {
            return new NDarray(a.self.InvokeMethod("__neg__"));
        }

        // Return 	+self
        public static NDarray operator +(NDarray a)
        {
            return new NDarray(a.self.InvokeMethod("__pos__"));
        }

        // ndarray.__abs__(self)  // C# doesn't have an operator for that

        // Return 	~self
        public static NDarray operator ~(NDarray a)
        {
            return new NDarray(a.self.InvokeMethod("__invert__"));
        }

        //------------------------------
        // Arithmetic operators:
        //------------------------------

        // Return self+value.
        public static NDarray operator +(NDarray a, ValueType obj)
        {
            return new NDarray(a.self.InvokeMethod("__add__", obj.ToPython()));
        }

        // Return value+self.
        public static NDarray operator +(ValueType obj, NDarray a)
        {
            return new NDarray(a.self.InvokeMethod("__add__", obj.ToPython()));
        }

        // Return self-value.
        public static NDarray operator -(NDarray a, ValueType obj)
        {
            return new NDarray(a.self.InvokeMethod("__sub__", obj.ToPython()));
        }

        // Return value-self.
        public static NDarray operator -(ValueType obj, NDarray a)
        {
            var firstElemAsArray = np.asarray(obj);
            return new NDarray(firstElemAsArray.self.InvokeMethod("__sub__", a.self));
        }

        // Return self*value.
        public static NDarray operator *(NDarray a, ValueType obj)
        {
            return new NDarray(a.self.InvokeMethod("__mul__", obj.ToPython()));
        }

        // Return value*self.
        public static NDarray operator *(ValueType obj, NDarray a)
        {
            return new NDarray(a.self.InvokeMethod("__mul__", obj.ToPython()));
        }

        // Return self/value.
        public static NDarray operator /(NDarray a, ValueType obj)
        {
            return new NDarray(a.self.InvokeMethod("__truediv__", obj.ToPython()));
        }

        // Return value/self.
        public static NDarray operator /(ValueType obj, NDarray a)
        {
            if (operator_module == null)
                operator_module = Py.Import("operator");
            return new NDarray(operator_module.InvokeMethod("__truediv__", obj.ToPython(), a.self));
        }

        // Return element-wise self+array.
        public static NDarray operator +(NDarray a, NDarray obj)
        {
            return new NDarray(a.self.InvokeMethod("__add__", obj.self));
        }

        // Return element-wise self-array.
        public static NDarray operator -(NDarray a, NDarray obj)
        {
            return new NDarray(a.self.InvokeMethod("__sub__", obj.self));
        }

        // Return element-wise self*array.
        public static NDarray operator *(NDarray a, NDarray obj)
        {
            return new NDarray(a.self.InvokeMethod("__mul__", obj.self));
        }

        // Return element-wise self/array.
        public static NDarray operator /(NDarray a, NDarray obj)
        {
            return new NDarray(a.self.InvokeMethod("__truediv__", obj.self));
        }

        ///// <summary>
        ///// Return self/value.
        ///// </summary>
        //public static NDarray truediv(NDarray a, ValueType obj)
        //{
        //    return new NDarray(a.self.InvokeMethod("__truediv__", obj.ToPython()));
        //}

        /// <summary>
        /// Return self//value. 
        /// </summary>
        public NDarray floordiv(NDarray a, ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__floordiv__", obj.ToPython()));
        }

        // Return self%value.
        public static NDarray operator %(NDarray a, ValueType obj)
        {
            return new NDarray(a.self.InvokeMethod("__mod__", obj.ToPython()));
        }

        /// <summary>
        /// Return divmod(value). 
        /// </summary>
        public NDarray divmod(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__divmod__", obj.ToPython()));
        }

        /// <summary>
        /// Return pow(value). 
        /// </summary>
        public NDarray pow(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__pow__", obj.ToPython()));
        }

        /// <summary>
        /// Return self&lt;&lt;value.
        /// </summary>
        public static NDarray operator <<(NDarray a, int obj)
        {
            return new NDarray(a.self.InvokeMethod("__lshift__", obj.ToPython()));
        }

        /// <summary>
        /// Return self&gt;&gt;value.
        /// </summary>
        public static NDarray operator >>(NDarray a, int obj)
        {
            return new NDarray(a.self.InvokeMethod("__rshift__", obj.ToPython()));
        }

        /// <summary>
        /// Return self&value.
        /// </summary>
        public static NDarray operator &(NDarray a, int obj)
        {
            return new NDarray(a.self.InvokeMethod("__and__", obj.ToPython()));
        }

        /// <summary>
        /// Return self&value.
        /// </summary>
        public static NDarray operator &(NDarray a, NDarray b)
        {
            return new NDarray(a.self.InvokeMethod("__and__", b.self));
        }

        /// <summary>
        /// Return self|value.
        /// </summary>
        public static NDarray operator |(NDarray a, int obj)
        {
            return new NDarray(a.self.InvokeMethod("__or__", obj.ToPython()));
        }

        /// <summary>
        /// Return self|value.
        /// </summary>
        public static NDarray operator |(NDarray a, NDarray b)
        {
            return new NDarray(a.self.InvokeMethod("__or__", b.self));
        }

        /// <summary>
        /// Return self^value.
        /// </summary>
        public static NDarray operator ^(NDarray a, int obj)
        {
            return new NDarray(a.self.InvokeMethod("__xor__", obj.ToPython()));
        }

        /// <summary>
        /// Return self^value.
        /// </summary>
        public static NDarray operator ^(NDarray a, NDarray b)
        {
            return new NDarray(a.self.InvokeMethod("__xor__", b.self));
        }

        //------------------------------
        // Arithmetic, in-place:
        //------------------------------

        /// <summary>
        /// Return self+=value.
        /// </summary>
        public NDarray iadd(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__iadd__", obj.ToPython()));
        }

        /// <summary>
        /// Return self-=value.
        /// </summary>
        public NDarray isub(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__isub__", obj.ToPython()));
        }

        /// <summary>
        /// Return self*=value.
        /// </summary>
        public NDarray imul(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__imul__", obj.ToPython()));
        }

        /// <summary>
        /// Return self/=value.
        /// </summary>
        public NDarray idiv(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__itruediv__", obj.ToPython()));
        }

        /// <summary>
        /// Return self/=value.
        /// </summary>
        public NDarray itruediv(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__itruediv__", obj.ToPython()));
        }

        /// <summary>
        /// Return self//=value. 
        /// </summary>
        public NDarray ifloordiv(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__floordiv__", obj.ToPython()));
        }

        /// <summary>
        /// Return self%value.
        /// </summary>
        public NDarray imod(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__imod__", obj.ToPython()));
        }

        /// <summary>
        /// Return inplace pow(value). 
        /// </summary>
        public NDarray ipow(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__ipow__", obj.ToPython()));
        }

        /// <summary>
        /// Return inplace self&lt;&lt;value.
        /// </summary>
        public NDarray ilshift(int obj)
        {
            return new NDarray(self.InvokeMethod("__ilshift__", obj.ToPython()));
        }

        /// <summary>
        /// Return inplace self&gt;&gt;value.
        /// </summary>
        public NDarray irshift(int obj)
        {
            return new NDarray(self.InvokeMethod("__irshift__", obj.ToPython()));
        }

        /// <summary>
        /// Return self&=value.
        /// </summary>
        public NDarray iand(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__iand__", obj.ToPython()));
        }

        /// <summary>
        /// Return self|=value.
        /// </summary>
        public NDarray ior(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__ior__", obj.ToPython()));
        }

        /// <summary>
        /// Return self^=value.
        /// </summary>
        public NDarray ixor(ValueType obj)
        {
            return new NDarray(self.InvokeMethod("__ixor__", obj.ToPython()));
        }

        /// <summary>
        /// Return self+=NDarray.
        /// </summary>
        public NDarray iadd(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__iadd__", obj.self));
        }

        /// <summary>
        /// Return self-=NDarray.
        /// </summary>
        public NDarray isub(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__isub__", obj.self));
        }

        /// <summary>
        /// Return self*=NDarray.
        /// </summary>
        public NDarray imul(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__imul__", obj.self));
        }

        /// <summary>
        /// Return self/=NDarray.
        /// </summary>
        public NDarray idiv(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__idiv__", obj.self));
        }

        /// <summary>
        /// Return self/=NDarray.
        /// </summary>
        public NDarray itruediv(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__itruediv__", obj.self));
        }

        /// <summary>
        /// Return self//=NDarray. 
        /// </summary>
        public NDarray ifloordiv(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__floordiv__", obj.self));
        }

        /// <summary>
        /// Return self%NDarray.
        /// </summary>
        public NDarray imod(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__imod__", obj.self));
        }

        /// <summary>
        /// Return inplace pow(NDarray). 
        /// </summary>
        public NDarray ipow(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__ipow__", obj.self));
        }

        /// <summary>
        /// Return inplace self&lt;&lt;NDarray.
        /// </summary>
        public NDarray ilshift(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__ilshift__", obj.self));
        }

        /// <summary>
        /// Return inplace self&gt;&gt;NDarray.
        /// </summary>
        public NDarray irshift(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__irshift__", obj.self));
        }

        /// <summary>
        /// Return self&=NDarray.
        /// </summary>
        public NDarray iand(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__iand__", obj.self));
        }

        /// <summary>
        /// Return self|=NDarray.
        /// </summary>
        public NDarray ior(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__ior__", obj.self));
        }

        /// <summary>
        /// Return self^=NDarray.
        /// </summary>
        public NDarray ixor(NDarray obj)
        {
            return new NDarray(self.InvokeMethod("__ixor__", obj.self));
        }

        // TODO:
        // ndarray.__matmul__($self, value, /)	Return self@value.
        //ndarray.__copy__() Used if copy.copy is called on an array.
        //ndarray.__deepcopy__(memo, /)   Used if copy.deepcopy is called on an array.
        //ndarray.__reduce__()    For pickling.
        //ndarray.__setstate__(state, /)  For unpickling.
        //ndarray.__contains__($self, key, /)	Return key in self.

        //ndarray.__int__(self)
        //ndarray.__long__
        //ndarray.__float__(self)
        //ndarray.__oct__
        //ndarray.__hex__
    }
}
