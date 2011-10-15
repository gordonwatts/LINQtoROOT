﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Deal with a "Where"-like clause. Basically, we have an expression which we evaluate, and we make sure that
    /// it goes!
    /// </summary>
    public class StatementFilter : StatementInlineBlockBase
    {
        /// <summary>
        /// Get the expresion we are going to test
        /// </summary>
        public IValue TestExpression { get; private set; }

        /// <summary>
        /// testExpression is what we test against to see if we should fire!
        /// </summary>
        /// <param name="testExpression"></param>
        public StatementFilter(IValue testExpression)
        {
            if (testExpression == null)
                throw new ArgumentNullException("testExpression");
            TestExpression = testExpression;
        }

        /// <summary>
        /// Return the code for this if statement.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            ///
            /// If there are no held statements, then we can optimize away the
            /// if statement!
            /// 

            if (Statements.Any() && TestExpression.RawValue != "false")
            {

                ///
                /// Now, emit the if statement and work from there
                /// 

                if (TestExpression.RawValue != "true")
                    yield return "if (" + TestExpression.RawValue + ")";
                foreach (var l in RenderInternalCode())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// We filter on one simple thing. If it is the case that the tests are the same,
        /// (identical), we do the combination, stealing the statemetns from the second one
        /// for ourselves. No renaming is required as this is a simple test!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentException("statement");
            var other = statement as StatementFilter;
            if (other == null)
                return false;

            if (other.TestExpression.RawValue != TestExpression.RawValue)
            {
                return false;
            }

            //
            // Since the if statements are the same, we can combine the interiors!
            //

            Combine(other, opt);
            return true;
        }

        /// <summary>
        /// Rename our variables
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            TestExpression.RenameRawValue(origName, newName);
            RenameBlockVariables(origName, newName);
        }
    }
}
