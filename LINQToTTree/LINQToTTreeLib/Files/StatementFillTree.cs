using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Files
{
    class StatementFillTree : IStatement
    {
        /// <summary>
        /// The stream info (a make_pair) that contains a TFile and TTree. For this
        /// statement we care only about the TTree pointer.
        /// </summary>
        private DeclarableParameter _fileAndTreePair;

        /// <summary>
        /// The values of everything we want to write out.
        /// </summary>
        private Tuple<IValue, string>[] _columnValues;

        /// <summary>
        /// Generate the code for a tree filling.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        public StatementFillTree(DeclarableParameter stream, Tuple<IValue, string>[] value)
        {
            this._fileAndTreePair = stream;
            this._columnValues = value;
        }

        /// <summary>
        /// Generate the code to fill the ntuple with the variables required.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            // Create the column info, including the variables where we will be caching everything.
            var fullLeafInfo = _columnValues
                .Select(l => new
                {
                    Val = l.Item1,
                    ColHeader = l.Item2,
                    Param = DeclarableParameter.CreateDeclarableParameterExpression(l.Item1.Type)
                })
                .ToArray();

            // Declare the static variables where we will be saving the values as they go.
            foreach (var leaf in fullLeafInfo)
            {
                yield return $"static {leaf.Param.Type.AsCPPType()} {leaf.Param.RawValue} = 0;";
            }

            // Book the leaves if this is the first time through.
            var isFirst = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            yield return $"static bool {isFirst.RawValue} = true;";
            yield return $"if ({isFirst.RawValue}) {{";
            yield return $"  {isFirst.RawValue} = false;";

            foreach (var leaf in fullLeafInfo)
            {
                yield return $"  {_fileAndTreePair.RawValue}.second->Branch(\"{leaf.ColHeader}\", &{leaf.Param.RawValue});";
            }

            yield return $"}}";

            // Now, set the variables that we need to fill
            foreach (var leaf in fullLeafInfo)
            {
                yield return $"{leaf.Param.RawValue} = {leaf.Val.RawValue};";
            }

            // And finally have ROOT collect them all.
            yield return $"{_fileAndTreePair.RawValue}.second->Fill();";
        }

        /// <summary>
        /// Make sure to propagate the rename through everything we have here.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            foreach (var variable in _columnValues)
            {
                variable.Item1.RenameRawValue(originalName, newName);
            }
        }

        /// <summary>
        /// This statement has side effects, so we should never have to deal with it here.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            return false;
        }

        /// <summary>
        /// Track who is our parent, in which block we are currently being held.
        /// </summary>
        public IStatement Parent { get; set; }

    }
}
