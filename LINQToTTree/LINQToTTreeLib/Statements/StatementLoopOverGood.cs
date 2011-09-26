
using System;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Loop over a set of indicies only if they are marked "true" in the accompanying array.
    /// </summary>
    public class StatementLoopOverGood : StatementInlineBlockBase
    {
        private IValue _indiciesToCheck;
        private IValue _indexIsGood;
        private IDeclaredParameter _index;

        /// <summary>
        /// Simple loop over a set of indicies, passing on only those that satisfy the actual index.
        /// </summary>
        /// <param name="indiciesToCheck"></param>
        /// <param name="indexIsGood"></param>
        /// <param name="index"></param>
        public StatementLoopOverGood(IValue indiciesToCheck, IValue indexIsGood, IDeclaredParameter index)
        {
            if (indiciesToCheck == null)
                throw new ArgumentNullException("indiciesToCheck");
            if (indexIsGood == null)
                throw new ArgumentNullException("indexIsGood");
            if (index == null)
                throw new ArgumentNullException("index");

            _indiciesToCheck = indiciesToCheck;
            _indexIsGood = indexIsGood;
            _index = index;
        }

        /// <summary>
        /// Render the loop and if statement...
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("for (int index=0; index < {0}.size(); index++)", _indiciesToCheck.RawValue);
            yield return "{";
            yield return string.Format("  if ({0}[index])", _indexIsGood.RawValue);
            yield return "  {";
            yield return string.Format("    int {0} = {1}[index];", _index.RawValue, _indiciesToCheck.RawValue);
            foreach (var l in RenderInternalCode())
            {
                yield return "    " + l;
            }
            yield return "  }";
            yield return "}";
        }

        /// <summary>
        /// Try to combine two of these guys
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException();
            if (this == statement)
                throw new ArgumentException("Can't comebine with self!");

            var otherLoop = statement as StatementLoopOverGood;
            if (otherLoop == null)
                return false;

            // Are they the same? _index is independent and we can alter it.
            throw new NotSupportedException();

#if false
            if (otherLoop._indexIsGood.RawValue == _indexIsGood.RawValue
                && otherLoop._indiciesToCheck.RawValue == _indiciesToCheck.RawValue)
            {
                if (!(opt.TryRenameVarialbeOneLevelUp(otherLoop._index.RawValue, _index)))
                    return false;

                Combine(otherLoop, opt);
                return true;
            }
            else
            {
                return false;
            }
#endif
        }

        /// <summary>
        /// Rename the variables here.
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            _index.RenameRawValue(origName, newName);
            _indexIsGood.RenameRawValue(origName, newName);
            _indiciesToCheck.RenameRawValue(origName, newName);

            RenameBlockVariables(origName, newName);
        }
    }
}
