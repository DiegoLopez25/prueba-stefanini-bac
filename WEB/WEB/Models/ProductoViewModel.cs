namespace WEB.Models
{
    public class ProductoViewModel
    {
        public int Idpro { get; set; }
        public string Codigo { get; set; } = null!;
        public string NombreProducto { get; set; } = null!;
        public decimal Precio { get; set; }
    }

    public class ProductoListResponse
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<ProductoViewModel> Items { get; set; } = new();
    }
}
