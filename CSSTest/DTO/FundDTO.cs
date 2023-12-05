using System.ComponentModel.DataAnnotations;

namespace CSSTest.DTO
{
    public class FundDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public FundDTO() { }

        public FundDTO(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
