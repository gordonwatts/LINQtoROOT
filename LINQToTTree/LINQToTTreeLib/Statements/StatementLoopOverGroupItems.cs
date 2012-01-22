using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Used in a group-by set of commands. This is for the interior loop over items.
    /// </summary>
    class StatementLoopOverGroupItems : StatementInlineBlockBase, IStatementLoop
    {
        private IValue _groupArray;
        private IValue _counter;

        public StatementLoopOverGroupItems(IValue arrayToLoopOver, IValue counter)
        {
            _groupArray = arrayToLoopOver;
            _counter = counter;
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
                yield return string.Format("for (int {0} = 0; {0} <= {1}.size(); {0}++)", _counter.RawValue, _groupArray.RawValue);
                foreach (var l in RenderInternalCode())
                {
                    yield return "  " + l;
                }
            }
        }

        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            throw new NotImplementedException();
        }

        public override void RenameVariable(string origName, string newName)
        {
            throw new NotImplementedException();
        }

    }
}
