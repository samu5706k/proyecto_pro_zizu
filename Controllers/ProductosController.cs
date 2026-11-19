using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

[Authorize(Roles = "Administrador")]
public class ProductosController : Controller
{
    private readonly ProductoRepositorio _repo;

    public ProductosController(ProductoRepositorio repo)
    {
        _repo = repo;
    }

    // LISTAR Y BUSCAR
    public IActionResult Index(string? buscar)
    {
        var lista = string.IsNullOrWhiteSpace(buscar)
            ? _repo.ObtenerTodos()
            : _repo.Buscar(buscar);

        ViewBag.Buscar = buscar;

        return View(lista);
    }

    // MOSTRAR DETALLES
    public IActionResult Details(int id)
    {
        var producto = _repo.ObtenerPorId(id);

        if (producto is null)
            return NotFound();

        return View(producto);
    }

    // MOSTRAR REGISTRO
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Producto());
    }

    // GUARDAR PRODUCTO
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Producto producto)
    {
        producto.Nombre = producto.Nombre?.Trim()
            ?? string.Empty;

        producto.Categoria = producto.Categoria?.Trim()
            ?? string.Empty;

        if (!ModelState.IsValid)
            return View(producto);

        _repo.Agregar(producto);

        TempData["Exito"] =
            "Producto registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR EDICIÓN
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var producto = _repo.ObtenerPorId(id);

        if (producto is null)
            return NotFound();

        return View(producto);
    }

    // GUARDAR CAMBIOS
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Producto producto)
    {
        if (id != producto.Id)
            return NotFound();

        producto.Nombre = producto.Nombre?.Trim()
            ?? string.Empty;

        producto.Categoria = producto.Categoria?.Trim()
            ?? string.Empty;

        if (!ModelState.IsValid)
            return View(producto);

        if (!_repo.Actualizar(producto))
            return NotFound();

        TempData["Exito"] =
            "Producto actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR CONFIRMACIÓN
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var producto = _repo.ObtenerPorId(id);

        if (producto is null)
            return NotFound();

        return View(producto);
    }

    // ELIMINAR PRODUCTO
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!_repo.Eliminar(id))
            return NotFound();

        TempData["Exito"] =
            "Producto eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
