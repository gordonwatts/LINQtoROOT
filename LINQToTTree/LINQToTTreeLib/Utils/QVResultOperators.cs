using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
#pragma warning disable 649
        [ImportMany(typeof(IQVScalarResultOperator))]
        IEnumerable<IQVScalarResultOperator> ScalarOperators;

        [ImportMany(typeof(IQVCollectionResultOperator))]
        IEnumerable<IQVCollectionResultOperator> CollectionOperators;
#pragma warning restore 649

        /// <summary>
        /// Keep track of the result operators that we've already looked at for
        /// fast lookup!
        /// </summary>
        Dictionary<Type, IQVScalarResultOperator> _scalarMappedOperators = new Dictionary<Type, IQVScalarResultOperator>();

        /// <summary>
        /// Keep track of the result operators that we've already looked at for
        /// fast lookup!
        /// </summary>
        Dictionary<Type, IQVCollectionResultOperator> _collectionMappedOperators = new Dictionary<Type, IQVCollectionResultOperator>();

        public QVResultOperators()
        {
        }

        /// <summary>
        /// Find someone that is able to deal with our type. Cache it if we can!
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal IQVScalarResultOperator FindScalarROProcessor(Type t)
        {
            ///
            /// Simple protections
            /// 

            if (t == null)
                throw new ArgumentNullException("Can't find the result operator for a null type");

            ///
            /// See if we know about it already
            /// 

            if (_scalarMappedOperators.ContainsKey(t))
            {
                return _scalarMappedOperators[t];
            }

            ///
            /// Find it!
            /// 

            IQVScalarResultOperator processor = null;
            if (ScalarOperators != null)
            {
                var processors = from o in ScalarOperators
                                 where o.CanHandle(t)
                                 select o;
                processor = processors.FirstOrDefault();
            }
            _scalarMappedOperators[t] = processor;
            return processor;

        }

        /// <summary>
        /// Find someone that is able to deal with our type. Cache it if we can!
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal IQVCollectionResultOperator FindCollectionROProcessor(Type t)
        {
            ///
            /// Simple protections
            /// 

            if (t == null)
                throw new ArgumentNullException("Can't find the result operator for a null type");

            ///
            /// See if we know about it already
            /// 

            if (_collectionMappedOperators.ContainsKey(t))
            {
                return _collectionMappedOperators[t];
            }

            ///
            /// Find it!
            /// 

            IQVCollectionResultOperator processor = null;
            if (ScalarOperators != null)
            {
                var processors = from o in CollectionOperators
                                 where o.CanHandle(t)
                                 select o;
                processor = processors.FirstOrDefault();
            }
            _collectionMappedOperators[t] = processor;
            return processor;

        }
    }
}
