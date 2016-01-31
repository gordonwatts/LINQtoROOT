using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace LINQToTTreeLib.Variables.Savers
{
    /// <summary>
    /// Save simple guys - like an integer.
    /// </summary>
    [Export(typeof(IVariableSaver))]
    public class SaveSimpleVariable : IVariableSaver
    {
        private class VTypeInfo
        {
            public string _htype;
            public Func<ROOTNET.NTH1, object> _converter;
        };

        /// <summary>
        /// Return the list of all the things we want to cache to make up this variable.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public string[] GetCachedNames(IDeclaredParameter iVariable)
        {
            return new string[] { iVariable.RawValue };
        }

        /// <summary>
        /// A list of the types we know how to deal with.
        /// </summary>
        private static Dictionary<Type, VTypeInfo> _types = new Dictionary<Type, VTypeInfo>() { 
           {typeof(int), new VTypeInfo() {_htype = "TH1I", _converter = h => (int)h.GetBinContent(1)}},
           {typeof(double), new VTypeInfo() {_htype = "TH1D", _converter = h => (double)h.GetBinContent(1)}}
        };


        /// <summary>
        /// We can deal with things like integers.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public bool CanHandle(IDeclaredParameter iVariable)
        {
            if (_types.ContainsKey(iVariable.Type))
                return true;

            return false;
        }

        /// <summary>
        /// Generate the code that will save this guy to a file.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> SaveToFile(IDeclaredParameter iVariable)
        {
            var tn = _types[iVariable.Type]._htype;

            yield return string.Format("{0} *{1}_hist = new {0}(\"{1}\", \"var transport\", 1, 0.0, 1.0);", tn, iVariable.RawValue);
            yield return iVariable.RawValue + "_hist->SetDirectory(0);";
            yield return iVariable.RawValue + "_hist->SetBinContent(1, " + iVariable.RawValue + ");";
            yield return "Book(" + iVariable.RawValue + "_hist);";
        }

        /// <summary>
        /// what include files do we need?
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
        {
            yield return _types[iVariable.Type]._htype + ".h";
        }

        /// <summary>
        /// We need to load the variable saver back in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IDeclaredParameter iVariable, ROOTNET.Interface.NTObject[] obj)
        {
            var h = obj[0] as ROOTNET.NTH1;
            if (h == null)
                throw new InvalidOperationException("Object of type '" + obj[0].ClassName() + "' is not an integer histogram, which is what we were expecting for this result!");

            object result = _types[iVariable.Type]._converter(h);
            return (T)result;
        }
    }
}
