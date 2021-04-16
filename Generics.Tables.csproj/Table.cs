using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics.Tables
{
    public class Table<TRow, TColumn, TValue>
    {
        private static DataSet<TRow, TColumn, TValue> databinding;
        public List<TRow> Rows { get => Open.Rows; }
        public List<TColumn> Columns { get => Open.Columns; }

        public Table()
        {
            databinding = new DataSet<TRow, TColumn, TValue>();
        }

        public void AddRow(TRow row)
        {
            if (!Rows.Contains(row))
                Rows.Add(row);
        }

        public void AddColumn(TColumn column)
        {
            if (!Columns.Contains(column))
                Columns.Add(column);
        }

        Func<bool, DataSet<TRow, TColumn, TValue>> bufferWithExistedOpen = (isExisted) =>
        {
            var buffer =new DataSet<TRow,TColumn,TValue>();
            buffer.CopyOf(databinding);
            buffer.IsExistedOpen = isExisted;
            return buffer;
        };

        public DataSet<TRow, TColumn, TValue> Open => bufferWithExistedOpen(false);
        public DataSet<TRow, TColumn, TValue> Existed => bufferWithExistedOpen(true);
    }

    public class DataSet<TRow, TColumn, TValue>
    {
        public List<TRow> Rows { get; private set; }
        public List<TColumn> Columns { get; private set; }
        public Dictionary<Tuple<TRow, TColumn>, TValue> Values { get; private set; }
        public bool IsExistedOpen;

        public DataSet()
        {
            Values = new Dictionary<Tuple<TRow, TColumn>, TValue>();
            Rows = new List<TRow>();
            Columns = new List<TColumn>();
        }

        public DataSet<TRow,TColumn,TValue> CopyOf(DataSet<TRow,TColumn,TValue> data)
        {
            Rows = data.Rows;
            Columns = data.Columns;
            Values = data.Values;
            return this;
        }

        public TValue this[TRow row, TColumn column]
        {
            get
            {
                var key = Tuple.Create(row, column);
                if ((!Rows.Contains(row) || !Columns.Contains(column)) && IsExistedOpen)
                        throw new ArgumentException();
                if (!Values.ContainsKey(key) && IsExistedOpen)
                    Values[key] = default;
                else if (!Values.ContainsKey(key))
                    return default;
                if (!Rows.Contains(row))
                        Rows.Add(row);
                if (!Columns.Contains(column))
                        Columns.Add(column);
                return Values[key];
            }
            set
            {
                var key = Tuple.Create(row, column);
                if (IsExistedOpen)
                {
                    if (!Rows.Contains(row) || !Columns.Contains(column))
                        throw new ArgumentException();
                    else
                        Values[key] = value;
                }
                else
                {
                    if (!Rows.Contains(row))
                        Rows.Add(row);
                    if (!Columns.Contains(column))
                        Columns.Add(column);
                    Values[key] = value;
                }
            }
        }
    }
}
