﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Used in a group-by set of commands. This is for the interior loop over items.
    /// </summary>
    public class StatementLoopOverGroupItems : StatementInlineBlockBase, IStatementLoop
    {
        private IValue _groupArray;
        private DeclarableParameter _counter;

        public StatementLoopOverGroupItems(IValue arrayToLoopOver)
        {
            _groupArray = arrayToLoopOver;
            _counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            if (_groupArray == null)
                throw new ArgumentNullException("_groupArray");
            if (_counter == null)
                throw new ArgumentNullException("counter");
        }

        /// <summary>
        /// Return a string that looks like the loop item index - the core of the loop.
        /// </summary>
        public string LoopItemIndex
        {
            get
            {
                return string.Format("{0}[{1}]", _groupArray.RawValue, _counter.RawValue);
            }
        }

        /// <summary>
        /// Return the code. This code is actually pretty simple - we just loop over the secondary
        /// array.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                yield return string.Format("for (int {0} = 0; {0} < {1}.size(); {0}++)", _counter.RawValue, _groupArray.RawValue);
                foreach (var l in RenderInternalCode())
                {
                    yield return "  " + l;
                }
            }
        }

        /// <summary>
        /// See if we can combine statements
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementLoopOverGroupItems;
            if (other == null)
                return false;

            if (_groupArray.RawValue != other._groupArray.RawValue)
                return false;

            // We declare counter explicitly in the loop above - so we need to force the rename below rather
            // that rely on declared parameters.
            other.RenameVariable(other._counter.RawValue, _counter.RawValue);

            Combine(other, opt);
            return true;
        }

        /// <summary>
        /// Rename any variables we know about in this statement.
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            if (string.IsNullOrWhiteSpace(origName))
                throw new ArgumentNullException("origName");
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentNullException("newName");
            RenameBlockVariables(origName, newName);
            _groupArray.RenameRawValue(origName, newName);
            _counter.RenameRawValue(origName, newName);
        }

        /// <summary>
        /// Return the counter that we use to walk over the group items.
        /// </summary>
        public System.Linq.Expressions.Expression Counter { get { return _counter; } }
    }
}
