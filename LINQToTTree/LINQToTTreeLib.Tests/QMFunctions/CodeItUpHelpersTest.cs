using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LINQToTTreeLib.Tests.QMFunctions
{
    [TestClass]
    public class CodeItUpHelpersTest
    {
        [TestMethod]
        public void SimpleFunction()
        {
            IQMFuncExecutable f = QMFuncUtils.GenerateFunction();
            var lines = f.CodeItUp().ToArray();

            lines.DumpToConsole();
            Assert.IsTrue(lines.Where(l => l.Contains(string.Format("int {0} ()", f.Name))).Any(), "no decl");
        }
    }
}
