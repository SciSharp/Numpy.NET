using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numpy.Models
{
    /// <summary>
    /// Special type for axis parameters that can be automatically cast implicitly from int or int[]
    /// </summary>
    public class Axis
    {
        /// <summary>
        /// Default axis, corresponds to axis=null in Numpy
        /// </summary>
        public Axis() { }

        /// <summary>
        /// Single axis, corresponds to axis=x in Numpy
        /// </summary>
        /// <param name="axis"></param>
        public Axis(int axis)
        {
            Axes = new[] {axis};
        }

        /// <summary>
        /// Multiple axes, corresponds to axis=(x,y, ...) in Numpy
        /// </summary>
        /// <param name="axes"></param>
        public Axis(params int[] axes)
        {
            Axes = axes;
        }

        public readonly int[] Axes = null;

        public static implicit operator Axis(int axis)
        {
            return new Axis(axis);
        }

        public static implicit operator Axis(int[] axes)
        {
            return new Axis(axes);
        }

        #region Equality


        public override bool Equals(object obj)
        {
            var b = obj as Axis;
            if (b == null)
            {
                if (Axes == null)
                    return true;
                else if (Axes.Length == 0)
                    return true;
                return false;
            }
            return Enumerable.SequenceEqual(Axes, b.Axes);
        }

        public static bool operator ==(Axis a, Axis b)
        {
            if (Object.ReferenceEquals(a, null) && Object.ReferenceEquals(b, null))
                return true;
            if (Object.ReferenceEquals(a, null))
                return b.Equals(a);
            return a.Equals(b);
        }

        public static bool operator !=(Axis a, Axis b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            if (Axes == null)
                return 0;
            return Axes.GetHashCode();
        }

        #endregion
    }


}
