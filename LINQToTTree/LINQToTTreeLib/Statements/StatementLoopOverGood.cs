
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Loop over a set of indicies only if they are marked "true" in the accompanying array.
    /// </summary>
    class StatementLoopOverGood : StatementInlineBlock
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
            foreach (var l in base.CodeItUp())
            {
                yield return "    " + l;
            }
            yield return "  }";
            yield return "}";
        }
    }
}
