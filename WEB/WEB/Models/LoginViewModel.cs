namespace WEB.Models
{
    public class LoginViewModel
    {
        public string Usuario { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string Usuario { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string TipoUsuario { get; set; } = null!;
    }
}
