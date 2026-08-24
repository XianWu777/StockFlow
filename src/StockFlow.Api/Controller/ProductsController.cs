using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Products;

namespace StockFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(new[]
            {
               new
               {
                    Id=1,
                    Name="Product 1",
                    Price=10.99m
               } 
            });
        }
    }
}