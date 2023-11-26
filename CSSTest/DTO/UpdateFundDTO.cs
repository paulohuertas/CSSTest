using System.ComponentModel.DataAnnotations;

namespace CSSTest.DTO
{
    public class UpdateFundDTO
    {
        [Required(ErrorMessage = "Field is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}
