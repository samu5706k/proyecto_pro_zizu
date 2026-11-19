
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace gimnasio.Controllers;

[Authorize(Roles = "Administrador")]
public class AdministradoresController : Controller
{
    private readonly AdministradorRepositorio _repo;

    public AdministradoresController(AdministradorRepositorio repo)
    {
        _repo = repo;
    }

    // LISTAR ADMINISTRADORES
    public IActionResult Index(string? buscar)
    {
        var lista = string.IsNullOrWhiteSpace(buscar)
            ? _repo.ObtenerTodos()
            : _repo.Buscar(buscar);

        ViewBag.Buscar = buscar;
        return View(lista);
    }

    // MOSTRAR FORMULARIO DE REGISTRO
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Administrador());
    }

    // REGISTRAR ADMINISTRADOR CON CONTRASEÑA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(
        Administrador administrador,
        string? password,
        string? confirmarPassword)
    {
        administrador.Ci =
            administrador.Ci?.Trim() ?? string.Empty;

        administrador.Email =
            administrador.Email?.Trim() ?? string.Empty;

        // Comprobar CI
        if (_repo.ExisteCi(administrador.Ci))
        {
            ModelState.AddModelError(
                nameof(Administrador.Ci),
                "Ya existe un administrador con ese CI.");
        }

        // Comprobar correo
        if (_repo.ExisteEmail(administrador.Email))
        {
            ModelState.AddModelError(
                nameof(Administrador.Email),
                "Ya existe un administrador con ese correo.");
        }

        // Validar contraseña
        if (string.IsNullOrEmpty(password) ||
            password.Length < 12)
        {
            ModelState.AddModelError(
                "password",
                "La contraseña debe tener al menos 12 caracteres.");
        }

        // Confirmar contraseña
        if (password != confirmarPassword)
        {
            ModelState.AddModelError(
                "confirmarPassword",
                "Las contraseñas no coinciden.");
        }

        if (!ModelState.IsValid)
            return View(administrador);

        // Generar hash seguro
        var hasher = new PasswordHasher<Administrador>();

        string hash = hasher.HashPassword(
            administrador,
            password!);

        // Guardar administrador y credenciales
        _repo.AgregarConCredenciales(administrador, hash);

        TempData["Exito"] =
            "Administrador registrado con contraseña correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR FORMULARIO DE EDICIÓN
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        return View(administrador);
    }

    // GUARDAR CAMBIOS DE ADMINISTRADOR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Administrador administrador)
    {
        if (id != administrador.Id)
            return NotFound();

        administrador.Ci =
            administrador.Ci?.Trim() ?? string.Empty;

        if (_repo.ExisteCi(administrador.Ci, administrador.Id))
        {
            ModelState.AddModelError(
                nameof(Administrador.Ci),
                "Ya existe otro administrador con ese CI.");
        }

        if (!ModelState.IsValid)
            return View(administrador);

        if (!_repo.Actualizar(administrador))
            return NotFound();

        TempData["Exito"] =
            "Administrador actualizado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        return View(administrador);
    }

    // ELIMINAR ADMINISTRADOR
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        _repo.Eliminar(id);

        TempData["Exito"] =
            "Administrador eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // MOSTRAR ACCESOS
    public IActionResult Accesos()
    {
        return View(_repo.ObtenerTodos());
    }

    // ACTIVAR O DESACTIVAR ADMINISTRADOR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarEstado(int id)
    {
        var administrador = _repo.ObtenerPorId(id);

        if (administrador is null)
            return NotFound();

        administrador.Activo = !administrador.Activo;

        _repo.Actualizar(administrador);

        TempData["Exito"] =
            "Estado de acceso actualizado.";

        return RedirectToAction(nameof(Accesos));
    }
}
