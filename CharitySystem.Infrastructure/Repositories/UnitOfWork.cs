using CharitySystem.Domain.Interfaces;
using CharitySystem.Infrastructure.Data;
using CharitySystem.Domain.Entities;

namespace CharitySystem.Infrastructure.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            Families = new GenericRepository<Family>(_context);
            FamilyMembers = new GenericRepository<FamilyMember>(_context);
            Clothes = new GenericRepository<Cloth>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderDetails = new GenericRepository<OrderDetail>(_context);
        } 

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Family> Families { get; private set; }
        public IGenericRepository<FamilyMember> FamilyMembers { get; private set; }
        public IGenericRepository<Cloth> Clothes { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<OrderDetail> OrderDetails { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public void Dispose() 
        {
            _context.Dispose();
        }
    }
}
