using Microsoft.AspNetCore.Mvc;
using SpeedyShop.Api.Services;

namespace SpeedyShop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderWorkshopService orderService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetOrder(int id)
    {
        var result = await orderService.GetOrderDetailsChattyAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("customer/{customerId:int}/history")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetHistory(int customerId)
    {
        return Ok(await orderService.GetHistoryLargePayloadAsync(customerId));
    }

    [HttpGet("recent")]
    public ActionResult<object> GetRecent([FromQuery] DateTime? since = null)
    {
        return Ok(orderService.GetRecentOrders(since));
    }
}