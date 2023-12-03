using System.Collections.Specialized;
using System.Data;
using System.Reflection;

namespace CSSTest.Models.Helper
{
    public class DataReaderAdapter<T> : IDataReader
    {
        public int Depth { get; } = 0;

        public bool IsClosed => _items.Count == _currentIndex;

        public int RecordsAffected => -1;

        private readonly IList<T> _items;
        private List<string> _columnNames;
        private int _currentIndex;

        private readonly Value _values;

        private OrderedDictionary FieldDictionary { get; }
        public DataReaderAdapter(IList<T> items)
        {
            _items = items;
            _currentIndex = -1;
            FieldDictionary = PrepareFieldLookup();
        }

        public DataReaderAdapter(Value value)
        {
            _values = value;
            _currentIndex = -1;
            FieldDictionary = PrepareFieldLookup();
        }

        private static OrderedDictionary PrepareFieldLookup()
        {
            var fieldlLookup = new OrderedDictionary();
            int i = 0;

            foreach (var property in typeof(T).GetProperties())
            {
                if (property.Name.Contains("FundId") || !property.Name.Contains("Fund"))
                    fieldlLookup.Add(property.Name, new KeyValuePair<int, string>(i++, property.Name));
            }

            return fieldlLookup;
        }

        public List<string> ColumnNames
        {
            get
            {
                if (_columnNames == null)
                {
                    _columnNames = new List<string>();
                    foreach (var kvp in FieldDictionary)
                    {
                        var colName = ((dynamic)kvp).Key;
                        _columnNames.Add(colName);
                    }
                }
                return _columnNames;
            }
        }

        public int FieldCount => FieldDictionary.Count;

        public void Dispose()
        {

        }

        public object GetValue(int i)
        {
            string propName = ((dynamic)FieldDictionary[i]).Value;

            object currentItem = null;

            if (_items != null)
            {
                currentItem = _items[_currentIndex];
            }

            if (_values != null && currentItem == null)
            {
                currentItem = _values;
            }

            var propertyInfo = currentItem.GetType().GetProperty(propName);
            object value = null;

            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(currentItem, null);
            }

            return value;
        }

        public bool IsDBNull(int i)
        {
            return false;
        }

        public int GetOrdinal(string name)
        {
            return ((dynamic)FieldDictionary[name]).Key;
        }

        public bool Read()
        {
            if (_items != null)
            {
                if (_currentIndex < _items.Count - 1)
                {
                    _currentIndex++;
                    return true;
                }
            }

            if (_values != null)
            {
                Type type = _values.GetType();
                PropertyInfo[] props = type.GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i].Name.Contains("FundId") || !props[i].Name.Contains("Fund"))
                    {
                        if (_currentIndex < i)
                        {
                            _currentIndex++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #region properties not implemented
        public object this[int i] => throw new NotImplementedException();

        public object this[string name] => throw new NotImplementedException();

        #endregion

        #region methods not implemented

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
