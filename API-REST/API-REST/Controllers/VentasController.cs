using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_REST.Models;
using API_REST.Models.DTOS;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VentasController : ControllerBase
    {
        private readonly DbVentasContext _context;

        public VentasController(DbVentasContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVentas(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] int? idVendedor,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            IQueryable<Venta> query = _context.Ventas
                .Include(v => v.IdvendedorNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.IdproNavigation);

            if (fechaInicio.HasValue)
                query = query.Where(v => v.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(v => v.Fecha <= fechaFin.Value.AddDays(1));

            if (idVendedor.HasValue)
                query = query.Where(v => v.Idvendedor == idVendedor.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var ventas = await query
                .OrderByDescending(v => v.Fecha)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    idventa = v.Idventa,
                    fecha = v.Fecha,
                    vendedor = v.IdvendedorNavigation.Nombre,
                    idvendedor = v.Idvendedor,
                    total = v.Total,
                    detalles = v.DetalleVenta.Select(d => new
                    {
                        idde = d.Idde,
                        producto = d.IdproNavigation.Producto1,
                        codigo = d.IdproNavigation.Codigo,
                        cantidad = d.Cantidad,
                        precio = d.Precio,
                        iva = d.Iva,
                        total = d.Total
                    }).ToList()
                })
                .ToListAsync();

            var result = new
            {
                totalItems,
                pageNumber,
                pageSize,
                totalPages,
                items = ventas
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.IdvendedorNavigation)
                .Include(v => v.DetalleVenta)
                    .ThenInclude(d => d.IdproNavigation)
                .FirstOrDefaultAsync(v => v.Idventa == id);

            if (venta == null)
                return NotFound(new { message = "Venta no encontrada" });

            var result = new
            {
                idventa = venta.Idventa,
                fecha = venta.Fecha,
                vendedor = venta.IdvendedorNavigation.Nombre,
                idvendedor = venta.Idvendedor,
                total = venta.Total,
                detalles = venta.DetalleVenta.Select(d => new
                {
                    idde = d.Idde,
                    idpro = d.Idpro,
                    producto = d.IdproNavigation.Producto1,
                    codigo = d.IdproNavigation.Codigo,
                    cantidad = d.Cantidad,
                    precio = d.Precio,
                    iva = d.Iva,
                    total = d.Total
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenta([FromBody] CreateVentaDTO createVentaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Usuario no autenticado" });

            var idVendedor = int.Parse(userIdClaim.Value);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar que todos los productos existan
                var productosIds = createVentaDto.Detalles.Select(d => d.Idpro).Distinct().ToList();
                var productos = await _context.Productos
                    .Where(p => productosIds.Contains(p.Idpro))
                    .ToDictionaryAsync(p => p.Idpro);

                if (productos.Count != productosIds.Count)
                    return BadRequest(new { message = "Uno o más productos no existen" });

                // Calcular el total de la venta
                decimal totalVenta = 0;
                var detallesVenta = new List<DetalleVenta>();

                foreach (var detalle in createVentaDto.Detalles)
                {
                    var producto = productos[detalle.Idpro];
                    var precio = producto.Precio;
                    var iva = precio * detalle.Cantidad * 0.13m; // 13% IVA
                    var total = (precio * detalle.Cantidad) + iva;

                    detallesVenta.Add(new DetalleVenta
                    {
                        Fecha = DateTime.Now,
                        Idpro = detalle.Idpro,
                        Cantidad = detalle.Cantidad,
                        Precio = precio,
                        Iva = iva,
                        Total = total
                    });

                    totalVenta += total;
                }

                // Crear la venta
                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    Idvendedor = idVendedor,
                    Total = totalVenta
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // Asignar el idventa a los detalles
                foreach (var detalle in detallesVenta)
                {
                    detalle.Idventa = venta.Idventa;
                }

                _context.DetalleVentas.AddRange(detallesVenta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Cargar los datos completos para la respuesta
                var ventaCreada = await _context.Ventas
                    .Include(v => v.IdvendedorNavigation)
                    .Include(v => v.DetalleVenta)
                        .ThenInclude(d => d.IdproNavigation)
                    .FirstAsync(v => v.Idventa == venta.Idventa);

                var result = new
                {
                    idventa = ventaCreada.Idventa,
                    fecha = ventaCreada.Fecha,
                    vendedor = ventaCreada.IdvendedorNavigation.Nombre,
                    idvendedor = ventaCreada.Idvendedor,
                    total = ventaCreada.Total,
                    detalles = ventaCreada.DetalleVenta.Select(d => new
                    {
                        idde = d.Idde,
                        idpro = d.Idpro,
                        producto = d.IdproNavigation.Producto1,
                        codigo = d.IdproNavigation.Codigo,
                        cantidad = d.Cantidad,
                        precio = d.Precio,
                        iva = d.Iva,
                        total = d.Total
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetVenta), new { id = venta.Idventa }, result);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error al crear la venta", detail = ex.Message });
            }
        }
    }
}
