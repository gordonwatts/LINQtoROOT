using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables.Savers
{
    /// <summary>
    /// Save simple guys - like an integer.
    /// </summary>
    [Export(typeof(IVariableSaver))]
    class SaveSimpleVariable : IVariableSaver
    {
        /// <summary>
        /// We can deal with things like integers.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public bool CanHandle(IVariable iVariable)
        {
            if (iVariable.GetType() == typeof(VarInteger))
                return true;

            return false;
        }

        /// <summary>
        /// Generate the code that will save this guy to a file.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> SaveToFile(IVariable iVariable)
        {
            yield return "TH1F *" + iVariable.RawValue + "_hist = new TH1F(\"" + iVariable.RawValue + "\", \"var transport\", 1, 0.0, 1.0);";
            yield return iVariable.RawValue + "_hist->SetBinContent(1, " + iVariable.RawValue + ");";
            yield return "Book(" + iVariable.RawValue + "_hist);";
        }

        /// <summary>
        /// what include files do we need?
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IVariable iVariable)
        {
            yield return "TH1F.h";
        }

        /// <summary>
        /// We need to load the variable saver back in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IVariable iVariable, ROOTNET.Interface.NTObject obj)
        {
            var intHist = obj as ROOTNET.Interface.NTH1;
            if (intHist == null)
                throw new InvalidOperationException("Object of type '" + obj.ClassName() + "' is not an integer histogram, which is what we were expecting for this result!");

            object result = (int)intHist.GetBinContent(1);
            return (T)result;
        }
    }
}
