using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables.Savers
{
    [Export(typeof(VariableSaverManager))]
    public class VariableSaverManager
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
        public IVariableSaver Get(IVariable iVariable)
        {
            if (_varSaverList == null)
                throw new InvalidOperationException("The list of variable saver objects is null - was MEF correctly initalized!?");

            var saver = (from s in _varSaverList
                         where s.CanHandle(iVariable)
                         select s).FirstOrDefault();
            if (saver == null)
                throw new InvalidOperationException("Unable to find an IVariableSaver for " + iVariable.GetType().Name);
            return saver;
        }

    }
}
