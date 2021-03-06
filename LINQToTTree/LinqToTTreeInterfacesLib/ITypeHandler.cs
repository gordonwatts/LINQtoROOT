﻿using System;
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
        /// The user (or system) has referenced a constant of some sort in the query code. This should figure out
        /// how to move the item accross the wire. Return a value! This mehtod is called very late in the translation
        /// process - when all parameter replacement, translation, etc., has occured.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        IValue ProcessConstantReference(ConstantExpression expr,
            IGeneratedQueryCode codeEnv,
            CompositionContainer container);

        /// <summary>
        /// A constant reference has appeared in the code. It can be transformed by this method into
        /// another expression if desired. This is called fiarly "early" in the process.
        /// </summary>
        /// <remarks>
        /// This method
        /// should always return the incoming expression if it isn't implemented if any constnat expressions
        /// will occur (or you'll get a crash!).
        /// </remarks>
        /// <param name="expr"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        Expression ProcessConstantReferenceExpression(ConstantExpression expr,
            CompositionContainer container);

        /// <summary>
        /// Process a method call against an object. This happens fiarly early in the process, and
        /// should be used when basic translation needs to happen.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container">If expression or others need to be built, this container will be needed for MEF</param>
        /// <returns></returns>
        Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container);

        /// <summary>
        /// A method call needs to be put into C++ - This method should handle that.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container);

        /// <summary>
        /// Process an object creation call. The result, which is the resulting expression. Note that when
        /// this area of code goes out of scope the object should automatically be deleted! :-)
        /// </summary>
        /// <remarks>
        /// This is called fairly late in the trnaslation process - after all argument substitions has been done.
        /// </remarks>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container);

        /// <summary>
        /// Process a member reference ("instan.Key" for example). A null return means that
        /// it couldn't be handled. Throw means it should have been handled, but wasn't.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        IValue ProcessMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container);
    }
}
