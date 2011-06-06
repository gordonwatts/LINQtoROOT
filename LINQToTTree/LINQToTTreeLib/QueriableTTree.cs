﻿using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Represents the querieable item we will be running against.
    /// </summary>
    public class QueriableTTree<T> : QueryableBase<T>
    {
        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a single source file.
        /// </summary>
        public QueriableTTree(FileInfo rootFile, string treeName)
            : base(CreateLINQToTTreeParser(), new TTreeQueryExecutor(new FileInfo[] { rootFile }, treeName, typeof(T)))
        {
            TraceHelpers.TraceInfo(1, string.Format("Creating new Queriable ttree with 1 file for tree '{0}'", treeName));
        }

        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a multiple source files.
        /// </summary>
        public QueriableTTree(FileInfo[] rootFiles, string treeName)
            : base(CreateLINQToTTreeParser(), new TTreeQueryExecutor(rootFiles, treeName, typeof(T)))
        {
            TraceHelpers.TraceInfo(1, string.Format("Creating new Queriable ttree with {1} file for tree '{0}'", treeName, rootFiles.Length));
        }

        /// <summary>
        /// Debugging Aid: Get/Set to force a re-evaluation of an expression, even if it exists in the cache.
        /// </summary>
        public bool IgnoreQueryCache
        {
            set
            {
                ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).IgnoreQueryCache = value;
            }
            get
            {
                return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).IgnoreQueryCache;
            }
        }

        /// <summary>
        /// Debugging Aid: Get/Set clean up query submission files (C++ files) after a successful run of a query
        /// </summary>
        public bool CleanupQuery
        {
            set
            {
                ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).CleanupQuery = value;
            }
            get
            {
                return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).CleanupQuery;
            }
        }

        /// <summary>
        /// Called by the LINQ infrastructure. No need to do much of anything here.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>
        public QueriableTTree(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        /// <summary>
        /// Create the parser we want to use to parse the LINQ query. If we need to add
        /// some operators or other custom things into the parser, this is where it is done.
        /// See https://www.re-motion.org/jira/browse/RM-3724 for sample code.
        /// </summary>
        private static IQueryParser CreateLINQToTTreeParser()
        {
            ///
            /// Right now we are not doing anything special, so just do the default!
            /// 

            return QueryParser.CreateDefault();
            ///(this.Provider as DefaultQueryProvider).ExpressionTreeParser.NodeTypeRegistry.Register(xxx.SupportedMethods, typeof(xxx));
        }
    }
}
