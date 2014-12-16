// <copyright file="TypeHandlerReplacementCallTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerReplacementCall</summary>
    [PexClass(typeof(TypeHandlerReplacementCall))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    [DeploymentItem(@"ConfigData\default.classmethodmappings")]
    public partial class TypeHandlerReplacementCallTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for CanHandle(Type)</summary>
        //[PexMethod]
        public bool CanHandle([PexAssumeUnderTest]TypeHandlerReplacementCall target, Type t)
        {
            bool result = target.CanHandle(t);
            return result;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.CanHandle(TypeHandlerReplacementCall, Type)
        }

        /// <summary>Test stub for ProcessMethodCall(MethodCallExpression, IValue&amp;, IGeneratedCode, ICodeContext)</summary>
        ///[PexMethod]
        public Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerReplacementCall target,
            MethodCallExpression expr,
            IGeneratedQueryCode gc,
            ICodeContext context
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, gc, context, MEFUtilities.MEFContainer);
            return result01;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.ProcessMethodCall(TypeHandlerReplacementCall, MethodCallExpression, IValue&, IGeneratedCode, ICodeContext)
        }

        [PexMethod]
        public IValue CodeMethodCall(
            [PexAssumeUnderTest]TypeHandlerReplacementCall target,
            MethodCallExpression expr,
            IGeneratedQueryCode gc
        )
        {
            var result01 = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);
            return result01;
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestProcessConstantReference()
        {
            var p = new TypeHandlerReplacementCall();
            p.ProcessConstantReference(null, null, null);
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
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var result = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);

            Assert.AreEqual(typeof(double), result.Type, "the type that came back ins't right");
            Assert.AreEqual("noArg()", result.RawValue, "raw translation incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRightClassBadMethod()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArgDude", "noArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRigthClassMethodBadArgs()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "oneArg", "oneArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("oneArg"), new Expression[] { Expression.Constant((int)1) });
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRigthClassMethodBadArgsDefined()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg", new Tuple<string, string>[] { new Tuple<string, string>(typeof(int).FullName, "int") });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);
        }

        [TestMethod]
        public void TestCallWithArgs()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "oneArg", "oneArg", new Tuple<string, string>[] { new Tuple<string, string>(typeof(int).FullName, "int") });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("oneArg"), new Expression[] { Expression.Constant((int)10) });
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);

            Assert.AreEqual("oneArg((int)10)", r.RawValue, "incorrected coded method argument");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSameMethodsDifferentArgTypes()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "oneArg", "oneArg", new Tuple<string, string>[] { new Tuple<string, string>(typeof(float).FullName, "float") });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("oneArg"), new Expression[] { Expression.Constant((int)10) });
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);
        }

        public class ParseTest
        {
            static public double sin(double x) { return x; }
            static public double cos(double x) { return x; }
            static public double tan(double x) { return x; }
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

            var result = CodeMethodCall(target, e1, gc);
            Assert.AreEqual("sin((double)10.3)", result.RawValue, "sin incorrect");

            result = CodeMethodCall(target, e2, gc);
            Assert.AreEqual("f1((int)10,(int)20)", result.RawValue, "f1 incorrect");

            result = CodeMethodCall(target, e3, gc);
            Assert.AreEqual("f2((double)10.3,(double)20.3)", result.RawValue, "f2 incorrect");
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

            var result = CodeMethodCall(target, e1, gc);
            Assert.AreEqual("freak((double)10.3)", result.RawValue, "sin incorrect");
        }

        [TestMethod]
        public void TestIncludes()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg", null, new string[] { "cmath" });

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var result = CodeMethodCall(new TypeHandlerReplacementCall(), e, gc);

            Assert.AreEqual(1, gc.IncludeFiles.Count(), "# include files");
            Assert.AreEqual("cmath", gc.IncludeFiles.First(), "include filename");
        }

        [TestMethod]
        public void TestParseInclude()
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendLine("include: cmath1 cmath3");
            bld.AppendLine("ParseTest sin(System.Double) => freak(double)");
            bld.AppendLine("include: cmath2");
            bld.AppendLine("ParseTest cos(System.Double) => cos(double)");
            bld.AppendLine("include: ");
            bld.AppendLine("ParseTest tan(System.Double) => tan(double)");
            var target = new TypeHandlerReplacementCall();
            target.Parse(new StringReader(bld.ToString()));

            var gc = new GeneratedCode();
            var context = new CodeContext();

            var e0 = Expression.Call(null, typeof(ParseTest).GetMethod("tan"), new Expression[] { Expression.Constant((double)10.3) });
            var result = CodeMethodCall(target, e0, gc);
            Assert.AreEqual(0, gc.IncludeFiles.Count(), "# include files after none should have been added");

            var e1 = Expression.Call(null, typeof(ParseTest).GetMethod("cos"), new Expression[] { Expression.Constant((double)10.3) });
            result = CodeMethodCall(target, e1, gc);
            Assert.AreEqual(1, gc.IncludeFiles.Count(), "# include files");
            Assert.AreEqual("cmath2", gc.IncludeFiles.First(), "include filename");

            var e2 = Expression.Call(null, typeof(ParseTest).GetMethod("sin"), new Expression[] { Expression.Constant((double)10.3) });
            result = CodeMethodCall(target, e2, gc);
            Assert.AreEqual(3, gc.IncludeFiles.Count(), "# include files");
            Assert.IsTrue(gc.IncludeFiles.Contains("cmath1"), "cmath1 missing");
            Assert.IsTrue(gc.IncludeFiles.Contains("cmath2"), "cmath1 missing");
            Assert.IsTrue(gc.IncludeFiles.Contains("cmath3"), "cmath1 missing");
        }

        [TestMethod]
        public void TestAutoLoad()
        {
            var tanmethod = from m in typeof(Math).GetMethods()
                            where m.Name == "Tan"
                            where m.ReturnParameter.ParameterType == typeof(double)
                            select m;

            var e0 = Expression.Call(null, tanmethod.FirstOrDefault(), new Expression[] { Expression.Constant((double)10.3) });

            var gc = new GeneratedCode();
            var context = new CodeContext();
            var target = new TypeHandlerReplacementCall();

            CodeMethodCall(target, e0, gc);
        }
    }
}
