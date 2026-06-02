namespace SpeedyShop.Api.Services;

public interface IOrderWorkshopService
{
    Task<object?> GetOrderDetailsChattyAsync(int orderId);
    Task<IReadOnlyList<object>> GetHistoryLargePayloadAsync(int customerId);
    IReadOnlyList<object> GetRecentOrders(DateTime? since);
}