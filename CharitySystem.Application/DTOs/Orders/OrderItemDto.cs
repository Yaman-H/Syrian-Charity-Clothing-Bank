using System.ComponentModel.DataAnnotations;

namespace CharitySystem.Application.DTOs.Orders
{
    public class OrderItemDto
    {
        [Required]
        public int ClothId { get; set; }
    }
}
