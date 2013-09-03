using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests.Expressions
{
    [TestClass]
    public class FindDeclarableParametersTest
    {
        [TestMethod]
        public void TestFindNothing()
        {
            var t = FindDeclarableParameters.FindAll(Expression.Constant(10)).Count();
            Assert.AreEqual(t, 0, "no decl param in a constant expression.");
        }

        [TestMethod]
        public void TestSimpleFind()
        {
            var t = FindDeclarableParameters.FindAll(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int))).Count();
            Assert.AreEqual(t, 1, "single decl statement.");
        }

        [TestMethod]
        public void TestNestedFind()
        {
            var a = Expression.Add(
                DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)),
                DeclarableParameter.CreateDeclarableParameterExpression(typeof(double))
                );
            var t = FindDeclarableParameters.FindAll(a).Count();
            Assert.AreEqual(t, 2, "Two different decl in an add statement.");
        }
    }
}
