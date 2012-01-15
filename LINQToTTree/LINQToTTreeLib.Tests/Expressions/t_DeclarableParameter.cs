using System.Collections.Generic;
using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for DeclarableParameterTest and is intended
    ///to contain all DeclarableParameterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DeclarableParameterTest
    {
        /// <summary>
        ///A test for CreateDeclarableParameterMapExpression
        ///</summary>
        [TestMethod()]
        public void CreateDeclarableParameterMapExpressionTest()
        {
            var r = DeclarableParameter.CreateDeclarableParameterMapExpression(typeof(int), typeof(double));
            Assert.AreEqual(typeof(Dictionary<int, double>), r.Type, "map type not right");
        }
    }
}
