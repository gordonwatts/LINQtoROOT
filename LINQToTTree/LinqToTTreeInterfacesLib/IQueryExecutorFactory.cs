using System;

namespace LinqToTTreeInterfacesLib
{
    public interface IQueryExecutorFactory
    {
        /// <summary>
        /// The scheme we handel
        /// </summary>
        string Scheme { get; }

        IQueryExectuor Create(IExecutionEnvironment exeReq, string[] referencedLeafNames);
    }
}
