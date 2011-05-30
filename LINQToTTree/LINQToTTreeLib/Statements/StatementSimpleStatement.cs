using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Statement that holds onto a generic line. This could be, for example, a single line with side-effects that
    /// has been processed by GetExpression.
    /// </summary>
    public class StatementSimpleStatement : IStatement
    {
        public StatementSimpleStatement(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line can't be null!");

            ///
            /// Strip off all ending semi-colons. They will get added back
            /// in when we "codeitup".
            /// 

            Line = line.Trim();
            while (Line.EndsWith(";"))
            {
                Line = Line.Substring(0, Line.Length - 1);
                Line = Line.Trim();
            }

            ///
            /// Empty lines just aren't allowed! :-)
            /// 

            if (string.IsNullOrWhiteSpace(Line))
                throw new ArgumentException("line can't be empty");

        }
        public string Line { get; private set; }

        /// <summary>
        /// Return the simple statement, along with a semi-colon!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return Line + ";";
        }
    }
}
