using System;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>A factory for LINQToTTreeLib.Expressions.DeclarableParameter instances</summary>
    public static partial class DeclarableParameterFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Expressions.DeclarableParameter instances</summary>
        [PexFactoryMethod(typeof(DeclarableParameter))]
        public static DeclarableParameter Create(int vStyle, Type t1, Type t2)
        {
            if (vStyle == 0)
                return DeclarableParameter.CreateDeclarableParameterExpression(t1);

            if (vStyle == 1)
                return DeclarableParameter.CreateDeclarableParameterArrayExpression(t1);

            return DeclarableParameter.CreateDeclarableParameterMapExpression(t1, t2);
        }
    }
}
