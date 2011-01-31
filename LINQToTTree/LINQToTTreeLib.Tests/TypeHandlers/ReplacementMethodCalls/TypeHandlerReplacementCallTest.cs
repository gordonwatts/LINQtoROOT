// <copyright file="TypeHandlerReplacementCallTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerReplacementCall</summary>
    [PexClass(typeof(TypeHandlerReplacementCall))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHandlerReplacementCallTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TypeHandlerReplacementCall.ClearTypeList();
        }
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]TypeHandlerReplacementCall target, Type t)
        {
            bool result = target.CanHandle(t);
            return result;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.CanHandle(TypeHandlerReplacementCall, Type)
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedCode)</summary>
        [PexMethod]
        public IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerReplacementCall target,
            ConstantExpression expr,
            IGeneratedCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv);
            return result;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.ProcessConstantReference(TypeHandlerReplacementCall, ConstantExpression, IGeneratedCode)
        }

        /// <summary>Test stub for ProcessMethodCall(MethodCallExpression, IValue&amp;, IGeneratedCode, ICodeContext)</summary>
        [PexMethod]
        public Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerReplacementCall target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedCode gc,
            ICodeContext context
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, out result, gc, context);
            return result01;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.ProcessMethodCall(TypeHandlerReplacementCall, MethodCallExpression, IValue&, IGeneratedCode, ICodeContext)
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestProcessConstantReference()
        {
            ProcessConstantReference(new TypeHandlerReplacementCall(), null, null);
        }

        class SimpleTest
        {
            public static double noArg() { return 0.0; }
            public static double oneArg(int freak) { return 1.0; }
        }

        [TestMethod]
        public void TestCanHandleSimpleTestWithNoFile()
        {
            var r = CanHandle(new TypeHandlerReplacementCall(), typeof(SimpleTest));
            Assert.IsFalse(r, "no spec file and so nothing should have been loaded");
        }

        [TestMethod]
        public void TestCanHandleSimpleTestWithLoadedType()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg");
            var r = CanHandle(new TypeHandlerReplacementCall(), typeof(SimpleTest));
            Assert.IsTrue(r, "everything should have been loaded");
        }

        [TestMethod]
        public void TestNoArg()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);

            Assert.AreEqual(typeof(double), result.Type, "the type that came back ins't right");
            Assert.AreEqual("noArg()", result.RawValue, "raw translation incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRightClassBadMethod()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArgDude", "noArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRigthClassMethodBadArgs()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "oneArg", "oneArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("oneArg"), new Expression[] { Expression.Constant((int)1) });
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRigthClassMethodBadArgsDefined()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg", new Tuple<string, string>[] { new Tuple<string, string>(typeof(int).FullName, "int") });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);
        }

        [TestMethod]
        public void TestCallWithArgs()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "oneArg", "oneArg", new Tuple<string, string>[] { new Tuple<string, string>(typeof(int).FullName, "int") });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("oneArg"), new Expression[] { Expression.Constant((int)10) });
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);

            Assert.AreEqual("oneArg(10)", result.RawValue, "incorrected coded method argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSameMethodsDifferentArgTypes()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "oneArg", "oneArg", new Tuple<string, string>[] { new Tuple<string, string>(typeof(float).FullName, "float") });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("oneArg"), new Expression[] { Expression.Constant((int)10) });
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);
        }

        public class ParseTest
        {
            static public double sin(double x) { return x; }
            static public double f1(int x, int y) { return x; }
            static public double f2(double x, double y) { return x; }
        }

        [TestMethod]
        public void TestStringParsing()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("#<classtype Name> func(arg1 FullName type, arg2 FullName type) => c++func(cppargtype, cppargtype)");
            bld.AppendLine("ParseTest sin(System.Double) => sin(double)");
            bld.AppendLine("ParseTest f1(System.Int32, System.Int32) => f1(int, int)");
            bld.AppendLine("ParseTest f2(System.Double,System.Double) => f2(double,double)");

            var target = new TypeHandlerReplacementCall();
            target.Parse(new StringReader(bld.ToString()));

            var e1 = Expression.Call(null, typeof(ParseTest).GetMethod("sin"), new Expression[] { Expression.Constant((double)10.3) });
            var e2 = Expression.Call(null, typeof(ParseTest).GetMethod("f1"), new Expression[] { Expression.Constant((int)10), Expression.Constant((int)20) });
            var e3 = Expression.Call(null, typeof(ParseTest).GetMethod("f2"), new Expression[] { Expression.Constant((double)10.3), Expression.Constant((double)20.3) });

            var gc = new GeneratedCode();
            var context = new CodeContext();
            IValue result = null;

            ProcessMethodCall(target, e1, out result, gc, context);
            Assert.AreEqual("sin(10.3)", result.RawValue, "sin incorrect");

            ProcessMethodCall(target, e2, out result, gc, context);
            Assert.AreEqual("f1(10,20)", result.RawValue, "f1 incorrect");

            ProcessMethodCall(target, e3, out result, gc, context);
            Assert.AreEqual("f2(10.3,20.3)", result.RawValue, "f2 incorrect");
        }

        [TestMethod]
        public void TestFunctionMerge()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("ParseTest sin(System.Double) => freak(double)");
            var target = new TypeHandlerReplacementCall();
            target.Parse(new StringReader(bld.ToString()));

            bld = new StringBuilder();
            bld.AppendLine("ParseTest sin(System.Double) => sin(double)");
            target.Parse(new StringReader(bld.ToString()));

            var e1 = Expression.Call(null, typeof(ParseTest).GetMethod("sin"), new Expression[] { Expression.Constant((double)10.3) });

            var gc = new GeneratedCode();
            var context = new CodeContext();
            IValue result = null;

            ProcessMethodCall(target, e1, out result, gc, context);
            Assert.AreEqual("freak(10.3)", result.RawValue, "sin incorrect");
        }

        [TestMethod]
        public void TestIncludes()
        {
            Assert.Inconclusive("Some mathes should have include files!");
        }
    }
}
