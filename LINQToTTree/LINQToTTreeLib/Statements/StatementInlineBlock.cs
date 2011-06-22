using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implements a block of statements with declarations at the start. It is its own scope - so
    /// everything declared will disappear when we leave this guy. Pretty dumb, actually.
    /// </summary>
    public class StatementInlineBlock : IBookingStatementBlock
    {
        /// <summary>
        /// The list of statements in this block.
        /// </summary>
        public IEnumerable<IStatement> Statements
        {
            get { return _statements; }
        }

        /// <summary>
        /// Keep track of the statements we know about!
        /// </summary>
        private List<IStatement> _statements = new List<IStatement>();

        /// <summary>
        /// Adds a statement to the end of the list of statements that we know about.
        /// </summary>
        /// <param name="statement"></param>
        public void Add(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("Cannot add a null statement");

            _statements.Add(statement);
        }

        private List<IVariable> _variables = new List<IVariable>();

        /// <summary>
        /// Add a booking
        /// </summary>
        /// <param name="variableToDeclare"></param>
        public void Add(IVariable variableToDeclare)
        {
            if (variableToDeclare == null)
                throw new ArgumentNullException("Must not declare a null variable");

            var findOld = from v in _variables
                          where v.VariableName == variableToDeclare.VariableName
                          select v;
            if (findOld.FirstOrDefault() != null)
                throw new ArgumentException("Variable '" + variableToDeclare.VariableName + "' has already been declared in this block!");

            _variables.Add(variableToDeclare);
        }

        /// <summary>
        /// Return the variables in the block
        /// </summary>
        public IEnumerable<IVariable> DeclaredVariables
        {
            get { return _variables; }
        }

        /// <summary>
        /// Return this translated to code, inside curly braced. First variable decl and then the statements.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> CodeItUp()
        {
            var goodVars = (from v in _variables
                            where v.Declare
                            select v).ToArray();

            if (_statements.Count > 0 || goodVars.Length > 0)
            {
                yield return "{";

                foreach (var v in goodVars)
                {
                    string varDecl = Variables.VarUtils.AsCPPType(v.Type) + " " + v.VariableName;
                    if (v.InitialValue != null)
                    {
                        varDecl = varDecl + "=" + v.InitialValue.RawValue;
                    }
                    varDecl += ";";
                    yield return "  " + varDecl;
                }

                var sublines = from s in _statements
                               from l in s.CodeItUp()
                               select l;
                foreach (var l in sublines)
                {
                    yield return "  " + l;
                }
                yield return "}";
            }
        }

        /// <summary>
        /// Try to combine this statement with another statement. We do simple append unless it is
        /// another inline block. In that case we make sure to lift things out.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public virtual bool TryCombineStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement should not be null");

            if (statement.GetType() == typeof(StatementInlineBlock))
            {
                var block = statement as StatementInlineBlock;
                var statements = block.Statements;
                if (statements != null)
                {
                    if (statements == this.Statements)
                        throw new ArgumentException("Can't add our own statements to ourselves");

                    foreach (var s in statements)
                    {
                        Add(s);
                    }
                }

                var declVars = block.DeclaredVariables;
                if (declVars != null)
                {
                    foreach (var v in block.DeclaredVariables)
                    {
                        Add(v);
                    }
                }
            }
            else
            {
                Add(statement);
            }
            return true;
        }


        public bool IsSameStatement(IStatement statement)
        {
            throw new NotImplementedException();
        }

        public void RenameVariable(string originalName, string newName)
        {
            throw new NotImplementedException();
        }
    }
}
