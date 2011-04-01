
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Any variable that is going to be passed around, etc., by the system is defined by this guy.
    /// </summary>
    public interface IVariable : IValue
    {
        /// <summary>
        /// Returns the actual variable name that we are representing
        /// </summary>
        string VariableName { get; }

        /// <summary>
        /// Get the initial value of the variable. "null" is allowed here, and it indicates it should
        /// be set to default(int), whatever that is!
        /// </summary>
        IValue InitialValue { get; set; }

        /// <summary>
        /// If true then this variable needs a declaration in a statement block. Otherwise it is
        /// declared in something like a for loop - so nothing explicit is required.
        /// </summary>
        bool Declare { get; set; }
    }
}
