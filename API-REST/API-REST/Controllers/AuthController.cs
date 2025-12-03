using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API_REST.Models;
using API_REST.Models.DTOS;
using API_REST.Services;

namespace API_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DbVentasContext _context;
        private readonly JwtService _jwtService;

        public AuthController(DbVentasContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _context.Usuarios
                .Include(u => u.IdTipoNavigation)
                .FirstOrDefaultAsync(u => u.Usuario1 == loginDto.Usuario);

            if (usuario == null)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            if (!PasswordHasher.VerifyPassword(loginDto.Password, usuario.Password))
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            var token = _jwtService.GenerateToken(usuario, usuario.IdTipoNavigation.Nombre);

            var response = new LoginResponseDTO
            {
                Token = token,
                Usuario = usuario.Usuario1,
                Nombre = usuario.Nombre,
                TipoUsuario = usuario.IdTipoNavigation.Nombre
            };

            return Ok(response);
        }
    }
}
