using TTreeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TTreeParser.Tests
{
    
    
    /// <summary>
    ///This is a test class for TemplateParserTest and is intended
    ///to contain all TemplateParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TemplateParserTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ParseForTemplates
        ///</summary>
        [TestMethod()]
        public void ParseForTemplatesSimpleTypeTest()
        {
            var r = TemplateParser.ParseForTemplates("int");
            Assert.IsInstanceOfType(r, typeof(TemplateParser.RegularDecl), "incorrect type came back");
            var d = r as TemplateParser.RegularDecl;
            Assert.AreEqual("int", d.Type, "Bad name of the decl");
        }

        [TestMethod]
        public void ParseForTemplatesSimpleVector()
        {
            var r = TemplateParser.ParseForTemplates("vector<int>");
            Assert.IsInstanceOfType(r, typeof(TemplateParser.TemplateInfo), "incorrect type came back");
            var t = r as TemplateParser.TemplateInfo;
            Assert.AreEqual("vector", t.TemplateName, "bad template name");
            Assert.AreEqual(1, t.Arguments.Length, "bad # of template arguments");
            Assert.IsInstanceOfType(t.Arguments[0], typeof(TemplateParser.RegularDecl), "bad tempmlate argument type");
            var arg0 = t.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg0.Type);
        }

        [TestMethod]
        public void ParseForTemplateSimpleVectorMultipleArg()
        {
            var r = TemplateParser.ParseForTemplates("map<int, string>");
            Assert.IsInstanceOfType(r, typeof(TemplateParser.TemplateInfo), "incorrect type came back");
            var t = r as TemplateParser.TemplateInfo;
            Assert.AreEqual("map", t.TemplateName, "bad template name");
            Assert.AreEqual(2, t.Arguments.Length, "bad # of template arguments");

            Assert.IsInstanceOfType(t.Arguments[0], typeof(TemplateParser.RegularDecl), "bad tempmlate argument type");
            var arg0 = t.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg0.Type);

            Assert.IsInstanceOfType(t.Arguments[1], typeof(TemplateParser.RegularDecl), "bad tempmlate argument type");
            var arg1 = t.Arguments[1] as TemplateParser.RegularDecl;
            Assert.AreEqual("string", arg1.Type);
        }

        [TestMethod]
        public void ParseForTemplateSimpleVectorNestingArg()
        {
            var r = TemplateParser.ParseForTemplates("vector<int, allocator<int> >");
            Assert.IsInstanceOfType(r, typeof(TemplateParser.TemplateInfo), "incorrect type came back");
            var t = r as TemplateParser.TemplateInfo;
            Assert.AreEqual("vector", t.TemplateName, "bad template name");
            Assert.AreEqual(2, t.Arguments.Length, "bad # of template arguments");

            Assert.IsInstanceOfType(t.Arguments[0], typeof(TemplateParser.RegularDecl), "bad tempmlate argument type");
            var arg0 = t.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg0.Type);

            Assert.IsInstanceOfType(t.Arguments[1], typeof(TemplateParser.TemplateInfo), "bad tempmlate argument type");
            var arg1 = t.Arguments[1] as TemplateParser.TemplateInfo;
            Assert.AreEqual("allocator", arg1.TemplateName);
            Assert.AreEqual(1, arg1.Arguments.Length, "incorrect # of arguments to nested template");
            Assert.IsInstanceOfType(arg1.Arguments[0], typeof(TemplateParser.RegularDecl), "Nested argument isn't right");
            var arg10 = arg1.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg10.Type, "incorrect nested argument type");
        }

        [TestMethod]
        public void ParseForTemplateSimpleVectorNestingOuchArg()
        {
            var r = TemplateParser.ParseForTemplates("map<int, vector<int, allocator<int> >, allocator<int> >");

            Assert.IsInstanceOfType(r, typeof(TemplateParser.TemplateInfo), "incorrect type came back");
            var t = r as TemplateParser.TemplateInfo;
            Assert.AreEqual("map", t.TemplateName, "bad template name");
            Assert.AreEqual(3, t.Arguments.Length, "bad # of template arguments");

            Assert.IsInstanceOfType(t.Arguments[0], typeof(TemplateParser.RegularDecl), "bad tempmlate argument type");
            var arg0 = t.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg0.Type);

            Assert.IsInstanceOfType(t.Arguments[1], typeof(TemplateParser.TemplateInfo), "bad tempmlate argument type");
            var arg1 = t.Arguments[1] as TemplateParser.TemplateInfo;
            Assert.AreEqual("vector", arg1.TemplateName);
            Assert.AreEqual(2, arg1.Arguments.Length, "incorrect # of arguments to nested template");
            Assert.IsInstanceOfType(arg1.Arguments[0], typeof(TemplateParser.RegularDecl), "vector 1st arg not right");
            Assert.IsInstanceOfType(arg1.Arguments[1], typeof(TemplateParser.TemplateInfo), "vector 2nd arg not right");
            var arg10 = arg1.Arguments[0] as TemplateParser.RegularDecl;
            var arg11 = arg1.Arguments[1] as TemplateParser.TemplateInfo;
            Assert.AreEqual("int", arg10.Type, "type of vector's 1st arg not right");
            Assert.AreEqual("allocator", arg11.TemplateName, "expected name of seocond tempmlate arg to vector not right");
            Assert.AreEqual(1, arg11.Arguments.Length, "incorrect # of arguments to allocator of vector");
            Assert.IsInstanceOfType(arg11.Arguments[0], typeof(TemplateParser.RegularDecl), "bad allocator argument type");
            var arg110 = arg11.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg110.Type, "bad type for allcoator argument of vector");

            Assert.IsInstanceOfType(t.Arguments[2], typeof(TemplateParser.TemplateInfo), "bad tempmlate argument type");
            var arg2 = t.Arguments[2] as TemplateParser.TemplateInfo;
            Assert.AreEqual("allocator", arg2.TemplateName);
            Assert.AreEqual(1, arg2.Arguments.Length, "incorrect # of arguments to allocator of map");
            Assert.IsInstanceOfType(arg2.Arguments[0], typeof(TemplateParser.RegularDecl), "bad allocator argument type for map");
            var arg20 = arg11.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg20.Type, "bad type for allcoator argument of map");
        }

        [TestMethod]
        public void ParseForTemplateSimpleVectorNestingOuchNoSpacesArg()
        {
            var r = TemplateParser.ParseForTemplates("map<int,vector<int,allocator<int> >,allocator<int> >");

            Assert.IsInstanceOfType(r, typeof(TemplateParser.TemplateInfo), "incorrect type came back");
            var t = r as TemplateParser.TemplateInfo;
            Assert.AreEqual("map", t.TemplateName, "bad template name");
            Assert.AreEqual(3, t.Arguments.Length, "bad # of template arguments");

            Assert.IsInstanceOfType(t.Arguments[0], typeof(TemplateParser.RegularDecl), "bad tempmlate argument type");
            var arg0 = t.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg0.Type);

            Assert.IsInstanceOfType(t.Arguments[1], typeof(TemplateParser.TemplateInfo), "bad tempmlate argument type");
            var arg1 = t.Arguments[1] as TemplateParser.TemplateInfo;
            Assert.AreEqual("vector", arg1.TemplateName);
            Assert.AreEqual(2, arg1.Arguments.Length, "incorrect # of arguments to nested template");
            Assert.IsInstanceOfType(arg1.Arguments[0], typeof(TemplateParser.RegularDecl), "vector 1st arg not right");
            Assert.IsInstanceOfType(arg1.Arguments[1], typeof(TemplateParser.TemplateInfo), "vector 2nd arg not right");
            var arg10 = arg1.Arguments[0] as TemplateParser.RegularDecl;
            var arg11 = arg1.Arguments[1] as TemplateParser.TemplateInfo;
            Assert.AreEqual("int", arg10.Type, "type of vector's 1st arg not right");
            Assert.AreEqual("allocator", arg11.TemplateName, "expected name of seocond tempmlate arg to vector not right");
            Assert.AreEqual(1, arg11.Arguments.Length, "incorrect # of arguments to allocator of vector");
            Assert.IsInstanceOfType(arg11.Arguments[0], typeof(TemplateParser.RegularDecl), "bad allocator argument type");
            var arg110 = arg11.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg110.Type, "bad type for allcoator argument of vector");

            Assert.IsInstanceOfType(t.Arguments[2], typeof(TemplateParser.TemplateInfo), "bad tempmlate argument type");
            var arg2 = t.Arguments[2] as TemplateParser.TemplateInfo;
            Assert.AreEqual("allocator", arg2.TemplateName);
            Assert.AreEqual(1, arg2.Arguments.Length, "incorrect # of arguments to allocator of map");
            Assert.IsInstanceOfType(arg2.Arguments[0], typeof(TemplateParser.RegularDecl), "bad allocator argument type for map");
            var arg20 = arg11.Arguments[0] as TemplateParser.RegularDecl;
            Assert.AreEqual("int", arg20.Type, "bad type for allcoator argument of map");
        }

        [TestMethod]
        public void ParseForTemplateSimpleVectorTrippleArg()
        {
            var r = TemplateParser.ParseForTemplates("vector<vector<vector<int> > >");
            Assert.IsInstanceOfType(r, typeof(TemplateParser.TemplateInfo), "incorrect type came back");
            var t = r as TemplateParser.TemplateInfo;
            Assert.AreEqual("vector", t.TemplateName, "bad template name");
            Assert.AreEqual(1, t.Arguments.Length, "bad # of template arguments");
            Assert.IsInstanceOfType(t.Arguments[0], typeof(TemplateParser.TemplateInfo), "vector's argument bad type");
            var arg0 = t.Arguments[0] as TemplateParser.TemplateInfo;

            Assert.AreEqual("vector", arg0.TemplateName, "bad template argument name");
            Assert.AreEqual(1, arg0.Arguments.Length, "bad # of arguments to vector<vector template");
            Assert.IsInstanceOfType(arg0.Arguments[0], typeof(TemplateParser.TemplateInfo), "bad return type for vector<vector argument");
            var arg00 = arg0.Arguments[0] as TemplateParser.TemplateInfo;

            Assert.AreEqual("vector", arg00.TemplateName, "bad template argument name");
            Assert.AreEqual(1, arg00.Arguments.Length, "bad # of arguments to vector<vector template");
            Assert.IsInstanceOfType(arg00.Arguments[0], typeof(TemplateParser.RegularDecl), "bad return type for vector<vector argument");
            var arg000 = arg00.Arguments[0] as TemplateParser.RegularDecl;

            Assert.AreEqual("int", arg000.Type, "bad type for innermost template");
        }
    }
}
