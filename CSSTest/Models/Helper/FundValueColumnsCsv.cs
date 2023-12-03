using LINQtoCSV;

namespace CSSTest.Models.Helper
{
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
}
