using gimnasio.Models;
using MySqlConnector;

namespace gimnasio.Services;

public class EntrenadorRepositorio
{
    private readonly string _conexion;
    public EntrenadorRepositorio(IConfiguration configuration)
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

    private static Entrenador Mapear(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("Id"), Nombre = r.GetString("Nombre"),
        Apellido = r.GetString("Apellido"), Ci = r.GetString("Ci"),
        Telefono = r.GetString("Telefono"), Email = r.GetString("Email"),
        Especialidad = r.GetString("Especialidad"),
        FechaContratacion = r.GetDateTime("FechaContratacion"),
        Salario = r.GetDecimal("Salario"), Activo = r.GetBoolean("Activo")
    };

    public Entrenador Agregar(Entrenador e)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"INSERT INTO entrenadores
            (Nombre,Apellido,Ci,Telefono,Email,Especialidad,FechaContratacion,Salario,Activo)
            VALUES (@Nombre,@Apellido,@Ci,@Telefono,@Email,@Especialidad,@Fecha,@Salario,@Activo);
            SELECT LAST_INSERT_ID();", cn);
        Parametros(cmd, e);
        e.Id = Convert.ToInt32(cmd.ExecuteScalar());
        return e;
    }

    public IReadOnlyList<Entrenador> ObtenerTodos()
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("SELECT * FROM entrenadores ORDER BY Id", cn);
        using var r = cmd.ExecuteReader();
        var lista = new List<Entrenador>();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public Entrenador? ObtenerPorId(int id)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("SELECT * FROM entrenadores WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Mapear(r) : null;
    }

    public bool ExisteCi(string ci, int? excluirId = null)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM entrenadores WHERE Ci=@Ci AND (@Excluir IS NULL OR Id<>@Excluir)", cn);
        cmd.Parameters.AddWithValue("@Ci", ci.Trim());
        cmd.Parameters.AddWithValue("@Excluir", (object?)excluirId ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public IReadOnlyList<Entrenador> Buscar(string texto)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"SELECT * FROM entrenadores
            WHERE Nombre LIKE @T OR Apellido LIKE @T OR Ci LIKE @T OR Especialidad LIKE @T
            ORDER BY Id", cn);
        cmd.Parameters.AddWithValue("@T", $"%{texto.Trim()}%");
        using var r = cmd.ExecuteReader();
        var lista = new List<Entrenador>();
        while (r.Read()) lista.Add(Mapear(r));
        return lista;
    }

    public bool Actualizar(Entrenador e)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(@"UPDATE entrenadores SET Nombre=@Nombre,Apellido=@Apellido,
            Ci=@Ci,Telefono=@Telefono,Email=@Email,Especialidad=@Especialidad,
            FechaContratacion=@Fecha,Salario=@Salario,Activo=@Activo WHERE Id=@Id", cn);
        Parametros(cmd, e);
        cmd.Parameters.AddWithValue("@Id", e.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand("DELETE FROM entrenadores WHERE Id=@Id", cn);
        cmd.Parameters.AddWithValue("@Id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private static void Parametros(MySqlCommand cmd, Entrenador e)
    {
        cmd.Parameters.AddWithValue("@Nombre", e.Nombre);
        cmd.Parameters.AddWithValue("@Apellido", e.Apellido);
        cmd.Parameters.AddWithValue("@Ci", e.Ci);
        cmd.Parameters.AddWithValue("@Telefono", e.Telefono);
        cmd.Parameters.AddWithValue("@Email", e.Email);
        cmd.Parameters.AddWithValue("@Especialidad", e.Especialidad);
        cmd.Parameters.AddWithValue("@Fecha", e.FechaContratacion.Date);
        cmd.Parameters.AddWithValue("@Salario", e.Salario);
        cmd.Parameters.AddWithValue("@Activo", e.Activo);
    }
}
