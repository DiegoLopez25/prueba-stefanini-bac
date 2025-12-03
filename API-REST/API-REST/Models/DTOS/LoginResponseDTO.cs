namespace API_REST.Models.DTOS
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string TipoUsuario { get; set; } = null!;
    }
}
