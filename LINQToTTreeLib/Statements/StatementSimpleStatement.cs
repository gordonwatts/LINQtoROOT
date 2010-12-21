using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Line = line;
        }
        public string Line { get; private set; }
    }
}
