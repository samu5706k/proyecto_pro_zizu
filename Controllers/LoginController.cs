using System.Security.Claims;
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

public class LoginController : Controller
{
    private readonly LoginRepositorio _repositorio;

    public LoginController(LoginRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
            return View(modelo);

        var admin = _repositorio.Validar(
            modelo.Email, modelo.Password);

        if (admin == null)
        {
            ModelState.AddModelError(
                "", "Correo o contraseña incorrectos.");

            return View(modelo);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.NombreCompleto),
            new(ClaimTypes.Email, admin.Email),
            new(ClaimTypes.Role, "Administrador")
        };

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identidad);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Login");
    }
}
