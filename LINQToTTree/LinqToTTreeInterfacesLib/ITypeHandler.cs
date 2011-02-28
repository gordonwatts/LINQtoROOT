using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Type Handlers sort the translation between C++ and C#... this plug-in (make sure to include Export) 
    /// allows for seperation of code when dealing with "complex" types. NOTE: once you have declared you can
    /// handle a particular type you must always handle it going forward! And if multiple people try to handle
    /// the same type only the first one will get a chance. :-)
    /// </summary>
    public interface ITypeHandler
    {
        /// <summary>
        /// Return true if you can deal with this particular type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool CanHandle(Type t);

        /// <summary>
        /// The user has referenced a constant of some sort in their code. This method should figure out
        /// how to ship the object accross the wire and how to load it, etc. (well, create the proper value for it,
        /// and add it to the list of the GeneratedCode!).
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        IValue ProcessConstantReference(ConstantExpression expr,
            IGeneratedCode codeEnv);

        /// <summary>
        /// Process a method call against this object. The result - which is the resulting expression - is put into
        /// the result, and return the expression (usually just a straight return).
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container">If expression or others need to be built, this container will be needed for MEF</param>
        /// <returns></returns>
        Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container);
    }
}
