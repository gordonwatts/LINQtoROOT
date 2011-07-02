
using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
namespace LINQToTTreeLib.Statements
{
    public class StatementPairLoop : StatementInlineBlock
    {
        private Variables.VarArray arrayRecord;
        private IVariable index1;
        private IVariable index2;

        public StatementPairLoop(VarArray arrayRecord, IVariable index1, IVariable index2)
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

        /// <summary>
        /// See if these are teh same statement or not.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool IsSameStatement(IStatement statement)
        {
            if (!base.IsSameStatement(statement))
                return false;

            var other = statement as StatementPairLoop;
            if (other == null)
                return false;

            return arrayRecord.RawValue == other.arrayRecord.RawValue
                && index1.RawValue == other.index1.RawValue
                && index2.RawValue == other.index2.RawValue;
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
                foreach (var l in base.CodeItUp())
                {
                    yield return "    " + l;
                }
                yield return "    breakSeen = false;";
                yield return "  }";
                yield return "  if (breakSeen) break;";
                yield return "}";
            }
        }
    }
}
