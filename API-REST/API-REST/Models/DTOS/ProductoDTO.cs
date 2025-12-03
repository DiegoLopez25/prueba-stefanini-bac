namespace API_REST.Models.DTOS
{
    public class ProductoDTO
    {
        public int Idpro { get; set; }
        public string Codigo { get; set; } = null!;
        public string NombreProducto { get; set; } = null!;
        public decimal Precio { get; set; }
    }
}