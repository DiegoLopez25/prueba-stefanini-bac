using Microsoft.AspNetCore.Mvc;
using WEB.Models;
using WEB.Services;

namespace WEB.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApiService _apiService;

        public VentasController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken"));
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var endpoint = $"Ventas?pageNumber={pageNumber}&pageSize={pageSize}";
            var result = await _apiService.GetAsync<VentaListResponse>(endpoint);
            
            return View(result ?? new VentaListResponse());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            // Obtener productos para el selector
            var productos = await _apiService.GetAsync<ProductoListResponse>("Productos?pageSize=1000");
            ViewBag.Productos = productos?.Items ?? new List<ProductoViewModel>();
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVentaViewModel model)
        {
            if (!IsAuthenticated())
                return Json(new { success = false, message = "No autenticado" });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos" });

            var (success, data, error) = await _apiService.PostAsync<VentaViewModel>("Ventas", model);

            if (success)
            {
                return Json(new { success = true, message = "Venta creada exitosamente" });
            }

            return Json(new { success = false, message = "Error al crear la venta: " + error });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var venta = await _apiService.GetAsync<VentaViewModel>($"Ventas/{id}");
            if (venta == null)
                return NotFound();

            return View(venta);
        }
    }
}
