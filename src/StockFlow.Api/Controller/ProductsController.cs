using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Products;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(ProductService productService,
        IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("GetAllAsync")]
    public async Task<IActionResult> GetAllAsync()
    {
        var a = await _productService.GetAllAsync(CancellationToken.None);
        return Ok(a);
    }

    [HttpGet("GetAllQueryAsync")]
    public async Task<IActionResult> GetAllQueryAsync(
        [FromQuery] GetProductsQuery query)
    {
        var a = await _productService.GetAllQueryAsync(query, CancellationToken.None);
        return Ok(a);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Description = "Created")]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Bad Request")]
    public async Task<IActionResult> Create(
            [FromBody] CreateProductRequest request,
            CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productService.UpdateAsync(id, request, cancellationToken);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}