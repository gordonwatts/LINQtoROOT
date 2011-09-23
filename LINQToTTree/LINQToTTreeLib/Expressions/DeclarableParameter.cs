using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using LINQToTTreeLib.Utils;
using Remotion.Linq.Utilities;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// An expression that represents a declarable parameter.
    /// </summary>
    /// <remarks>
    /// Often used in returns from various operators that have some value that can be used
    /// in the next phase of the query or as a result (i.e. Count, etc.).
    /// </remarks>
    public class DeclarableParameter : Expression
    {
        /// <summary>
        /// The expression type for testing to see if it is a declared variable.
        /// </summary>
        public const ExpressionType ExpressionType = (ExpressionType)110002;

        /// <summary>
        /// Declare a variable of the given type. The variable name will be assigned
        /// using the usual global number generator (to keep everything unique).
        /// </summary>
        /// <param name="varType"></param>
        /// <returns></returns>
        public static DeclarableParameter DeclarableParameterExpression(Type varType)
        {
            return new DeclarableParameter(varType, varType.CreateUniqueVariableName());
        }

        /// <summary>
        /// Build the variable.
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varName"></param>
        private DeclarableParameter(Type varType, string varName)
            : base (ExpressionType, varType)
        {
            ArgumentUtility.CheckNotNullOrEmpty("varName", varName);
            VariableName = varName;
        }

        /// <summary>
        /// Return the variable name for this declared variable.
        /// </summary>
        public string VariableName { get; private set; }

        /// <summary>
        /// Rename the parameter name to the new name as long as it matches the old name.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameParameter (string oldname, string newname)
        {
            if (VariableName == oldname)
                VariableName = newname;
        }

        /// <summary>
        /// Return the string for this variable - just the name in this case.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return VariableName;
        }

        /// <summary>
        /// Initial value to set this declared variable to. If
        /// null it shoudl be set to the default value (like "0" for int).
        /// Assume everything is explicitly initalized!
        /// </summary>
        public string InitialValue { get; set; }
    }
}
