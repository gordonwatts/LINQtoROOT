
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.Variables;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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

            ///
            /// Get the variables that we are going to be shipping back and forth.
            /// 

            Dictionary<string, object> result = new Dictionary<string, object>();
            result["ResultVariables"] = TranslateVariable(code.ResultValues, code);

            ///
            /// The actual code. This is just a sequence of loops. It would be, under normal circumstances, just that. However,
            /// there is a limitation in the code generator that means you can't have more than 250 loops in one function. If we aren't
            /// combining loops it is easy to have more than 250 plots. So we split this up into functions of about 100 outter loops
            /// per function. We then call these functions from the main loop. Fortunately, there are no local variables. :-)
            /// We cache the codeing blocks here b/c we have to access them several times.
            /// 

            const int queriesPerFunction = 100;
            var queryBlocks = code.QueryCode().ToArray();
            int numberOfBlocks = 1 + queryBlocks.Length / queriesPerFunction;
            result["NumberOfQueryFunctions"] = numberOfBlocks;
            result["QueryFunctionBlocks"] = TranslateQueryBlocks(queryBlocks, queriesPerFunction, numberOfBlocks);
            result["SlaveTerminateStatements"] = TranslateFinalizingVariables(code.ResultValues, code);

            // Functions have to be written out too.
            result["QueryMemberFunctions"] = code.Functions.SelectMany(f => f.CodeItUp());
            result["QueryCacheBools"] = code.Functions.Select(f => f.CacheVariableGood.RawValue);

            ///
            /// Next, go through everything and extract the include files
            /// 

            AddIncludeFiles(code);

            return result;
        }

        /// <summary>
        /// Chunk the quries up into blocks and return a function block (2D array).
        /// </summary>
        /// <param name="queryBlocks"></param>
        /// <param name="queriesPerFunction"></param>
        /// <returns></returns>
        private IEnumerable<IEnumerable<string>> TranslateQueryBlocks(IStatementCompound[] queryBlocks, int queriesPerFunction, int numberBlocks)
        {
            for (int i = 0; i < numberBlocks; i++)
            {
                var queires = queryBlocks.Skip(queriesPerFunction * i).Take(queriesPerFunction);
                yield return TranslateOneQueryBlockSet(queires);
            }
        }

        /// <summary>
        /// Return a set of strings for a function block. This is just flattening them all out.
        /// </summary>
        /// <param name="queires"></param>
        /// <returns></returns>
        private IEnumerable<string> TranslateOneQueryBlockSet(IEnumerable<IStatementCompound> queires)
        {
            return queires.SelectMany(q => q.CodeItUp());
        }

        /// <summary>
        /// Look at all the sources we can of include files and make sure they get added
        /// to the code base so they can be "included". :-)
        /// </summary>
        /// <param name="code"></param>
        private void AddIncludeFiles(IExecutableCode code)
        {
            var includesFromResults = from v in code.ResultValues
                                      where v != null
                                      where v.Type.IsROOTClass()
                                      select v.Type.Name.Substring(1) + ".h";

            var includesFromSavers = from v in code.ResultValues
                                     where v != null
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
            private IDeclaredParameter _iVariable;

            public VarInfo(IDeclaredParameter iVariable)
            {
                // TODO: Complete member initialization
                this._iVariable = iVariable;
            }
            public string VariableName { get { return _iVariable.ParameterName; } }
            public string VariableType { get { return Variables.VarUtils.AsCPPType(_iVariable.Type); } } // C++ type
            public string InitialValue { get { return _iVariable.InitialValue.RawValue; } }
        }

        /// <summary>
        /// Trnaslate the variable type/name into something for our output code. If this variable requires an
        /// include file, then we need to make sure it goes in here!
        /// </summary>
        /// <param name="vars"></param>
        /// <returns></returns>
        private IEnumerable<VarInfo> TranslateVariable(IEnumerable<IDeclaredParameter> vars, IExecutableCode gc)
        {
            return from v in vars
                   select new VarInfo(v);
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
        private IEnumerable<string> TranslateFinalizingVariables(IEnumerable<IDeclaredParameter> iVariable, IExecutableCode gc)
        {
            return from v in iVariable
                   let saver = _saver.Get(v)
                   from line in saver.SaveToFile(v)
                   select line;
        }
    }
}
