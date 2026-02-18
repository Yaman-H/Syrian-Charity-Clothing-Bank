using CharitySystem.Application.DTOs.Orders;
using CharitySystem.Application.Interfaces;
using CharitySystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CharitySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("User ID was not found in the token.");
            }

            int id = int.Parse(userId);
            try
            {
                var orderId = await _orderService.CreateOrderAsync(id, dto);
                return Ok(new { message = "Order was successfully created", orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all-orders")] 
        [Authorize(Roles = "WarehouseManager, Admin")] 
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("manager/update-status/{id}")]
        [Authorize(Roles = "WarehouseManager, Admin")] 
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int status) 
        {
            try 
            {
                await _orderService.UpdateOrderStatusAsync(id, (OrderStatus)status);
                return Ok(new { message = "Order status has been successfully updated." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
