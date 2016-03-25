using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static LINQToTTreeLib.Optimization.OptimizationUtils;

namespace LINQToTTreeLib.Tests.Optimization
{
    [TestClass]
    public class OptimizationUtilsTest
    {
        [TestMethod]
        public void CommuteDifferentStatements()
        {
            var s1 = CreateConstAssignStatement();
            var s2 = CreateConstAssignStatement();
            Assert.IsTrue(StatementCommutes(s1.Item2, s2.Item2));
        }

        [TestMethod]
        public void CommuteAssignToSameStatements()
        {
            var s1 = CreateConstAssignStatement();
            var s2 = CreateConstAssignStatement(s1.Item1);
            Assert.IsFalse(StatementCommutes(s1.Item2, s2.Item2));
        }

        [TestMethod]
        public void CommuteAssignFromCommonDependent()
        {
            var pCommon = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = CreateAssignStatement(pCommon);
            var s2 = CreateAssignStatement(pCommon);
            Assert.IsTrue(StatementCommutes(s1.Item2, s2.Item2));
        }

        [TestMethod]
        public void CommuteDependentStatements()
        {
            var sConst = CreateConstAssignStatement();
            var sValue = CreateAssignStatement(sConst.Item1);
            Assert.IsFalse(StatementCommutes(sConst.Item2, sValue.Item2));
        }

        /// <summary>
        /// Helper to create an assignment to a constant.
        /// </summary>
        /// <returns></returns>
        Tuple<IDeclaredParameter, IStatement> CreateConstAssignStatement(IDeclaredParameter p1 = null)
        {
            if (p1 == null)
            {
                p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            }
            IStatement s1 = new StatementAssign(p1, new ValSimple("5", typeof(int)));
            return Tuple.Create(p1, s1);
        }

        Tuple<IDeclaredParameter, IStatement> CreateAssignStatement(IValue valToAssign, IDeclaredParameter p1 = null)
        {
            if (p1 == null)
            {
                p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            }
            IStatement s1 = new StatementAssign(p1, valToAssign);
            return Tuple.Create(p1, s1);
        }
    }
}
