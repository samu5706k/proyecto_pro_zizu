
using gimnasio.Models;
using gimnasio.Services;
using Microsoft.AspNetCore.Mvc;

namespace gimnasio.Controllers;

public class EstadisticasController : Controller
{
    private readonly ClienteRepositorio _clientes;
    private readonly EntrenadorRepositorio _entrenadores;
    private readonly AdministradorRepositorio _administradores;

    public EstadisticasController(
        ClienteRepositorio clientes,
        EntrenadorRepositorio entrenadores,
        AdministradorRepositorio administradores)
    {
        _clientes = clientes;
        _entrenadores = entrenadores;
        _administradores = administradores;
    }

    public IActionResult Index()
    {
        var clientes = _clientes.ObtenerTodos();
        var entrenadores = _entrenadores.ObtenerTodos();
        var administradores = _administradores.ObtenerTodos();

        var modelo = new EstadisticasViewModel();

        // Estadísticas de clientes
        modelo.TotalClientes = clientes.Count;
        modelo.ClientesActivos = clientes.Count(c => c.Activo);
        modelo.ClientesInactivos =
            modelo.TotalClientes - modelo.ClientesActivos;

        modelo.ClientesRegistradosMes = clientes.Count(c =>
            c.FechaRegistro.Month == DateTime.Today.Month &&
            c.FechaRegistro.Year == DateTime.Today.Year);

        // Estadísticas de entrenadores
        modelo.TotalEntrenadores = entrenadores.Count;
        modelo.EntrenadoresActivos =
            entrenadores.Count(e => e.Activo);

        modelo.EntrenadoresInactivos =
            modelo.TotalEntrenadores - modelo.EntrenadoresActivos;

        // Estadísticas de administradores
        modelo.TotalAdministradores = administradores.Count;
        modelo.AdministradoresActivos =
            administradores.Count(a => a.Activo);

        modelo.AdministradoresInactivos =
            modelo.TotalAdministradores - modelo.AdministradoresActivos;

        // Total general de personas registradas
        modelo.TotalPersonas =
            modelo.TotalClientes +
            modelo.TotalEntrenadores +
            modelo.TotalAdministradores;

        modelo.TotalActivos =
            modelo.ClientesActivos +
            modelo.EntrenadoresActivos +
            modelo.AdministradoresActivos;

        // Entrenadores agrupados por especialidad
        modelo.PorEspecialidad = entrenadores
            .GroupBy(e => e.Especialidad)
            .Select(g => new EstadisticaItem
            {
                Nombre = g.Key,
                Cantidad = g.Count()
            })
            .OrderByDescending(e => e.Cantidad)
            .ToList();

        return View(modelo);
    }
}
