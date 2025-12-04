namespace WEB.Models
{
    public class ReporteVentasPorVendedorViewModel
    {
        public int IdVendedor { get; set; }
        public string NombreVendedor { get; set; } = null!;
        public int TotalVentas { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class ReporteProductosMasVendidosViewModel
    {
        public int Idpro { get; set; }
        public string Codigo { get; set; } = null!;
        public string NombreProducto { get; set; } = null!;
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
    }

    public class ReporteVentasPorFechaViewModel
    {
        public DateTime Fecha { get; set; }
        public int TotalVentas { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class ResumenGeneralViewModel
    {
        public int TotalVentas { get; set; }
        public decimal MontoTotalVentas { get; set; }
        public int TotalProductosVendidos { get; set; }
        public decimal PromedioVenta { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
