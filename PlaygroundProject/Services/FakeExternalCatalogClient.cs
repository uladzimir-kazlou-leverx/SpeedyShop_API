namespace SpeedyShop.Api.Services;

public sealed class FakeExternalCatalogClient : IExternalCatalogClient
{
    public async Task<string> GetSupplierStatusAsync(int productId)
    {
        await Task.Delay(90);
        return $"Supplier status for product {productId}: in-stock";
    }

    public async Task<object> GetInventoryAsync()
    {
        await Task.Delay(250);
        return new { Warehouses = 12, LowStockSkus = 219, GeneratedAt = DateTimeOffset.UtcNow };
    }

    public async Task<object> GetPricingAsync()
    {
        await Task.Delay(300);
        return new { ActivePromotions = 37, Currency = "USD", GeneratedAt = DateTimeOffset.UtcNow };
    }

    public async Task<object> GetRecommendationsAsync()
    {
        await Task.Delay(350);
        return new { Models = 4, TrendingCategories = new[] { "Home", "Electronics", "Outdoors" }, GeneratedAt = DateTimeOffset.UtcNow };
    }
}