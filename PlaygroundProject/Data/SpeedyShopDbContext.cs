using Microsoft.EntityFrameworkCore;
using SpeedyShop.Api.Models;

namespace SpeedyShop.Api.Data;

public sealed class SpeedyShopDbContext(DbContextOptions<SpeedyShopDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(8000);
        modelBuilder.Entity<Product>().Property(p => p.MetadataJson).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<Order>().Property(o => o.Total).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderItem>().Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Review>().Property(r => r.Body).HasMaxLength(4000);

        modelBuilder.Entity<Category>().HasData(
            Enumerable.Range(1, 20).Select(i => new Category { Id = i, Name = $"Category {i}", Description = $"Department {i}" }));

        // WORKSHOP: Performance Issue - intentionally no indexes for CustomerId, ProductId, or CreatedAt on hot tables.
        // Participants should add indexes such as Orders(CustomerId, CreatedAt), Reviews(ProductId), and Products(CreatedAt).
    }
}