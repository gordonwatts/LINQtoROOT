using System.Collections.Generic;
using System.Linq;
using TTreeDataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TTreeParser.Tests
{
    public static class TUtils
    {
        /// <summary>
        /// Find a class in the list, return null if we can't find it.
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ROOTClassShell FindClass(this IEnumerable<ROOTClassShell> classes, string name)
        {
            Assert.AreEqual(1, classes.Where(c => c.Name == name).Count(), string.Format("# of classes called {0}", name));
            return classes.Where(c => c.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Find the class item for a particular item.
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static IClassItem FindItem(this ROOTClassShell cls, string itemName)
        {
            return cls.Items.Where(i => i.Name == itemName).FirstOrDefault();
        }
    }
}
