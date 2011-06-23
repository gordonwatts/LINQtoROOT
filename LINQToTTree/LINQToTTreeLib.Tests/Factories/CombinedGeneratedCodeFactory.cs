using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib
{
    /// <summary>A factory for LINQToTTreeLib.CombinedGeneratedCode instances</summary>
    public static partial class CombinedGeneratedCodeFactory
    {
        class namedVar : IVariable
        {
            public namedVar(string name)
            {
                VariableName = name;
                Type = typeof(int);
                InitialValue = new ValSimple("5", typeof(int));
                Declare = false;
                RawValue = "hi";
            }

            public string VariableName { get; set; }

            public IValue InitialValue { get; set; }

            public bool Declare { get; set; }

            public string RawValue { get; set; }

            public System.Type Type { get; set; }


            public void RenameRawValue(string oldname, string newname)
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>A factory for LINQToTTreeLib.CombinedGeneratedCode instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.CombinedGeneratedCode")]
        public static CombinedGeneratedCode Create(string[] varNamesToTransfer, string[] includeFileNames, string[] resultNames, IBookingStatementBlock[] statementBlocks, string[] leafnames)
        {
            CombinedGeneratedCode combinedGeneratedCode = new CombinedGeneratedCode();

            if (varNamesToTransfer != null)
                foreach (var item in varNamesToTransfer)
                {
                    combinedGeneratedCode.QueueVariableForTransfer(new KeyValuePair<string, object>(item, 44));
                }

            if (includeFileNames != null)
                foreach (var item in includeFileNames)
                {
                    combinedGeneratedCode.AddIncludeFile(item);
                }

            if (resultNames != null)
                foreach (var item in resultNames)
                {
                    combinedGeneratedCode.AddResult(new namedVar(item));
                }

            if (statementBlocks != null)
                combinedGeneratedCode.AddQueryBlocks(statementBlocks);

            foreach (var item in leafnames)
            {
                combinedGeneratedCode.AddReferencedLeaf(item);
            }

            return combinedGeneratedCode;
        }
    }
}
