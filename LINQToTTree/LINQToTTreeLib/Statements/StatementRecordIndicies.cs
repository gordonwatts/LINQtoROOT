
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Sits inside a loop and records the integer that it is given on each call by pushing it onto a vector. It *does not*
    /// check for uniqueness of that integer that is pushed on - this is pretty simple. The vector it is pushing onto should
    /// be declared at an outter level to be of any use. :-)
    /// </summary>
    public class StatementRecordIndicies : IStatement
    {
        /// <summary>
        /// The integer to record
        /// </summary>
        private IValue _intToRecord;

        /// <summary>
        /// The array to be storing things in
        /// </summary>
        private IValue _storageArray;

        /// <summary>
        /// Create a statement that will record this index into this array each time through.
        /// </summary>
        /// <param name="intToRecord">Integer that should be cached on each time through</param>
        /// <param name="storageArray">The array where the indicies should be written</param>
        public StatementRecordIndicies(IValue intToRecord, IValue storageArray)
        {
            if (intToRecord == null)
                throw new ArgumentNullException("intToRecord");
            if (storageArray == null)
                throw new ArgumentNullException("storageArray");

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

        /// <summary>
        /// Check to see if the statements are similar or not.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            var other = statement as StatementRecordIndicies;
            if (other == null)
                return false;

            return _intToRecord.RawValue == other._intToRecord.RawValue
                && _storageArray.RawValue == other._storageArray.RawValue;
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rename the variables.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _intToRecord.RenameRawValue(originalName, newName);
            _storageArray.RenameRawValue(originalName, newName);

        }

        /// <summary>
        /// Attempt to combine two record statements. Since we are only a single and non-complex
        /// statement, we do this if we are the "same". :-)
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentException("statement");

            var asRecord = statement as StatementRecordIndicies;
            if (asRecord == null)
                return false;

            if (_intToRecord.RawValue != asRecord._intToRecord.RawValue
                || _storageArray.RawValue != _storageArray.RawValue)
                return false;

            //
            // These two are the same. So elminate one by returning true.
            //

            return true;
        }
    }
}
