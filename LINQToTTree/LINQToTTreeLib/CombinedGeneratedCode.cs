﻿using System;
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
            ///
            /// We have some pretty stiff requirements on code
            /// 

            if (code == null)
                throw new ArgumentNullException("code cannot be null");
            if (code == this)
                throw new ArgumentException("Can't add code to itself!");

            var varsToTrans = code.VariablesToTransfer;
            if (varsToTrans == null)
                throw new ArgumentNullException("Generated Code Varaibles to Transfer can't be null");

            var includeFiles = code.IncludeFiles;
            if (includeFiles == null)
                throw new ArgumentNullException("Generated code Include Files can't be null");

            var resultValues = code.ResultValues;
            if (resultValues == null)
                throw new ArgumentNullException("Generated code Result Values can't be null");

            var codeItems = code.QueryCode().ToArray();

            ///
            /// Variables that we need to queue for transfer
            /// 

            foreach (var v in varsToTrans)
            {
                QueueVariableForTransfer(v);
            }

            ///
            /// Include Files - only add if we don't have them on the list already.
            /// 

            foreach (var inc in includeFiles)
            {
                AddIncludeFile(inc);
            }

            ///
            /// Result values - killer if they are named the same thing!
            /// 

            foreach (var item in resultValues)
            {
                AddResult(item);
            }

            //
            // Add the referenced leaf names
            //

            foreach (var leaf in code.ReferencedLeafNames)
            {
                _leavesReferenced.Add(leaf);
            }

            ///
            /// Finally, we need to combine the query code. Now, there is a limitation (by design) in the C++ compiler:
            /// http://connect.microsoft.com/VisualStudio/feedback/details/100734/c-function-with-many-unnested-loops-generates-error-fatal-error-c1061-compiler-limit-blocks-nested-too-deeply
            /// 
            /// So - if we want to generate more than 250 plots in one run, and we are doing no loop combining, then we will be in trouble. To get around this
            /// Anything that can't folded into itself has to be kept "seperate". Sucks, but what can you do?
            /// 

            AddQueryBlocks(codeItems);
        }

        /// <summary>
        /// Add in the code blocks. Must not be null.
        /// </summary>
        /// <param name="codeBlocks"></param>
        internal void AddQueryBlocks(IStatementCompound[] codeBlocks)
        {
            if (codeBlocks == null || codeBlocks.Any(i => i == null) || codeBlocks.Length == 0)
                throw new ArgumentException("Queries must have code blocks and they can't be null and there must be at least one");
            _queryBlocks.AddRange(codeBlocks);
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
        /// List of query blocks - 1 per query.
        /// </summary>
        private List<IStatementCompound> _queryBlocks = new List<IStatementCompound>();

        /// <summary>
        /// Return, one after the other, the query code.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStatementCompound> QueryCode()
        {
            return _queryBlocks;
        }

        /// <summary>
        /// Copy of the leaf names referenced.
        /// </summary>
        private HashSet<string> _leavesReferenced = new HashSet<string>();

        /// <summary>
        /// Return a uniqe list of the names referenced by this
        /// set of code blocks.
        /// </summary>
        public IEnumerable<string> ReferencedLeafNames
        {
            get { return _leavesReferenced; }
        }
    }
}
