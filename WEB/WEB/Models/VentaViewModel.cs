namespace WEB.Models
{
    public class VentaViewModel
    {
        public int Idventa { get; set; }
        public DateTime Fecha { get; set; }
        public string Vendedor { get; set; } = null!;
        public int Idvendedor { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaViewModel> Detalles { get; set; } = new();
    }

    public class DetalleVentaViewModel
    {
        public int Idde { get; set; }
        public int Idpro { get; set; }
        public string Producto { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }

    public class CreateVentaViewModel
    {
        public List<DetalleVentaCreateViewModel> Detalles { get; set; } = new();
    }

    public class DetalleVentaCreateViewModel
    {
        public int Idpro { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }

    public class VentaListResponse
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<VentaViewModel> Items { get; set; } = new();
    }
}
