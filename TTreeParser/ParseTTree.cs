using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTreeDataModel;
using System.Text.RegularExpressions;

namespace TTreeParser
{
    /// <summary>
    /// Given a TTree interface, inspect it, and return a complete set of classes that have to be generated
    /// to mock it on the .NET side.
    /// </summary>
    public class ParseTTree
    {
        /// <summary>
        /// Look at everything we have in this tree and see if we can't generate a class correctly.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public IEnumerable<ROOTClassShell> GenerateClasses(ROOTNET.Interface.NTTree tree)
        {
            ///
            /// The outlying class is going to be called by the name of the tree.
            /// 

            var masterClass = new ROOTClassShell(tree.PRName);

            foreach (var c in ExtractClassesFromBranchList(masterClass, tree.GetListOfBranches().AsEnumerable().Cast<ROOTNET.Interface.NTBranch>()))
            {
                yield return c;
            }
            yield return masterClass;

        }

        /// <summary>
        /// Populate a class with the stuff we find in the branches we are looking at.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="branches"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractClassesFromBranchList(ROOTClassShell container, IEnumerable<ROOTNET.Interface.NTBranch> branches)
        {
            ///
            /// Now, loop through each branch and see if we can't convert everything!
            /// 

            foreach (var branch in branches)
            {
                ///
                /// What kind of branch is this? Is it a leaf branch or is it a "class" branch. That is, does it have
                /// sub leaves on it? The key question to ask is how many sub-branches are there here?
                /// 

                if (branch.PRListOfBranches.PREntries == 0)
                {
                    foreach (var leaf in branch.GetListOfLeaves().AsEnumerable().Cast<ROOTNET.Interface.NTLeaf>())
                    {
                        if (leaf is ROOTNET.Interface.NTLeafElement)
                        {
                            ExtractMultiLeafItem(container, leaf as ROOTNET.Interface.NTLeafElement);
                        }
                        else
                        {
                            IClassItem toAdd = ExtractSimpleItem(leaf);
                            container.Add(toAdd);
                        }
                    }
                }
                else
                {
                    var rc = ExtractROOTClass(container, branch);
                    ROOTClassShell lastOne = null;
                    foreach (var rootClass in rc)
                    {
                        yield return rootClass;
                        lastOne = rootClass;
                    }
                }
            }
        }

        /// <summary>
        /// This is one of those split classes - split only in leaves. So it has sub-leaves. We have to dip down into
        /// that and parse those. :(
        /// </summary>
        /// <param name="container"></param>
        /// <param name="leaf"></param>
        private void ExtractMultiLeafItem(ROOTClassShell container, ROOTNET.Interface.NTLeafElement leaf)
        {
        }

        /// <summary>
        /// Extract info for a sub-class! The last one is the top level class we are currently parsing!
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            ///
            /// First, figure out what kind of class this is. For example, might it be a stl vector? If so,
            /// then we need to parse that up!
            /// 

            if (branch.GetClassName().Contains("<"))
            {
                return ExtractROOTTemplateClass(container, branch);
            }
            else
            {
                return ExtractROOTPlainClass(container, branch);
            }
        }

        private Regex templateParameterFinder = new Regex("(?<tclass>\\w+)\\<(?<types>.*)\\>$");
        /// <summary>
        /// This is a plane root class. Do the extraction!
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTTemplateClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            var m = templateParameterFinder.Match(branch.GetClassName());
            if (!m.Success)
                throw new ArgumentException("Couldn't parse '" + branch.GetClassName() + "' for template objects!");
            var templateArgClass = m.Groups["types"].Value.Trim();
            var templateClass = m.Groups["tclass"].Value.Trim();

            if (templateArgClass.Contains("<"))
            {
                throw new NotImplementedException("Can't do nested template classes yet");
            }

            ///
            /// Now that we have parsed out the name of the template class and the sub-class we are going to be calling this
            /// mess, we can go after the actual guys. This object could be sub-classed. However, we can't really tell b/c it
            /// may well be that it hasn't been correctly built - it could have been only built using some class that ROOT
            /// doesn't fully know about. So, we look at all the members, and find all the class names. And grab the last one.
            /// This probably will fail due to multiple-inherritance, but we'll give it a try.
            /// 

            var result = new ROOTClassShell(templateArgClass);
            var subbranches = branch.GetListOfBranches().AsEnumerable().Cast<ROOTNET.Interface.NTBranch>();

            result.SubClassName = (from s in subbranches
                                   where s.GetClassName() != templateArgClass
                                   select s.GetClassName()).LastOrDefault();

            var newClassBranches = from s in subbranches
                                   where s.GetClassName() == templateArgClass
                                   select s;

            foreach (var c in ExtractClassesFromBranchList(result, newClassBranches))
            {
                yield return c;
            }

            yield return result;

            ///
            /// Put a reference into the container that we are adding items to so that it appears! :-)
            /// 

            if (templateClass == "vector")
            {
                container.Add(new ItemVector(result));
            }
            else
            {
                throw new NotImplementedException("Can't deal with template class of type '" + templateArgClass + "'");
            }

#if false
            ///
            /// Look at the class layout... This is totally useless! :-)
            /// 

            var tc = ROOTNET.NTClass.GetClass(templateArgClass);
            var members = tc.PRListOfAllPublicDataMembers;
            var memberNames = (from mem in members.AsEnumerable().Cast<ROOTNET.Interface.NTDataMember>()
                               select new Tuple<string, string>(mem.PRName, mem.PRFullTypeName)).ToArray();
            foreach (var item in memberNames)
            {
                var dude = item;
            }
#endif
        }

        /// <summary>
        /// There is a template in here... so we will have to do something "special" for it.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTPlainClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A simple item, or an item, at least, that root knows how to deal with and for which
        /// we should have a complete, portable, guy ready for.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private IClassItem ExtractSimpleItem(ROOTNET.Interface.NTLeaf leaf)
        {
            string className = TypeDefTranslator.ResolveTypedef(leaf.PRTypeName);
            var c = leaf.GetLeafCount();
            IClassItem toAdd = null;

            if (IsROOTClass(className))
            {
                toAdd = new ItemROOTClass(leaf.PRName, className);
            }
            else
            {
                toAdd = new ItemSimpleType(leaf.PRName, className);
            }

            if (toAdd == null)
            {
                throw new InvalidOperationException("Unknown type - cant' translate '" + className + "'.");
            }
            return toAdd;
        }

        /// <summary>
        /// Figure out if this is a root class or not!
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private bool IsROOTClass(string className)
        {
            var c = ROOTNET.NTClass.GetClass(className);
            return c != null;
        }
    }
}
