
namespace LinqToTTreeInterfacesLib
{
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
        /// This variable's inital value is "complex" and must be transfered over the wire in some way other than staight into the code
        /// (for example, a ROOT object that needs to be written to a TFile).
        /// </summary>
        /// <param name="v"></param>
        void QueueForTransfer(string key, object value);

        /// <summary>
        /// Returns the outter most coding block
        /// </summary>
        IBookingStatementBlock CodeBody { get; }
    }
}
