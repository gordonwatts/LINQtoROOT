﻿using Doddle.Reporting;

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
