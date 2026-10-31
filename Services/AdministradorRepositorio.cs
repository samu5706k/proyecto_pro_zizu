
using System.Text.Json;
using gimnasio.Models;

namespace gimnasio.Services;

public class AdministradorRepositorio
{
    private readonly string _rutaArchivo;
    private readonly object _candado = new();
    private List<Administrador> _administradores = new();

    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = true
    };

    public AdministradorRepositorio(IWebHostEnvironment entorno)
    {
        string carpeta = Path.Combine(entorno.ContentRootPath, "datos");
        Directory.CreateDirectory(carpeta);

        _rutaArchivo = Path.Combine(carpeta, "administradores.json");
        Cargar();
    }

    public Administrador Agregar(Administrador administrador)
    {
        lock (_candado)
        {
            administrador.Id = _administradores.Count == 0
                ? 1
                : _administradores.Max(a => a.Id) + 1;

            _administradores.Add(administrador.Clonar());
            Guardar();

            return administrador;
        }
    }

    public IReadOnlyList<Administrador> ObtenerTodos()
    {
        lock (_candado)
        {
            return _administradores
                .OrderBy(a => a.Id)
                .Select(a => a.Clonar())
                .ToList();
        }
    }

    public Administrador? ObtenerPorId(int id)
    {
        lock (_candado)
        {
            return _administradores
                .FirstOrDefault(a => a.Id == id)?.Clonar();
        }
    }

    public bool ExisteCi(string ci, int? excluirId = null)
    {
        lock (_candado)
        {
            return _administradores.Any(a =>
                a.Ci.Equals(ci.Trim(), StringComparison.OrdinalIgnoreCase)
                && a.Id != excluirId);
        }
    }

    public IReadOnlyList<Administrador> Buscar(string texto)
    {
        texto = texto.Trim();

        lock (_candado)
        {
            return _administradores
                .Where(a =>
                    a.NombreCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || a.Ci.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || a.Telefono.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || a.Email.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Id)
                .Select(a => a.Clonar())
                .ToList();
        }
    }

    public bool Actualizar(Administrador modificado)
    {
        lock (_candado)
        {
            int indice = _administradores.FindIndex(a => a.Id == modificado.Id);

            if (indice < 0)
                return false;

            _administradores[indice] = modificado.Clonar();
            Guardar();

            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            int eliminados = _administradores.RemoveAll(a => a.Id == id);

            if (eliminados == 0)
                return false;

            Guardar();
            return true;
        }
    }

    private void Cargar()
    {
        if (!File.Exists(_rutaArchivo))
            return;

        try
        {
            string json = File.ReadAllText(_rutaArchivo);
            _administradores =
                JsonSerializer.Deserialize<List<Administrador>>(json) ?? new();
        }
        catch (JsonException)
        {
            _administradores = new();
        }
    }

    private void Guardar()
    {
        File.WriteAllText(
            _rutaArchivo,
            JsonSerializer.Serialize(_administradores, Opciones));
    }
}
