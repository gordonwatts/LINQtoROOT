using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Provider;

namespace PSPROOFUtils
{
    /// <summary>
    /// Coordinate reading the files, one-by-one, back from the PROOF server.
    /// </summary>
    class DSContentReader : IContentReader
    {
        private ProofDataSetItem _item;

        /// <summary>
        /// Create a content reader that will spin back the files in a dataset.
        /// </summary>
        /// <param name="item"></param>
        public DSContentReader(ProofDataSetItem item)
        {
            this._item = item;
            ResetReader(0, System.IO.SeekOrigin.Begin);
        }

        /// <summary>
        /// The reader that moves through the items.
        /// </summary>
        IEnumerator<ROOTNET.Interface.NTUrl> _reader;

        /// <summary>
        /// Helper function to make reading the urls nice and easy to code up...
        /// Let the compiler do all the state machine work! :-)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ROOTNET.Interface.NTUrl> GetUrlReader()
        {
            foreach (var item in _item.GetFileInfoEnumerator())
            {
                item.ResetUrl();
                var url = item.NextUrl();
                while (url != null)
                {
                    yield return url;
                    url = item.NextUrl();
                }
            }
        }

        /// <summary>
        /// Reset our reader, and seek to the given offset from the start.
        /// </summary>
        /// <param name="p"></param>
        private void ResetReader(long offset, SeekOrigin origin)
        {
            if (origin != SeekOrigin.Begin)
                throw new ArgumentException("Can't seek anywhere but from the start of the list of datasetes in a proof dataset");

            _reader = GetUrlReader().GetEnumerator();
            for (int i = 0; i < offset; i++)
            {
                _reader.MoveNext();
            }
        }

        /// <summary>
        /// Finished reading. Not much to do here, actually.
        /// </summary>
        public void Close()
        {
            _reader = null;
            _item = null;
        }

        /// <summary>
        /// Read this many urls from the stream.
        /// </summary>
        /// <param name="readCount"></param>
        /// <returns></returns>
        public System.Collections.IList Read(long readCount)
        {
            var r = new List<ROOTNET.Interface.NTUrl>();
            for (int i = 0; i < readCount; i++)
            {
                if (_reader.MoveNext())
                {
                    r.Add(_reader.Current);
                }
                else
                {
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// Reset where we are reading from.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        public void Seek(long offset, System.IO.SeekOrigin origin)
        {
            ResetReader(offset, origin);
        }

        /// <summary>
        /// We don't hold onto anything worth disposing, so...
        /// </summary>
        public void Dispose()
        {
        }
    }
}
