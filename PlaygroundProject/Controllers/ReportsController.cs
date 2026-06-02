using Microsoft.AspNetCore.Mvc;
using SpeedyShop.Api.Services;

namespace SpeedyShop.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(IReportWorkshopService reportService) : ControllerBase
{
    [HttpGet("product-quality")]
    [Produces("text/plain")]
    public async Task<IActionResult> ProductQuality()
    {
        return Content(await reportService.BuildAllocationHeavyReportAsync(), "text/plain");
    }
}