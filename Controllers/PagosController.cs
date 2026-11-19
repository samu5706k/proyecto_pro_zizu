
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

[Authorize(Roles = "Administrador")]
public class PagosController : Controller
{
    private readonly PagoRepositorio _repo;
    private readonly MembresiaRepositorio _membresias;

    public PagosController(
        PagoRepositorio repo,
        MembresiaRepositorio membresias)
    {
        _repo = repo;
        _membresias = membresias;
    }

    private void CargarMembresias()
    {
        ViewBag.Membresias = _membresias.ObtenerTodos();
    }

    public IActionResult Index(
        string? buscar, int? membresiaId)
    {
        var lista = _repo.ObtenerTodos(buscar, membresiaId);

        ViewBag.Buscar = buscar;
        ViewBag.MembresiaId = membresiaId;

        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var pago = _repo.ObtenerPorId(id);

        if (pago == null)
            return NotFound();

        return View(pago);
    }

    [HttpGet]
    public IActionResult Create(int? membresiaId)
    {
        CargarMembresias();

        return View(new Pago
        {
            MembresiaId = membresiaId ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Pago modelo)
    {
        ValidarMonto(modelo);

        if (!ModelState.IsValid)
        {
            CargarMembresias();
            return View(modelo);
        }

        _repo.Agregar(modelo);

        TempData["Exito"] =
            "Pago registrado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var pago = _repo.ObtenerPorId(id);

        if (pago == null)
            return NotFound();

        CargarMembresias();
        return View(pago);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Pago modelo)
    {
        if (id != modelo.Id)
            return NotFound();

        if (_repo.ObtenerPorId(id) == null)
            return NotFound();

        ValidarMonto(modelo, id);

        if (!ModelState.IsValid)
        {
            CargarMembresias();
            return View(modelo);
        }

        _repo.Actualizar(modelo);

        TempData["Exito"] =
            "Pago actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var pago = _repo.ObtenerPorId(id);

        if (pago == null)
            return NotFound();

        return View(pago);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!_repo.Eliminar(id))
            return NotFound();

        TempData["Exito"] =
            "Pago eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private void ValidarMonto(Pago pago, int? pagoActualId = null)
    {
        var membresia = _membresias.ObtenerPorId(
            pago.MembresiaId);

        if (membresia == null)
        {
            ModelState.AddModelError(
                nameof(Pago.MembresiaId),
                "Selecciona una membresía existente.");

            return;
        }

        decimal disponible =
            membresia.Precio - membresia.TotalPagado;

        // Si estamos editando, recuperar el importe
        // del pago actual antes de calcular el saldo.
        if (pagoActualId.HasValue)
        {
            var anterior = _repo.ObtenerPorId(
                pagoActualId.Value);

            if (anterior != null &&
                anterior.MembresiaId == pago.MembresiaId)
            {
                disponible += anterior.Monto;
            }
        }

        if (pago.Monto > Math.Max(0m, disponible))
        {
            ModelState.AddModelError(
                nameof(Pago.Monto),
                $"El monto supera el saldo disponible "
                + $"de Bs {Math.Max(0m, disponible):N2}.");
        }
    }
}
