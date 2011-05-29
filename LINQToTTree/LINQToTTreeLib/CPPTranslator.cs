
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib
{
    /// <summary>
    /// This is a very simple class to collect the code that will do the translation
    /// from the Statement and Variable to the actual code that is emitted to C++
    /// files.
    /// </summary>
    [Export(typeof(CPPTranslator))]
    public class CPPTranslator
    {
        /// <summary>
        /// Do the code translation itself.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Dictionary<string, object> TranslateGeneratedCode(IExecutableCode code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            Dictionary<string, object> result = new Dictionary<string, object>();
            result["ResultVariables"] = TranslateVariable(code.ResultValues, code);
            result["ProcessStatements"] = TranslateStatements(code.CodeStatements);
            result["SlaveTerminateStatements"] = TranslateFinalizingVariables(code.ResultValues, code);

            ///
            /// Next, go through everything and extract the include files
            /// 

            AddIncludeFiles(code);

            return result;
        }

        /// <summary>
        /// Look at all the sources we can of include files and make sure they get added
        /// to the code base so they can be "included". :-)
        /// </summary>
        /// <param name="code"></param>
        private void AddIncludeFiles(IExecutableCode code)
        {
            var includesFromResults = from v in code.ResultValues
                                      where v.Type.IsROOTClass()
                                      select v.Type.Name.Substring(1) + ".h";

            var includesFromSavers = from v in code.ResultValues
                                     let saver = _saver.Get(v)
                                     from inc in saver.IncludeFiles(v)
                                     select inc;

            foreach (var incFile in includesFromResults.Concat(includesFromSavers))
            {
                code.AddIncludeFile(incFile);
            }
        }

        /// <summary>
        /// Helper var that we send off to the macro processor. We have to massage to get from our internal rep into
        /// somethign that can be used directly by the C++ code.
        /// </summary>
        public class VarInfo
        {
            private IVariable _iVariable;

            public VarInfo(IVariable iVariable)
            {
                // TODO: Complete member initialization
                this._iVariable = iVariable;
            }
            public string VariableName { get { return _iVariable.VariableName; } }
            public string VariableType { get { return Variables.VarUtils.AsCPPType(_iVariable.Type); } } // C++ type
            public string InitialValue { get { return _iVariable.InitialValue.RawValue; } }
        }

        /// <summary>
        /// Trnaslate the variable type/name into something for our output code. If this variable requires an
        /// include file, then we need to make sure it goes in here!
        /// </summary>
        /// <param name="vars"></param>
        /// <returns></returns>
        private IEnumerable<VarInfo> TranslateVariable(IEnumerable<LinqToTTreeInterfacesLib.IVariable> vars, IExecutableCode gc)
        {
            return from v in vars
                   select new VarInfo(v);
        }

        /// <summary>
        /// Translate the incoming statements into somethiRawng that can be send to the C++ compiler.
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        private IEnumerable<string> TranslateStatements(IStatement statements)
        {
            return statements.CodeItUp();
        }

        /// <summary>
        /// Load up the variable saver so we can write out variable info.
        /// </summary>
#pragma warning disable 0649
        [Import]
        private IVariableSaverManager _saver;
#pragma warning restore 0649

        /// <summary>
        /// Given a variable that has to be transmitted back accross the wire,
        /// generate the statements that are required to make sure that it goes there!
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        private IEnumerable<string> TranslateFinalizingVariables(IEnumerable<IVariable> iVariable, IExecutableCode gc)
        {
            return from v in iVariable
                   let saver = _saver.Get(v)
                   from line in saver.SaveToFile(v)
                   select line;
        }
    }
}
