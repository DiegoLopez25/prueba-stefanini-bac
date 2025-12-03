using System;
using System.Collections.Generic;

namespace API_REST.Models;

public partial class TipoUsuario
{
    public int IdTipo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
