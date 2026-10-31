
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

public class AdministradoresController : Controller
{
    private readonly AdministradorRepositorio _repo;

    public AdministradoresController(AdministradorRepositorio repo)
    {
        _repo = repo;
    }

    public IActionResult Index(string? buscar)
    {
        var lista = string.IsNullOrWhiteSpace(buscar)
            ? _repo.ObtenerTodos()
            : _repo.Buscar(buscar);

        ViewBag.Buscar = buscar;
        return View(lista);
    }

    public IActionResult Create()
    {
        return View(new Administrador());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(Administrador administrador)
    {
        administrador.Ci = administrador.Ci?.Trim() ?? string.Empty;

        if (_repo.ExisteCi(administrador.Ci))
            ModelState.AddModelError(nameof(Administrador.Ci),
                "Ya existe un administrador con ese CI.");

        if (!ModelState.IsValid)
            return View(administrador);

        _repo.Agregar(administrador);

        TempData["Exito"] = "Administrador registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        return View(administrador);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Administrador administrador)
    {
        if (id != administrador.Id)
            return NotFound();

        administrador.Ci = administrador.Ci?.Trim() ?? string.Empty;

        if (_repo.ExisteCi(administrador.Ci, administrador.Id))
            ModelState.AddModelError(nameof(Administrador.Ci),
                "Ya existe otro administrador con ese CI.");

        if (!ModelState.IsValid)
            return View(administrador);

        if (!_repo.Actualizar(administrador))
            return NotFound();

        TempData["Exito"] = "Administrador actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        return View(administrador);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        _repo.Eliminar(id);

        TempData["Exito"] = "Administrador eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Accesos()
    {
        return View(_repo.ObtenerTodos());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CambiarEstado(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        administrador.Activo = !administrador.Activo;
        _repo.Actualizar(administrador);

        TempData["Exito"] = "Estado de acceso actualizado.";
        return RedirectToAction(nameof(Accesos));
    }
}
