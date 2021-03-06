﻿using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// Everything needed to reference a function for a QueryModel, as well as emit it to C++
    /// We deal with both single value and sequence QM functions. There is some overlap in functionality
    /// and also some not. It would probably be good to split this up at some point, perhaps make it into
    /// a subclass.
    /// </summary>
    public class QMFuncSource : IQMFunctionSource
    {
        /// <summary>
        /// Get the header info for this function.
        /// </summary>
        private QMFuncHeader _header;

        /// <summary>
        /// If this is a sequence type, then we need to mess with the cache type.
        /// </summary>
        private Type _sequenceType;

        /// <summary>
        /// Create the QM Func Source starting from a pre-parsed bunch of header info.
        /// </summary>
        /// <param name="f"></param>
        public QMFuncSource(QMFuncHeader f)
        {
            this._header = f;
            Name = "QMFunction".CreateUniqueVariableName();
            Arguments = f.Arguments.Select(a => new QMFunctionArgument(a));
            StatementBlock = null;

            // If this is a sequence, then we need to get a non-normal type.
            if (_header.IsSequence)
            {
                _sequenceType = _header.QM.GetResultType().GetGenericArguments().First();
                _sequenceType = _sequenceType.MakeArrayType();
            }

            // Now we can create the cached variables, etc.
            CacheVariable = DeclarableParameter.CreateDeclarableParameterExpression(ResultType);
            CacheVariableGood = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));

        }

        /// <summary>
        /// Get the cache variable used by this fellow.
        /// </summary>
        public IDeclaredParameter CacheVariable { get; private set; }

        /// <summary>
        /// Get the cache variable good used by this fellow.
        /// </summary>
        public IDeclaredParameter CacheVariableGood { get; private set; }

        /// <summary>
        /// Empty ctor.
        /// </summary>
        public QMFuncSource()
        {
            this._header = null;
        }

        /// <summary>
        /// The name this member function in C++ source code.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns an in-order list of arguments used by this function.
        /// </summary>
        public IEnumerable<IQMArgument> Arguments { get; private set; }

        /// <summary>
        /// The list of statements that actually process this guy.
        /// </summary>
        public IStatementCompound StatementBlock { get; private set; }

        /// <summary>
        /// Return true if the QM text translation matches the QM we are holding onto here.
        /// </summary>
        /// <param name="qmText"></param>
        /// <returns></returns>
        internal bool Matches(string qmText)
        {
            return _header.QMText == qmText;
        }

        /// <summary>
        /// Get the type of this function's return. Sequence type isn't quite the same...
        /// </summary>
        public Type ResultType
        {
            get
            {
                if (IsSequence)
                {
                    return _sequenceType;
                }
                else
                {
                    return _header.QM.GetResultType();
                }
            }
        }

        /// <summary>
        /// Remember the code body for later use, along with the result that we will be
        /// returning.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="resultExpression"></param>
        public void SetCodeBody(IStatementCompound statements)
        {
            StatementBlock = statements;
        }

        /// <summary>
        /// Return the QM text
        /// </summary>
        public string QueryModelText { get { return _header.QMText; } }

        /// <summary>
        /// See if this executable function matches another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Matches(IQMFuncExecutable other)
        {
            return QueryModelText == other.QueryModelText;
        }

        /// <summary>
        /// Rename all references to this function in our block.
        /// </summary>
        /// <param name="oldfname"></param>
        /// <param name="newfname"></param>
        public void RenameFunctionReference(string oldfname, string newfname)
        {
            StatementBlock.RenameVariable(oldfname, newfname);
        }

        /// <summary>
        /// Is this a QM that represents a sequence?
        /// </summary>
        public bool IsSequence { get { return _header.IsSequence; } }

        /// <summary>
        /// Save sequence variables. Only valid if this guy is a sequence.
        /// </summary>
        /// <param name="loopIndexVariable"></param>
        /// <param name="loopExpression"></param>
        public void SequenceVariable(IDeclaredParameter loopIndexVariable, Expression loopExpression)
        {
            if (!IsSequence)
                throw new InvalidOperationException("Can't stash sequence info in a non-sequence QM!");

            OldLoopExpression = loopExpression;
            OldLoopIndexVariable = loopIndexVariable;
        }

        /// <summary>
        /// Return the index var that was used to capture this sequence.
        /// </summary>
        public IDeclaredParameter OldLoopIndexVariable { get; private set; }

        /// <summary>
        /// Return the loop expression that was setup when this guy was captured.
        /// </summary>
        public System.Linq.Expressions.Expression OldLoopExpression { get; private set; }
    }
}
