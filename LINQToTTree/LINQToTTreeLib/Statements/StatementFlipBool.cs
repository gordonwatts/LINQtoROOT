using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Flip a bool variable using the "!" operator.
    /// </summary>
    public class StatementFlipBool : IStatement
    {
        private Variables.VarSimple _var;

        /// <summary>
        /// Create a statement that will flip the bool value
        /// </summary>
        /// <param name="aresult"></param>
        public StatementFlipBool(Variables.VarSimple aresult)
        {
            if (aresult.Type != typeof(bool))
                throw new ArgumentException("Can only flip the logical value of a bool variable!");

            this._var = aresult;
        }

        public IEnumerable<string> CodeItUp()
        {
            yield return string.Format("{0} = !{0};", _var.RawValue);
        }


        public bool IsSameStatement(IStatement statement)
        {
            throw new NotImplementedException();
        }

        public void RenameVariable(string originalName, string newName)
        {
            throw new NotImplementedException();
        }


        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
