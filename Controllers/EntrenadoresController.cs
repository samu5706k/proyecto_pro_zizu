using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

public class EntrenadoresController : Controller
{
    private readonly EntrenadorRepositorio _repo;

    public EntrenadoresController(EntrenadorRepositorio repo) => _repo = repo;

    // ===== PANTALLA 1 - READ: listado y búsqueda =====
    public IActionResult Index(string? buscar)
    {
        var lista = string.IsNullOrWhiteSpace(buscar) ? _repo.ObtenerTodos() : _repo.Buscar(buscar);
        ViewBag.Buscar = buscar;
        return View(lista);
    }

    // ===== Detalle de un entrenador (parte de Read) =====
    public IActionResult Details(int id)
    {
        var entrenador = _repo.ObtenerPorId(id);
        if (entrenador is null) return NotFound();
        return View(entrenador);
    }

    // ===== PANTALLA 2 - CREATE =====
    public IActionResult Create() => View(new Entrenador());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(Entrenador entrenador)
    {
        entrenador.Ci = entrenador.Ci?.Trim() ?? string.Empty;

        if (_repo.ExisteCi(entrenador.Ci))
            ModelState.AddModelError(nameof(Entrenador.Ci), "Ya existe un entrenador con ese CI.");

        if (!ModelState.IsValid) return View(entrenador);

        _repo.Agregar(entrenador);
        TempData["Exito"] = $"Entrenador {entrenador.NombreCompleto} registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // ===== PANTALLA 3 - UPDATE =====
    public IActionResult Edit(int id)
    {
        var entrenador = _repo.ObtenerPorId(id);
        if (entrenador is null) return NotFound();
        return View(entrenador);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Entrenador entrenador)
    {
        if (id != entrenador.Id) return NotFound();

        entrenador.Ci = entrenador.Ci?.Trim() ?? string.Empty;

        if (_repo.ExisteCi(entrenador.Ci, entrenador.Id))
            ModelState.AddModelError(nameof(Entrenador.Ci), "Ya existe otro entrenador con ese CI.");

        if (!ModelState.IsValid) return View(entrenador);

        if (!_repo.Actualizar(entrenador)) return NotFound();

        TempData["Exito"] = $"Los datos de {entrenador.NombreCompleto} se actualizaron correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // ===== PANTALLA 4 - DELETE (confirmación + borrado) =====
    public IActionResult Delete(int id)
    {
        var entrenador = _repo.ObtenerPorId(id);
        if (entrenador is null) return NotFound();
        return View(entrenador);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var entrenador = _repo.ObtenerPorId(id);
        if (entrenador is null) return NotFound();

        _repo.Eliminar(id);
        TempData["Exito"] = $"Entrenador {entrenador.NombreCompleto} eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // ===== PANTALLA 5 - Reportes y estadísticas =====
    public IActionResult Reportes()
    {
        var lista = _repo.ObtenerTodos();
        var modelo = new ReporteViewModel { Total = lista.Count };

        if (lista.Count > 0)
        {
            modelo.Activos = lista.Count(e => e.Activo);
            modelo.Inactivos = lista.Count - modelo.Activos;
            modelo.PlanillaMensual = lista.Where(e => e.Activo).Sum(e => e.Salario);
            modelo.SalarioPromedio = lista.Average(e => e.Salario);
            modelo.MasAntiguo = lista.OrderBy(e => e.FechaContratacion).First();
            modelo.MayorSalario = lista.OrderByDescending(e => e.Salario).First();
            modelo.PorEspecialidad = lista
                .GroupBy(e => e.Especialidad, StringComparer.OrdinalIgnoreCase)
                .Select(g => new EspecialidadConteo(g.Key, g.Count()))
                .OrderByDescending(x => x.Cantidad)
                .ToList();
        }

        return View(modelo);
    }
}
