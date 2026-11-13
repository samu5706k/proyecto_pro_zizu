using gimnasio.Models;
using MySqlConnector;

namespace gimnasio.Services;

public class AdministradorRepositorio
{
    private readonly string _conexion;
    public AdministradorRepositorio(IConfiguration configuration)
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

    private static Administrador Mapear(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("Id"), Nombre = r.GetString("Nombre"),
        Apellido = r.GetString("Apellido"), Ci = r.GetString("Ci"),
        Telefono = r.GetString("Telefono"), Email = r.GetString("Email"),
        Cargo = r.GetString("Cargo"), NivelAcceso = r.GetString("NivelAcceso"),
        Activo = r.GetBoolean("Activo")
    };

    public Administrador Agregar(Administrador a)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"INSERT INTO administradores
            (Nombre,Apellido,Ci,Telefono,Email,Cargo,NivelAcceso,Activo)
            VALUES (@Nombre,@Apellido,@Ci,@Telefono,@Email,@Cargo,@Nivel,@Activo);
            SELECT LAST_INSERT_ID();", cn);
        Parametros(cmd, a);
        a.Id = Convert.ToInt32(cmd.ExecuteScalar());
        return a;
    }

    public IReadOnlyList<Administrador> ObtenerTodos()
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("SELECT * FROM administradores ORDER BY Id", cn);
        using var r = cmd.ExecuteReader();
        var lista = new List<Administrador>();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public Administrador? ObtenerPorId(int id)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("SELECT * FROM administradores WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public bool ExisteCi(string ci, int? excluirId = null)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM administradores WHERE Ci=@Ci AND (@Excluir IS NULL OR Id<>@Excluir)", cn);
        cmd.Parameters.AddWithValue("@Ci", ci.Trim());
        cmd.Parameters.AddWithValue("@Excluir", (object?)excluirId ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public IReadOnlyList<Administrador> Buscar(string texto)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"SELECT * FROM administradores
            WHERE Nombre LIKE @T OR Apellido LIKE @T OR Ci LIKE @T OR Telefono LIKE @T OR Email LIKE @T
            ORDER BY Id", cn);
        cmd.Parameters.AddWithValue("@T", $"%{texto.Trim()}%");
        using var r = cmd.ExecuteReader();
        var lista = new List<Administrador>();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public bool Actualizar(Administrador a)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"UPDATE administradores SET Nombre=@Nombre,Apellido=@Apellido,
            Ci=@Ci,Telefono=@Telefono,Email=@Email,Cargo=@Cargo,NivelAcceso=@Nivel,Activo=@Activo WHERE Id=@Id", cn);
        Parametros(cmd, a);
        cmd.Parameters.AddWithValue("@Id", a.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("DELETE FROM administradores WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static void Parametros(MySqlCommand cmd, Administrador a)
    {
        cmd.Parameters.AddWithValue("@Nombre", a.Nombre);
        cmd.Parameters.AddWithValue("@Apellido", a.Apellido);
        cmd.Parameters.AddWithValue("@Ci", a.Ci);
        cmd.Parameters.AddWithValue("@Telefono", a.Telefono);
        cmd.Parameters.AddWithValue("@Email", a.Email);
        cmd.Parameters.AddWithValue("@Cargo", a.Cargo);
        cmd.Parameters.AddWithValue("@Nivel", a.NivelAcceso);
        cmd.Parameters.AddWithValue("@Activo", a.Activo);
    }
}
