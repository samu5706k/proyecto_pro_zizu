using System.Text.Json;
using gimnasio.Models;

namespace gimnasio.Services;

/// <summary>
/// Guarda los entrenadores en un archivo JSON local (carpeta "datos").
/// Es seguro para varias peticiones simultáneas gracias a un lock.
/// </summary>
public class EntrenadorRepositorio
{
    private readonly string _rutaArchivo;
    private readonly object _candado = new();
    private List<Entrenador> _entrenadores = new();

    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };

    public EntrenadorRepositorio(IWebHostEnvironment entorno)
    {
        string carpeta = Path.Combine(entorno.ContentRootPath, "datos");
        Directory.CreateDirectory(carpeta);
        _rutaArchivo = Path.Combine(carpeta, "entrenadores.json");
        Cargar();
    }

    // ---------- CREATE ----------
    public Entrenador Agregar(Entrenador entrenador)
    {
        lock (_candado)
        {
            entrenador.Id = _entrenadores.Count == 0 ? 1 : _entrenadores.Max(e => e.Id) + 1;
            _entrenadores.Add(entrenador.Clonar());
            Guardar();
            return entrenador;
        }
    }

    // ---------- READ ----------
    public IReadOnlyList<Entrenador> ObtenerTodos()
    {
        lock (_candado)
            return _entrenadores.OrderBy(e => e.Id).Select(e => e.Clonar()).ToList();
    }

    public Entrenador? ObtenerPorId(int id)
    {
        lock (_candado)
            return _entrenadores.FirstOrDefault(e => e.Id == id)?.Clonar();
    }

    public bool ExisteCi(string ci, int? excluirId = null)
    {
        lock (_candado)
            return _entrenadores.Any(e =>
                e.Ci.Equals(ci.Trim(), StringComparison.OrdinalIgnoreCase) && e.Id != excluirId);
    }

    public IReadOnlyList<Entrenador> Buscar(string texto)
    {
        texto = texto.Trim();
        lock (_candado)
            return _entrenadores
                .Where(e => e.NombreCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase)
                         || e.Ci.Contains(texto, StringComparison.OrdinalIgnoreCase)
                         || e.Especialidad.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.Id)
                .Select(e => e.Clonar())
                .ToList();
    }

    // ---------- UPDATE ----------
    public bool Actualizar(Entrenador modificado)
    {
        lock (_candado)
        {
            int indice = _entrenadores.FindIndex(e => e.Id == modificado.Id);
            if (indice < 0) return false;
            _entrenadores[indice] = modificado.Clonar();
            Guardar();
            return true;
        }
    }

    // ---------- DELETE ----------
    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            int eliminados = _entrenadores.RemoveAll(e => e.Id == id);
            if (eliminados == 0) return false;
            Guardar();
            return true;
        }
    }

    // ---------- Persistencia ----------
    private void Cargar()
    {
        if (!File.Exists(_rutaArchivo)) return;
        try
        {
            string json = File.ReadAllText(_rutaArchivo);
            _entrenadores = JsonSerializer.Deserialize<List<Entrenador>>(json) ?? new();
        }
        catch (JsonException)
        {
            _entrenadores = new();
        }
    }

    private void Guardar()
    {
        File.WriteAllText(_rutaArchivo, JsonSerializer.Serialize(_entrenadores, Opciones));
    }
}
