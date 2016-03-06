using LINQToTTreeLib.Utils;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Convert the AsCSV extension method into an QueryModel information block.
    /// </summary>
    class AsTTreeExpressionNode : AsFileExpressionNode
    {
        /// <summary>
        /// What extensions can we handle here?
        /// </summary>
        public static MethodInfo[] SupportedMethods =
             new[] {
                TypeUtils.GetSupportedMethod (() => FileHelperQueryExtensions.AsTTree((IQueryable<object>) null, (string) null, (string) null, (FileInfo) null, (string[]) null)),
             };

        readonly Expression _treeName;
        readonly Expression _treeTitle;

        /// <summary>
        /// Initialize with the file and column headers
        /// </summary>
        /// <param name="parseInfo"></param>
        /// <param name="fileInfo"></param>
        /// <param name="columnNames"></param>
        public AsTTreeExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression treeName, Expression treeTitle, Expression fileInfo, Expression columnNames)
            : base(parseInfo, fileInfo, columnNames)
        {
            _treeName = treeName;
            _treeTitle = treeTitle;
        }

        /// <summary>
        /// Create the actual re-linq resolution operator.
        /// </summary>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new AsTTreeResultOperator(
                (_treeName as ConstantExpression).Value as string, (_treeTitle as ConstantExpression).Value as string,
                (_fileInfo as ConstantExpression).Value as FileInfo, _columnNames);
        }
    }
}
