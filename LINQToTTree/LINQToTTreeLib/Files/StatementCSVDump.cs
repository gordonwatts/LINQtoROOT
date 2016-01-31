using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Dumps a list of values to the some output.
    /// </summary>
    class StatementCSVDump : IStatement
    {
        private IValue _stream;
        private IValue _item;

        public IStatement Parent { get; set; }

        /// <summary>
        /// Create a csv statement.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="item"></param>
        public StatementCSVDump(IValue stream, IValue item)
        {
            _stream = stream;
            _item = item;
        }
 
        /// <summary>
        /// Return the coding statements.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return $"{_stream.RawValue} << {_item.RawValue} << std::endl;";
        }

        /// <summary>
        /// Rename anything we are holding onto.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _stream.RenameRawValue(originalName, newName);
            _item.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// Figure out if we can combine these guys.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            throw new NotImplementedException();
        }
    }
}
