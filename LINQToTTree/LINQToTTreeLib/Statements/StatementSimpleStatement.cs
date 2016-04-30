using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Statement that holds onto a generic line. This could be, for example, a single line with side-effects that
    /// has been processed by GetExpression.
    /// </summary>
    public class StatementSimpleStatement : IStatement, ICMStatementInfo
    {
        /// <summary>
        /// Create a simple statement line, with a future to generate the actual line.
        /// </summary>
        /// <param name="futureLine"></param>
        /// <param name="addSemicolon"></param>
        public StatementSimpleStatement(Func<string> futureLine, bool addSemicolon = true, string[] dependentVars = null, string[] resultVars = null)
        {
            futureLine
                .ThrowIfNull(() => new ArgumentException("StatemeintSimpleStatment should not be called with a null input line"));

            AddSemicolon = addSemicolon;
            _statementGenerator = new EvalStringOnce(() => CleanLine(futureLine(), AddSemicolon));

            NeverLift = false;
            DependentVariables = dependentVars == null ? new string[0] : dependentVars;
            ResultVariables = resultVars == null ? new string[0] : resultVars;
        }

        /// <summary>
        /// Create a simple statement line with a given string.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="addSemicolon"></param>
        public StatementSimpleStatement(string line, bool addSemicolon = true, string[] dependentVars = null, string[] resultVars = null)
        {
            if (line == null)
                throw new ArgumentNullException("line can't be null!");

            AddSemicolon = addSemicolon;

            _statementGenerator = new EvalStringOnce(() => CleanLine(line, AddSemicolon));

            NeverLift = false;
            DependentVariables = dependentVars == null ? new string[0] : dependentVars;
            ResultVariables = resultVars == null ? new string[0] : resultVars;
        }

        /// <summary>
        /// Clean up the line of semicolons, etc.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="addSemicolon"></param>
        /// <returns></returns>
        private static string CleanLine (string line, bool addSemicolon)
        {
            ///
            /// Strip off all ending semi-colons. They will get added back
            /// in when we "codeitup".
            /// 

            line = line.Trim();
            if (addSemicolon)
            {
                while (line.EndsWith(";"))
                {
                    line = line.Substring(0, line.Length - 1);
                    line = line.Trim();
                }
            }

            ///
            /// Empty lines just aren't allowed! :-)
            /// 

            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException("line can't be empty");

            return line;
        }

        /// <summary>
        /// Class to track logic for evaluating a function for a result once.
        /// </summary>
        class EvalStringOnce
        {
            Func<string> _evaluator;
            string _result = null;

            public EvalStringOnce(Func<string> evaluator)
            {
                _evaluator = evaluator;
            }

            /// <summary>
            /// Apply a transform to the string
            /// </summary>
            /// <param name="transform"></param>
            public void ApplyFunc(Func<string, string> transform)
            {
                if (_evaluator == null)
                {
                    _result = transform(_result);
                } else
                {
                    var oldEval = _evaluator;
                    _evaluator = () => transform(oldEval());
                }
            }

            /// <summary>
            /// Return the Value, evaluating it if need be
            /// </summary>
            public string Value
            {
                get
                {
                    if (_evaluator != null)
                    {
                        _result = _evaluator();
                        _evaluator = null;
                    }
                    return _result;
                }
            }
        }

        /// <summary>
        /// So we call the code it up only once.
        /// </summary>
        private EvalStringOnce _statementGenerator = null;

        /// <summary>
        /// Returns the line text
        /// </summary>
        public string Line { get { return _statementGenerator.Value; } }

        /// <summary>
        /// Will a semicolon be added to this line when it is dumped?
        /// </summary>
        public bool AddSemicolon { get; private set; }

        /// <summary>
        /// Return the simple statement, along with a semi-colon!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return ToString();
        }

        /// <summary>
        /// For debugging, return the line
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string semi = AddSemicolon ? ";" : "";
            return Line + semi;
        }

        /// <summary>
        /// Rename any variables we have in here.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _statementGenerator.ApplyFunc(a => a.ReplaceVariableNames(originalName, newName));
        }

        /// <summary>
        /// See if we can combine. For us that works only if we have
        /// identical statements!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            var other = statement as StatementSimpleStatement;
            if (other == null)
                return false;

            return other.Line == Line
                && other.AddSemicolon == AddSemicolon;
        }

        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// A list of all variables used as input
        /// </summary>
        public IEnumerable<string> DependentVariables
        {
            get; private set;
        }

        /// <summary>
        /// A list of all variables that this guy changes
        /// </summary>
        public IEnumerable<string> ResultVariables
        {
            get; private set;
        }

        /// <summary>
        /// We are ok with this being lifted.
        /// </summary>
        public bool NeverLift
        {
            get; set;
        }
    }
}
