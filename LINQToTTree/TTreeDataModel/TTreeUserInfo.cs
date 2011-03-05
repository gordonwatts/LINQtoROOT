
namespace TTreeDataModel
{
    /// <summary>
    /// This class contains user information about the ntuple - things the user
    /// will likely want to change.
    /// </summary>
    public class TTreeUserInfo
    {
        /// <summary>
        /// The list of groups the arrays for this ttree fall info
        /// </summary>
        public ArrayGroup[] Groups { get; set; }
    }
}
