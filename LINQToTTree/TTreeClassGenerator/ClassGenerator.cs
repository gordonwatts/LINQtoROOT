using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TTreeDataModel;

namespace TTreeClassGenerator
{
    /// <summary>
    /// Starting from the class interface, generate a class that can be used in LINQ queires.
    /// All classes are generated in a single output file.
    /// </summary>
    public class ClassGenerator
    {
        /// <summary>
        /// GIven an existing file that contains the XML class spec, generate a CS file
        /// that conatins all the definitions needed. Everything will be burried insize
        /// the namespaceName.
        /// </summary>
        /// <param name="inputXMLFile"></param>
        /// <param name="outputCSFile"></param>
        public void GenerateClasss(FileInfo inputXMLFile, FileInfo outputCSFile, string namespaceName)
        {
            ///
            /// Quick checks
            /// 

            if (inputXMLFile == null)
                throw new ArgumentNullException("inputXMLFile");
            if (!inputXMLFile.Exists)
                throw new ArgumentException("inputXMLFile does not currently exist!");

            ///
            /// Load up the XML file
            /// 

            var classSpec = LoadFromXMLFile(inputXMLFile);

            ///
            /// Next, see if we can find the user specification file
            /// 

            Dictionary<string, TTreeUserInfo> userInfoMap = new Dictionary<string, TTreeUserInfo>();
            foreach (var c in classSpec.Classes)
            {
                var xmlUserFileName = c.UserInfoPath;
                if (!string.IsNullOrEmpty(xmlUserFileName))
                {
                    var xmlFile = new FileInfo(xmlUserFileName);
                    if (!xmlFile.Exists)
                    {
                        xmlFile = new FileInfo(inputXMLFile.DirectoryName + "\\" + xmlUserFileName);
                        if (!xmlFile.Exists)
                            throw new ArgumentException("Unable to find user selection file '" + xmlUserFileName + " - " + xmlFile.FullName);
                    }

                    userInfoMap[c.Name] = LoadUserInfoFromXMLFile(xmlFile);
                }
            }

            ///
            /// Now do the actual work!
            /// 

            GenerateClasss(classSpec, outputCSFile, namespaceName, userInfoMap);
        }

