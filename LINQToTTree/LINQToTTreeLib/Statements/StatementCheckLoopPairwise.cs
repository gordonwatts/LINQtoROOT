﻿
using System.Collections.Generic;
using LINQToTTreeLib.Variables;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Given an array of indicies, makes a loop over them. One then
    /// can add a check (or similar) to the interiror.
    /// </summary>
    class StatementCheckLoopPairwise : StatementInlineBlock
    {
        private VarArray _indciesToInspect;
        private VarSimple _index1;
        private VarSimple _index2;

        private VarArray _whatIsGood;

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
        public StatementCheckLoopPairwise(VarArray indiciesToInspect,
            VarSimple index1, VarSimple index2, VarArray passedArray)
        {
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

            yield return string.Format("for (int index = 0; index < {0}.size(); index++) {1}.push_back(true);", _indciesToInspect.RawValue, _whatIsGood.RawValue);

            //
            // Loop over each one, only do it if it is still marked good. Note that for the inner loop
            // we still compare to everything - this is b/c we want it with reference to all objects.
            // Think of 3 jets and we want only the jets that are more that DR of 0.8 apart from
            // each other. On the other hand, since we are comparing to the same list, the results
            // are assumed to be symmetric, so we don't have to do anything.
            //

            yield return string.Format("for (int index1 = 0; index1 < {0}.size(); index1++)", _indciesToInspect.RawValue);
            yield return "{";
            yield return string.Format("  if({0}[index1])", _whatIsGood.RawValue);
            yield return "  {";
            yield return string.Format("    for (int index2 = index1+1; index2 < {0}.size(); index2++)", _indciesToInspect.RawValue);
            yield return "    {";

            //
            // Now the test. If the test fails not really worth it to go on further.
            //

            yield return string.Format("        int {0} = {1}[index1];", _index1.RawValue, _indciesToInspect.RawValue);
            yield return string.Format("        int {0} = {1}[index2];", _index2.RawValue, _indciesToInspect.RawValue);

            //
            // Do the other things that have been added to our code!
            //

            foreach (var l in base.CodeItUp())
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
    }
}