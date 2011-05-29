using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Combined results - to be used for conversion to real code and execution.
    /// </summary>
    public class CombinedGeneratedCode : IExecutableCode
    {
        /// <summary>
        /// Add one of the query's to our list of queries we are tracking here.
        /// </summary>
        /// <param name="code"></param>
        public void AddGeneratedCode(IExecutableCode code)
        {
            if (code == null)
                throw new ArgumentNullException("code cannot be null");
            if (code.CodeStatements == null)
                throw new ArgumentNullException("There is no code to be combined!");
            if (code == this)
                throw new ArgumentException("Can't add code to itself!");

            ///
            /// Variables that we need to queue for transfer
            /// 

            foreach (var v in code.VariablesToTransfer)
            {
                QueueVariableForTransfer(v);
            }

            ///
            /// Include Files - only add if we don't have them on the list already.
            /// 

            foreach (var inc in code.IncludeFiles)
            {
                AddIncludeFile(inc);
            }

            ///
            /// Result values - killer if they are named the same thing!
            /// 

            foreach (var item in code.ResultValues)
            {
                AddResult(item);
            }

            ///
            /// Finally, combine the code!
            /// 

            AddCodeStatement(code.CodeStatements);
        }

        /// <summary>
        /// Adds a new code statement to our block
        /// </summary>
        /// <param name="code"></param>
        public void AddCodeStatement(IStatement code)
        {
            if (CodeStatements == null)
            {
                CodeStatements = new Statements.StatementInlineBlock();
            }

            if (!CodeStatements.TryCombineStatement(code))
                throw new ArgumentException("Unable to add a new code body to the combined code block - The combine failed!");
        }

        /// <summary>
        /// Add a result. Very bad if it isn't unique.
        /// </summary>
        /// <param name="var"></param>
        public void AddResult(IVariable var)
        {
            if (var == null)
                throw new ArgumentNullException("Can't add a null result");

            var sameV = from v in _results
                        where v.VariableName == var.VariableName
                        select v;
            if (sameV.Any())
                throw new ArgumentException(string.Format("Attempt to add duplicate result named '{0}' to a combined code.", var.VariableName));

            _results.Add(var);
        }

        /// <summary>
        /// Add a variable that should be transfered over to the client for processing.
        /// </summary>
        /// <param name="v"></param>
        public void QueueVariableForTransfer(KeyValuePair<string, object> v)
        {
            if (_varsToTransfer.ContainsKey(v.Key))
                throw new ArgumentException(string.Format("Varaible {0} is being added from a new code block to an old one that already cotains it!", v.Key));
            _varsToTransfer[v.Key] = v.Value;
        }

        /// <summary>
        /// List of objects we want to transfer
        /// </summary>
        Dictionary<string, object> _varsToTransfer = new Dictionary<string, object>();

        /// <summary>
        /// Return all the variables we need to watch
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> VariablesToTransfer
        {
            get { return _varsToTransfer; }
        }

        /// <summary>
        /// Keep track of all results we are going to want back
        /// </summary>
        List<IVariable> _results = new List<IVariable>();

        /// <summary>
        /// Returns all the result values that we care about.
        /// </summary>
        public IEnumerable<IVariable> ResultValues
        {
            get { return _results; }
        }

        /// <summary>
        /// Adds an include file
        /// </summary>
        /// <param name="includeName"></param>
        public void AddIncludeFile(string includeName)
        {
            if (string.IsNullOrWhiteSpace(includeName))
                throw new ArgumentException("Include filename is empty and thus illegal!");

            if (!_includeFiles.Contains(includeName))
                _includeFiles.Add(includeName);
        }

        List<string> _includeFiles = new List<string>();

        /// <summary>
        /// Returns all the include files
        /// </summary>
        public IEnumerable<string> IncludeFiles
        {
            get { return _includeFiles; }
        }

        /// <summary>
        /// Returns the code body of this guy.
        /// </summary>
        public IStatementCompound CodeStatements { get; private set; }
    }
}
