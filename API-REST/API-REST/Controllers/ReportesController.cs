using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_REST.Models;
using Microsoft.AspNetCore.Authorization;

namespace API_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly DbVentasContext _context;

        public ReportesController(DbVentasContext context)
        {
            _context = context;
        }

        [HttpGet("ventas-por-vendedor")]
        public async Task<IActionResult> GetVentasPorVendedor(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            IQueryable<Venta> query = _context.Ventas.Include(v => v.IdvendedorNavigation);

            if (fechaInicio.HasValue)
                query = query.Where(v => v.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(v => v.Fecha <= fechaFin.Value.AddDays(1));

            var reporte = await query
                .GroupBy(v => new { v.Idvendedor, v.IdvendedorNavigation.Nombre })
                .Select(g => new
                {
                    idVendedor = g.Key.Idvendedor,
                    nombreVendedor = g.Key.Nombre,
                    totalVentas = g.Count(),
                    montoTotal = g.Sum(v => v.Total)
                })
                .OrderByDescending(r => r.montoTotal)
                .ToListAsync();

            return Ok(reporte);
        }

        [HttpGet("productos-mas-vendidos")]
        public async Task<IActionResult> GetProductosMasVendidos(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] int top = 10)
        {
            IQueryable<DetalleVenta> query = _context.DetalleVentas
                .Include(d => d.IdproNavigation)
                .Include(d => d.IdventaNavigation);

            if (fechaInicio.HasValue)
                query = query.Where(d => d.IdventaNavigation.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(d => d.IdventaNavigation.Fecha <= fechaFin.Value.AddDays(1));

            var reporte = await query
                .GroupBy(d => new
                {
                    d.Idpro,
                    d.IdproNavigation.Codigo,
                    d.IdproNavigation.Producto1
                })
                .Select(g => new
                {
                    idpro = g.Key.Idpro,
                    codigo = g.Key.Codigo,
                    nombreProducto = g.Key.Producto1,
                    cantidadVendida = g.Sum(d => d.Cantidad),
                    totalVentas = g.Sum(d => d.Total)
                })
                .OrderByDescending(r => r.cantidadVendida)
                .Take(top)
                .ToListAsync();

            return Ok(reporte);
        }

        [HttpGet("ventas-por-fecha")]
        public async Task<IActionResult> GetVentasPorFecha(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            IQueryable<Venta> query = _context.Ventas;

            if (fechaInicio.HasValue)
                query = query.Where(v => v.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(v => v.Fecha <= fechaFin.Value.AddDays(1));

            var reporte = await query
                .GroupBy(v => v.Fecha.Date)
                .Select(g => new
                {
                    fecha = g.Key,
                    totalVentas = g.Count(),
                    montoTotal = g.Sum(v => v.Total)
                })
                .OrderByDescending(r => r.fecha)
                .ToListAsync();

            return Ok(reporte);
        }

        [HttpGet("resumen-general")]
        public async Task<IActionResult> GetResumenGeneral(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin)
        {
            IQueryable<Venta> ventasQuery = _context.Ventas;
            IQueryable<DetalleVenta> detallesQuery = _context.DetalleVentas
                .Include(d => d.IdventaNavigation);

            if (fechaInicio.HasValue)
            {
                ventasQuery = ventasQuery.Where(v => v.Fecha >= fechaInicio.Value);
                detallesQuery = detallesQuery.Where(d => d.IdventaNavigation.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                var fechaFinAjustada = fechaFin.Value.AddDays(1);
                ventasQuery = ventasQuery.Where(v => v.Fecha <= fechaFinAjustada);
                detallesQuery = detallesQuery.Where(d => d.IdventaNavigation.Fecha <= fechaFinAjustada);
            }

            var totalVentas = await ventasQuery.CountAsync();
            var montoTotalVentas = await ventasQuery.SumAsync(v => (decimal?)v.Total) ?? 0;
            var totalProductosVendidos = await detallesQuery.SumAsync(d => (int?)d.Cantidad) ?? 0;
            var promedioVenta = totalVentas > 0 ? montoTotalVentas / totalVentas : 0;

            var resumen = new
            {
                totalVentas,
                montoTotalVentas,
                totalProductosVendidos,
                promedioVenta,
                fechaInicio,
                fechaFin
            };

            return Ok(resumen);
        }
    }
}
