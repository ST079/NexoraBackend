
using Microsoft.EntityFrameworkCore;
using NexoraBackend.Application.Entities;

namespace NexoraBackend.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    // public DbSet<User> Users { get; set; }
    // public DbSet<Order> Orders { get; set; }
    public DbSet<ProductEntity> ProductEntity { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}