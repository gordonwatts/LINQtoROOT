using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables.Savers
{
    [Export(typeof(IVariableSaverManager))]
    public class VariableSaverManager : IVariableSaverManager
    {
        /// <summary>
        /// MEF list of guys that can help us save and load up variables.
        /// </summary>
#pragma warning disable 0649
        [ImportMany]
        IEnumerable<IVariableSaver> _varSaverList;
#pragma warning restore 0649

        /// <summary>
        /// Find a saver for a particular variable.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IVariableSaver Get(IDeclaredParameter iVariable)
        {
            if (iVariable == null)
                throw new ArgumentNullException("iVariable can't be null");
            if (_varSaverList == null)
                throw new InvalidOperationException("The list of variable saver objects is null - was MEF correctly initalized!?");

            var saver = (from s in _varSaverList
                         where s.CanHandle(iVariable)
                         select s).FirstOrDefault();
            if (saver == null)
                throw new InvalidOperationException("Unable to find an IVariableSaver for " + iVariable.Type.Name
                    + ". This means that you are trying to transmit an object back from PROOF or a TTree::Process that I don't know how to flatten into a binary stream and combine multiple results from (PROOF)!");
            return saver;
        }

    }
}
