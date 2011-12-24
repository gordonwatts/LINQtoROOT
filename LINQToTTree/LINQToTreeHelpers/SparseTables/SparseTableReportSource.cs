using Doddle.Reporting;

namespace LINQToTreeHelpers.SparseTables
{
    /// <summary>
    /// Returns a report source for a doodle report.
    /// </summary>
    internal class SparseTableReportSource : IReportSource
    {
        /// <summary>
        /// Internal reference to the table we are generating the report for.
        /// </summary>
        private SparseTable _table;

        /// <summary>
        /// Create a report source for a given table.
        /// </summary>
        /// <param name="tbl"></param>
        public SparseTableReportSource(SparseTable tbl)
        {
            this._table = tbl;
        }

        /// <summary>
        /// Given a row name and a column name, return it. Generally used by the reporting
        /// interface for turning this into a report.
        /// </summary>
        /// <param name="rowNameObj">The row object returned by GetItems</param>
        /// <param name="colName">The name of the column</param>
        /// <returns>The data item for this field</returns>
        public object GetFieldValue(object rowNameObj, string colName)
        {
            if (rowNameObj == null)
                return string.Empty;

            var rowName = rowNameObj as string;
            if (colName == "TheRowName")
                return rowName;

            return _table[colName, rowName];
        }

        /// <summary>
        /// Return a list of all columns this report knows about. These will be come the column headers
        /// </summary>
        /// <returns></returns>
        public ReportFieldCollection GetFields()
        {
            ReportFieldCollection result = new ReportFieldCollection();

            result.Add("TheRowName", typeof(string));
            foreach (var col in _table.ListOfColumns)
            {
                result.Add(col, typeof(float));
            }

            return result;
        }

        /// <summary>
        /// Returns an item for each row in the array
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerable GetItems()
        {
            foreach (var item in _table.ListOfRows)
            {
                yield return item;
            }
        }
    }
}
