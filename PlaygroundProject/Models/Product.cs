namespace SpeedyShop.Api.Models;

public sealed class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string MetadataJson { get; set; } = "{}";
    public string InternalNotes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = "seed";
    public string UpdatedBy { get; set; } = "seed";
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}