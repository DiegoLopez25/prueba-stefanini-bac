using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API_REST.Models;
using API_REST.Models.DTOS;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly DbVentasContext _context;

    public ProductosController(DbVentasContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductos([FromQuery] string? search, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        IQueryable<Producto> query = _context.Productos;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(p => p.Codigo.Contains(s) || p.Producto1.Contains(s) && p.DeletedAt == null);
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .OrderBy(p => p.Idpro)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductoDTO
            {
                Idpro = p.Idpro,
                Codigo = p.Codigo,
                NombreProducto = p.Producto1,
                Precio = p.Precio,
                SoftDelete = p.DeletedAt
            })
            .Where(p => p.SoftDelete == null)
            .ToListAsync();

        var result = new
        {
            totalItems,
            pageNumber,
            pageSize,
            totalPages,
            items
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
    {
        var producto = await _context.Productos.FindAsync(id);

        if (producto == null) return NotFound(new { mensaje = "El Producto no se ha encontrado" });

        var productoDTO = new ProductoDTO
            {
                Idpro = producto.Idpro,
                Codigo = producto.Codigo,
                NombreProducto = producto.Producto1,
                Precio = producto.Precio
            };
        return Ok(productoDTO);
    }

    [HttpPost]
    public async Task<ActionResult<ProductoDTO>> Post([FromBody] ProductoDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(dto.Codigo))
            return BadRequest(new { message = "El código es requerido." });

        var exists = await _context.Productos.AnyAsync(p => p.Codigo == dto.Codigo);
        if (exists)
            return Conflict(new { message = "El código ya existe." });


        var producto = new Producto
            {

                Codigo = dto.Codigo,
                Producto1 = dto.NombreProducto,
                Precio = dto.Precio
            };        

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        var productoDto = new ProductoDTO
        {
            Idpro = producto.Idpro,
            Codigo = producto.Codigo,
            NombreProducto = producto.Producto1,
            Precio = producto.Precio
        };  
        return CreatedAtAction(nameof(GetProducto), new { id = producto.Idpro }, productoDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] ProductoDTO dto)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound(new { mensaje = "El Producto no se ha encontrado en los registros" });
        
        // Verificar si el código ya existe en otro producto
        var exists = await _context.Productos.AnyAsync(p => p.Codigo == dto.Codigo && p.Idpro != id);
        if (exists)
            return Conflict(new { message = "El código ya existe en otro producto." });
        
        producto.Codigo = dto.Codigo;
        producto.Producto1 = dto.NombreProducto;
        producto.Precio = dto.Precio;
        try
        {
            await _context.SaveChangesAsync();
            return NoContent(); 
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { message = "Error interno del servidor", detail = "Ocurrió un error al guardar los cambios en la base de datos." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
       var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound(new { mensaje = "El Producto no se ha encontrado en los registros" });

        producto.DeletedAt = DateTime.UtcNow;
        try
        {
            await _context.SaveChangesAsync();
            return NoContent(); 
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
        }
    }

    
}