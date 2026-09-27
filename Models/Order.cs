using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShopApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string? UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
