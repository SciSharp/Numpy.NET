using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Models
{
    public class TestFile
    {
        /// <summary>
        /// Name of the test file without extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The tests in that test file
        /// </summary>
        public List<TestCase> TestCases { get; set; } = new List<TestCase>();

        public string SubDir { get; set; }
    }
}
