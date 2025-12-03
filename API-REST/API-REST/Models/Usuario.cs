using System;
using System.Collections.Generic;

namespace API_REST.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdTipo { get; set; }

    public virtual TipoUsuario IdTipoNavigation { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
