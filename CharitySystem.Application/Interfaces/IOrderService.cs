using CharitySystem.Application.DTOs.Orders;
using CharitySystem.Application.DTOs.Statistics;
using CharitySystem.Domain.Enums;

namespace CharitySystem.Application.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
