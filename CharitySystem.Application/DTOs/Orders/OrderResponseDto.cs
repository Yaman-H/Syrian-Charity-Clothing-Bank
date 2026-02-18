using CharitySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharitySystem.Application.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string? FamilyCode { get; set; } 
        public string? ShippingAddress { get; set; }
        public int TotalPoints { get; set; }
        public List<OrderDetailResponseDto> Items { get; set; }
    }
}
