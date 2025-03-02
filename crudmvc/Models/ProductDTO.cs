using System.ComponentModel.DataAnnotations;

namespace crudmvc.Models
{
    public class ProductDTO
    {
        [Required]
        public string Name { get; set; } = "";
        [Required]
        public string Brand { get; set; } = "";
        [Required]
        public string Category { get; set; } = "";
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = "";
        public IFormFile? ImageFile { get; set; }

    }
}
