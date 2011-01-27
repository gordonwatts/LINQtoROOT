using System;
using System.Collections.Generic;
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
        public IVariableScopeHolder Add(string varName, IValue replacementName)
        {
            if (varName == null || replacementName == null)
                throw new ArgumentNullException("Can't setup an Add that is null!");

            if (varName.Length == 0)
                throw new ArgumentException("var and replacement must be real strings");

            ///
            /// See if there was somethign there already that we are going to replace
            /// 

            IVariableScopeHolder popper = null;
            if (_parameterReplacement.ContainsKey(varName))
            {
                popper = new CCReplacement(this, varName, _parameterReplacement[varName]);
            }
            else
            {
                popper = new CCReplacementNull();
            }

            ///
            /// Ok, now do the replacement
            /// 

            _parameterReplacement[varName] = replacementName;

            return popper;
        }

        /// <summary>
        /// We have no context to keep track of.
        /// </summary>
        private class CCReplacementNull : IVariableScopeHolder
        {
            public void Pop()
            {
            }
        }

        /// <summary>
        /// Keep track of a context
        /// </summary>
        private class CCReplacement : IVariableScopeHolder
        {
            private CodeContext _context;
            private string _varName;
            private IValue _oldVal;

            public CCReplacement(CodeContext codeContext, string varName, IValue iValue)
            {
                _context = codeContext;
                _varName = varName;
                _oldVal = iValue;
            }

            /// <summary>
            /// Go back to what we had!
            /// </summary>
            public void Pop()
            {
                _context.Add(_varName, _oldVal);
            }
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
