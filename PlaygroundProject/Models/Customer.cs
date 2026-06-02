namespace SpeedyShop.Api.Models;

public sealed class Customer
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}