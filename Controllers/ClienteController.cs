
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

public class ClienteController : Controller
{
    private readonly ClienteRepositorio _repo;

    public ClienteController(ClienteRepositorio repo)
    {
        _repo = repo;
    }

    // READ: mostrar y buscar clientes
    public IActionResult Index(string? buscar)
    {
        var lista = string.IsNullOrWhiteSpace(buscar)
            ? _repo.ObtenerTodos()
            : _repo.Buscar(buscar);

        ViewBag.Buscar = buscar;
        return View(lista);
    }

    // READ: ver un cliente
    public IActionResult Details(int id)
    {
        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    // CREATE: mostrar formulario
    public IActionResult Create()
    {
        return View(new Cliente());
    }

    // CREATE: guardar cliente
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Cliente cliente)
    {
        cliente.Ci = cliente.Ci?.Trim() ?? "";
        cliente.Email = cliente.Email?.Trim() ?? "";

        if (_repo.ExisteCi(cliente.Ci))
        {
            ModelState.AddModelError(
                nameof(Cliente.Ci),
                "Ya existe un cliente con ese CI.");
        }

        if (!ModelState.IsValid)
            return View(cliente);

        _repo.Agregar(cliente);

        TempData["Exito"] =
            $"Cliente {cliente.NombreCompleto} registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // UPDATE: mostrar formulario de edición
    public IActionResult Edit(int id)
    {
        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    // UPDATE: guardar cambios
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
            return NotFound();

        cliente.Ci = cliente.Ci?.Trim() ?? "";
        cliente.Email = cliente.Email?.Trim() ?? "";

        if (_repo.ExisteCi(cliente.Ci, cliente.Id))
        {
            ModelState.AddModelError(
                nameof(Cliente.Ci),
                "Otro cliente ya tiene ese CI.");
        }

        if (!ModelState.IsValid)
            return View(cliente);

        if (!_repo.Actualizar(cliente))
            return NotFound();

        TempData["Exito"] =
            $"Los datos de {cliente.NombreCompleto} se actualizaron correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // DELETE: mostrar confirmación
    public IActionResult Delete(int id)
    {
        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    // DELETE: eliminar definitivamente
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var cliente = _repo.ObtenerPorId(id);

        if (cliente == null)
            return NotFound();

        _repo.Eliminar(id);

        TempData["Exito"] =
            $"Cliente {cliente.NombreCompleto} eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
