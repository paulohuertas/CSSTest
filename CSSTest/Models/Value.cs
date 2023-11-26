using System.ComponentModel.DataAnnotations;

namespace CSSTest.Models
{
    public class Value
    {
        [Key]
        public int ValueId { get; set; }
        public DateTime ValueDate { get; set; }
        public double ValueDouble { get; set; }
        public int FundId { get; set; }
        public Fund Fund { get; set; }
        public Value() { }
    }
}
