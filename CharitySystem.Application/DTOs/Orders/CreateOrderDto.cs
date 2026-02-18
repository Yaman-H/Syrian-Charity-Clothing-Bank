using System.ComponentModel.DataAnnotations;

namespace CharitySystem.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string? ShippingAddress { get; set; } 
        [Required]
        public List<OrderItemDto> Items { get; set; } 
    }
}
