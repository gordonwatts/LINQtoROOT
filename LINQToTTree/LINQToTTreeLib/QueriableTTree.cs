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
        /// ctor called when the user has opened a new TTree or TChain that is local to this machine. We specify the
        /// input ROOT file and the tree name we need to find in that root file.
        /// </summary>
        public QueriableTTree(FileInfo rootFile, string treeName)
            : base(new TTreeQueryExecutor(new FileInfo[] { rootFile }, treeName, typeof(T)))
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
