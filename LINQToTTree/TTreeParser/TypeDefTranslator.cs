using System.Collections.Generic;
using System.Linq;

namespace TTreeParser
{
    /// <summary>
    /// Utilities to resolve a typedef in ROOT (to get from Int_t -> "int").
    /// </summary>
    static class TypeDefTranslator
    {
        public static string ResolveTypedef(string type)
        {
            Init();
            if (_translationTable.ContainsKey(type))
            {
                return _translationTable[type];
            }
            return type;
        }

        /// <summary>
        /// Keep track of all the translations
        /// </summary>
        private static Dictionary<string, string> _translationTable = null;

        /// <summary>
        /// Keep track of the number of typedef's we've seen.
        /// </summary>
        private static int _numgerOfTypedefs = -1;

        /// <summary>
        /// Load up everything from root
        /// </summary>
        private static void Init()
        {
            ///
            /// Check if we've run or the # of items we've found has changed.
            /// 

            var typedefList = ROOTNET.NTROOT.gROOT.ListOfTypes;
            if (_numgerOfTypedefs == typedefList.Entries)
                return;
            _numgerOfTypedefs = typedefList.Entries;

            ///
            /// Ok - get the typedef list cache up and running
            /// 

            if (_translationTable == null)
            {
                _translationTable = new Dictionary<string, string>();
            }

            foreach (var typeDef in typedefList.Cast<ROOTNET.Interface.NTDataType>())
            {
                if (typeDef.Name != typeDef.FullTypeName)
                {
                    _translationTable[typeDef.Name] = typeDef.FullTypeName;
                }
                if (typeDef.Name == "Option_t")
                    _translationTable[typeDef.Name] = "const char";
            }
            _translationTable["Text_t"] = "char";
        }
    }
}
