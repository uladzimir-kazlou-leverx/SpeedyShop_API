namespace SpeedyShop.Api.Models;

public sealed class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string InternalFraudNotes { get; set; } = string.Empty;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}