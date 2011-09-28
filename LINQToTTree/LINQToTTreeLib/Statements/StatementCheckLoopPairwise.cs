
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Given an array of indicies, makes a loop over them. One then
    /// can add a check (or similar) to the interiror.
    /// </summary>
    public class StatementCheckLoopPairwise : StatementInlineBlockBase, IStatementLoop
    {
        private IDeclaredParameter _indciesToInspect;
        private IDeclaredParameter _index1;
        private IDeclaredParameter _index2;

        private IDeclaredParameter _whatIsGood;

        /// <summary>
        /// Loop over all the indicies we are given (sort of like an indicrect) in a double loop. For each one
        /// make sure that they satisfy the function. if we find any failing, mark both as bad. You must set
        /// the TEST variable, but do it after this guy is part of the generatedcode - scoping must be tracked
        /// correctly!!
        /// </summary>
        /// <param name="indiciesToInspect">The list of indicies we should set index1 and index2 to</param>
        /// <param name="index1">The name we should use for index 1</param>
        /// <param name="index2">the name we should use for index 2</param>
        /// <param name="passedArray">The initially empty bool vector that we will mark any index that satisfies everything as true</param>
        public StatementCheckLoopPairwise(IDeclaredParameter indiciesToInspect,
            IDeclaredParameter index1, IDeclaredParameter index2, IDeclaredParameter passedArray)
        {
            if (indiciesToInspect == null)
                throw new ArgumentNullException("indiciesToInspect");
            if (index1 == null)
                throw new ArgumentNullException("index1");
            if (index2 == null)
                throw new ArgumentNullException("index2");
            if (passedArray == null)
                throw new ArgumentNullException("passedArray");
            if (!passedArray.Type.IsArray)
                throw new ArgumentException("passedArray isn't an array type");

            _indciesToInspect = indiciesToInspect;
            _index1 = index1;
            _index2 = index2;
            _whatIsGood = passedArray;
        }

        /// <summary>
        /// Return the code to implement this
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            //
            // Make sure that the list of bools is reset initially. Assume it is empty when we
            // are called.
            //

            yield return string.Format("for (int index = 0; index < {0}.size(); index++) {1}.push_back(true);", _indciesToInspect.ParameterName, _whatIsGood.ParameterName);

            //
            // Loop over each one, only do it if it is still marked good. Note that for the inner loop
            // we still compare to everything - this is b/c we want it with reference to all objects.
            // Think of 3 jets and we want only the jets that are more that DR of 0.8 apart from
            // each other. On the other hand, since we are comparing to the same list, the results
            // are assumed to be symmetric, so we don't have to do anything.
            //

            yield return string.Format("for (int index1 = 0; index1 < {0}.size(); index1++)", _indciesToInspect.ParameterName);
            yield return "{";
            yield return string.Format("  if({0}[index1])", _whatIsGood.ParameterName);
            yield return "  {";
            yield return string.Format("    for (int index2 = index1+1; index2 < {0}.size(); index2++)", _indciesToInspect.ParameterName);
            yield return "    {";

            //
            // Now the test. If the test fails not really worth it to go on further.
            //

            yield return string.Format("        int {0} = {1}[index1];", _index1.RawValue, _indciesToInspect.ParameterName);
            yield return string.Format("        int {0} = {1}[index2];", _index2.RawValue, _indciesToInspect.ParameterName);

            //
            // Do the other things that have been added to our code!
            //

            foreach (var l in RenderInternalCode())
            {
                yield return "        " + l;
            }

            //
            // And clean it all up!
            //

            yield return "    }"; // Inner lop (index2)
            yield return "  }"; // The if this index is worth looking at
            yield return "}"; // Outter for loop

        }

        /// <summary>
        /// Attempt to combine two of these statements. We can do this iff the source expression that
        /// we are testing is the same. Then the two indicies need to be renamed (it is assumed that we have total
        /// control - as you can see from the generated code above).
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementCheckLoopPairwise;
            if (other == null)
                return false;

            if (_indciesToInspect.ParameterName != other._indciesToInspect.ParameterName)
                return false;

            //
            // Rename the various guys. Note that index1 and index2 are declared by us. So it is only our sub-blocks that have to deal with
            // that. So we do a "local" renaming.
            //

            var rename = opt.TryRenameVarialbeOneLevelUp(other._whatIsGood.RawValue, _whatIsGood);
            if (!rename)
                return false;
            other.RenameVariable(other._index1.RawValue, _index1.RawValue);
            other.RenameVariable(other._index2.RawValue, _index2.RawValue);

            //
            // Combine the sub-blocks now
            //

            Combine(other as StatementInlineBlockBase, opt);

            return true;
        }

        /// <summary>
        /// Rename the variables inside this guy
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            _index1.RenameRawValue(origName, newName);
            _index2.RenameRawValue(origName, newName);
            _whatIsGood.RenameParameter(origName, newName);
            _indciesToInspect.RenameParameter(origName, newName);

            RenameBlockVariables(origName, newName);
        }
    }
}
