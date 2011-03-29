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

        private static Dictionary<string, string> _translationTable = null;

        /// <summary>
        /// Load up everything from root
        /// </summary>
        private static void Init()
        {
            if (_translationTable != null)
            {
                return;
            }
            _translationTable = new Dictionary<string, string>();

            foreach (var typeDef in ROOTNET.NTROOT.gROOT.GetListOfTypes().AsEnumerable().Cast<ROOTNET.Interface.NTDataType>())
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
