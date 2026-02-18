using CharitySystem.Domain.Entities;

namespace CharitySystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Family> Families { get; }
        IGenericRepository<FamilyMember> FamilyMembers { get; }
        IGenericRepository<Cloth> Clothes { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderDetail> OrderDetails { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
    }
}
