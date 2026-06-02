using Microsoft.EntityFrameworkCore;
using SpeedyShop.Api.Data;

namespace SpeedyShop.Api.Services;

public sealed class ReportWorkshopService(SpeedyShopDbContext db) : IReportWorkshopService
{
    public async Task<string> BuildAllocationHeavyReportAsync()
    {
        var products = await db.Products.Take(2500).ToListAsync();
        var reviews = await db.Reviews.Take(10000).ToListAsync();

        // WORKSHOP: Performance Issue - excessive memory allocations: repeated string concatenation and unnecessary collections.
        var report = "SpeedyShop Product Quality Report\n";
        foreach (var product in products)
        {
            var productReviews = reviews.Where(r => r.ProductId == product.Id).ToList();
            var ratings = productReviews.Select(r => r.Rating).ToList();
            var average = ratings.Count > 0 ? ratings.Average() : 0;
            report += product.Id + "," + product.Name + "," + product.Sku + "," + average + "," + product.Description.Substring(0, Math.Min(120, product.Description.Length)) + "\n";
        }

        return report;
    }
}