using System.ComponentModel.DataAnnotations;

namespace API_REST.Models.DTOS
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        public string Usuario { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = null!;
    }
}
