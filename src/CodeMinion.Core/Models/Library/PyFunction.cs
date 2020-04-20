using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Models
{
    /// <summary>
    /// Information about Python function
    /// </summary>
    public class PyFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PyFunction"/> class.
        /// </summary>
        public PyFunction()
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
        /// Gets or sets the type of the return.
        /// </summary>
        /// <value>
        /// The type of the return.
        /// </value>
        public string ReturnType { get; set; }

        /// <summary>
        /// Gets or sets the return argument.
        /// </summary>
        /// <value>
        /// The return argument.
        /// </value>
        public string ReturnArg { get; set; }

        /// <summary>
        /// Gets or sets the document string.
        /// </summary>
        /// <value>
        /// The document string.
        /// </value>
        public string DocStr { get; set; }

        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public List<PyFuncArg> Parameters { get; set; }
    }
}