        /// <summary>
        /// Generate the classes given the input classes
        /// </summary>
        /// <param name="classSpec"></param>
        /// <param name="outputCSFile"></param>
        /// <param name="namespaceName"></param>
        public void GenerateClasss(NtupleTreeInfo classSpec, FileInfo outputCSFile, string namespaceName, IDictionary<string, TTreeUserInfo> userInfo = null)
        {
            ///
            /// Parameter checks
            /// 

            if (outputCSFile == null)
                throw new ArgumentNullException("outputCSFile");
            if (string.IsNullOrWhiteSpace(namespaceName))
                throw new ArgumentNullException("namespaceName");

            bool isbad = namespaceName.Any(c => !char.IsLetterOrDigit(c));
            if (isbad || !char.IsLetter(namespaceName[0]))
            {
                throw new ArgumentException("Namespace is not a legal C++ name");
            }

            if (classSpec == null)
                throw new ArgumentNullException("classSpec");

            if (classSpec.Classes == null)
                throw new ArgumentNullException("classSpec.Classes");

            foreach (var c in classSpec.Classes)
            {
                if (c.NtupleProxyPath == null)
                    throw new ArgumentNullException("Class '" + c.Name + "' has no ntuple proxy. Can't generate a class for it.");
                if (!File.Exists(c.NtupleProxyPath))
                    throw new ArgumentNullException("Class '" + c.Name + "'s ntuple proxy does not exist at " + c.NtupleProxyPath + ". Can't generate a class for it.");
            }

            foreach (var c in classSpec.ClassImplimintationFiles)
            {
                if (c == null)
                    throw new ArgumentNullException("Class support files can't be null");
                if (!File.Exists(c))
                    throw new ArgumentException("Can't fine class support file '" + c + "'.");
            }

            ///
            /// Ok, open the output file
            /// 

            using (var output = outputCSFile.CreateText())
            {
                ///
                /// Write out the header and using clauses
                /// 

                output.WriteLine("//");
                output.WriteLine("// Automatically Generated File - do not modify!");
                output.WriteLine("// Generated on {0} by TTreeClassGenerator::ClassGenerator", DateTime.Now);
                output.WriteLine("// Translated ntuple classes");
                foreach (var cls in classSpec.Classes)
                {
                    output.WriteLine("// - ntuple {0}", cls.Name);
                }

                output.WriteLine("//");
                output.WriteLine();

                output.WriteLine("using System.IO;");
                output.WriteLine("using LINQToTTreeLib;");
                output.WriteLine("using LinqToTTreeInterfacesLib;");
                output.WriteLine("using LINQToTTreeLib.CodeAttributes;");
                output.WriteLine("using System.Linq.Expressions;");

                output.WriteLine();
                output.WriteLine("namespace {0} {{", namespaceName);

                ///
                /// Now, write something out for each class
                /// 

                foreach (var cls in classSpec.Classes)
                {
                    ///
                    /// Write out the info class that contains everything needed to process this.
                    /// We could use attribute programing here, but that takes more code at the other
                    /// end, so until there is a real reason, we'll do it this way.
                    /// 
                    /// This is a kludge until we do more work to fix things with attributes, etc.
                    /// 

                    Action writeOutExtra = () =>
                    {
                        output.WriteLine("    public static string _gProxyFile=@\"" + cls.NtupleProxyPath + "\";");
                        output.WriteLine("    public static string[] _gObjectFiles= {");
                        foreach (var item in classSpec.ClassImplimintationFiles)
                        {
                            output.WriteLine("      @\"" + item + "\",");
                        }
                        output.WriteLine("    };");
                    };

                    ///
                    /// First, if there is a translated object model, write it out.
                    /// 

                    var rawClassName = cls.Name;
                    if (userInfo != null)
                    {
                        if (userInfo.ContainsKey(cls.Name))
                        {
                            if (RequiresTranslation(userInfo[cls.Name]))
                            {
                                rawClassName = rawClassName + "TranslatedTo";
                                var varTypes = FindVariableTypes(cls.Items);
                                WriteTranslatedObjectStructure(output, userInfo[cls.Name], cls.Name, rawClassName, varTypes, writeOutExtra);
                                output.WriteLine();
                                output.WriteLine();
                            }
                        }
                    }

                    output.WriteLine("  public class {0} : IExpressionHolder", rawClassName);
                    output.WriteLine("  {");
                    output.WriteLine("    public Expression HeldExpression {get; private set;}");
                    output.WriteLine("    public {0} (Expression expr) {{ HeldExpression = expr; }}", rawClassName);
                    output.WriteLine();

                    ///
                    /// These fields will never be set or accessed - so we turn off some warnings the compiler will generate.
                    /// They are dummies so that intellisense works and we can do the translation properly.
                    /// 

                    output.WriteLine("#pragma warning disable 0649");

                    foreach (var item in cls.Items)
                    {
                        WriteItem(item, output);
                    }

                    output.WriteLine("#pragma warning restore 0649");

                    writeOutExtra();

                    output.WriteLine("  }"); // End of the class
                    output.WriteLine();
                    output.WriteLine();

                    ///
                    /// Next, write out the queriable classes so we can actually do the queires
                    /// against the trees!
                    /// 

                    output.WriteLine("  /// Helper classes");
                    output.WriteLine("  public static class Queryable{0}", cls.Name);
                    output.WriteLine("  {");
                    output.WriteLine("    /// Create a LINQ to TTree interface for a file and optional tree name");
                    output.WriteLine("    public static QueriableTTree<{0}> Create (this FileInfo rootFile, string treeName = \"{0}\")", cls.Name);
                    output.WriteLine("    {");
                    output.WriteLine("      return new QueriableTTree<{0}>(rootFile, treeName);", cls.Name);
                    output.WriteLine("    }");
                    output.WriteLine("    /// Create a LINQ to TTree interface for a list of files and optional tree name");
                    output.WriteLine("    public static QueriableTTree<{0}> Create (this FileInfo[] rootFiles, string treeName = \"{0}\")", cls.Name);
                    output.WriteLine("    {");
                    output.WriteLine("      return new QueriableTTree<{0}>(rootFiles, treeName);", cls.Name);
                    output.WriteLine("    }");
                    output.WriteLine("  }");
                }

                ///
                /// Done!
                /// 

                output.WriteLine("}"); // For the namespace

                output.Close();
            }
        }

