using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    static class IValueUtils
    {
        /// <summary>
        /// Turn a IValue back into an expression. Seems backwards, but there are times we need
        /// to evaluate an expression in the current "climate" and then pass its result around
        /// (along with its dependencies).
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Expression AsExpression (this IValue v)
        {
            return new ValueExpression(v);
        }
    }
}
