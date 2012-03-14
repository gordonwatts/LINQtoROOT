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

            foreach (var c in ExtractClassesFromBranchList(masterClass, tree.GetListOfBranches().Cast<ROOTNET.Interface.NTBranch>()))
            {
                yield return c;
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
            // We are going to build our own class type here.
            //

            //container.Add(new ItemROOTClass(branch.Name, branch.GetClassName()));
            //var treeClass = new ROOTClassShell(cls.Name);

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

            //
            // Next, check if it is an C++ array.
            // 

            if (leaf.Title.Contains("["))
                return ExtractCArrayInfo(leaf);

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
        /// This looks like a C style array - that is "[" and "]" are being used. Extract the index in it
        /// and pass it along.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private IClassItem ExtractCArrayInfo(ROOTNET.Interface.NTLeaf leaf)
        {
            //
            // Grab the name first.
            //

            var m = _arrParser.Match(leaf.Title);
            if (!m.Success)
                return null;

            var vname = m.Groups["vname"].Value;
            var tname = TypeDefTranslator.ResolveTypedef(leaf.TypeName).SimpleCPPTypeToCSharpType();
            for (int i = 0; i < m.Groups["iname"].Captures.Count; i++)
            {
                tname += "[]";
            }
            var arr = new ItemCStyleArray(tname, vname);

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
