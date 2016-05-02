
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using System.Linq;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Sits inside a loop and records the integer that it is given on each call by pushing it onto a vector. It *does not*
    /// check for uniqueness of that integer that is pushed on - this is pretty simple. The vector it is pushing onto should
    /// be declared at an outer level to be of any use. :-)
    /// </summary>
    public class StatementRecordIndicies : IStatement, ICMStatementInfo
    {
        /// <summary>
        /// The integer to record
        /// </summary>
        private IValue _intToRecord;

        /// <summary>
        /// The array to be storing things in
        /// </summary>
        private IDeclaredParameter _storageArray;

        /// <summary>
        /// Create a statement that will record this index into this array each time through.
        /// </summary>
        /// <param name="intToRecord">Integer that should be cached on each time through</param>
        /// <param name="storageArray">The array where the indicies should be written</param>
        public StatementRecordIndicies(IValue intToRecord, IDeclaredParameter storageArray)
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
            yield return string.Format("{0}.push_back({1});", _storageArray.ParameterName, _intToRecord.RawValue);
        }

        /// <summary>
        /// To make the same, what renames are required?
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            var otherS = other as StatementRecordIndicies;
            if (otherS == null)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            return Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(_storageArray, otherS._storageArray)
                .RequireAreSame(_intToRecord, otherS._intToRecord)
                .ExceptFor(replaceFirst);
        }

        /// <summary>
        /// Rename the variables.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _intToRecord.RenameRawValue(originalName, newName);
            _storageArray.RenameParameter(originalName, newName);
        }

        /// <summary>
        /// Attempt to combine two record statements. We are a bit different, we have 
        /// an object we depend on - which is record... and that has to be the same. The
        /// other one we need to propagate.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentException("statement");

            if (opt == null)
                throw new ArgumentNullException("opt");

            var asRecord = statement as StatementRecordIndicies;
            if (asRecord == null)
                return false;

            if (_intToRecord.RawValue != asRecord._intToRecord.RawValue)
                return false;

            //
            // Since int to record is the same, we do a rename and move on!
            //

            return opt.TryRenameVarialbeOneLevelUp(asRecord._storageArray.RawValue, _storageArray);
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// We are self modifying, sadly (e.g. not idempotent)
        /// </summary>
        public IEnumerable<string> DependentVariables
        {
            get { return _storageArray.Dependants.Concat(_intToRecord.Dependants).Select(v => v.RawValue); }
        }

        /// <summary>
        /// We update the storage array.
        /// </summary>
        public IEnumerable<string> ResultVariables
        {
            get { return _intToRecord.Dependants.Select(s => s.RawValue); }
        }

        /// <summary>
        /// Life as long as it is ok.
        /// </summary>
        public bool NeverLift
        {
            get { return false; }
        }
    }
}
