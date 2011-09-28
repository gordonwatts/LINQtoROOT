
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Simple interface to get the variable saver guys
    /// </summary>
    public interface IVariableSaverManager
    {
        /// <summary>
        /// Returns a variable saver for the particular variable.
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        IVariableSaver Get(IDeclaredParameter var);
    }
}
