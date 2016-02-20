﻿using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Base class for result operators that write out a file with data in them.
    /// </summary>
    public abstract class ROAsFile : IQVScalarResultOperator
    {
        /// <summary>
        /// Override in order to see what can be grabbed.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public abstract bool CanHandle(Type resultOperatorType);

        /// <summary>
        /// We never deal with identity queries for this guy.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Tuple<bool, Expression> ProcessIdentityQuery(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dealing with the process is the most important thing we do, so delagate it!
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public abstract Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container);

        /// <summary>
        /// Helper function that extracts all expressions needed to calculate each value.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        protected static List<Expression> ExtractItemValueExpressions(QueryModel queryModel)
        {
            var streamType = queryModel.SelectClause.Selector.Type;
            var streamSelector = queryModel.SelectClause.Selector;

            var itemValues = new List<Expression>();
            if (streamType == typeof(double))
            {
                // A single stream of doubles
                itemValues.Add(queryModel.SelectClause.Selector);

            }
            else if (streamType.Name.StartsWith("Tuple"))
            {
                var targs = streamType.GenericTypeArguments.Zip(Enumerable.Range(1, 100), (t, c) => Tuple.Create(t, c));
                foreach (var templateType in targs)
                {
                    itemValues.Add(Expression.PropertyOrField(streamSelector, $"Item{templateType.Item2}"));
                }
            }
            else if (streamType.GetFields().Length > 0)
            {
                foreach (var fName in streamType.GetFields().Select(f => f.Name))
                {
                    itemValues.Add(Expression.PropertyOrField(streamSelector, fName));
                }
            }
            else
            {
                throw new InvalidOperationException($"Do not know how to generate CSV file from a sequence of {streamType.Name} objects!");
            }

            return itemValues;
        }

    }
}