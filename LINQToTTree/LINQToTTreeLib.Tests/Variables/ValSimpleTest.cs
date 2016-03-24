// <copyright file="ValSimpleTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for ValSimple</summary>
    [TestClass]
    public partial class ValSimpleTest
    {
        [TestMethod]
        public void RenameMethodCall()
        {
            var target = new ValSimple("(*aNTH1F_1233).Fill(((double)aInt32_326),1.0*((1.0*1.0)*1.0))", typeof(int));
            target.RenameRawValue("aInt32_326", "aInt32_37");
            Assert.AreEqual("(*aNTH1F_1233).Fill(((double)aInt32_37),1.0*((1.0*1.0)*1.0))", target.RawValue);
        }

        [TestMethod]
        public void ValSimpleNullDependents()
        {
            var v = new ValSimple("5", typeof(int), null);
            Assert.IsNotNull(v.Dependants);
            Assert.AreEqual(0, v.Dependants.Count());
        }

        [TestMethod]
        public void ValSimpleWithDependents()
        {
            var d = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v = new ValSimple($"5+{d.RawValue}", typeof(int), new IDeclaredParameter[] { d });
            Assert.IsNotNull(v.Dependants);
            Assert.AreEqual(1, v.Dependants.Count());
        }

        [TestMethod]
        public void ValSimpleRenameWithDependents()
        {
            var d = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v = new ValSimple($"5+{d.RawValue}", typeof(int), new IDeclaredParameter[] { d });

            v.RenameRawValue(d.RawValue, "my_go_1");
            Assert.AreEqual("5+my_go_1", v.RawValue);
            Assert.AreEqual("my_go_1", v.Dependants.First().RawValue);
        }
    }
}
