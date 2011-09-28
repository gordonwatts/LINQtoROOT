
using System.Linq.Expressions;
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
    public interface IGeneratedQueryCode
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
        void Add(IDeclaredParameter v);

        /// <summary>
        /// Add in an outter scope. Fails badly if that outter scope doesn't exist yet!
        /// </summary>
        /// <param name="v"></param>
        void AddOneLevelUp(IDeclaredParameter v);

        /// <summary>
        /// Add outside the current loop. Walks back up the scoping until it finds a loop construct that
        /// is active. If there is none, that causes a crash! :-)
        /// </summary>
        /// <param name="indexSeen"></param>
        void AddOutsideLoop(IDeclaredParameter indexSeen);

        /// <summary>
        /// This variable's inital value is "complex" and must be transfered over the wire in some way other than staight into the code
        /// (for example, a ROOT object that needs to be written to a TFile).
        /// </summary>
        /// <param name="value">Object to be saved</param>
        /// <returns>The key that you can use to look it up</returns>
        string QueueForTransfer(object value);

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
        void SetResult(Expression result);

        /// <summary>
        /// Set no-result (i.e. set it to null).
        /// </summary>
        void ResetResult();

        /// <summary>
        /// Returns the value that is the result of this calculation.
        /// </summary>
        Expression ResultValue { get; }

        /// <summary>
        /// Get/Set teh current scope...
        /// </summary>
        IScopeInfo CurrentScope { get; set; }

        /// <summary>
        /// How far down in the hierarchy of statements are we?
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// Add a leaf reference. Tracking for later optimization
        /// </summary>
        /// <param name="leafName">Name of the ntuple leaf - should be exactly as it appears in the code</param>
        void AddReferencedLeaf(string leafName);

        /// <summary>
        /// Pop up one level in the nesting hierarchy. This means one full booking level (it has to be a booking level).
        /// </summary>
        void Pop();
    }
}
