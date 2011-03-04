using System;
using System.Linq;
using TTreeDataModel;

namespace TTreeParser
{
    /// <summary>
    /// Analyze the linked arrays in a TTree.
    /// </summary>
    public class ArrayAnalyzer
    {
        /// <summary>
        /// Analyze a tree for a pattern.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="eventsToAnalyze"># of events to analyze. Zero means the whole tree</param>
        public void AnalyzeTTree(ROOTClassShell classinfo, ROOTNET.Interface.NTTree tree, int eventsToAnalyze)
        {
            if (tree == null)
                throw new ArgumentNullException("tree must not be null");
            if (eventsToAnalyze < 0)
                throw new ArgumentException("# of events to analyze must be zero ore better.");

            ///
            /// Get the raw numbers out
            /// 

            var rawnumbers = DetermineAllArrayLengths(classinfo, tree, eventsToAnalyze);
        }

        /// <summary>
        /// Run a TTree analysis and get the array sizes for the ntuple. Returns an entry for each
        /// "event" that is analyzed, with the array names in each one.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="eventsToAnalyze"></param>
        internal Tuple<string, int>[][] DetermineAllArrayLengths(ROOTClassShell classinfo, ROOTNET.Interface.NTTree tree, long eventsToAnalyze)
        {
            if (tree.Entries == 0)
                throw new ArgumentException("Tree has no entries and can't be analyzed!");
            if (classinfo == null)
                throw new ArgumentNullException("the class info cannot be null!");

            ///
            /// Create the counters
            /// 

            var arrays = from item in classinfo.Items
                         where item.ItemType.Contains("[]")
                         select item.Name;

            var counterlist = (from item in arrays
                               select new ROOTNET.NTTreeFormula(item, item + "@.size()", tree)).ToArray();

            ///
            /// Now run through everything
            /// 

            long entriesToTry = eventsToAnalyze;
            if (entriesToTry == 0)
                entriesToTry = tree.Entries;
            if (entriesToTry > tree.Entries)
                entriesToTry = tree.Entries;

            var eventNtupleData = (from entry in Enumerable.Range(0, (int)entriesToTry)
                                   select ComputeCounters(counterlist, tree, entry)).ToArray();

            foreach (var item in eventNtupleData)
            {
                Console.WriteLine("Entry");
                foreach (var line in item)
                {
                    Console.WriteLine("  {0}: {1}", line.Item1, line.Item2);
                }
            }

            ///
            /// Make sure to clean up
            /// 

            foreach (var c in counterlist)
            {
                c.Delete();
            }

            return eventNtupleData;
        }

        /// <summary>
        /// Loop thorugh a selection of counters and get the sizes
        /// </summary>
        /// <param name="counterlist"></param>
        /// <param name="tree"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        private Tuple<string, int>[] ComputeCounters(ROOTNET.NTTreeFormula[] counterlist, ROOTNET.Interface.NTTree tree, int entry)
        {
            tree.LoadTree(entry);
            var pairs = from c in counterlist
                        select Tuple.Create(c.Name, (int)c.EvalInstance());
            return pairs.ToArray();
        }
    }
}