        /// <summary>
        /// Find a mapping from varname -> type
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IDictionary<string, string> FindVariableTypes(List<IClassItem> list)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var v in list)
            {
                result[v.Name] = v.ItemType;
            }
            return result;
        }

        /// <summary>
        /// We have to write out the translated class. Sigh!! :-)
        /// </summary>
        /// <param name="tTreeUserInfo"></param>
        /// <param name="p"></param>
        /// <param name="rawClassName"></param>
        private void WriteTranslatedObjectStructure(TextWriter output, TTreeUserInfo tTreeUserInfo, string className, string translateToName, IDictionary<string, string> varTypes, Action kludgeWriter)
        {
            ///
            /// Main class header
            /// 

            output.WriteLine("  [TranslateToClass(typeof({0}))]", translateToName);
            output.WriteLine("  public class {0}", className);
            output.WriteLine("  {");

            ///
            /// First, do the ungrouped variables
            /// 

            var ungrouped = (from g in tTreeUserInfo.Groups
                             where g.Name == "ungrouped"
                             select g).FirstOrDefault();
            if (ungrouped != null)
            {
                foreach (var v in ungrouped.Variables)
                {
                    WriteVariableRenameDefinition(output, v, varTypes, className, false);
                }
            }


            ///
            /// Now do the groups
            /// 

            foreach (var grp in tTreeUserInfo.Groups.Where(g => g.Name != "ungrouped"))
            {
                output.WriteLine("    [TTreeVariableGrouping]");
                output.WriteLine("    public {0}{1}[] {1};", className, grp.Name);
            }

            ///
            /// And the object is finished.
            /// 

            kludgeWriter();
            output.WriteLine("  }");
            output.WriteLine();
            output.WriteLine();

            ///
            /// Now do all the group classes
            /// 

            foreach (var grp in tTreeUserInfo.Groups.Where(g => g.Name != "ungrouped"))
            {
                output.WriteLine("  public class {0}{1}", className, grp.Name);
                output.WriteLine("  {");

                foreach (var v in grp.Variables)
                {
                    output.WriteLine("    [TTreeVariableGrouping]");
                    WriteVariableRenameDefinition(output, v, varTypes, className, true);
                }

                output.WriteLine("  }");
                output.WriteLine();
                output.WriteLine();
            }
        }

        /// <summary>
        /// Write out a variable definition
        /// </summary>
        /// <param name="output"></param>
        /// <param name="v"></param>
        private void WriteVariableRenameDefinition(TextWriter output, VariableInfo v, IDictionary<string, string> varTypes, string baseTypeName, bool removeOneArrayDecl)
        {
            output.WriteLine("#pragma warning disable 0649");
            var cppVarName = v.NETName;
            if (v.NETName != v.TTreeName && string.IsNullOrEmpty(v.IndexToGroup))
            {
                cppVarName = v.TTreeName;
                output.WriteLine("    [RenameVariable(\"{0}\")]", v.TTreeName);
            }
            if (!string.IsNullOrEmpty(v.IndexToGroup))
            {
                output.WriteLine("    [IndexToOtherObjectArray(typeof({0}), \"{1}\")]", baseTypeName, v.IndexToGroup);
            }

            var typeName = varTypes[cppVarName];
            if (removeOneArrayDecl)
            {
                if (!typeName.EndsWith("[]"))
                    throw new ArgumentException("Attempting to remove an array indirection from type '" + typeName + "' and can't find a [] at the end!");
                typeName = typeName.Substring(0, typeName.Length - 2);
            }

            output.WriteLine("    public {0} {1};", typeName, v.NETName);
            output.WriteLine("#pragma warning restore 0649");
        }

        /// <summary>
        /// See if any translation is required.
        /// </summary>
        /// <param name="tTreeUserInfo"></param>
        /// <returns></returns>
        private bool RequiresTranslation(TTreeUserInfo tTreeUserInfo)
        {
            if (tTreeUserInfo.Groups.Length > 1)
                return true;
            if (tTreeUserInfo.Groups[0].Name != "ungrouped")
                return true;

            var anyRenames = tTreeUserInfo.Groups[0].Variables.Where(g => g.TTreeName != g.NETName).Any();
            return anyRenames;
        }

        /// <summary>
        /// Write out the data in a single item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="output"></param>
        private void WriteItem(IClassItem item, StreamWriter output)
        {
            if (item == null)
                throw new ArgumentNullException("item - can't have a null item in an ntuple!");

            string t = item.ItemType;
            output.WriteLine("    public {0} {1};", t, item.Name);
        }

        /// <summary>
        /// Loads in an input file and returns the classes.
        /// </summary>
        /// <param name="inputXMLFile"></param>
        /// <returns></returns>
        private NtupleTreeInfo LoadFromXMLFile(FileInfo inputXMLFile)
        {
            XmlSerializer x = new XmlSerializer(typeof(NtupleTreeInfo));
            using (var reader = inputXMLFile.OpenText())
            {
                var result = x.Deserialize(reader) as NtupleTreeInfo;
                reader.Close();
                return result;
            }
        }

        /// <summary>
        /// Load in a user info file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private TTreeUserInfo LoadUserInfoFromXMLFile(FileInfo inputFile)
        {
            XmlSerializer x = new XmlSerializer(typeof(TTreeUserInfo));
            using (var reader = inputFile.OpenText())
            {
                var result = x.Deserialize(reader) as TTreeUserInfo;
                reader.Close();
                return result;
            }
        }
    }
}
