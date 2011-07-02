
using System;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Loop over a set of indicies only if they are marked "true" in the accompanying array.
    /// </summary>
    public class StatementLoopOverGood : StatementInlineBlock
    {
        private IValue _indiciesToCheck;
        private IValue _indexIsGood;
        private IValue _index;

        /// <summary>
        /// Simple loop over a set of indicies, passing on only those that satisfy the actual index.
        /// </summary>
        /// <param name="indiciesToCheck"></param>
        /// <param name="indexIsGood"></param>
        /// <param name="index"></param>
        public StatementLoopOverGood(IValue indiciesToCheck, IValue indexIsGood, IValue index)
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
        /// Test to see if the statement is the same
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool IsSameStatement(IStatement statement)
        {
            if (!base.IsSameStatement(statement))
                return false;

            var other = statement as StatementLoopOverGood;
            if (other == null)
                return false;

            return _index.RawValue == other._index.RawValue
                && _indexIsGood.RawValue == other._indexIsGood.RawValue
                && _indiciesToCheck.RawValue == other._indiciesToCheck.RawValue;
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
            foreach (var l in base.CodeItUp())
            {
                yield return "    " + l;
            }
            yield return "  }";
            yield return "}";
        }
    }
}
