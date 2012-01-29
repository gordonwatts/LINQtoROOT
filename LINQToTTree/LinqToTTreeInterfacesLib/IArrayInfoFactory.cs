
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Given an expression find an array info object that can generate
    /// a loop on demand.
    /// </summary>
    /// <remarks>
    /// These must be coded so that one and only one can ever be activated. There is no
    /// order of precidence in these!!
    /// </remarks>
    public interface IArrayInfoFactory
    {
        /// <summary>
        /// Get the array info. Return null if this factory can't deal with this array type.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="ReGetIArrayInfo">Having parsed it here, hand it off to be re-parsed. Make sure recusion isn't going to happen here!!</param>
        /// <returns></returns>
        IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo);
    }
}
