
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Sits inside a loop and records the integer that it is given on each call by pushing it onto a vector. It *does not*
    /// check for uniqueness.
    /// </summary>
    class StatementRecordIndicies : IStatement
    {
        private LinqToTTreeInterfacesLib.IValue iValue;
        private Variables.VarArray arrayRecord;

        /// <summary>
        /// Create a statement that will record this index into this array each time through.
        /// </summary>
        /// <param name="iValue"></param>
        /// <param name="arrayRecord"></param>
        public StatementRecordIndicies(IValue iValue, Variables.VarArray arrayRecord)
        {
            // TODO: Complete member initialization
            this.iValue = iValue;
            this.arrayRecord = arrayRecord;
        }

        /// <summary>
        /// Returns a IValue that represents 
        /// </summary>
        public IValue HolderArray { get; private set; }

        /// <summary>
        /// Return the code for this statement.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return string.Format("{0}.push_back({1});", arrayRecord.RawValue, iValue.RawValue);
        }
    }
}
