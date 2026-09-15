using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public required decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public required string SKU { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
