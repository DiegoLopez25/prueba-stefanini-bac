using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WEB.Models;
using WEB.Filters;

namespace WEB.Controllers
{
    [RoleAuthorize("admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken"));
        }

        public IActionResult Index()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            ViewBag.Usuario = HttpContext.Session.GetString("Nombre");
            ViewBag.TipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            return View();
        }

        public IActionResult Privacy()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
