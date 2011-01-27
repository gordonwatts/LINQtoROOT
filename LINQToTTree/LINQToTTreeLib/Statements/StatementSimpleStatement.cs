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
            Line = line.Trim();
            if (Line.EndsWith(";"))
                Line = Line.Substring(0, Line.Length - 1);
        }
        public string Line { get; private set; }

        public IEnumerable<string> CodeItUp()
        {
            yield return Line + ";";
        }
    }
}
