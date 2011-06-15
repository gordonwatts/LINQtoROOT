using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Someone is using a break statement to pop out of a loop.
    /// </summary>
    class StatementBreak : IStatement
    {
        public IEnumerable<string> CodeItUp()
        {
            yield return "break;";
        }
    }
}
