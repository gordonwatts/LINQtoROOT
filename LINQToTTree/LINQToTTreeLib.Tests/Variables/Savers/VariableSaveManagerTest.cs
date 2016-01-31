using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables.Savers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROOTNET.Interface;

namespace LINQToTTreeLib.Tests.Variables.Savers
{
    [TestClass]
    public class VariableSaveManagerTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.AddPart(new LINQToTTreeLib.Variables.Savers.SaveSimpleVariable());
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void SaverManagerNormalObject()
        {
            VariableSaverManager mgr = GetInitializedSaveManager();

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s = mgr.Get(p);
        }

        /// <summary>
        /// Returns a save manager ready to go.
        /// </summary>
        /// <returns></returns>
        private static VariableSaverManager GetInitializedSaveManager()
        {
            var mgr = new VariableSaverManager();
            MEFUtilities.Compose(mgr);
            return mgr;
        }
    }
}
