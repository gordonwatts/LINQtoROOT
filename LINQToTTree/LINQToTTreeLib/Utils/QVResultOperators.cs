using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// A class to help manage all the various query result operators.
    /// </summary>
    [Export(typeof(QVResultOperators))]
    class QVResultOperators
    {
        /// <summary>
        /// The list of valid result operators!
        /// </summary>
        [ImportMany(typeof(IQVResultOperator))]
        IEnumerable<IQVResultOperator> Operators;

        /// <summary>
        /// Keep track of the result operators that we've already looked at for
        /// fast lookup!
        /// </summary>
        Dictionary<Type, IQVResultOperator> _mappedOperators = new Dictionary<Type, IQVResultOperator>();

        public QVResultOperators()
        {
        }

        /// <summary>
        /// Find someone that is able to deal with our type. Cache it if we can!
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal IQVResultOperator FindROProcessor(Type t)
        {
            ///
            /// Simple protections
            /// 

            if (t == null)
                throw new ArgumentNullException("Can't find the result operator for a null type");

            ///
            /// See if we know about it already
            /// 

            if (_mappedOperators.ContainsKey(t))
            {
                return _mappedOperators[t];
            }

            ///
            /// Find it!
            /// 

            IQVResultOperator processor = null;
            if (Operators != null)
            {
                var processors = from o in Operators
                                 where o.CanHandle(t)
                                 select o;
                processor = processors.FirstOrDefault();
            }
            _mappedOperators[t] = processor;
            return processor;

        }
    }
}
