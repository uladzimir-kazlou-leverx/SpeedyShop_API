namespace SpeedyShop.Api.Services;

public interface IExternalCatalogClient
{
    Task<string> GetSupplierStatusAsync(int productId);
    Task<object> GetInventoryAsync();
    Task<object> GetPricingAsync();
    Task<object> GetRecommendationsAsync();
}