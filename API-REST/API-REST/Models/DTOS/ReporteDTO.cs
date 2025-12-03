namespace API_REST.Models.DTOS
{
    public class ReporteVentasPorVendedorDTO
    {
        public int IdVendedor { get; set; }
        public string NombreVendedor { get; set; } = null!;
        public int TotalVentas { get; set; }
        public decimal MontoTotal { get; set; }
    }

    public class ReporteProductosMasVendidosDTO
    {
        public int Idpro { get; set; }
        public string Codigo { get; set; } = null!;
        public string NombreProducto { get; set; } = null!;
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
    }

    public class ReporteVentasPorFechaDTO
    {
        public DateTime Fecha { get; set; }
        public int TotalVentas { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
