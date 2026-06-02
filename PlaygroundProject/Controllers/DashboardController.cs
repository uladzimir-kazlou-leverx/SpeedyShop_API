using Microsoft.AspNetCore.Mvc;
using SpeedyShop.Api.Services;

namespace SpeedyShop.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IExternalCatalogClient externalCatalogClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<object>> GetDashboard()
    {
        // WORKSHOP: Performance Issue - sequential external requests; replace with Task.WhenAll.
        var inventory = await externalCatalogClient.GetInventoryAsync();
        var pricing = await externalCatalogClient.GetPricingAsync();
        var recommendations = await externalCatalogClient.GetRecommendationsAsync();
        return Ok(new { Inventory = inventory, Pricing = pricing, Recommendations = recommendations });
    }
}