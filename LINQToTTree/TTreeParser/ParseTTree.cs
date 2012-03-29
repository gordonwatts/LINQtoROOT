using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
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
        /// Init the proxy generator. Default output is the current directory.
        /// </summary>
        public ParseTTree()
        {
            ProxyGenerationLocation = new DirectoryInfo(".");
        }

        /// <summary>
        /// Look at everything we have in this tree and see if we can't generate a class correctly.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public IEnumerable<ROOTClassShell> GenerateClasses(ROOTNET.Interface.NTTree tree)
        {
            if (tree == null)
                throw new ArgumentNullException("tree must not be null");

            ///
            /// The outlying class is going to be called by the name of the tree.
            /// 

            var masterClass = new ROOTClassShell(tree.Name);

            var subClassesByName = from sc in ExtractClassesFromBranchList(masterClass, tree.ListOfBranches.Cast<ROOTNET.Interface.NTBranch>())
                                   group sc by sc.Name;
            var subClasses = from scg in subClassesByName
                             where ForceClassesSame(scg)
                             select scg.First();
            foreach (var sc in subClasses)
            {
                yield return sc;
            }


            ///
            /// Last thing we need to do is create a proxy for the class.
            /// 

            FileInfo f = MakeProxy(tree);
            masterClass.NtupleProxyPath = f.FullName;

            ///
            /// Work around a ROOT bug that doesn't allow for unloading of classes
            /// in the STL after a query is run. Actually, it will unload them, but somehow keeps
            /// references in memory.
            /// 

            masterClass.ClassesToGenerate = ExtractSTLDictionaryReferences(f);

            ///
            /// Analyze the TTree arrays.
            /// 

            var at = new ArrayAnalyzer();
            var groupInfo = at.AnalyzeTTree(masterClass, tree, 100);

            ///
            /// Any item that isn't an array should be added to the list and
            /// put in the "ungrouped" array.
            /// 

            AddNonArrays(masterClass, groupInfo, "ungrouped");

            ///
            /// Write out the user info
            /// 

            masterClass.UserInfoPath = WriteUserInfo(groupInfo, tree.SanitizedName()).FullName;

            ///
            /// Return the master class
            /// 

            yield return masterClass;
        }

        /// <summary>
        /// Make sure that this list of classes is totally in common.
        /// </summary>
        /// <param name="scg"></param>
        /// <returns></returns>
        private bool ForceClassesSame(IEnumerable<ROOTClassShell> scg)
        {
            var f = scg.First();
            foreach (var o in scg.Skip(1))
            {
                if (f.IsTClonesArrayClass != o.IsTClonesArrayClass)
                    throw new InvalidDataException(string.Format("IsTClonesArrayClass not the same in duplicate {0} classes.", f.Name));
                if (f.IsTopLevelClass != o.IsTopLevelClass)
                    throw new InvalidDataException(string.Format("IsTopLevelClass is not the same in duplicate {0} classes.", f.Name));
                if (f.Items.Count != o.Items.Count)
                    throw new InvalidDataException(string.Format("Number of items is not the same in duplicate {0} classes.", f.Name));
                foreach (var item in f.Items.Zip(o.Items, (n1, n2) => Tuple.Create(n1, n2)))
                {
                    if (item.Item1.Name != item.Item2.Name)
                        throw new InvalidDataException(string.Format("Duplicate classes {0} are defined with different item names", f.Name));
                    if (item.Item1.ItemType != item.Item2.ItemType)
                        throw new InvalidDataException(string.Format("Duplicate classes {0} are defined with different item types", f.Name));

                }
            }

            return true;
        }

        /// <summary>
        /// When an ntuple is parsed it contains many dictionary classes that have to be
        /// generated. It turns out that if vector<int> and similar are generated then you end
        /// up with the the query not being allowed to unload safely. To get around this we
        /// remove dictionary generation for stl classes here and place them as dictoinaries that
        /// are generated ahead of time.
        /// 
        /// We do a bunch of the work in memory as the files should be no more than a few
        /// kilobytes.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ClassForDictionary[] ExtractSTLDictionaryReferences(FileInfo f)
        {
            var fixedFile = new StringBuilder();
            List<ClassForDictionary> genRequests = new List<ClassForDictionary>();

            var knownSTLHeaders = new string[] { "map", "vector", "list" };

            using (var reader = f.OpenText())
            {
                /// #pragma link6 C++ class vector<float>;
                var classExtractions = new Regex("(link|link6) C\\+\\+ class (?<cls>[^;]+);");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    bool writeLine = true;
                    if (line.StartsWith("#pragma link"))
                    {
                        var m = classExtractions.Match(line);
                        if (m.Success)
                        {
                            var className = m.Groups["cls"].Value;
                            writeLine = !knownSTLHeaders.Any(stlname => className.StartsWith(stlname));
                            if (!writeLine)
                            {
                                var headers = new StringBuilder();
                                foreach (var stl in knownSTLHeaders)
                                {
                                    if (className.Contains(stl))
                                    {
                                        if (headers.Length > 0)
                                            headers.Append(",");
                                        headers.Append(stl);
                                    }
                                }
                                genRequests.Add(new ClassForDictionary() { classSpec = className, includeFiles = headers.ToString() });
                            }
                        }
                    }

                    if (writeLine)
                        fixedFile.AppendLine(line);
                }
            }

            ///
            /// Now, replace the proxy file
            /// 

            using (var writer = f.CreateText())
            {
                writer.Write(fixedFile.ToString());
                writer.Close();
            }

            ///
            /// And return everything needed
            /// 

            return genRequests.ToArray();
        }

        /// <summary>
        /// Find all non-arrays and add them to our ungrouped group. Since this
        /// grouping is what drives the class generation, this is very much something
        /// we have to do! :-)
        /// </summary>
        /// <param name="masterClass"></param>
        /// <param name="groupToAddTo">What is the name of the group we should add them to?</param>
        private void AddNonArrays(ROOTClassShell masterClass, List<ArrayGroup> groups, string groupToAddTo)
        {
            ///
            /// First, get all non-arrays. Arrays are all declared with [] right now,
            /// so we need only look for that.
            /// 

            var nonarrays = (from item in masterClass.Items
                             where !item.ItemType.Contains("[]")
                             select item.Name).ToArray();
            if (nonarrays.Length == 0)
                return;

            ///
            /// Turn the names into variables
            /// 

            var varInfo = nonarrays.ToVariableInfo();

            ///
            /// See if the group we should add them to exists.
            ///

            var grp = (from g in groups
                       where g.Name == groupToAddTo
                       select g).FirstOrDefault();
            if (grp == null)
            {
                grp = new ArrayGroup() { Name = groupToAddTo, Variables = varInfo.ToArray() };
                groups.Add(grp);
            }
            else
            {
                grp.Variables = (grp.Variables.Concat(varInfo)).ToArray();
            }
        }

        /// <summary>
        /// Write out the user info file. This file contains things like group names, etc., that the user
        /// might want to change. Do not destroy the old one. Rather, move it out of the way!
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        private FileInfo WriteUserInfo(List<ArrayGroup> groupInfo, string treeName)
        {
            ///
            /// First job is to figure out where we will put the file. If there is one there already, then
            /// we chose a new filename. :-)
            /// 

            FileInfo userInfoFile = null;
            int index = 0;
            do
            {
                userInfoFile = new FileInfo(ProxyGenerationLocation.FullName + "\\" + treeName + "Config-" + index.ToString("000") + ".ntup");
                index = index + 1;
            } while (userInfoFile.Exists);

            ///
            /// Write out the info.
            /// 

            using (var writer = userInfoFile.CreateText())
            {
                XmlSerializer output = new XmlSerializer(typeof(TTreeUserInfo));
                var userInfo = new TTreeUserInfo() { Groups = groupInfo.ToArray() };
                output.Serialize(writer, userInfo);
                writer.Close();
            }

            return userInfoFile;

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
            FileInfo macroFile = new FileInfo(createItDir + @"\junk_macro_parsettree_" + tree.SanitizedName() + ".C");
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

                result = new FileInfo("ntuple_" + tree.SanitizedName() + ".h");
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
                    foreach (var leaf in branch.GetListOfLeaves().Cast<ROOTNET.Interface.NTLeaf>())
                    {
                        try
                        {
                            IClassItem toAdd = ExtractSimpleItem(leaf);
                            if (toAdd != null)
                                container.Add(toAdd);
                        }
                        catch (Exception e)
                        {
                            SimpleLogging.Log("Warning: Unable to transltae ntuple leaf '" + leaf.Name + "': " + e.Message);
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
        }

        /// <summary>
        /// There is a plain class that needs to be extracted. We usually end up here becuase it is a split class of
        /// some sort. For now, we will assume it is a real root class.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTPlainClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            //
            // Double chekc that this is a root class. Since we are looking at the Tree, ROOT's system would certianly have declared it
            // internally. If we can't find the cls info, then we are in trouble.
            // 

            var cls = ROOTNET.NTClass.GetClass(branch.GetClassName());
            if (cls == null)
            {
                throw new NotImplementedException("The class '" + branch.GetClassName() + "' is not known to ROOT's type systems - and I can't proceed unless it is");
            }

            //
            // Now that we have the cls info, we have to determine what sort of class is this. Is it a known class (TLorentzVector, for example), or
            // is it something that is declared only in the ROOT file and we don't have any info on how to use it - but we can parse it. This guy will
            // throw if it can't deal with what we've given it.
            //

            var newClasses = ParseTTreeReferencedClass(container, branch, cls);

            return newClasses;
        }

        /// <summary>
        /// We have a ROOT class referenced in a TTree (either directly in a leaf, or perhaps burried in something else like a vector<>). Figure out, and 
        /// act appropriately if:
        /// 1) This is a known root class - like TLorentzVector (do nothing).
        /// 2) A class defined only in a ROOT file (define it).
        /// 3) Something else
        /// </summary>
        /// <param name="container"></param>
        /// <param name="branch"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ParseTTreeReferencedClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch, ROOTNET.Interface.NTClass cls)
        {
            //
            // Strings are not well handled at this point.
            //

            if (cls.Name == "string")
            {
                throw new NotImplementedException("The class 'string' is not translated yet!");
            }

            //
            // If it has some public data members, then it is a real ROOT class, and whatever was done to define it and make it known
            // to ROOT here we assume will also be done when the full blown LINQ interface is run (i.e. some loaded C++ files).
            //

            if (
                (cls.ListOfAllPublicDataMembers != null && cls.ListOfAllPublicDataMembers.Entries > 0)
                || (cls.ListOfAllPublicMethods != null && cls.ListOfAllPublicMethods.Entries > 0)
                )
            {
                container.Add(new ItemROOTClass(branch.Name, branch.GetClassName()));
                return Enumerable.Empty<ROOTClassShell>();
            }

            //
            // If we are here, then we are dealing with a locally defined class. One that ROOT made up on the fly. In short, not one
            // that we are going to have a translation for. So, we have to build the meta data for it.
            //

            return BuildMetadataForTTreeClass(container, branch, cls);
        }

        /// <summary>
        /// Keep track of the class names we've used already so we keep everything unique.
        /// </summary>
        private IDictionary<string, int> _classNameCounter = new Dictionary<string, int>();

        /// <summary>
        /// A class has been defined only in the local tree. We need to parse through it and extract
        /// enough into to build a class on our own so that it can be correctly referenced in LINQ.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="branch"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> BuildMetadataForTTreeClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch, ROOTNET.Interface.NTClass cls)
        {
            //
            // Get a unique classname. Attempt to do "the right thing". In particular, if this is a clones array of some
            // struct, it is not a "named" class, so use the name of the clones array instead.
            //

            string className = cls.Name;
            bool mightBeClonesArray = false;
            if (branch is ROOTNET.Interface.NTBranchElement)
            {
                var cn = (branch as ROOTNET.Interface.NTBranchElement).ClonesName;
                if (!string.IsNullOrWhiteSpace(cn))
                {
                    className = cn;
                    mightBeClonesArray = true;
                }
            }

            if (_classNameCounter.ContainsKey(className))
            {
                _classNameCounter[className] += 1;
                className = string.Format("{0}_{1}", className, _classNameCounter[className]);
            }
            else
            {
                _classNameCounter[className] = 0;
            }

            //
            // We will define the class, and it will be exactly what is given to use by the
            // tree.
            //

            container.Add(new ItemSimpleType(branch.Name, className));

            //
            // We are going to build our own class type here.
            //

            var treeClass = new ROOTClassShell(className) { IsTopLevelClass = false };

            //
            // Now, loop over the branches and add them in, returning any classes we had to generate.
            //

            foreach (var c in ExtractClassesFromBranchList(treeClass, branch.ListOfBranches.Cast<ROOTNET.Interface.NTBranch>()))
            {
                yield return c;
            }

            //
            // The arrays in a tclones arrays are funny. The proxy generated parses them as seperate arrays, but their length is
            // basically the size of the tclonesarray. So we have to use that as the length. This is implied be cause we've marked
            // this class as a tclones array class already (above - the IsTTreeSubClass). So for the index we mark it as an index,
            // but we just marked the bound as "implied" - this will be picked up by the code when it is generated later on.
            //

            if (mightBeClonesArray)
            {
                var cBoundName = string.Format("{0}_", branch.Name);
                var cstyleArrayIndicies = from item in treeClass.Items
                                          where item is ItemCStyleArray
                                          let citem = item as ItemCStyleArray
                                          from index in citem.Indicies
                                          where !index.indexConst && index.indexBoundName == cBoundName
                                          select index;
                bool foundTClonesArray = false;
                foreach (var item in cstyleArrayIndicies)
                {
                    item.indexBoundName = "implied";
                    foundTClonesArray = true;
                }
                treeClass.IsTClonesArrayClass = foundTClonesArray;
            }

            //
            // Finally, the one we just built!
            //

            yield return treeClass;
        }

        /// <summary>
        /// Holds the leaf info we need to pass around in order to parse a leaf's type
        /// and variable name.
        /// </summary>
        private struct SimpleLeafInfo
        {
            public string Name;
            public string Title;
            public string TypeName;

            public SimpleLeafInfo(ROOTNET.Interface.NTLeaf leaf)
            {
                Name = leaf.Name;
                Title = leaf.Title;
                TypeName = leaf.TypeName;
            }

            public SimpleLeafInfo(SimpleLeafInfo old)
            {
                Name = old.Name;
                Title = old.Title;
                TypeName = old.TypeName;
            }
        }

        /// <summary>
        /// Create the item info struct for an item that is simple. That is, simple arrays, or objects
        /// that root knows how to deal with. This is actually complex, as ROOT has several ways of specifying this,
        /// unfortunately.
        /// 1) There is the item type (leaf.TypeName). This could be "int" or "vector-blah-blah"
        /// 2) In the title there may be a "[stuff]" whihc means if it was "int" it becomes "int[]". :-)
        ///    This is for dealing with a c-style array.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private IClassItem ExtractSimpleItem(ROOTNET.Interface.NTLeaf leaf)
        {
            return ExtractSimpleItem(new SimpleLeafInfo(leaf));
        }

        private IClassItem ExtractSimpleItem(SimpleLeafInfo leaf)
        {
            string className = TypeDefTranslator.ResolveTypedef(leaf.TypeName);

            //
            // Check if it is a c-style array. If it is, then the title will contain
            // the specification we need to look at for the c-style array bit. This will get
            // called recursively to parse the actual type after the c-style array bit is stripped off.
            // 

            if (leaf.Title.Contains("["))
                return ExtractCArrayInfo(leaf);

            //
            // Next, if the type is a template, special parse that.
            // 

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
            if (IsROOTClass(className) && className != "string")
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
        /// Look for an array specification from a ROOT title.
        /// </summary>
        private Regex _arrParser = new Regex(@"^(?<vname>\w+)(?:\[(?<iname>\w+)\])+$");

        /// <summary>
        /// This looks like a C style array - that is "[" and "]" are being used, and in the title. Extract the index in it
        /// and pass it along.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private IClassItem ExtractCArrayInfo(SimpleLeafInfo leaf)
        {
            //
            // Grab the name first.
            //

            var m = _arrParser.Match(leaf.Title);
            if (!m.Success)
                return null;

            var vname = m.Groups["vname"].Value;

            //
            // Next job is to parse the remaining type into a simple item that we can use...
            // this allows for "arbitrary" items that are C-style arrays.
            //

            var baseItem = ExtractSimpleItem(new SimpleLeafInfo() { Title = vname, Name = vname, TypeName = leaf.TypeName });

            //
            // Great! And then the new type will be whatever the type is of this base item, but indexed...
            //

            var tname = baseItem.ItemType;
            for (int i = 0; i < m.Groups["iname"].Captures.Count; i++)
            {
                tname += "[]";
            }
            var arr = new ItemCStyleArray(tname, baseItem);

            //
            // Now, loop through and grab all the indicies we can find.
            //

            int index = 0;
            foreach (var indexBound in m.Groups["iname"].Captures.Cast<Capture>())
            {
                index++;
                var iname = indexBound.Value;
                bool isConst = iname.All(char.IsDigit);
                arr.Add(0, iname, isConst);
            }
            return arr;
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
        private IClassItem ExtractTemplateItem(SimpleLeafInfo leaf, TemplateParser.TemplateInfo templateInfo)
        {
            if (templateInfo.TemplateName == "vector")
            {
                return new ItemVector(TemplateParser.TranslateToCSharp(templateInfo), ExtractVarName(leaf));
            }
            else
            {
                throw new NotImplementedException("We rae not able to handle template classes other than vector: '" + templateInfo.TemplateName + "'");
            }
        }

        /// <summary>
        /// Correctly extract the leaf name. If it is an array, then it might not be what we are expecting here...
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private string ExtractVarName(SimpleLeafInfo leaf)
        {
            var m = _arrParser.Match(leaf.Title);
            if (!m.Success)
                return leaf.Name;

            return m.Groups["vname"].Value;
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
