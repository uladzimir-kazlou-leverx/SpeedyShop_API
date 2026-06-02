using Microsoft.EntityFrameworkCore;
using SpeedyShop.Api.Models;

namespace SpeedyShop.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SpeedyShopDbContext>();

        var databaseProvider = configuration.GetValue<string>("Database:Provider") ?? "Sqlite";
        if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            await db.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        if (!configuration.GetValue("Seed:RunOnStartup", true) || await db.Products.AnyAsync(cancellationToken)) return;

        var products = configuration.GetValue("Seed:Products", 50_000);
        var customers = configuration.GetValue("Seed:Customers", 20_000);
        var orders = configuration.GetValue("Seed:Orders", 100_000);
        var reviews = configuration.GetValue("Seed:Reviews", 500_000);
        var batch = configuration.GetValue("Seed:BatchSize", 2_000);
        var random = new Random(42);

        for (var i = 1; i <= customers; i += batch)
        {
            db.Customers.AddRange(Enumerable.Range(i, Math.Min(batch, customers - i + 1)).Select(id => new Customer
            {
                Id = id, Email = $"customer{id}@example.com", FullName = $"Customer {id}", AddressLine1 = $"{id} Main Street",
                City = $"City {id % 250}", Country = "US", CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 1000))
            }));
            await db.SaveChangesAsync(cancellationToken); db.ChangeTracker.Clear();
        }

        var lipsum = string.Join(' ', Enumerable.Repeat("Fast shipping premium durable eco-friendly limited edition product", 50));
        for (var i = 1; i <= products; i += batch)
        {
            db.Products.AddRange(Enumerable.Range(i, Math.Min(batch, products - i + 1)).Select(id => new Product
            {
                Id = id, Name = $"Product {id}", Sku = $"SKU-{id:000000}", Price = random.Next(5, 500), CategoryId = random.Next(1, 21),
                Description = lipsum,
                MetadataJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    color = id % 2 == 0 ? "red" : "blue",
                    warehouse = id % 15,
                    tags = Enumerable.Range(1, 25).Select(t => $"tag-{t}-{id}").ToArray()
                }),
                InternalNotes = string.Join(';', Enumerable.Range(1, 20).Select(n => $"internal-note-{n}-{Guid.NewGuid()}")),
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 1500)), UpdatedAt = DateTime.UtcNow
            }));
            await db.SaveChangesAsync(cancellationToken); db.ChangeTracker.Clear();
        }

        for (var i = 1; i <= orders; i += batch)
        {
            var orderBatch = Enumerable.Range(i, Math.Min(batch, orders - i + 1)).Select(id => new Order
            {
                Id = id, CustomerId = random.Next(1, customers + 1), CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 730)),
                Status = id % 7 == 0 ? "Returned" : "Completed", Total = random.Next(20, 2000), ShippingAddress = $"{id} Main Street",
                InternalFraudNotes = string.Join(' ', Enumerable.Repeat("fraud-score low manual-review none", 20))
            }).ToList();
            db.Orders.AddRange(orderBatch);
            db.OrderItems.AddRange(orderBatch.SelectMany(o => Enumerable.Range(1, random.Next(2, 6)).Select(_ => new OrderItem
            { OrderId = o.Id, ProductId = random.Next(1, products + 1), Quantity = random.Next(1, 4), UnitPrice = random.Next(5, 500) })));
            await db.SaveChangesAsync(cancellationToken); db.ChangeTracker.Clear();
        }

        for (var i = 1; i <= reviews; i += batch)
        {
            db.Reviews.AddRange(Enumerable.Range(i, Math.Min(batch, reviews - i + 1)).Select(id => new Review
            {
                Id = id, ProductId = random.Next(1, products + 1), CustomerId = random.Next(1, customers + 1), Rating = random.Next(1, 6),
                Title = $"Review {id}", Body = string.Join(' ', Enumerable.Repeat("Helpful review with lots of words and detail", 12)),
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 1000))
            }));
            await db.SaveChangesAsync(cancellationToken); db.ChangeTracker.Clear();
        }
    }
}