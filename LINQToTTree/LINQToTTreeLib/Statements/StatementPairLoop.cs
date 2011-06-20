
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

        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                yield return string.Format("for(int {0} = 0; {0} < {1}.size(); {0}++)", index1.RawValue, arrayRecord.RawValue);
                yield return string.Format("  for(int {0} = {1}+1; {0} < {2}.size(); {0}++)", index2.RawValue, index1.RawValue, arrayRecord.RawValue);
                foreach (var l in base.CodeItUp())
                    yield return "  " + l;
            }
        }
    }
}
