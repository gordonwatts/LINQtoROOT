﻿using System.Collections.Generic;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Any statement that implements this contains sub-statements.
    /// </summary>
    public interface IStatementCompound : IStatement
    {
        /// <summary>
        /// The list of statements that are in our "context"
        /// </summary>
        IEnumerable<IStatement> Statements { get; }

        /// <summary>
        /// Add a new statement to the compound block. It will be appended to the
        /// end of the block (after the last added statement).
        /// </summary>
        /// <param name="statement"></param>
        void Add(IStatement statement);
    }
}
