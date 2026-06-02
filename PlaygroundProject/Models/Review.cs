namespace SpeedyShop.Api.Models;

public sealed class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}