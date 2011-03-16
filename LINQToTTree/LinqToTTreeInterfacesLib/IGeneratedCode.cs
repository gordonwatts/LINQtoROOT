
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Object that holds onto the current scope for this generated object.
    /// </summary>
    public interface IScopeInfo
    {

    }

    /// <summary>
    /// Interface for implementing an object that will contain a complete single query
    /// </summary>
    public interface IGeneratedCode
    {
        /// <summary>
        /// Add a new statement to the current spot where the "writing" currsor is pointed.
        /// </summary>
        /// <param name="s"></param>
        void Add(IStatement s);

        /// <summary>
        /// Book a variable at the inner most scope
        /// </summary>
        /// <param name="v"></param>
        void Add(IVariable v);

        /// <summary>
        /// Add in an outter scope. Fails badly if that outter scope doesn't exist yet!
        /// </summary>
        /// <param name="v"></param>
        void AddOneLevelUp(IVariable v);

        /// <summary>
        /// This variable's inital value is "complex" and must be transfered over the wire in some way other than staight into the code
        /// (for example, a ROOT object that needs to be written to a TFile).
        /// </summary>
        /// <param name="v"></param>
        void QueueForTransfer(string key, object value);

        /// <summary>
        /// Returns the outter most coding block
        /// </summary>
        IBookingStatementBlock CodeBody { get; }

        /// <summary>
        /// Adds an include file to be included for this query's C++ file.
        /// </summary>
        /// <param name="filename"></param>
        void AddIncludeFile(string filename);

        /// <summary>
        /// Set the result of the current code contex.
        /// </summary>
        /// <param name="result"></param>
        void SetResult(IVariable result);

        /// <summary>
        /// Returns the value that is the result of this calculation.
        /// </summary>
        IVariable ResultValue { get; }

        /// <summary>
        /// Get/Set teh current scope...
        /// </summary>
        IScopeInfo CurrentScope { get; set; }

        /// <summary>
        /// How far down in the hierarchy of statements are we?
        /// </summary>
        int Depth { get; }
    }
}
