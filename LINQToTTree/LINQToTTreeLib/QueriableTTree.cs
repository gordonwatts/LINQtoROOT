using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.QueryVisitors;
using LINQToTTreeLib.relinq;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using LINQToTTreeLib.Files;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Represents the querieable item we will be running against. The ctors fo this class take several different style
    /// inputs.
    /// </summary>
    public class QueriableTTree<T> : QueryableBase<T>
    {
        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a single source file.
        /// </summary>
        /// <param name="rootFile">The uri of a root file. Should use one of the support schemes (see docs)</param>
        /// <param name="treeName">Name of the tree in the list of files we are to process</param>
        public QueriableTTree(Uri rootFile, string treeName)
            : base(CreateLINQToTTreeParser(), new TTreeQueryExecutor(new Uri[] { rootFile }, treeName, typeof(T)))
        {
            TraceHelpers.TraceInfo(1, $"Creating new Queriable ttree with 1 Uri for tree '{treeName}'");
        }

        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a multiple source files.
        /// </summary>
        /// <param name="rootFiles">The uri of a root file. Should use one of the support schemes (see docs)</param>
        /// <param name="treeName">Name of the tree in the list of files we are to process</param>
        public QueriableTTree(Uri[] rootFiles, string treeName)
            : base(CreateLINQToTTreeParser(), new TTreeQueryExecutor(rootFiles, treeName, typeof(T)))
        {
            TraceHelpers.TraceInfo(1, string.Format("Creating new Queriable ttree with {1} file for tree '{0}'", treeName, rootFiles.Length));
        }

        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a multiple source files.
        /// </summary>
        /// <param name="rootFile">A complete and existing file that we should run over</param>
        /// <param name="treeName">Name of the tree in the list of files we are to process</param>
        public QueriableTTree(FileInfo rootFile, string treeName)
            : base(CreateLINQToTTreeParser(), new TTreeQueryExecutor(new Uri[] { new Uri("file://" + rootFile.FullName) }, treeName, typeof(T)))
        {
            TraceHelpers.TraceInfo(1, string.Format("Creating new Queriable ttree with 1 file for tree '{0}'", treeName));
        }

        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a multiple source files.
        /// </summary>
        /// <param name="rootFiles">A complete and existing file list that we should run over</param>
        /// <param name="treeName">Name of the tree in the list of files we are to process</param>
        public QueriableTTree(FileInfo[] rootFiles, string treeName)
            : base(CreateLINQToTTreeParser(), new TTreeQueryExecutor(rootFiles.Select(u => new Uri("file://" + u.FullName)).ToArray(), treeName, typeof(T)))
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
        /// Get/Set the compile debug flag. When the generated C++ is compiled, if this flag is set, line number
        /// symbols are included. If your query is crashing in generated code, setting this can help pin point
        /// exeactly where the crash is occuring. This is less useful when doing remote execution (i.e. PROOF).
        /// </summary>
        public bool CompileDebug
        {
            set
            {
                ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).CompileDebug = value;
            }
            get
            {
                return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).CompileDebug;
            }
        }

        /// <summary>
        /// Print verbose messges to stdout as we run things
        /// </summary>
        public bool Verbose
        {
            set { ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).Verbose = value; }
            get { return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).Verbose; }
        }

        /// <summary>
        /// Get/Set flag telling the query processor to re-check the dates of the input file list when deciding
        /// if a cache result is valie... This can
        /// dramatically slow down a run if the files are accross the network (and there are 100's of input files).
        /// </summary>
        public bool RecheckFileDatesOnEachQuery
        {
            set
            {
                ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).RecheckFileDatesOnEachQuery = value;
            }
            get
            {
                return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).RecheckFileDatesOnEachQuery;
            }
        }

        /// <summary>
        /// Get/Set flag telling the query processor to break into the debugger just before calling TTree::Process. C++ code will be compiled
        /// and loaded at this point so you can load up the source file and set a break point.
        /// </summary>
        /// <remarks>Do not forget to set the code to build with debug symbols or you will not be able to set a break point in the generated source code.</remarks>
        public bool BreakToDebugger
        {
            set
            {
                ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).BreakToDebugger = value;
            }
            get
            {
                return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).BreakToDebugger;
            }
        }

        /// <summary>
        /// Get/Set flag that controls statement optimization. This step is done to make the run over the ntuple execute more quickly (sometimes x2).
        /// However, the optimizer may have bugs, and if you suspect it, try a test run with this flag set to false.
        /// </summary>
        public bool UseStatementOptimizer
        {
            set
            {
                ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).UseStatementOptimizer = value;
            }
            get
            {
                return ((Provider as DefaultQueryProvider).Executor as TTreeQueryExecutor).UseStatementOptimizer;
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
        internal static IQueryParser CreateLINQToTTreeParser()
        {
            //
            // Provider for our classes, and they also go into the whole pot of soup
            //

            var mreg = new MethodInfoBasedNodeTypeRegistry();
            mreg.Register(UniqueCombinationsExpressionNode.SupportedMethods, typeof(UniqueCombinationsExpressionNode));
            mreg.Register(PairWiseAllExpressionNode.SupportedMethods, typeof(PairWiseAllExpressionNode));
            mreg.Register(AsQueriableExpressionNode.SupportedMethods, typeof(AsQueriableExpressionNode));
            mreg.Register(AsCSVExpressionNode.SupportedMethods, typeof(AsCSVExpressionNode));
            mreg.Register(AsTTreeExpressionNode.SupportedMethods, typeof(AsTTreeExpressionNode));
            mreg.Register(ConcatExpressionNode.GetSupportedMethods(), typeof(ConcatExpressionNode));
            mreg.Register(TakeSkipExpressionNode.SupportedMethods, typeof(TakeSkipExpressionNode));

            var defaultNodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();

            var newProvider = new CompoundNodeTypeProvider(new INodeTypeProvider[] { mreg, defaultNodeTypeProvider });

            //
            // All the various transformers we need...
            //

            var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
            transformerRegistry.Register(new PropertyExpressionTransformer());
            transformerRegistry.Register(new EnumerableRangeExpressionTransformer());
            transformerRegistry.Register(new CreateTupleExpressionTransformer());
            transformerRegistry.Register<MethodCallExpression>(new ExpressionVariableInvokeExpressionTransformer(new ExpressionType[] { ExpressionType.Call }));
            transformerRegistry.Register<InvocationExpression>(new ExpressionVariableInvokeExpressionTransformer(new ExpressionType[] { ExpressionType.Invoke }));

            //
            // Create the query provider
            //

            var expressionTreeParser = new ExpressionTreeParser(
                newProvider,
                ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry));

            return new QueryParser(expressionTreeParser);
        }
    }
}
