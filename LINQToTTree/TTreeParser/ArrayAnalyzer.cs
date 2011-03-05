using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public ArrayGroup[] AnalyzeTTree(ROOTClassShell classinfo, ROOTNET.Interface.NTTree tree, int eventsToAnalyze)
        {
            if (tree == null)
                throw new ArgumentNullException("tree must not be null");
            if (eventsToAnalyze < 0)
                throw new ArgumentException("# of events to analyze must be zero ore better.");

            ///
            /// Get the raw numbers out
            /// 

            var rawnumbers = DetermineAllArrayLengths(classinfo, tree, eventsToAnalyze);

            ///
            /// Next, parse the variables into groups
            /// 

            var groups = DetermineGroups(rawnumbers);
            return groups;
        }

        /// <summary>
        /// Does a compare of the groups that we know about in a group list.
        /// </summary>
        class CompareGroups : IEqualityComparer<string[]>
        {
            public bool Equals(string[] strs1, string[] strs2)
            {
                return ConvertedString(strs1) == ConvertedString(strs2);
            }

            public int GetHashCode(string[] strings)
            {
                return ConvertedString(strings).GetHashCode();
            }

            private string ConvertedString(string[] strings)
            {
                var sorted = from s in strings
                             orderby s ascending
                             select s;
                StringBuilder sb = new StringBuilder();
                foreach (var s in sorted)
                {
                    sb.Append(s);
                }
                return sb.ToString();
            }
        }


        /// <summary>
        /// Group the arrays by # of entries. Do it for each event.
        /// </summary>
        /// <param name="rawnumbers"></param>
        /// <returns></returns>
        internal ArrayGroup[] DetermineGroups(Tuple<string, int>[][] rawnumbers)
        {
            ///
            /// Get grouping for a single event
            ///

            var groupsPerEvent = (from g in rawnumbers
                                  select DetermineGroups(g)).ToArray();

            ///
            /// Basically, we need to look at the groups form each one. Anytime any of them
            /// go out of matching, we need to pull them out.
            /// 

            var groups = new HashSet<string[]>(new CompareGroups());
            var usedVars = new HashSet<string>();
            var zeroVars = new HashSet<string>();
            foreach (var evtGroup in groupsPerEvent)
            {
                ///
                /// Zeros we handle funny. If the array had no entry we basically treat it as
                /// giving us no extra info. However, in the end we will want to add these to
                /// the "ungrouped" list (makes it easy for the user), so we need to make sure we
                /// don't loose track of them!
                /// 

                var zVars = from g in evtGroup
                            where g.Item1 == 0
                            from gvar in g.Item2
                            select gvar;
                foreach (var zv in zVars)
                {
                    zeroVars.Add(zv);
                }

                var goodGroupsVarLists = from g in evtGroup
                                         where g.Item1 > 0
                                         select g.Item2;

                ///
                /// Now look at any group that has any info and match it up with what
                /// we have seen in past events. Split groups when needed (which is a bit of a pain).
                /// 

                foreach (var g in goodGroupsVarLists)
                {
                    ///
                    /// If this is a totally new set of variables, just add them. If the
                    /// list isn't in there in total, we have to go through the pain of adding them.
                    /// 

                    if (!g.Where(x => usedVars.Contains(x)).Any())
                    {
                        groups.Add(g);
                        foreach (var varName in g)
                        {
                            usedVars.Add(varName);
                        }
                    }
                    else
                    {
                        if (!groups.Contains(g))
                        {
                            List<string> currentGroup = new List<string>(g);
                            while (currentGroup.Count > 0)
                            {
                                var usedGroupInfo = (from oldGroup in groups
                                                     let commonVars = (oldGroup.Intersect(currentGroup)).ToArray()
                                                     where commonVars.Length > 0
                                                     select new
                                                     {
                                                         OldGroup = oldGroup,
                                                         CommonVars = commonVars
                                                     }).FirstOrDefault();

                                /// If there were no common groups we've gotten down to the bottom.
                                if (usedGroupInfo == null)
                                {
                                    groups.Add(currentGroup.ToArray());
                                    foreach (var oldVarName in currentGroup)
                                    {
                                        usedVars.Add(oldVarName);
                                    }
                                    currentGroup.Clear();
                                }
                                else
                                {
                                    /// Split the group.
                                    /// 1. Remove the common guys from the common group so we don't consider them again
                                    /// 2. Create a new group made up of the common guys
                                    /// 3. Remove the old group
                                    /// 4. Remove the common vars from the old group
                                    /// 5. Put the new slightly smaller group back in.

                                    List<string> oldBigGroup = new List<string>(usedGroupInfo.OldGroup);
                                    groups.Remove(usedGroupInfo.OldGroup);

                                    foreach (var commonVarName in usedGroupInfo.CommonVars)
                                    {
                                        currentGroup.Remove(commonVarName);
                                        oldBigGroup.Remove(commonVarName);
                                    }

                                    groups.Add(usedGroupInfo.CommonVars);
                                    groups.Add(oldBigGroup.ToArray());
                                }

                            }
                        }
                    }

                }
            }

            ///
            /// The last thing to do is to create the final list of groups. There is one special one, the ungrouped list.
            /// This comes from two sources. First any group that is a single item. Second is any variable that was "zero"
            /// all the way through.
            /// 

            var ungrouped = (from g in groups
                             where g.Length == 1
                             select g[0]).ToArray();

            var alwaysZero = (from zv in zeroVars
                              where !usedVars.Contains(zv)
                              select zv).ToArray();

            List<ArrayGroup> aGroups = new List<ArrayGroup>();
            if (ungrouped.Length > 0 || alwaysZero.Length > 0)
                aGroups.Add(new ArrayGroup() { Name = "ungrouped", Variables = ungrouped.Concat(alwaysZero).ToArray() });

            ///
            /// Now make any group that is larger than 1 into a real group
            /// And return the list
            /// 

            var goodGroups = from g in groups
                             where g.Length > 1
                             select g;
            int index = 1;
            foreach (var g in goodGroups)
            {
                aGroups.Add(new ArrayGroup() { Name = "group" + index.ToString(), Variables = g });
                index = index + 1;
            }

            return aGroups.ToArray();
        }

        /// <summary>
        /// Given the string and # of entries, gather everything up that happened group toggether
        /// everyone with a common # of entries.
        /// </summary>
        /// <param name="eventnumbers"></param>
        /// <returns></returns>
        internal IEnumerable<Tuple<int, string[]>> DetermineGroups(Tuple<string, int>[] eventnumbers)
        {
            var gp = from entry in eventnumbers
                     group entry.Item1 by entry.Item2;

            var groupsAsTuples = from g in gp
                                 select Tuple.Create(g.Key, g.ToArray());
            return groupsAsTuples;
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
