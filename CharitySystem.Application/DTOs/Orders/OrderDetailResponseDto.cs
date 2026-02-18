using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharitySystem.Application.DTOs.Orders
{
    public class OrderDetailResponseDto
    {
        public string ClothName { get; set; }
        public string ClothCode { get; set; }
        public int Quantity { get; set; }
    }
}
