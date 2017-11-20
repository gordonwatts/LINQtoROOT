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
        private IValue[] _items;

        public IStatement Parent { get; set; }

        /// <summary>
        /// Create a csv statement.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="item"></param>
        public StatementCSVDump(IValue stream, IValue[] items)
        {
            _stream = stream;
            _items = items;
        }
 
        /// <summary>
        /// Return the coding statements.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            var bld = new StringBuilder();
            foreach (var item in _items.Select(i => i.RawValue))
            {
                if (bld.Length != 0)
                {
                    bld.Append(" \",\" <<");
                }
                bld.AppendFormat($"({item}) <<");
            }
            yield return $"{_stream.RawValue} << {bld.ToString()} std::endl;";
        }

        /// <summary>
        /// Rename anything we are holding onto.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _stream.RenameRawValue(originalName, newName);
            foreach(var i in _items)
            {
                i.RenameRawValue(originalName, newName);
            }
        }

        /// <summary>
        /// Since this statement has an external side-effect, it can't be combined
        /// ever - it should never appear mroe than once.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            return false;
        }
    }
}
