using System;
using System.Collections.Generic;

namespace API_REST.Models;

public partial class DetalleVenta
{
    public int Idde { get; set; }

    public DateTime Fecha { get; set; }

    public int Idventa { get; set; }

    public int Idpro { get; set; }

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public virtual Producto IdproNavigation { get; set; } = null!;

    public virtual Venta IdventaNavigation { get; set; } = null!;
}
