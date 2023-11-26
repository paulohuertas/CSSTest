using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Data;
using ClosedXML.Excel;
using CSSTest.Data;
using LINQtoCSV;

namespace CSSTest.Models.Helper
{
    public class DataHelper
    {
        public static Random random = new Random();

        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public DataHelper() { }

        public DataHelper(IConfiguration configuration, ApplicationDbContext context)
        {
            _config = configuration;
            _context = context;
        }

        public List<Fund> CreateFunds(int start = 1, int finish = 5)
        {
            List<Fund> result = new List<Fund>();

            var list = Enumerable.Range(start, finish).Select(f => new Fund
            {
                Name = CreateFullName(random.Next(3)),
                Description = CreateRandomDescription()
            }).ToList();

            var funds = new DataReaderAdapter<Fund>(list);

            DataHelper dataHelper = new DataHelper(_config, _context);

            dataHelper.InsertFundToDb(funds);

            var values = dataHelper.CreateFundValues(list, 1, list.Count);

            return list;
        }
        public List<Value> CreateFundValues(List<Fund> funds, int start, int finish)
        {
            List<Value> result = new List<Value>();

            DataHelper dataHelper = new DataHelper(_config, _context);

            if (funds.Count > 0)
            {
                var list = Enumerable.Range(start, finish).Select((v, index) => new Value
                {
                    ValueDate = DateTime.Now,
                    ValueDouble = Math.Round(random.NextDouble() * random.Next(100000), 2),
                    FundId = _context.Funds.FirstOrDefault(f => f.Name == funds[index].Name).Id
                }).ToList();

               var values = new DataReaderAdapter<Value>(list);

               dataHelper.InsertValuesToDb(values);

                return list;
            }

            return null;
        }

        public void InsertFundToDb(DataReaderAdapter<Fund> _items)
        {
            string schema = "dbo";
            string tableName = "Funds";

            string? connectionString = _config.GetSection("ConnectionStrings")["FundConnection"];

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var bulkyCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = $"{schema}.[{tableName}]",
                    BatchSize = 100000
                };

                bulkyCopy.WriteToServer(_items);
                bulkyCopy.Close();

            }
        }
        public void InsertValuesToDb(DataReaderAdapter<Value> _items)
        {
            string schema = "dbo";
            string tableName = "Value";

            string? connectionString = _config.GetSection("ConnectionStrings")["FundConnection"];

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var bulkyCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = $"{schema}.[{tableName}]",
                    BatchSize = 100000
                };

                bulkyCopy.WriteToServer(_items);
                bulkyCopy.Close();

            }
        }

        public string CreateFullName(int totalSurnames = 1)
        {
            string surNames = String.Empty;

            for (int i = 0; i <= totalSurnames; i++)
            {
                surNames += CreateRandomSurnames() + " ";
            }

            string firstName = CreateRandomName();

            string fullName = firstName + " " + surNames;

            return fullName;
        }


        public string CreateRandomName()
        {
            string[] names = new string[]
            {
                "Paulo", "Pedro", "Alexandre", "Filipe", "Gabriel", "Andre", "Maria", "Carlos", "Mayara", "Flavio", "Diogo", "Janice", "Eva", "Isabelle", "Aoife",
                "Thiago", "Gabriela", "Caoimhe", "Niamh", "Luiz", "Leandro", "Sandra", "Ana", "Raissa", "Beatriz", "Julia", "Vinicius", "Larissa"
            };

            int size = names.Length;

            int position = random.Next(size);

            return names[position];
        }

        public string CreateRandomSurnames()
        {
            string[] names = new string[]
            {
                "Reyes", "Harmon", "Olson", "Todd", "Turner", "Byrd", "Tyler", "Lowe", "Knight", "Daniel",
                "Erickson", "Ramos", "Valdez", "Blake", "Little", "Schultz", "Adkins", "Ingram", "Sandoval",
                "Norris", "Ortiz","Delgado", "Smith", "Patel", "Hammond", "Wagner", "Moreno", "Bolton", "Beck",
                "Ward", "Ryan", "Chambers", "Grant", "Williams", "Guerrero", "Mejia", "Brewer", "Spencer", "Schmidt",
                "Lang", "Vaughan", "Simpson", "Harris", "Marsh", "Tucker", "Cruz", "Estrada", "Young",
                "Hill", "Savage"
            };

            int size = names.Length;

            int position = random.Next(size);

            return names[position];
        }

        public string CreateRandomDescription()
        {
            string[] description = new string[]
            {
                "Mutual Fund", "Exchange-Rate Fund", "Bond", "Money Market Fund", "Stock Fund", "Bond Fund", "Close-end Fund", "Index Fund",
                "Unit trust Fund", "Fixed Income Fund"
            };

            int size = description.Length;
            int position = random.Next(size);

            return description[position];
        }

        public IQueryable<FundValueColumnsCsv> ExportToExcel()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string savePath = System.IO.Path.Combine(desktop + "exported.csv");


            var query = from f in _context.Funds
                        join val in _context.Value on f.Id equals val.FundId
                        select new FundValueColumnsCsv
                        {
                            fund_id = f.Id,
                            fund_name = f.Name,
                            fund_description = f.Description,
                            value_date = val.ValueDate,
                            value_double = val.ValueDouble
                        };

            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true,
                FileCultureName = "en-US"
            };

            CsvContext csvContext = new CsvContext();
            csvContext.Write(query, savePath, csvFileDescription);

            return query;

        }
    }

    public class FundValueColumnsCsv
    {
        [CsvColumn(FieldIndex = 1)]
        public int fund_id { get; set; }
        [CsvColumn(FieldIndex = 2)]
        public string fund_name { get; set; }
        [CsvColumn(FieldIndex = 3)]
        public string fund_description { get; set; }
        [CsvColumn(FieldIndex = 4)]
        public DateTime value_date { get; set; }
        [CsvColumn(FieldIndex = 5)]
        public Double value_double { get; set; }

    }

    public class DataReaderAdapter<T> : IDataReader
    {
        public int Depth { get; } = 0;

        public bool IsClosed => _items.Count == _currentIndex;

        public int RecordsAffected => -1;

        private readonly IList<T> _items;
        private List<string> _columnNames;
        private int _currentIndex;

        private OrderedDictionary FieldDictionary { get; }
        public DataReaderAdapter(IList<T> items)
        {
            _items = items;
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
            var currentItem = _items[_currentIndex];

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
            if (_currentIndex < _items.Count - 1)
            {
                _currentIndex++;
                return true;
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
