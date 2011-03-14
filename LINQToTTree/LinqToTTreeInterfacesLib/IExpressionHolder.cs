using System.Linq.Expressions;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Holds onto an expression
    /// </summary>
    public interface IExpressionHolder
    {
        Expression HeldExpression { get; }
    }
}
