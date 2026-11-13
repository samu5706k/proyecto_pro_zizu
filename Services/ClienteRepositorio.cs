using gimnasio.Models;
using MySqlConnector;

namespace gimnasio.Services;

public class ClienteRepositorio
{
    private readonly string _conexion;
    public ClienteRepositorio(IConfiguration configuration)
    {
        _conexion = configuration.GetConnectionString("Gimnasio") ?? "";
        if (string.IsNullOrWhiteSpace(_conexion))
            throw new InvalidOperationException(
                "Falta configurar ConnectionStrings:Gimnasio. Consulta README.md para configurar User Secrets.");
    }

    private MySqlConnection Abrir()
    {
        var cn = new MySqlConnection(_conexion);
        cn.Open();
        return cn;
    }

    private static Cliente Mapear(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("Id"), Nombre = r.GetString("Nombre"),
        Apellido = r.GetString("Apellido"), Ci = r.GetString("Ci"),
        Telefono = r.GetString("Telefono"), Email = r.GetString("Email"),
        Direccion = r.GetString("Direccion"),
        FechaNacimiento = r.IsDBNull(r.GetOrdinal("FechaNacimiento")) ? null : r.GetDateTime("FechaNacimiento"),
        FechaRegistro = r.GetDateTime("FechaRegistro"), Activo = r.GetBoolean("Activo")
    };

    public Cliente Agregar(Cliente c)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"INSERT INTO clientes
            (Nombre,Apellido,Ci,Telefono,Email,Direccion,FechaNacimiento,FechaRegistro,Activo)
            VALUES (@Nombre,@Apellido,@Ci,@Telefono,@Email,@Direccion,@Nacimiento,@Registro,@Activo);
            SELECT LAST_INSERT_ID();", cn);
        Parametros(cmd, c);
        c.Id = Convert.ToInt32(cmd.ExecuteScalar());
        return c;
    }

    public IReadOnlyList<Cliente> ObtenerTodos()
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("SELECT * FROM clientes ORDER BY Id", cn);
        using var r = cmd.ExecuteReader();
        var lista = new List<Cliente>();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public Cliente? ObtenerPorId(int id)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("SELECT * FROM clientes WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public bool ExisteCi(string ci, int? excluirId = null)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM clientes WHERE Ci=@Ci AND (@Excluir IS NULL OR Id<>@Excluir)", cn);
        cmd.Parameters.AddWithValue("@Ci", ci.Trim());
        cmd.Parameters.AddWithValue("@Excluir", (object?)excluirId ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public IReadOnlyList<Cliente> Buscar(string texto)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"SELECT * FROM clientes
            WHERE Nombre LIKE @T OR Apellido LIKE @T OR Ci LIKE @T OR Telefono LIKE @T OR Email LIKE @T
            ORDER BY Id", cn);
        cmd.Parameters.AddWithValue("@T", $"%{texto.Trim()}%");
        using var r = cmd.ExecuteReader();
        var lista = new List<Cliente>();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public bool Actualizar(Cliente c)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"UPDATE clientes SET Nombre=@Nombre,Apellido=@Apellido,
            Ci=@Ci,Telefono=@Telefono,Email=@Email,Direccion=@Direccion,
            FechaNacimiento=@Nacimiento,FechaRegistro=@Registro,Activo=@Activo WHERE Id=@Id", cn);
        Parametros(cmd, c);
        cmd.Parameters.AddWithValue("@Id", c.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("DELETE FROM clientes WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static void Parametros(MySqlCommand cmd, Cliente c)
    {
        cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
        cmd.Parameters.AddWithValue("@Apellido", c.Apellido);
        cmd.Parameters.AddWithValue("@Ci", c.Ci);
        cmd.Parameters.AddWithValue("@Telefono", c.Telefono);
        cmd.Parameters.AddWithValue("@Email", c.Email);
        cmd.Parameters.AddWithValue("@Direccion", c.Direccion ?? "");
        cmd.Parameters.AddWithValue("@Nacimiento", (object?)c.FechaNacimiento?.Date ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Registro", c.FechaRegistro.Date);
        cmd.Parameters.AddWithValue("@Activo", c.Activo);
    }
}
