using System;
using System.Collections.Generic;

namespace API_REST.Models;

public partial class Venta
{
    public int Idventa { get; set; }

    public DateTime Fecha { get; set; }

    public int Idvendedor { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Usuario IdvendedorNavigation { get; set; } = null!;
}
