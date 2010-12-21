using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Keeps track of some scoping stuff - like names of parameters, etc.
    /// </summary>
    public class CodeContext : ICodeContext
    {
        Dictionary<string, IValue> _parameterReplacement = new Dictionary<string, IValue>();
        /// <summary>
        /// Add a parameter replacement to the list.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementName"></param>
        public void Add(string varName, IValue replacementName)
        {
            if (varName == null || replacementName == null)
                throw new ArgumentNullException("Can't setup an Add that is null!");

            if (varName.Length == 0)
                throw new ArgumentException("var and replacement must be real strings");

            _parameterReplacement[varName] = replacementName;
        }

        /// <summary>
        /// Get a replacement substitution - given the name and expected type.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IValue GetReplacement(string varname, Type type)
        {
            if (varname == null)
                throw new ArgumentNullException("Can't lookup a null var name!");

            if (type == null)
                throw new ArgumentNullException("Can't lookup a var name with a null type!");

            if (varname.Length == 0)
                throw new ArgumentException("Variables must be non-zero length!");

            if (!_parameterReplacement.ContainsKey(varname))
                return new ValSimple(varname, type);
            return _parameterReplacement[varname];
        }
    }
}
