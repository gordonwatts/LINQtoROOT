using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;

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
            : base(new TTreeQueryExecutor(new FileInfo[] { rootFile }, treeName, typeof(T)))
        {
            DefineExtraOperators();
        }

        /// <summary>
        /// Create a new LINQ Querable object that will go after a TTree. The ntuple type must have been
        /// generated with the proper meta data so things like the scanner source file can be found!
        /// Runs on data in a multiple source files.
        /// </summary>
        public QueriableTTree(FileInfo[] rootFiles, string treeName)
            : base(new TTreeQueryExecutor(rootFiles, treeName, typeof(T)))
        {
            DefineExtraOperators();
        }


        /// <summary>
        /// Called by the LINQ infrastructure. No need to do much of anything here.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>
        public QueriableTTree(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
            DefineExtraOperators();
        }

        /// <summary>
        /// Define new operators that we want this version of re-linq to look for and correctly deal with.
        /// </summary>
        private void DefineExtraOperators()
        {
            ///(this.Provider as DefaultQueryProvider).ExpressionTreeParser.NodeTypeRegistry.Register(xxx.SupportedMethods, typeof(xxx));
        }
    }
}
