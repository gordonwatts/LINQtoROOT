
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// A group of items that are 
    /// </summary>
    public class ArrayGroup
    {
        /// <summary>
        /// The name of the group
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// The list of variable names that this group contains.
        /// </summary>
        public VariableInfo[] Variables { get; set; }

        /// <summary>
        /// Any comments the user wants to show up in the intellisense
        /// </summary>
        [XmlAttribute]
        public string Comment { get; set; }

        /// <summary>
        /// A custom class name, rather than the automatically generated one.
        /// </summary>
        [XmlAttribute]
        public string ClassName { get; set; }

        /// <summary>
        /// The NET name of a variable we can use as the ".size()" variable - so we can use it to figure
        /// out how big this array is.
        /// </summary>
        /// <remarks>
        ///  Normally this is just the alphabetically first variable in there. However, sometimes this is useful to
        ///  specify some other variable.
        /// When to use: 
        ///    - You are trying to get things to run with a new and old version of a TTree. The new variable is something
        ///      like "aaVar". This means the new variable will now be used as the .size() - which means you can't run on
        ///      the old TTree any longer. So specify a variable that is present in both versions of TTree using this
        ///      feature.
        ///  </remarks>
        [XmlAttribute]
        public string NETNameOfVariableToUseAsArrayLength { get; set; }
    }
}
