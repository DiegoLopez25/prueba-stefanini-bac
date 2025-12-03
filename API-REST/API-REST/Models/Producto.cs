using System;
using System.Collections.Generic;

namespace API_REST.Models;

public partial class Producto
{
    public int Idpro { get; set; }

    public string Codigo { get; set; } = null!;

    public string Producto1 { get; set; } = null!;

    public decimal Precio { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();
}
