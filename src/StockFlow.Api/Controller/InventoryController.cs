

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockFlow.Application.Inventory;

namespace StockFlow.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryService.GetAllAsync(cancellationToken);

        return Ok(inventorys);
    }

    [HttpGet("InventoryMovements")]
    public async Task<IActionResult> GetMovementsAsync(CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryService.GetMovementsAsync(cancellationToken);

        return Ok(inventorys);
    }

    [HttpGet("{productId}/movements")]
    public async Task<IActionResult> GetInventoryMovementsAsync(
        Guid productId,
        [FromQuery] GetInventoryMovementsQuery query,
        CancellationToken cancellationToken)
    {
        var inventorys = await _inventoryService.GetMovementsByProductIdAsync(productId, query, cancellationToken);
        Console.WriteLine($"inventorys : {JsonConvert.SerializeObject(query)} , {JsonConvert.SerializeObject(inventorys)}");
        return Ok(inventorys);
    }

    [HttpPost("{productId}/stock-in")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> StockIn(Guid productId, StockInRequest request)
    {
        Console.WriteLine($"Try to StockIn {productId}");
        await _inventoryService.StockInAsync(productId, request, CancellationToken.None);

        return Ok();
    }

    [HttpPost("{productId}/stock-out")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> StockOut(Guid productId, StockOutRequest request)
    {
        Console.WriteLine($"Try to StockOut {productId}");
        await _inventoryService.StockOutAsync(productId, request, CancellationToken.None);

        return Ok();
    }
}