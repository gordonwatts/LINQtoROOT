using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A statement that looks for any or all - and breaks the loop when it finds somethign that allows it to pop-out
    /// early.
    /// </summary>
    public class StatementAnyAllDetector : IStatement
    {
        /// <summary>
        /// The predicate to test for.
        /// </summary>
        private IValue Predicate;

        /// <summary>
        /// The variable that should hold our result when we see it. We expect it to be declared above,
        /// but also this is not a dependent variable!
        /// </summary>
        private IDeclaredParameter Result;

        /// <summary>
        /// Value to set Result to when the predicate fires. True/False.
        /// </summary>
        private string ResultValueToBe;

        /// <summary>
        /// Should we put a break statement at the end?
        /// </summary>
        private IValue ResultFastTest;

        /// <summary>
        /// Look at predicate. Set result to the marked value when the predicate fires, and also pop out of the
        /// current loop.
        /// </summary>
        /// <param name="predicate">The test that needs to be done. Null if it is "any" test</param>
        /// <param name="aresult">The result that should be altered, and testsed against.</param>
        /// <param name="aresultFastTest">A text expression we can use to short-curit the predicate if it has already gone.</param>
        /// <param name="markedValue">How we should set the result when the if statement fires</param>
        /// <param name="addBreak">Add a break statement to terminate loop early  - this is dangerous as when loops are combined...</param>
        /// <remarks>
        /// We keep the aresultFastTest seperate so that we can make sure try-combine works properly when we have two
        /// identical guys.
        /// </remarks>
        public StatementAnyAllDetector(IValue predicate, IDeclaredParameter aresult, IValue aresultFastTest, string markedValue)
        {
            if (aresultFastTest == null)
                throw new ArgumentNullException("aresultFastTest");
            if (aresult == null)
                throw new ArgumentNullException("aresult");
            if (markedValue == null)
                throw new ArgumentNullException("markedValue");

            Predicate = predicate;
            Result = aresult;
            ResultValueToBe = markedValue;
            ResultFastTest = aresultFastTest;
        }

        //ifstatement.Add(new Statements.StatementAssign(aresult, new Variables.ValSimple(markedValue, typeof(bool))));
        //ifstatement.Add(new Statements.StatementBreak());

        /// <summary>
        /// Return the code for this guy!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            string prv = "";
            if (Predicate != null)
                prv = string.Format(" && {0}", Predicate.RawValue);
            yield return string.Format("if ({0}{1}) {{", ResultFastTest.RawValue, prv);
            yield return string.Format("  {0} = {1};", Result.RawValue, ResultValueToBe);
            yield return "}";
        }

        /// <summary>
        /// Rename all the variables we know about.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            if (Predicate != null)
                Predicate.RenameRawValue(originalName, newName);
            Result.RenameRawValue(originalName, newName);
            ResultFastTest.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// We can combine if our predicates look the same.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementAnyAllDetector;
            if (other == null)
                return false;

            if (Predicate == null)
            {
                if (other.Predicate != null)
                    return false;
            }
            else
            {
                if (other.Predicate == null)
                    return false;

                if (other.Predicate.RawValue != Predicate.RawValue
                    || other.ResultValueToBe != ResultValueToBe)
                    return false;
            }

            //
            // As long as nothing crazy is going on with result, then we
            // can definately combine these two!
            //

            if (optimize == null)
                throw new ArgumentNullException("optimize");

            return optimize.TryRenameVarialbeOneLevelUp(other.Result.RawValue, Result);
        }

        /// <summary>
        /// Track the envinroment this statement is embedded in.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
