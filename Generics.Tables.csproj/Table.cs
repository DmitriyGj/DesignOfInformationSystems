using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics.Tables
{
    public class Table<TRow, TColumn, TValue>
    { 
        public List<TRow> Rows { get; set; }
        public List<TColumn> Columns { get; set; }

        public Dictionary<Tuple<TRow, TColumn>, TValue> Values ;
        private ExistedIndexer<TRow, TColumn, TValue> existed { get;  }
        private NotExistedIndexer<TRow, TColumn, TValue> notexisted { get; }

        public Table()
        {
            Rows = new List<TRow>();
            Columns = new List<TColumn>();
            Values = new Dictionary<Tuple<TRow, TColumn>, TValue>(); 
            existed= new ExistedIndexer<TRow, TColumn, TValue>(this);
            notexisted = new NotExistedIndexer<TRow, TColumn, TValue>(this);
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

        public NotExistedIndexer<TRow, TColumn, TValue> Open => notexisted;

        public ExistedIndexer<TRow, TColumn, TValue> Existed =>existed;
    }

    public class NotExistedIndexer<TRow, TColumn, TValue> 
    {
        Table<TRow, TColumn, TValue> table;
        public NotExistedIndexer(Table<TRow, TColumn, TValue> table)
        {
            this.table = table;
        }

        public TValue this[TRow row, TColumn column]
        {
            get
            {
                var key = Tuple.Create(row, column);
                if ((!table.Rows.Contains(row) && !table.Columns.Contains(column)) || !table.Values.ContainsKey(key))
                    return default;
                if (!table.Rows.Contains(row))
                    table.Rows.Add(row);
                if (!table.Columns.Contains(column))
                    table.Columns.Add(column);
                return table.Values[key];
            }

            set
            {
                var key = Tuple.Create(row, column);
                if (!table.Rows.Contains(row))
                    table.Rows.Add(row);
                if (!table.Columns.Contains(column))
                    table.Columns.Add(column);
                table.Values[key] = value;
            }
        }
    }

    public class ExistedIndexer<TRow,TColumn, TValue>
    {
        Table<TRow, TColumn, TValue> table;
        public ExistedIndexer(Table<TRow, TColumn, TValue> table)
        {
            this.table = table;
        }

        public TValue this[TRow row, TColumn column]
        {
            get
            {
                var key = Tuple.Create(row, column);
                if (!table.Rows.Contains(row) || !table.Columns.Contains(column))
                    throw new ArgumentException();
                else if (!table.Values.ContainsKey(key) )
                    table.Values[key] = default;
                return table.Values[key];
            }
            set
            {
                var key = Tuple.Create(row, column);
                if (!table.Rows.Contains(row) || !table.Columns.Contains(column))
                    throw new ArgumentException();
                else
                    table.Values[key] = value;
            }
        }
    }
}
