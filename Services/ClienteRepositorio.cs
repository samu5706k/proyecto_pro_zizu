using System.Text.Json;
using gimnasio.Models;

namespace gimnasio.Services;

public class ClienteRepositorio
{
    private readonly string _rutaArchivo;
    private readonly object _candado = new();
    private List<Cliente> _clientes = new();

    private static readonly JsonSerializerOptions Opciones = new()
    {
        WriteIndented = true
    };

    public ClienteRepositorio(IWebHostEnvironment entorno)
    {
        string carpeta = Path.Combine(entorno.ContentRootPath, "datos");
        Directory.CreateDirectory(carpeta);

        _rutaArchivo = Path.Combine(carpeta, "clientes.json");
        Cargar();
    }

    public Cliente Agregar(Cliente cliente)
    {
        lock (_candado)
        {
            cliente.Id = _clientes.Count == 0
                ? 1
                : _clientes.Max(c => c.Id) + 1;

            _clientes.Add(cliente.Clonar());
            Guardar();

            return cliente;
        }
    }

    public IReadOnlyList<Cliente> ObtenerTodos()
    {
        lock (_candado)
        {
            return _clientes
                .OrderBy(c => c.Id)
                .Select(c => c.Clonar())
                .ToList();
        }
    }

    public Cliente? ObtenerPorId(int id)
    {
        lock (_candado)
        {
            return _clientes.FirstOrDefault(c => c.Id == id)?.Clonar();
        }
    }

    public bool ExisteCi(string ci, int? excluirId = null)
    {
        lock (_candado)
        {
            return _clientes.Any(c =>
                c.Ci.Equals(ci.Trim(), StringComparison.OrdinalIgnoreCase)
                && c.Id != excluirId);
        }
    }

    public IReadOnlyList<Cliente> Buscar(string texto)
    {
        texto = texto.Trim();

        lock (_candado)
        {
            return _clientes
                .Where(c =>
                    c.NombreCompleto.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || c.Ci.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || c.Telefono.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || c.Email.Contains(texto, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Id)
                .Select(c => c.Clonar())
                .ToList();
        }
    }

    public bool Actualizar(Cliente modificado)
    {
        lock (_candado)
        {
            int indice = _clientes.FindIndex(c => c.Id == modificado.Id);

            if (indice < 0)
                return false;

            _clientes[indice] = modificado.Clonar();
            Guardar();

            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            int eliminados = _clientes.RemoveAll(c => c.Id == id);

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
            _clientes = JsonSerializer.Deserialize<List<Cliente>>(json) ?? new();
        }
        catch (JsonException)
        {
            _clientes = new();
        }
    }

    private void Guardar()
    {
        File.WriteAllText(
            _rutaArchivo,
            JsonSerializer.Serialize(_clientes, Opciones));
    }
}