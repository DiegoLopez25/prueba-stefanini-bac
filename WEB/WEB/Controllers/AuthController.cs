using Microsoft.AspNetCore.Mvc;
using WEB.Models;
using WEB.Services;

namespace WEB.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al home
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                var tipo = (HttpContext.Session.GetString("TipoUsuario") ?? string.Empty).ToLowerInvariant();
                if (tipo == "operador")
                    return RedirectToAction("Index", "Ventas");

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var (success, data, error) = await _apiService.PostAsync<LoginResponse>("Auth/login", model);

                if (success && data != null)
                {
                    HttpContext.Session.SetString("JWTToken", data.Token);
                    HttpContext.Session.SetString("Usuario", data.Usuario);
                    HttpContext.Session.SetString("Nombre", data.Nombre);
                    HttpContext.Session.SetString("TipoUsuario", data.TipoUsuario);

                    var tipo = (data.TipoUsuario ?? string.Empty).ToLowerInvariant();
                    if (tipo == "operador")
                        return RedirectToAction("Index", "Ventas");

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Error = error ?? "Usuario o contraseña incorrectos";
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Excepción: {ex.Message} | StackTrace: {ex.StackTrace}";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
