using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCRUD.Data;
using ProductCRUD.Model;

namespace ProductCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductAPIContext _context;

        public ProductsController(ProductAPIContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            var Products = await _context.GetAllProducts();
            return Ok(Products);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            //var product = await _context.GetProductById(id);
            var productSingle = await _context.GetAllProducts();
            var product = productSingle.Where(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            product.Id = id;
            await _context.UpdateProduct(product);

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            await _context.AddProduct(product);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _context.DeleteProduct(id);

            return NoContent();
        }

    }
}
