using CharitySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CharitySystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<Cloth> Clothes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        var converter = new BoolToZeroOneConverter<int>();
                        property.SetValueConverter(converter);
                    }
                }
            }

            // 1. Users Table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");
                entity.HasKey(e => e.UserID);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Fullname).IsRequired();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasConversion<string>();
                entity.Property(e => e.AccountStatus).HasConversion<string>();
                entity.Property(e => e.Gender).HasConversion<string>();
            });

            // 2. Families Table
            modelBuilder.Entity<Family>(entity =>
            {
                entity.ToTable("FAMILIES");
                entity.HasKey(e => e.FamilyID);
                entity.HasIndex(e => e.FamilyCode).IsUnique();
                entity.Property(e => e.FamilyCode).IsRequired().HasMaxLength(50);
                entity.HasOne(f => f.User)
                      .WithOne()
                      .HasForeignKey<Family>(f => f.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Family Members Table
            modelBuilder.Entity<FamilyMember>(entity =>
            {
                entity.ToTable("FAMILYMEMBERS");
                entity.HasKey(e => e.MemberID);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Gender).HasConversion<string>();
                entity.HasOne(m => m.Family)
                      .WithMany(f => f.Members)
                      .HasForeignKey(m => m.FamilyID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 4. Clothes Table
            modelBuilder.Entity<Cloth>(entity =>
            {
                entity.ToTable("CLOTHES");
                entity.HasKey(e => e.ClothID);
                entity.HasIndex(e => e.ClothCode).IsUnique();
                entity.Property(e => e.ClothCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ClothName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).HasConversion<string>();
            });

            // 5. Orders Table
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");
                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.OrderStatus).HasConversion<string>();
                entity.Property(e => e.OrderCode).IsRequired().HasMaxLength(50);
                entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(250);
                entity.HasOne(o => o.Family)
                      .WithMany(f => f.Orders)
                      .HasForeignKey(o => o.FamilyID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 6. Order Details Table
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("ORDERDETAILS");
                entity.HasKey(e => e.OrderDetailID);
                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(od => od.Cloth)
                      .WithMany()
                      .HasForeignKey(od => od.ClothID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}