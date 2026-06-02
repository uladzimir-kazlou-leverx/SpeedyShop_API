using Microsoft.AspNetCore.Mvc;
using SpeedyShop.Api.Services;

namespace SpeedyShop.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductWorkshopService productService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<object>> GetProducts([FromQuery] int take = 50)
    {
        take = Math.Clamp(take, 1, 200);
        return Ok(productService.GetProductsWithNPlusOne(take));
    }

    [HttpGet("popular")]
    public async Task<ActionResult<IReadOnlyList<object>>> GetPopular([FromQuery] int take = 25)
    {
        return Ok(await productService.GetPopularProductsAsync(Math.Clamp(take, 1, 100)));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<object>>> Search([FromQuery] string term = "Product 1")
    {
        return Ok(await productService.SearchInefficientlyAsync(term));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetLargeProductDetails(int id)
    {
        var result = await productService.GetLargeProductDetailsAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}