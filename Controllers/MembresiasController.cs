
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace gimnasio.Controllers;

[Authorize(Roles = "Administrador")]
public class MembresiasController : Controller
{
    private readonly MembresiaRepositorio _repo;
    private readonly ClienteRepositorio _clientes;

    public MembresiasController(
        MembresiaRepositorio repo,
        ClienteRepositorio clientes)
    {
        _repo = repo;
        _clientes = clientes;
    }

    private void CargarClientes()
    {
        ViewBag.Clientes = _clientes.ObtenerTodos();
    }

    public IActionResult Index(string? buscar)
    {
        var lista = string.IsNullOrWhiteSpace(buscar)
            ? _repo.ObtenerTodos()
            : _repo.Buscar(buscar);

        ViewBag.Buscar = buscar;
        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var membresia = _repo.ObtenerPorId(id);

        if (membresia == null)
            return NotFound();

        return View(membresia);
    }

    [HttpGet]
    public IActionResult Create()
    {
        CargarClientes();
        return View(new Membresia());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Membresia modelo)
    {
        Validar(modelo);

        if (!ModelState.IsValid)
        {
            CargarClientes();
            return View(modelo);
        }

        _repo.Agregar(modelo);

        TempData["Exito"] =
            "Membresía registrada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var membresia = _repo.ObtenerPorId(id);

        if (membresia == null)
            return NotFound();

        CargarClientes();
        return View(membresia);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Membresia modelo)
    {
        if (id != modelo.Id)
            return NotFound();

        var anterior = _repo.ObtenerPorId(id);

        if (anterior == null)
            return NotFound();

        Validar(modelo);

        if (modelo.Precio < anterior.TotalPagado)
        {
            ModelState.AddModelError(
                nameof(Membresia.Precio),
                "El precio no puede ser menor al total ya pagado.");
        }

        if (anterior.TotalPagado > 0 &&
            anterior.ClienteId != modelo.ClienteId)
        {
            ModelState.AddModelError(
                nameof(Membresia.ClienteId),
                "No puedes cambiar el socio de una membresía con pagos.");
        }

        if (!ModelState.IsValid)
        {
            CargarClientes();
            return View(modelo);
        }

        _repo.Actualizar(modelo);

        TempData["Exito"] =
            "Membresía actualizada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var membresia = _repo.ObtenerPorId(id);

        if (membresia == null)
            return NotFound();

        return View(membresia);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (_repo.ObtenerPorId(id) == null)
            return NotFound();

        if (_repo.TienePagos(id))
        {
            TempData["Error"] =
                "No se puede eliminar: esta membresía tiene pagos. "
                + "Puedes marcarla como inactiva.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            _repo.Eliminar(id);
        }
        catch (MySqlException ex) when (ex.Number == 1451)
        {
            TempData["Error"] =
                "No se puede eliminar porque tiene registros asociados.";

            return RedirectToAction(nameof(Index));
        }

        TempData["Exito"] =
            "Membresía eliminada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private void Validar(Membresia m)
    {
        if (_clientes.ObtenerPorId(m.ClienteId) == null)
        {
            ModelState.AddModelError(
                nameof(Membresia.ClienteId),
                "Selecciona un cliente existente.");
        }

        if (m.FechaVencimiento.Date < m.FechaInicio.Date)
        {
            ModelState.AddModelError(
                nameof(Membresia.FechaVencimiento),
                "La fecha de vencimiento no puede ser anterior al inicio.");
        }
    }
}
