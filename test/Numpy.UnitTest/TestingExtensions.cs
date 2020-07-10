using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numpy.UnitTest
{
    public static class TestingExtensions
    {
        // use this to simulate Python tuples, because we use arrays instead
        public static string repr(this NDarray[] self)
        {
            return "(" + string.Join(", ", self.Select(a => a.repr)) + ")";
        }
    }
}
