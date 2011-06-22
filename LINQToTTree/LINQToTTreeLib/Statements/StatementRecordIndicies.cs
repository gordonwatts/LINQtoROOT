
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Sits inside a loop and records the integer that it is given on each call by pushing it onto a vector. It *does not*
    /// check for uniqueness of that integer that is pushed on - this is pretty simple. The vector it is pushing onto should
    /// be declared at an outter level to be of any use. :-)
    /// </summary>
    class StatementRecordIndicies : IStatement
    {
        /// <summary>
        /// The integer to record
        /// </summary>
        private IValue _intToRecord;

        /// <summary>
        /// The array to be storing things in
        /// </summary>
        private Variables.VarArray _storageArray;

        /// <summary>
        /// Create a statement that will record this index into this array each time through.
        /// </summary>
        /// <param name="intToRecord">Integer that should be cached on each time through</param>
        /// <param name="storageArray">The array where the indicies should be written</param>
        public StatementRecordIndicies(IValue intToRecord, VarArray storageArray)
        {
            _intToRecord = intToRecord;
            _storageArray = storageArray;
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
            yield return string.Format("{0}.push_back({1});", _storageArray.RawValue, _intToRecord.RawValue);
        }
    }
}
