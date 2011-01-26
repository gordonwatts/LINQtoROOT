using System;
using System.ComponentModel.Composition;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Basic VM that returns a guy that always does something simple.
    /// </summary>
    [Export(typeof(IVariableSaverManager))]
    class VSBasic : IVariableSaverManager
    {
        public IVariableSaver Get(IVariable var)
        {
            if (var == null)
                throw new NotImplementedException("This should never happen");
            return new SimpleVar();
        }

        public class SimpleVar : IVariableSaver
        {
            public bool CanHandle(IVariable iVariable)
            {
                return true;
            }

            public System.Collections.Generic.IEnumerable<string> SaveToFile(IVariable iVariable)
            {
                yield return "save";
            }

            public System.Collections.Generic.IEnumerable<string> IncludeFiles(IVariable iVariable)
            {
                yield return "file.h";
            }

            public T LoadResult<T>(IVariable iVariable, ROOTNET.Interface.NTObject obj)
            {
                throw new NotImplementedException();
            }
        }

    }
}
