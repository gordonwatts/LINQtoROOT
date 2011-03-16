using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TTreeDataModel;

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

            var masterClass = new ROOTClassShell(tree.Name);

            foreach (var c in ExtractClassesFromBranchList(masterClass, tree.GetListOfBranches().AsEnumerable().Cast<ROOTNET.Interface.NTBranch>()))
            {
                yield return c;
            }

            ///
            /// Last thing we need to do is create a proxy for the class.
            /// 

            FileInfo f = MakeProxy(tree);
            masterClass.NtupleProxyPath = f.FullName;

            ///
            /// Return the master class
            /// 

            yield return masterClass;
        }

        /// <summary>
        /// Get/Set where we shoudl generate the proxy files. Normally, it is the current directory.
        /// </summary>
        public DirectoryInfo ProxyGenerationLocation { get; set; }

        /// <summary>
        /// Generate a proxy for the tree. This involves making the proxy and then putting in the proper
        /// text substitutions for later processing by our framework. Some ugly text hacking!
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private FileInfo MakeProxy(ROOTNET.Interface.NTTree tree)
        {
            ///
            /// First create the proxy. We have to create a darn temp macro name for that.
            /// 

            var oldDir = Environment.CurrentDirectory;
            var createItDir = new DirectoryInfo(Environment.CurrentDirectory);
            if (ProxyGenerationLocation != null)
                createItDir = ProxyGenerationLocation;
            FileInfo macroFile = new FileInfo(createItDir + @"\junk_macro_parsettree_" + tree.Name + ".C");
            FileInfo result = null;
            try
            {
                ///
                /// We can only use local directories
                /// 

                if (ProxyGenerationLocation != null)
                    Environment.CurrentDirectory = ProxyGenerationLocation.FullName;

                using (var writer = macroFile.CreateText())
                {
                    writer.WriteLine("void {0} () {{}}", Path.GetFileNameWithoutExtension(macroFile.Name));
                    writer.Close();
                }

                result = new FileInfo("ntuple_" + tree.Name + ".h");
                tree.MakeProxy(Path.GetFileNameWithoutExtension(result.Name), macroFile.Name, null, "nohist");
            }
            finally
            {

                ///
                /// Go back to where we were
                /// 

                Environment.CurrentDirectory = oldDir;
            }

            ///
            /// Next, add the proper things and remove some stuff
            /// 

            ///
            /// Return the location of the cpp file
            /// 

            return result;
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

                if (branch.ListOfBranches.Entries == 0)
                {
                    foreach (var leaf in branch.GetListOfLeaves().AsEnumerable().Cast<ROOTNET.Interface.NTLeaf>())
                    {
                        try
                        {
                            IClassItem toAdd = ExtractSimpleItem(leaf);
                            container.Add(toAdd);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Warning: Unable to transltae ntuple leaf '" + leaf.Name + "': " + e.Message);
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

        /// <summary>
        /// This is a plane root class. Do the extraction!
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTTemplateClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            ///
            /// Currently only setup to deal with some very specific types of vectors!
            /// 

            var parsedMatch = TemplateParser.ParseForTemplates(branch.GetClassName()) as TemplateParser.TemplateInfo;
            if (parsedMatch == null)
            {
                throw new InvalidOperationException("Can't parse a template, but that is the only thing we can do!");
            }

            if (parsedMatch.TemplateName != "vector")
                throw new NotImplementedException("We can't deal with a template other than a vector: " + branch.GetClassName() + ".");

            if (!(parsedMatch.Arguments[0] is TemplateParser.RegularDecl))
            {
                throw new NotImplementedException("We can't deal with nested templates: " + branch.GetClassName() + ".");
            }

            var templateArgClass = (parsedMatch.Arguments[0] as TemplateParser.RegularDecl).Type;

            ///
            /// Now we take a look at the class. This class must be known by ROOT - it just is not reliably possible
            /// to re-build a class from the TTree/TLeaf structure, unforunately (except under very limited circumstances).
            /// To that end, check to see if the class is a stub...
            /// 

            var classInfo = ROOTNET.NTClass.GetClass(templateArgClass);
            if (classInfo.ListOfBases == null)
            {
                /// Hopefully this check isn't too fragile!!
                throw new NotImplementedException("ROOT doesn't know about the class '" + templateArgClass + "' so it can't be parsed");
            }

            ///
            /// Add a new item into the container
            /// 

            container.Add(new ItemVector(TemplateParser.TranslateToCSharp(parsedMatch), branch.Name));

            ///
            /// If this is a ROOT class (like TLorentzVector) then we are done.
            /// 

            return Enumerable.Empty<ROOTClassShell>();

#if false
            /// Code below was are (busted) attempt at using the TBranch/TLeaf structure to parse out a class
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
#endif

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
        /// There is a plain class that needs to be extracted. We usually end up here becuase it is a split class of
        /// some sort. For now, we will assume it is a real root class.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTPlainClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            ///
            /// Double chekc that this is a root class
            /// 

            var cls = ROOTNET.NTClass.GetClass(branch.GetClassName());
            if (cls == null)
            {
                throw new NotImplementedException("The class '" + branch.GetClassName() + "' is not known to ROOT's type systems - and I can't proceed unless it is");
            }
            if (cls.GetListOfAllPublicDataMembers() == null)
            {
                throw new NotImplementedException("The class '" + branch.GetClassName() + "' is known as a class to ROOT, but nothign is specified - this looks like a shim generated by the I/O system rather than a real class - which it needs to be for ntuple conversion to continue. Please make sure the correct libraries are loaded!");
            }
            if (cls.Name == "string")
            {
                throw new NotImplementedException("The class 'string' is not translated yet!");
            }

            container.Add(new ItemROOTClass(branch.Name, branch.GetClassName()));

            return Enumerable.Empty<ROOTClassShell>();
        }

        /// <summary>
        /// A simple item, or an item, at least, that root knows how to deal with and for which
        /// we should have a complete, portable, guy ready for.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private IClassItem ExtractSimpleItem(ROOTNET.Interface.NTLeaf leaf)
        {
            string className = TypeDefTranslator.ResolveTypedef(leaf.TypeName);

            ///
            /// First, see if this is a template of some sort.
            /// 

            var result = TemplateParser.ParseForTemplates(className);
            if (result is TemplateParser.TemplateInfo)
            {
                return ExtractTemplateItem(leaf, result as TemplateParser.TemplateInfo);
            }

            ///
            /// Ok - so it is a single "object" or similar. So we need to look at it and figure
            /// out how to deal with it. It could be a root object or just an "int"
            ///

            var ln = NormalizeLeafName(leaf.Name);

            IClassItem toAdd = null;
            if (IsROOTClass(className))
            {
                toAdd = new ItemROOTClass(ln, className);
            }
            else
            {
                toAdd = new ItemSimpleType(ln, className.SimpleCPPTypeToCSharpType());
            }

            if (toAdd == null || toAdd.ItemType == null)
            {
                throw new InvalidOperationException("Unknown type - cant' translate '" + className + "'.");
            }
            return toAdd;
        }

        /// <summary>
        /// It is possible to use C# reserved keywords as leaf names. We have to alter them.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string NormalizeLeafName(string p)
        {
            if (p == "event")
                return "event_";
            return p;
        }

        /// <summary>
        /// We are going to deal with a template of some sort for a simple item. At the moment we can deal only
        /// with simple STL's...
        /// </summary>
        /// <param name="leaf"></param>
        /// <param name="templateInfo"></param>
        /// <returns></returns>
        private IClassItem ExtractTemplateItem(ROOTNET.Interface.NTLeaf leaf, TemplateParser.TemplateInfo templateInfo)
        {
            if (templateInfo.TemplateName == "vector")
            {
                return new ItemVector(TemplateParser.TranslateToCSharp(templateInfo), leaf.Name);
            }
            else
            {
                throw new NotImplementedException("We rae not able to handle template classes other than vector: '" + templateInfo.TemplateName + "'");
            }
        }

        /// <summary>
        /// Return true if this is a vector we are dealing with.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private bool IsSimpleVector(string className)
        {
            return className.StartsWith("vector<");
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
