﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    class Utils
    {
        internal static IStatement TestRenameOfStatement(LINQToTTreeLib.Statements.StatementIncrementInteger statement, string oldname, string newname)
        {
            var origianllines = statement.CodeItUp().ToArray();
            statement.RenameVariable(oldname, newname);
            var finallines = statement.CodeItUp().ToArray();

            Assert.AreEqual(origianllines.Length, finallines.Length, "# of lines change during variable rename");

            var varReplacer = new Regex(string.Format(@"\b{0}\b", oldname));

            var sharedlines = origianllines.Zip(finallines, (o, n) => Tuple.Create(o, n));
            foreach (var pair in sharedlines)
            {
                var orig = pair.Item1;
                var origReplafce = varReplacer.Replace(orig, newname);
                Assert.AreEqual(origReplafce, pair.Item2, "expected the renaming to be pretty simple.");
            }

            return statement;
        }
    }
}
