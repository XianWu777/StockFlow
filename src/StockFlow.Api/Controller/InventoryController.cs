

using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Inventory;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var inventorys = await _inventoryService.GetAllAsync(CancellationToken.None);

        return Ok(inventorys);
    }

    [HttpGet("InventoryMovements")]
    public async Task<IActionResult> GetInventoryMovementsAsync()
    {
        var inventorys = await _inventoryService.GetMovementsAsync(CancellationToken.None);

        return Ok(inventorys);
    }

    [HttpPost("{productId}/stock-in-test")]
    public async Task<IActionResult> StockInTest(Guid productId, StockInRequest request)
    {
        Console.WriteLine($"Try to StockIn Test {productId}");

        await _inventoryService.StockInAsync(productId, request, CancellationToken.None);

        return Ok();
    }

    [HttpPost("{productId}/stock-in")]
    public async Task<IActionResult> StockIn(Guid productId, StockInRequest request)
    {
        Console.WriteLine($"Try to StockIn {productId}");

        await _inventoryService.StockInAsync(productId, request, CancellationToken.None);

        return Ok();
    }

    [HttpPost("{productId}/stock-out")]
    public async Task<IActionResult> StockOut(Guid productId, StockOutRequest request)
    {
        Console.WriteLine($"Try to StockOut {productId}");

        await _inventoryService.StockOutAsync(productId, request, CancellationToken.None);

        return Ok();
    }
}