using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMinion
{
    public static class RegexExtensions
    {
        /// <summary>
        /// Returns the first explicit group or null if no match
        ///
        /// For example: if the regex is "ab(cd)ef" it will return cd
        ///     if the regex is "ab(cd(ef))" it will return cdef
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string FirstGroupOrNull(this Match self)
        {
            if (!self.Success || self.Groups.Count < 2)
                return null;
            return self.Groups[1].Value;
        }
    }
}
