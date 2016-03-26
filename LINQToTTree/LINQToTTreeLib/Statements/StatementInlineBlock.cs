using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using System.Linq;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implements a block of statements with declarations at the start. It is its own scope - so
    /// everything declared will disappear when we leave this guy. Pretty dumb, actually.
    /// </summary>
    public class StatementInlineBlock : StatementInlineBlockBase
    {
        /// <summary>
        /// Any statement can pop out that wants to - nothing is protected.
        /// </summary>
        public override bool AllowNormalBubbleUp { get { return true; } }

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
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement should not be null");

            if (!(statement is StatementInlineBlock))
            {
                return false;
            }

            //
            // Since it is an inline block, we can just try to combine the individual guys
            // that are deep in it. We do this by lifing statements out as much as we can.
            //

            Combine(statement as StatementInlineBlockBase, opt);

            return true;
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

        /// <summary>
        /// Return the index variables for this loop.
        /// </summary>
        public override IEnumerable<IDeclaredParameter> InternalResultVarialbes
        {
            get
            {
                return new IDeclaredParameter[] { };
            }
        }

        /// <summary>
        /// Since there is no gateway check like an if statement, this is automatically true.
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        public override bool CommutesWithGatingExpressions(ICMStatementInfo followStatement)
        {
            return true;
        }
    }
}
