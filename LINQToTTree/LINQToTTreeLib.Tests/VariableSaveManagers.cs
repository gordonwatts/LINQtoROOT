
using System;
using System.ComponentModel.Composition;
using System.Linq;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Basic VM that returns a guy that always does something simple.
    /// </summary>
    [Export(typeof(IVariableSaverManager))]
    class VSBasic : IVariableSaverManager
    {
        public IVariableSaver Get(IDeclaredParameter var)
        {
            if (var == null)
                throw new NotImplementedException("This should never happen");
            return new SimpleVar();
        }

        public class SimpleVar : IVariableSaver
        {
            public bool CanHandle(IDeclaredParameter iVariable)
            {
                return true;
            }

            public System.Collections.Generic.IEnumerable<string> SaveToFile(IDeclaredParameter iVariable)
            {
                yield return "save";
            }

            public System.Collections.Generic.IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
            {
                return Enumerable.Empty<string>();
            }

            public T LoadResult<T>(IDeclaredParameter iVariable, ROOTNET.Interface.NTObject[] obj)
            {
                throw new NotImplementedException();
            }

            public string[] GetCachedNames(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }
        }

    }
}
