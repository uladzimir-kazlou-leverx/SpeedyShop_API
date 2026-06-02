namespace SpeedyShop.Api.Services;

public interface IProductWorkshopService
{
    IReadOnlyList<object> GetProductsWithNPlusOne(int take);
    Task<IReadOnlyList<object>> GetPopularProductsAsync(int take);
    Task<IReadOnlyList<object>> SearchInefficientlyAsync(string term);
    Task<object?> GetLargeProductDetailsAsync(int id);
}