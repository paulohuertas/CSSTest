using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CSSTest.Models
{
    public class Fund
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<Value> FundValues { get; set; }

        public Fund() { }

        public Fund(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
