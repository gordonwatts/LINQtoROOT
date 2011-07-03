using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implements a block of statements with declarations at the start. It is its own scope - so
    /// everything declared will disappear when we leave this guy. Pretty dumb, actually.
    /// </summary>
    public class StatementInlineBlock : StatementInlineBlockBase
    {
        /// <summary>
        /// Return this translated to code, inside curly braced. First variable decl and then the statements.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            return RenderInternalCode();
        }

        /// <summary>
        /// Try to combine this statement with another statement. The key thing abou tinline blocks
        /// is they are meaningless seperations of code. So we just keep lifing the empty ones
        /// up to the proper leve.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement should not be null");

            //
            // If this is not a plain inline block, we can do a simple add
            //

            if (statement.GetType() != typeof(StatementInlineBlock))
            {
                Combine(new[] { statement });
                return true;
            }

            //
            // Since it is an inline block, we can just try to combine the individual guys
            // that are deep in it. We do this by lifing statements out as much as we can.
            //

            var otherInline = statement as StatementInlineBlock;
            Combine(otherInline.DeclaredVariables);
            foreach (var s in otherInline.Statements)
            {
                TryCombineStatement(s);
            }

            return true;
        }

        /// <summary>
        /// See if the statement is the same. We check for simple 
        /// consistency here, nothing more.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            var other = statement as StatementInlineBlock;
            if (other == null)
                return false;

            return base.IsSameStatement(other);
        }

        /// <summary>
        /// Rename all our guys!
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string originalName, string newName)
        {
            RenameBlockVariables(originalName, newName);
        }
    }
}
