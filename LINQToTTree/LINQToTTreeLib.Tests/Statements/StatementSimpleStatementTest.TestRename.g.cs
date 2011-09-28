using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>

namespace LINQToTTreeLib.Statements
{
    public partial class StatementSimpleStatementTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException351()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("\0", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, (string)null, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestRenameThrowsArgumentException670()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "\u0089", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException226()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestRenameThrowsArgumentException749()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "\0", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException440()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w\0", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException331()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "ww", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException854()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\0\ufeff", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, (string)null, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestRenameThrowsArgumentException210()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0100", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "\u0100");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename127()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0100", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w\0", "\u0100w");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0100", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestRenameThrowsArgumentException61()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\0\u0100\u2000", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "\0\u0100\u2000", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestRenameThrowsArgumentException311()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\ufeff\0\ufeff ", false);
    statementSimpleStatement1 = this.TestRename
                                    (statementSimpleStatement, "\ufeff\0\ufeff ", "\ufeff\0\ufeff ");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename202()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0100", false);
    statementSimpleStatement1 = this.TestRename(statementSimpleStatement, "w", "w");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0100", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException755()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("\0", true);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, (string)null, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException908()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("\0\0", true);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, (string)null, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename20201()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 = this.TestRename(statementSimpleStatement, "w", "w");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("w", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException872()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "ww\0", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException13()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "www", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException402()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename702()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\u0100");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("w\u0100", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename59()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0w");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename215()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\u00b5");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("w\u00b5", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename133()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename649()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$`");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename675()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0${");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException690()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$$");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename852()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$0");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException20()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$\'");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException721()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$&");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException681()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$+");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException326()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$_");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException495()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$`");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException243()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$1");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename200()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0${\u00fa");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException122()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0${\u00b3");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException27()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$0$X");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException983()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0${x\u00b5");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException332()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$0$0");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestRenameThrowsArgumentException580()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\0\u0100\u0100\u2000", false);
    statementSimpleStatement1 = this.TestRename(statementSimpleStatement, "w", "");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(AssertFailedException))]
public void TestRenameThrowsAssertFailedException918()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement = StatementSimpleStatementFactory.Create("w", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0$00");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename209()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0${\u00fe\u00b3");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void TestRename946()
{
    StatementSimpleStatement statementSimpleStatement;
    StatementSimpleStatement statementSimpleStatement1;
    statementSimpleStatement =
      StatementSimpleStatementFactory.Create("\u0089", false);
    statementSimpleStatement1 =
      this.TestRename(statementSimpleStatement, "w", "w\0${\u00fa}");
    Assert.IsNotNull((object)statementSimpleStatement1);
    Assert.AreEqual<string>("\u0089", statementSimpleStatement1.Line);
    Assert.AreEqual<bool>(false, statementSimpleStatement1.AddSemicolon);
    Assert.IsNull(statementSimpleStatement1.Parent);
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementSimpleStatement, (object)statementSimpleStatement1));
}
    }
}
