using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Statement that holds onto a generic line. This could be, for example, a single line with side-effects that
    /// has been processed by GetExpression.
    /// </summary>
    public class StatementSimpleStatement : IStatement
    {
        public StatementSimpleStatement(string line, bool addSemicolon = true)
        {
            if (line == null)
                throw new ArgumentNullException("line can't be null!");

            AddSemicolon = addSemicolon;

            ///
            /// Strip off all ending semi-colons. They will get added back
            /// in when we "codeitup".
            /// 

            Line = line.Trim();
            if (addSemicolon)
            {
                while (Line.EndsWith(";"))
                {
                    Line = Line.Substring(0, Line.Length - 1);
                    Line = Line.Trim();
                }
            }

            ///
            /// Empty lines just aren't allowed! :-)
            /// 

            if (string.IsNullOrWhiteSpace(Line))
                throw new ArgumentException("line can't be empty");

        }
        public string Line { get; private set; }

        /// <summary>
        /// Will a semicolon be added to this line when it is dumped?
        /// </summary>
        public bool AddSemicolon { get; private set; }

        /// <summary>
        /// Return the simple statement, along with a semi-colon!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            string semi = AddSemicolon ? ";" : "";
            yield return Line + semi;
        }

        /// <summary>
        /// Pretty sipmle x-check to see if the statement is the same. Since this
        /// is just a line...
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            var other = statement as StatementSimpleStatement;
            if (other == null)
                return false;
            return Line == other.Line && AddSemicolon == other.AddSemicolon;

        }

        /// <summary>
        /// Rename any variables we have in here.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            Line.ReplaceVariableNames(originalName, newName);
        }
    }
}
