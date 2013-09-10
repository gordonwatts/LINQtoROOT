using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTreeHelpers.SparseTables
{
    /// <summary>
    /// Implement a simple sparse table class with some "unique" input and output abilities.
    /// </summary>
    public class SparseTable
    {
        private class Column
        {
            public Dictionary<string, float> _values = new Dictionary<string, float>();
        }

        /// <summary>
        /// Holds the sparse table
        /// </summary>
        private Dictionary<string, Column> _table = new Dictionary<string, Column>();

        /// <summary>
        /// Returns a list of all the columns we know about.
        /// </summary>
        public string[] ListOfColumns
        {
            get
            {
                var keys = from k in _table.Keys
                           select k;
                return keys.ToArray();
            }
        }

        /// <summary>
        /// Returns a list of rows, sorted.
        /// </summary>
        public string[] ListOfRows
        {
            get
            {
                var allRows = from col in _table.Keys
                              from row in _table[col]._values.Keys
                              group row by row into g
                              orderby g.Key
                              select g.Key;
                return allRows.ToArray();

            }
        }

        /// <summary>
        /// Add a dictionary of rows to a given column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="vals"></param>
        public void Add(string columnName, Dictionary<string, float> vals)
        {
            if (columnName == null)
                throw new ArgumentNullException("column name must be valid");
            if (vals == null)
                throw new ArgumentNullException("values must not be null");

            var c = GetColumnOrCreate(columnName);
            foreach (var item in vals)
            {
                c._values.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Set the value of a specific cell
        /// </summary>
        /// <param name="columnName">Column name of cell to set</param>
        /// <param name="rowName">Row name of cell to set</param>
        /// <param name="val">Value to set the cell to</param>
        public void Add(string columnName, string rowName, float val)
        {
            if (columnName == null)
                throw new ArgumentNullException("column name must be valid");
            if (rowName == null)
                throw new ArgumentNullException("row name must be valid");

            var c = GetColumnOrCreate(columnName);
            c._values.Add(rowName, val);
        }

        /// <summary>
        /// Add a single row of values
        /// </summary>
        /// <param name="rowName"></param>
        /// <param name="vals"></param>
        public void AddRow(string rowName, Dictionary<string, float> vals)
        {
            if (rowName == null)
                throw new ArgumentNullException("column name must be valid");
            if (vals == null)
                throw new ArgumentNullException("values must not be null");

            foreach (var item in vals)
            {
                var col = GetColumnOrCreate(item.Key);
                col._values.Add(rowName, item.Value);
            }
        }

        /// <summary>
        /// Return a given column. If it doesn't exist, then create it.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private Column GetColumnOrCreate(string columnName)
        {
            if (!_table.ContainsKey(columnName))
                _table[columnName] = new Column();

            return _table[columnName];
        }

        /// <summary>
        /// Return the value. Zero otherwise.
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="rowName"></param>
        /// <returns></returns>
        public float this[string colName, string rowName]
        {
            get
            {
                if (colName == null || rowName == null)
                    return default(float);

                if (!_table.ContainsKey(colName))
                    return default(float);
                var col = _table[colName];
                if (!col._values.ContainsKey(rowName))
                    return default(float);
                return col._values[rowName];
            }
        }
    }
}
