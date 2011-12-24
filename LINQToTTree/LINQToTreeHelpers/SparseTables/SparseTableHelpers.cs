
using Doddle.Reporting;
namespace LINQToTreeHelpers.SparseTables
{
    /// <summary>
    /// Some extension methods to help with dealing with sparse tables.
    /// </summary>
    public static class SparseTableHelpers
    {
        /// <summary>
        /// Create a Doodle report source for this table.
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public static IReportSource ToReportSource(this SparseTable tbl)
        {
            return new SparseTableReportSource(tbl);
        }
    }
}
