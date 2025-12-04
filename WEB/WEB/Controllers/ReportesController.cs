using Microsoft.AspNetCore.Mvc;
using WEB.Models;
using WEB.Services;
using WEB.Filters;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace WEB.Controllers
{
    [RoleAuthorize("admin")]
    public class ReportesController : Controller
    {
        private readonly ApiService _apiService;

        public ReportesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken"));
        }

        public IActionResult Index()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VentasPorVendedor(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var endpoint = "Reportes/ventas-por-vendedor?";
            if (fechaInicio.HasValue)
                endpoint += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                endpoint += $"fechaFin={fechaFin.Value:yyyy-MM-dd}";

            var result = await _apiService.GetAsync<List<ReporteVentasPorVendedorViewModel>>(endpoint);

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View(result ?? new List<ReporteVentasPorVendedorViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> ProductosMasVendidos(DateTime? fechaInicio, DateTime? fechaFin, int top = 10)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var endpoint = $"Reportes/productos-mas-vendidos?top={top}&";
            if (fechaInicio.HasValue)
                endpoint += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                endpoint += $"fechaFin={fechaFin.Value:yyyy-MM-dd}";

            var result = await _apiService.GetAsync<List<ReporteProductosMasVendidosViewModel>>(endpoint);

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            ViewBag.Top = top;

            return View(result ?? new List<ReporteProductosMasVendidosViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> VentasPorFecha(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var endpoint = "Reportes/ventas-por-fecha?";
            if (fechaInicio.HasValue)
                endpoint += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                endpoint += $"fechaFin={fechaFin.Value:yyyy-MM-dd}";

            var result = await _apiService.GetAsync<List<ReporteVentasPorFechaViewModel>>(endpoint);

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View(result ?? new List<ReporteVentasPorFechaViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> ResumenGeneral(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var endpoint = "Reportes/resumen-general?";
            if (fechaInicio.HasValue)
                endpoint += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                endpoint += $"fechaFin={fechaFin.Value:yyyy-MM-dd}";

            var result = await _apiService.GetAsync<ResumenGeneralViewModel>(endpoint);

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View(result ?? new ResumenGeneralViewModel());
        }

        [HttpGet]
        public IActionResult ExportVentas()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExportVentasExcel(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var qs = "?pageSize=10000";
            if (fechaInicio.HasValue) 
                qs += "&fechaInicio=" + fechaInicio.Value.ToString("yyyy-MM-dd");
            if (fechaFin.HasValue) 
                qs += "&fechaFin=" + fechaFin.Value.ToString("yyyy-MM-dd");

            var ventasResp = await _apiService.GetAsync<VentaListResponse>("Ventas" + qs);
            var ventas = ventasResp?.Items ?? new List<VentaViewModel>();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Ventas");
            ws.Cell(1, 1).Value = "IdVenta";
            ws.Cell(1, 2).Value = "Fecha";
            ws.Cell(1, 3).Value = "Vendedor";
            ws.Cell(1, 4).Value = "Total";

            int row = 2;
            foreach (var v in ventas)
            {
                ws.Cell(row, 1).Value = v.Idventa;
                ws.Cell(row, 2).Value = v.Fecha.ToString("yyyy-MM-dd HH:mm");
                ws.Cell(row, 3).Value = v.Vendedor;
                ws.Cell(row, 4).Value = v.Total;
                row++;
            }

            var wsDet = wb.Worksheets.Add("Detalles");
            wsDet.Cell(1, 1).Value = "IdVenta";
            wsDet.Cell(1, 2).Value = "Codigo";
            wsDet.Cell(1, 3).Value = "Producto";
            wsDet.Cell(1, 4).Value = "Cantidad";
            wsDet.Cell(1, 5).Value = "Precio";
            wsDet.Cell(1, 6).Value = "IVA";
            wsDet.Cell(1, 7).Value = "Total";

            int r2 = 2;
            foreach (var v in ventas)
            {
                foreach (var d in v.Detalles)
                {
                    wsDet.Cell(r2, 1).Value = v.Idventa;
                    wsDet.Cell(r2, 2).Value = d.Codigo;
                    wsDet.Cell(r2, 3).Value = d.Producto;
                    wsDet.Cell(r2, 4).Value = d.Cantidad;
                    wsDet.Cell(r2, 5).Value = d.Precio;
                    wsDet.Cell(r2, 6).Value = d.Iva;
                    wsDet.Cell(r2, 7).Value = d.Total;
                    r2++;
                }
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            var bytes = ms.ToArray();
            var fileName = $"ventas_{(fechaInicio?.ToString("yyyyMMdd") ?? "start")}_{(fechaFin?.ToString("yyyyMMdd") ?? "end")}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportVentasPdf(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var qs = "?pageSize=10000";
            if (fechaInicio.HasValue) 
                qs += "&fechaInicio=" + fechaInicio.Value.ToString("yyyy-MM-dd");
            if (fechaFin.HasValue) 
                qs += "&fechaFin=" + fechaFin.Value.ToString("yyyy-MM-dd");

            var ventasResp = await _apiService.GetAsync<VentaListResponse>("Ventas" + qs);
            var ventas = ventasResp?.Items ?? new List<VentaViewModel>();

            using var ms = new MemoryStream();
            var doc = new PdfDocument();
            var page = doc.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;
            var gfx = XGraphics.FromPdfPage(page);
            var headerFont = new XFont("Verdana", 10, XFontStyle.Bold);
            var rowFont = new XFont("Verdana", 9, XFontStyle.Regular);

            double marginLeft = 40;
            double marginTop = 40;
            double marginRight = 40;
            double pageWidth = page.Width.Point - marginLeft - marginRight;
            double y = marginTop;

            // Title
            var title = $"Reporte de Ventas {fechaInicio?.ToString("yyyy-MM-dd") ?? ""} - {fechaFin?.ToString("yyyy-MM-dd") ?? ""}";
            gfx.DrawString(title, new XFont("Verdana", 12, XFontStyle.Bold), XBrushes.Black, 
                new XRect(marginLeft, y, pageWidth, 20), XStringFormats.TopLeft);
            y += 24;

            // Table columns: IdVenta, Fecha, Vendedor, Codigo, Producto, Cant, Precio, IVA, Total
            var cols = new[] { 50.0, 90.0, 110.0, 70.0, 160.0, 40.0, 60.0, 60.0, 60.0 };

            void DrawHeader()
            {
                double x = marginLeft;
                var headers = new[] { "Id", "Fecha", "Vendedor", "Código", "Producto", "Cant", "Precio", "IVA", "Total" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var rect = new XRect(x, y, cols[i], 20);
                    gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, rect);
                    gfx.DrawString(headers[i], headerFont, XBrushes.Black, rect, XStringFormats.Center);
                    x += cols[i];
                }
                y += 22;
            }

            DrawHeader();

            foreach (var v in ventas)
            {
                foreach (var d in v.Detalles)
                {
                    if (y > page.Height.Point - 80)
                    {
                        page = doc.AddPage();
                        page.Size = PdfSharpCore.PageSize.A4;
                        page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        y = marginTop;
                        DrawHeader();
                    }

                    double x = marginLeft;
                    var values = new[]
                    {
                        v.Idventa.ToString(),
                        v.Fecha.ToString("yyyy-MM-dd HH:mm"),
                        v.Vendedor,
                        d.Codigo,
                        d.Producto,
                        d.Cantidad.ToString(),
                        d.Precio.ToString("F2"),
                        d.Iva.ToString("F2"),
                        d.Total.ToString("F2")
                    };

                    for (int i = 0; i < values.Length; i++)
                    {
                        var rect = new XRect(x + 2, y, cols[i], 18);
                        gfx.DrawString(values[i], rowFont, XBrushes.Black, rect, XStringFormats.CenterLeft);
                        gfx.DrawRectangle(XPens.LightGray, new XRect(x, y - 2, cols[i], 20));
                        x += cols[i];
                    }
                    y += 20;
                }
            }

            doc.Save(ms);
            var bytes = ms.ToArray();
            var fileName = $"ventas_{(fechaInicio?.ToString("yyyyMMdd") ?? "start")}_{(fechaFin?.ToString("yyyyMMdd") ?? "end")}.pdf";
            return File(bytes, "application/pdf", fileName);
        }
    }
}
