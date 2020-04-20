using System;
using System.Collections.Generic;
using System.Text;
using CodeMinion.Core.Helpers;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Represents the methods of a non-static class which should be generated.
    /// 
    /// Note: the class itself is set up manually, this only generates more methods into that class.
    /// To generate a full class from scratch use ApiClass
    /// </summary>
    public class DynamicApi : Api
    {
        /// <summary>
        /// Class name is the name of a non-static class, i.e. NDArray
        /// </summary>
        public string ClassName { get; set; }
    }

    /// <summary>
    /// Represents a non-static class which should be generated. 
    /// </summary>
    public class ApiClass : Api
    {
        public string DocString;

        /// <summary>
        /// Class name is the name of a non-static class, i.e. NDArray
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Do not generate this class
        /// </summary>
        public bool Ignore { get; set; }

        public string BaseClass { get; set; } = "PythonObject";

        public List<Function> Constructors = new List<Function>();
    }
}
