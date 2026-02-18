using CharitySystem.Application.DTOs.Orders;
using CharitySystem.Application.DTOs.Statistics;
using CharitySystem.Application.Interfaces;
using CharitySystem.Domain.Entities;
using CharitySystem.Domain.Enums;
using CharitySystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CharitySystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var families = await _unitOfWork.Families.FindAsync(f => f.UserID == userId);
            var family = families.FirstOrDefault();

            if (family == null)
                throw new Exception("No user related to this family!.");

            if(!string.IsNullOrWhiteSpace(dto.ShippingAddress))
                dto.ShippingAddress = family.FamilyAddress;

            string generatedCode = await GenerateOrderCode();
            var order = new Order
            {
                FamilyID = family.FamilyID,
                OrderCode = generatedCode,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Processing, 
                ShippingAddress = family.FamilyAddress,
                TotalPoints = 0 
            };

            int totalPointsCalc = 0;

            foreach (var item in dto.Items)
            {
                var cloth = await _unitOfWork.Clothes.GetByIdAsync(item.ClothId);

                if (cloth == null)
                    throw new Exception($"The cloth {item.ClothId} not found.");

                if (!cloth.IsAvailable)
                    throw new Exception($"Sorry، the cloth '{cloth.ClothName}' has just been sold.");

                if (family.FamilyPoints < (totalPointsCalc + cloth.ClothPoints))
                     throw new Exception($"No points enough!.");

                cloth.IsAvailable = false; 
                _unitOfWork.Clothes.Update(cloth);
                totalPointsCalc += cloth.ClothPoints;

                var orderDetail = new OrderDetail
                {
                    Order = order,
                    ClothID = cloth.ClothID,
                    Quantity = 1,
                    Points = cloth.ClothPoints
                };
                await _unitOfWork.OrderDetails.AddAsync(orderDetail);
            }

            order.TotalPoints = totalPointsCalc;
            family.FamilyPoints -= totalPointsCalc;
            
            _unitOfWork.Families.Update(family);
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return order.OrderID;
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
    
            var orders = await _unitOfWork.Orders.GetQueryable() 
                .Include(o => o.Family)
                .Include(o => o.OrderDetails) 
                    .ThenInclude(od => od.Cloth)
                .OrderByDescending(o => o.OrderDate) 
                .ToListAsync();

            var response = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderID,
                OrderCode = o.OrderCode,
                OrderDate = o.OrderDate,
                Status = o.OrderStatus, 
                FamilyCode = o.Family?.FamilyCode ?? "No family with this code!",
                ShippingAddress = o.ShippingAddress,
                Items = [.. o.OrderDetails.Select(d => new OrderDetailResponseDto
                {
                    ClothName = d.Cloth?.ClothName ?? "No cloth with this name!",
                    ClothCode = d.Cloth?.ClothCode ?? "No cloth with this code!",
                    Quantity = d.Quantity
                })]
            }).ToList();

            return response;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) 
                throw new Exception("Order not found");
            if (order.OrderStatus == OrderStatus.Delivered && newStatus != OrderStatus.Delivered)
            {
                 throw new Exception("Can't change order which delivered");
            }
            order.OrderStatus = newStatus;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var familiesCount = await _unitOfWork.Families.GetQueryable().CountAsync();

            var clothesCount = await _unitOfWork.Clothes.GetQueryable().CountAsync(c => c.IsAvailable); 

            var pendingOrders = await _unitOfWork.Orders.GetQueryable()
                .CountAsync(o => o.OrderStatus == OrderStatus.Processing);

            var completedOrders = await _unitOfWork.Orders.GetQueryable()
                .CountAsync(o => o.OrderStatus == OrderStatus.Delivered);

            return new DashboardStatsDto
            {
                TotalFamilies = familiesCount,
                TotalClothes = clothesCount,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders
            };
        }
        public async Task<string> GenerateOrderCode()
        {
            string generatedCode;
            bool isUnique = false;
            do 
            {
                int randomNum = new Random().Next(10000, 99999);
                generatedCode = $"OR-{randomNum}";
        
                var codeCount = await _unitOfWork.Families.GetQueryable()
                    .CountAsync(f => f.FamilyCode == generatedCode);
        
                if (codeCount == 0) isUnique = true;
            } while (!isUnique);

            return generatedCode;
        }
    }
}
