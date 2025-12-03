using System.ComponentModel.DataAnnotations;

namespace API_REST.Models.DTOS
{
    public class VentaDTO
    {
        public int? Idventa { get; set; }
        public DateTime? Fecha { get; set; }
        public int Idvendedor { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaDTO> Detalles { get; set; } = new List<DetalleVentaDTO>();
    }

    public class DetalleVentaDTO
    {
        [Required(ErrorMessage = "El ID del producto es requerido")]
        public int Idpro { get; set; }
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        [Required(ErrorMessage = "El precio es requerido")]
        public decimal Precio { get; set; }
        
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }

    public class CreateVentaDTO
    {
        [Required(ErrorMessage = "Los detalles de venta son requeridos")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<DetalleVentaDTO> Detalles { get; set; } = new List<DetalleVentaDTO>();
    }
}
