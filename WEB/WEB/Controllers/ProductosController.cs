using Microsoft.AspNetCore.Mvc;
using WEB.Models;
using WEB.Services;
using WEB.Filters;

namespace WEB.Controllers
{
    [RoleAuthorize("admin")]
    public class ProductosController : Controller
    {
        private readonly ApiService _apiService;

        public ProductosController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken"));
        }

        public async Task<IActionResult> Index(string? search, int pageNumber = 1, int pageSize = 10)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var endpoint = $"Productos?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                endpoint += $"&search={Uri.EscapeDataString(search)}";

            var result = await _apiService.GetAsync<ProductoListResponse>(endpoint);
            
            ViewBag.Search = search;
            return View(result ?? new ProductoListResponse());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductoViewModel model)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var (success, data, error) = await _apiService.PostAsync<ProductoViewModel>("Productos", model);

            if (success)
            {
                TempData["Success"] = "Producto creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "Error al crear el producto: " + error;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var producto = await _apiService.GetAsync<ProductoViewModel>($"Productos/{id}");
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductoViewModel model)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var (success, data, error) = await _apiService.PutAsync<ProductoViewModel>($"Productos/{id}", model);

            if (success)
            {
                TempData["Success"] = "Producto actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "Error al actualizar el producto: " + error;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAuthenticated())
                return RedirectToAction("Login", "Auth");

            var (success, error) = await _apiService.DeleteAsync($"Productos/{id}");

            if (success)
            {
                TempData["Success"] = "Producto eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el producto: " + error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
