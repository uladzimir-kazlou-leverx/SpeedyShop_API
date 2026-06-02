using Microsoft.EntityFrameworkCore;
using SpeedyShop.Api.Data;

namespace SpeedyShop.Api.Services;

public sealed class OrderWorkshopService(SpeedyShopDbContext db) : IOrderWorkshopService
{
    public async Task<object?> GetOrderDetailsChattyAsync(int orderId)
    {
        // WORKSHOP: Performance Issue - chatty database access: separate query for order.
        var order = await db.Orders.SingleOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return null;

        // WORKSHOP: Performance Issue - separate query for customer instead of Include/projection.
        var customer = await db.Customers.SingleAsync(c => c.Id == order.CustomerId);

        // WORKSHOP: Performance Issue - separate query for items.
        var items = await db.OrderItems.Where(i => i.OrderId == orderId).ToListAsync();
        var productDetails = new List<object>();
        foreach (var item in items)
        {
            // WORKSHOP: Performance Issue - N+1 product lookups inside order details.
            var product = await db.Products.SingleAsync(p => p.Id == item.ProductId);
            productDetails.Add(new { Item = item, Product = product });

            // WORKSHOP: Performance Issue - multiple SaveChanges calls create unnecessary roundtrips.
            order.InternalFraudNotes += $" viewed-product-{product.Id}";
            await db.SaveChangesAsync();
        }

        return new { Order = order, Customer = customer, Items = productDetails };
    }

    public async Task<IReadOnlyList<object>> GetHistoryLargePayloadAsync(int customerId)
    {
        // WORKSHOP: Performance Issue - large payload response: full orders, full items, full products with internal fields.
        return await db.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(100)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Cast<object>()
            .ToListAsync();
    }

    public IReadOnlyList<object> GetRecentOrders(DateTime? since)
    {
        // WORKSHOP: Performance Issue - missing indexes on CreatedAt and CustomerId make this slow on large data.
        var start = since ?? DateTime.UtcNow.AddDays(-30);
        return db.Orders.Where(o => o.CreatedAt >= start).OrderByDescending(o => o.CreatedAt).Take(100).Cast<object>().ToList();
    }
}