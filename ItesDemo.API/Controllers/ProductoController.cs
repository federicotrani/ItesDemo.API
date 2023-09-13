using ItesDemo.API.Data;
using ItesDemo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItesDemo.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProductoController : ControllerBase
{
    private readonly ApiDbContext context;

    public ProductoController(ApiDbContext context)
    {
        this.context = context;
    }
  
    [HttpGet]
    public async Task<ActionResult<List<Producto>>> GetProductsAsync()
    {
        var products = await context.Productos.ToListAsync();

        if (products != null)
        {
            return Ok(products);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<Producto>>> GetProductByIdAsync(int id)
    {
        var product = await context.Productos.Where(x => x.id == id).FirstOrDefaultAsync();

        if (product != null)
        {
            return Ok(product);
        }
        else
        {
            return NotFound();
        }
    }


}