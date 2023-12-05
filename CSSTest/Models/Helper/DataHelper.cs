using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Data;
using ClosedXML.Excel;
using CSSTest.Data;
using LINQtoCSV;
using System.Reflection;
using CSSTest.Interfaces;

namespace CSSTest.Models.Helper
{
    public class DataHelper : IFund
    {
        public static Random random = new Random();

        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public IConfiguration _configuration { get { return _config; } }
        public DataHelper() { }

        public DataHelper(IConfiguration configuration, ApplicationDbContext context)
        {
            _config = configuration;
            _context = context;
        }
        public List<Fund> CreateFunds(int start = 1, int finish = 5)
        {
            var list = Enumerable.Range(start, finish).Select(f => new Fund
            {
                Name = CreateFullName(random.Next(3)),
                Description = CreateRandomDescription()
            }).ToList();

            var funds = new DataReaderAdapter<Fund>(list);

            DataHelper dataHelper = new DataHelper(_config, _context);

            this.InsertFundToDb(funds);

            this.CreateFundValues(1, 1000);

            return list;
        }
        public void CreateFundValues(int start, int finish)
        {
            while (start < finish)
            {
                int size = _context.Funds.Count();
                int rdn = random.Next(96, size);
                var f = _context.Funds.Where(f => f.Id == rdn).FirstOrDefault();

                if (f != null)
                {
                    Value value = new Value
                    {
                        ValueDate = DateTime.Now,
                        ValueDouble = Math.Round(random.NextDouble() * random.Next(100000), 2),
                        FundId = f.Id
                    };

                    var values = new DataReaderAdapter<Value>(value);

                    this.InsertValuesToDb(values);
                }

                start++;
            }
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

        public int GetFundIdRandomly()
        {
            int size = _context.Funds.Count();
            int rdn = random.Next(size);
            int id = _context.Funds.Where(f => f.Id == rdn).Select(f => f.Id).FirstOrDefault();
            return id;
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
            string path = Directory.GetCurrentDirectory();
            string savePath = System.IO.Path.Combine(path + ".csv");


            var query = from f in _context.Funds
                        join val in _context.Value on f.Id equals val.FundId
                        orderby f.Id
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
}
