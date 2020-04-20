using System;
using System.Collections.Generic;
using System.Text;

namespace CodeMinion.Core.Helpers
{
    public class CodeWriter
    {

        public CodeWriter() { }

        public CodeWriter(StringBuilder s)
        {
            StringBuilder = s;
        }

        public StringBuilder StringBuilder { get; set; } = new StringBuilder();
        public int IndentSpaces { get; set; } = 4;
        private int _level = 0;

        /// <summary>
        /// Write a line (with automatic indentation), line break is appended
        /// </summary>
        public void Out(string line, Action block=null)
        {
            if (_level > 0)
                StringBuilder.Append(new String(' ', IndentSpaces * _level));
            StringBuilder.AppendLine(line);
            if (block==null)
                return;
            Block(block);
        }

        /// <summary>
        /// Alias of Out
        /// </summary>
        public void AppendLine(string s="")
        {
            Out(s);
        }

        /// <summary>
        /// Insert an empty line
        /// </summary>
        public void Break()
        {
            Out("");
        }

        /// <summary>
        /// Increase the level of indentation
        /// </summary>
        public void Indent()
        {
             _level++;
        }

        /// <summary>
        /// Decrease the level of indentation
        /// </summary>
        public void Outdent()
        {
            _level--;
        }

        /// <summary>
        /// Increase the level of indentation for the generator action, then outdent
        /// </summary>
        public void Indent(Action a)
        {
            _level++;
            try
            {
                a();
            }
            finally
            {
                _level--;
            }
        }

        /// <summary>
        /// Generate an indented code block
        /// </summary>
        /// <param name="a"></param>
        /// <param name="opening_brace"></param>
        /// <param name="closing_brace"></param>
        public void Block(Action a, string opening_brace = "{", string closing_brace = "}")
        {
            Out(opening_brace);
            Indent(a);
            Out(closing_brace);
        }

        public override string ToString()
        {
            return StringBuilder.ToString();
        }
    }
}
