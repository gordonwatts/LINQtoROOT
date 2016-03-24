using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// An expression that represents a declarable parameter.
    /// </summary>
    /// <remarks>
    /// Often used in returns from various operators that have some value that can be used
    /// in the next phase of the query or as a result (i.e. Count, etc.).
    /// </remarks>
    public class DeclarableParameter : Expression, IDeclaredParameter
    {
        #region Creators
        /// <summary>
        /// Create a new variable that is an array.
        /// </summary>
        /// <param name="varType"></param>
        /// <returns></returns>
        public static DeclarableParameter CreateDeclarableParameterArrayExpression(Type varType)
        {
            return new DeclarableParameter(varType.MakeArrayType(1), string.Format("{0}Array", varType.CreateUniqueVariableName()));
        }

        /// <summary>
        /// Use only if you know what you are doing is going to render a unique name!
        /// </summary>
        /// <param name="name"></param>
        /// <param name="varType"></param>
        /// <returns></returns>
        public static DeclarableParameter CreateDeclarableParameterExpression(string name, Type varType)
        {
            return new DeclarableParameter(varType, name);
        }

        /// <summary>
        /// Declare a variable of the given type. The variable name will be assigned
        /// using the usual global number generator (to keep everything unique).
        /// </summary>
        /// <param name="varType"></param>
        /// <returns></returns>
        public static DeclarableParameter CreateDeclarableParameterExpression(Type varType)
        {
            return new DeclarableParameter(varType, varType.CreateUniqueVariableName());
        }


        /// <summary>
        /// Create a Dictionary type. This maps to a std::map....
        /// </summary>
        /// <param name="type"></param>
        /// <param name="type_2"></param>
        /// <returns></returns>
        public static DeclarableParameter CreateDeclarableParameterMapExpression(System.Type indexType, System.Type valueType)
        {
            var gDict = typeof(System.Collections.Generic.Dictionary<int, int>).GetGenericTypeDefinition();
            var sDict = gDict.MakeGenericType(new Type[] { indexType, valueType });
            return new DeclarableParameter(sDict, string.Format("{0}Map", sDict.CreateUniqueVariableName()));
        }
        #endregion

        /// <summary>
        /// The expression type for testing to see if it is a declared variable.
        /// </summary>
        public const ExpressionType ExpressionType = (ExpressionType)110002;
        private Type _type;

        /// <summary>
        /// Return the expression type
        /// </summary>
        public override ExpressionType NodeType { get { return ExpressionType; } }

        /// <summary>
        /// We don't have to do anything. So let it go.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        /// <summary>
        /// Build the variable.
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varName"></param>
        private DeclarableParameter(Type varType, string varName)
        {
            _type = varType;
            if (varName == null)
                throw new ArgumentNullException("varName");
            ParameterName = varName;
        }

        /// <summary>
        /// Return the type of this node
        /// </summary>
        public override Type Type { get { return _type; } }

        /// <summary>
        /// Return the variable name for this declared variable.
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Rename the parameter name to the new name as long as it matches the old name.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameParameter(string oldname, string newname)
        {
            if (ParameterName == oldname)
                ParameterName = newname;

            if (InitialValue != null)
                InitialValue.RenameRawValue(oldname, newname);
        }

        /// <summary>
        /// Return the string for this variable - just the name in this case.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ParameterName;
        }

        /// <summary>
        /// Initial value to set this declared variable to. If
        /// null it should be set to the default value (like "0" for int).
        /// Assume everything is explicitly initialized!
        /// </summary>
        public IValue InitialValue { get; set; }

        /// <summary>
        /// Set the initial value with this type.
        /// </summary>
        /// <param name="v"></param>
        public void SetInitialValue(string v)
        {
            InitialValue = new ValSimple(v, Type);
        }

        /// <summary>
        /// Return the parameter name as our value raw value.
        /// </summary>
        public string RawValue
        {
            get { return ParameterName; }
        }

        /// <summary>
        /// We depend only on ourselves!
        /// </summary>
        public IEnumerable<IDeclaredParameter> Dependants
        {
            get
            {
                return new IDeclaredParameter[] { this };
            }
        }

        /// <summary>
        /// Rename the raw value.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameRawValue(string oldname, string newname)
        {
            RenameParameter(oldname, newname);
        }
    }
}
