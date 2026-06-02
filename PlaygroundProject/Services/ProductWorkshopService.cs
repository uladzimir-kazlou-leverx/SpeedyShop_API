using Microsoft.EntityFrameworkCore;
using SpeedyShop.Api.Data;

namespace SpeedyShop.Api.Services;

public sealed class ProductWorkshopService(SpeedyShopDbContext db, IExternalCatalogClient externalCatalogClient) : IProductWorkshopService
{
    public IReadOnlyList<object> GetProductsWithNPlusOne(int take)
    {
        // WORKSHOP: Performance Issue - blocking call in frequently used product listing endpoint.
        Thread.Sleep(200);

        // WORKSHOP: Performance Issue - over-fetching full entities with huge Description, MetadataJson, InternalNotes, audit fields.
        var products = db.Products.OrderByDescending(p => p.CreatedAt).Take(take).ToList();
        var response = new List<object>();

        foreach (var product in products)
        {
            // WORKSHOP: Performance Issue - N+1 queries: category loaded separately for every product.
            var category = db.Categories.Single(c => c.Id == product.CategoryId);

            // WORKSHOP: Performance Issue - N+1 queries: reviews loaded separately for every product.
            var reviews = db.Reviews.Where(r => r.ProductId == product.Id).OrderByDescending(r => r.CreatedAt).Take(10).ToList();

            // WORKSHOP: Performance Issue - sync-over-async inside request processing blocks worker threads.
            var supplierStatus = externalCatalogClient.GetSupplierStatusAsync(product.Id).Result;

            response.Add(new { Product = product, Category = category, Reviews = reviews, SupplierStatus = supplierStatus });
        }

        return response;
    }

    public async Task<IReadOnlyList<object>> GetPopularProductsAsync(int take)
    {
        // WORKSHOP: Performance Issue - expensive aggregate is recomputed for every request; no IMemoryCache or Redis cache.
        return await db.OrderItems
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, UnitsSold = g.Sum(i => i.Quantity), Revenue = g.Sum(i => i.Quantity * i.UnitPrice) })
            .OrderByDescending(x => x.UnitsSold)
            .Take(take)
            .Join(db.Products, x => x.ProductId, p => p.Id, (x, p) => new
            {
                Product = p,
                x.UnitsSold,
                x.Revenue,
                ReviewCount = db.Reviews.Count(r => r.ProductId == p.Id)
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<object>> SearchInefficientlyAsync(string term)
    {
        // WORKSHOP: Performance Issue - inefficient LINQ: materializes first, filters in memory, uses Count() > 0.
        var allProducts = await db.Products.ToListAsync();
        var filtered = allProducts.Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
        if (filtered.Count() > 0)
        {
            var categories = await db.Categories.ToListAsync();
            return filtered.Select(p => new { Product = p, Category = categories.Where(c => c.Id == p.CategoryId).FirstOrDefault() }).Cast<object>().ToList();
        }

        return Array.Empty<object>();
    }

    public async Task<object?> GetLargeProductDetailsAsync(int id)
    {
        // WORKSHOP: Performance Issue - large payload response and over-fetching full related entities.
        var product = await db.Products.FindAsync(id);
        if (product is null) return null;

        var reviews = db.Reviews.Where(r => r.ProductId == id).Take(500).ToList();
        var orderItems = db.OrderItems.Where(i => i.ProductId == id).Take(500).ToList();
        return new { Product = product, Reviews = reviews, OrderItems = orderItems };
    }
}