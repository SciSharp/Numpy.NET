using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Information about python class
    /// </summary>
    public class PyClass
    {
        public PyClass()
        {
            Parameters = new List<PyFuncArg>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        public PyFunction[] Functions { get; set; }

        /// <summary>
        /// Gets or sets the document string.
        /// </summary>
        /// <value>
        /// The document string.
        /// </value>
        public string DocStr { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public string[] Args { get; set; }

        /// <summary>
        /// Gets or sets the defaults.
        /// </summary>
        /// <value>
        /// The defaults.
        /// </value>
        public string[] Defaults { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public List<PyFuncArg> Parameters { get; set; }
    }
}
