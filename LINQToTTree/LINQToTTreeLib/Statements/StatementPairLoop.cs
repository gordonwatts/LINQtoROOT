
using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    public class StatementPairLoop : StatementInlineBlockBase, IStatementLoop
    {
        private IDeclaredParameter arrayRecord;
        private IDeclaredParameter index1;
        private IDeclaredParameter index2;

        public StatementPairLoop(IDeclaredParameter arrayRecord, IDeclaredParameter index1, IDeclaredParameter index2)
        {
            if (arrayRecord == null)
                throw new ArgumentNullException("arrayRecord");
            if (index1 == null)
                throw new ArgumentNullException("index1");
            if (index2 == null)
                throw new ArgumentNullException("index2");

            // TODO: Complete member initialization
            this.arrayRecord = arrayRecord;
            this.index1 = index1;
            this.index2 = index2;
        }

        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                yield return string.Format("for(int {0} = 0; {0} < {1}.size(); {0}++)", index1.RawValue, arrayRecord.RawValue);
                yield return "{";
                yield return "  bool breakSeen = true;";
                yield return string.Format("  for(int {0} = {1}+1; {0} < {2}.size(); {0}++)", index2.RawValue, index1.RawValue, arrayRecord.RawValue);
                yield return "  {";
                yield return "    breakSeen = true;";
                foreach (var l in RenderInternalCode())
                {
                    yield return "    " + l;
                }
                yield return "    breakSeen = false;";
                yield return "  }";
                yield return "  if (breakSeen) break;";
                yield return "}";
            }
        }

        /// <summary>
        /// We can combine these two statements iff the array record we are looping
        /// over is the same. Rename the index after that!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var otherPairLoop = statement as StatementPairLoop;
            if (otherPairLoop == null)
                return false;

            if (otherPairLoop.arrayRecord.RawValue != arrayRecord.RawValue)
                return false;

            // Just make sure the index guys are renamed!

            otherPairLoop.RenameVariable(otherPairLoop.index1.RawValue, index1.RawValue);
            otherPairLoop.RenameVariable(otherPairLoop.index2.RawValue, index2.RawValue);

            // Now, combine them!

            Combine(otherPairLoop, opt);

            return true;
        }

        /// <summary>
        /// Rename the variables we know about here!
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            index1.RenameRawValue(origName, newName);
            index2.RenameRawValue(origName, newName);
            arrayRecord.RenameRawValue(origName, newName);
            RenameBlockVariables(origName, newName);
        }
    }
}
