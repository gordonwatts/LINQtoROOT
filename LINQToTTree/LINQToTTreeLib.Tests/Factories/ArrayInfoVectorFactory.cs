using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>A factory for LINQToTTreeLib.Expressions.ArrayInfoVector+StatementVectorLoop instances</summary>
    public static partial class ArrayInfoVectorFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Expressions.ArrayInfoVector+StatementVectorLoop instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Expressions.ArrayInfoVector+StatementVectorLoop")]
        public static StatementForLoop CreateStatementVectorLoop(Type baseType)
        {
            var arrType = baseType.MakeArrayType();
            var expr = Expression.Parameter(arrType);
            var ainfo = new ArrayInfoVector(expr);

            var gc = new GeneratedCode();
            var cc = new CodeContext();
            var r = ainfo.AddLoop(gc, cc, MEFUtilities.MEFContainer);

            var st = gc.CodeBody.Statements.Last();

            return st as StatementForLoop;
        }
    }
}
