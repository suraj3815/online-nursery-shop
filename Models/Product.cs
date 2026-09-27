using System.ComponentModel.DataAnnotations;

namespace PlantShopApp.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "Indoor";

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public string ImageUrl { get; set; } = "https://images.unsplash.com/photo-1596547609652-9cb5b4d45d5a?w=500";
    }
}
