
using gimnasio.Models;
using MySqlConnector;

namespace gimnasio.Services;

public class MembresiaRepositorio
{
    private readonly string _conexion;

    public MembresiaRepositorio(IConfiguration config)
    {
        _conexion = config.GetConnectionString("Gimnasio")
            ?? throw new InvalidOperationException(
                "Falta la conexión Gimnasio.");
    }

    private MySqlConnection Abrir()
    {
        var cn = new MySqlConnection(_conexion);
        cn.Open();
        return cn;
    }

    private const string ConsultaBase = @"
        SELECT m.Id, m.ClienteId, m.Plan,
               m.FechaInicio, m.FechaVencimiento,
               m.Precio, m.Activa,
               CONCAT(c.Nombre, ' ', c.Apellido) AS ClienteNombre,
               (SELECT COALESCE(SUM(p.Monto), 0)
                FROM pagos p
                WHERE p.MembresiaId = m.Id) AS TotalPagado
        FROM membresias m
        INNER JOIN clientes c ON c.Id = m.ClienteId";

    private static Membresia Mapear(MySqlDataReader r)
    {
        return new Membresia
        {
            Id = r.GetInt32("Id"),
            ClienteId = r.GetInt32("ClienteId"),
            Plan = r.GetString("Plan"),
            FechaInicio = r.GetDateTime("FechaInicio"),
            FechaVencimiento = r.GetDateTime("FechaVencimiento"),
            Precio = r.GetDecimal("Precio"),
            Activa = r.GetBoolean("Activa"),
            ClienteNombre = r.GetString("ClienteNombre"),
            TotalPagado = r.GetDecimal("TotalPagado")
        };
    }

    public IReadOnlyList<Membresia> ObtenerTodos()
    {
        using var cn = Abrir();
        using var cmd = new MySqlCommand(
            ConsultaBase + " ORDER BY m.Id DESC", cn);

        using var r = cmd.ExecuteReader();
        var lista = new List<Membresia>();

        while (r.Read())
            lista.Add(Mapear(r));

        return lista;
    }

    public IReadOnlyList<Membresia> Buscar(string texto)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            ConsultaBase + @"
            WHERE m.Plan LIKE @Texto
               OR c.Nombre LIKE @Texto
               OR c.Apellido LIKE @Texto
               OR c.Ci LIKE @Texto
            ORDER BY m.Id DESC", cn);

        cmd.Parameters.AddWithValue(
            "@Texto", $"%{texto.Trim()}%");

        using var r = cmd.ExecuteReader();
        var lista = new List<Membresia>();

        while (r.Read())
            lista.Add(Mapear(r));

        return lista;
    }

    public Membresia? ObtenerPorId(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            ConsultaBase + " WHERE m.Id = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        using var r = cmd.ExecuteReader();

        return r.Read() ? Mapear(r) : null;
    }

    public Membresia Agregar(Membresia m)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            INSERT INTO membresias
            (ClienteId, Plan, FechaInicio,
             FechaVencimiento, Precio, Activa)
            VALUES
            (@ClienteId, @Plan, @Inicio,
             @Fin, @Precio, @Activa)", cn);

        Parametros(cmd, m);
        cmd.ExecuteNonQuery();

        m.Id = Convert.ToInt32(cmd.LastInsertedId);
        return m;
    }

    public bool Actualizar(Membresia m)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            UPDATE membresias
            SET ClienteId = @ClienteId,
                Plan = @Plan,
                FechaInicio = @Inicio,
                FechaVencimiento = @Fin,
                Precio = @Precio,
                Activa = @Activa
            WHERE Id = @Id", cn);

        Parametros(cmd, m);
        cmd.Parameters.AddWithValue("@Id", m.Id);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool TienePagos(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM pagos WHERE MembresiaId = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            "DELETE FROM membresias WHERE Id = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void Parametros(
        MySqlCommand cmd, Membresia m)
    {
        cmd.Parameters.AddWithValue("@ClienteId", m.ClienteId);
        cmd.Parameters.AddWithValue("@Plan", m.Plan);
        cmd.Parameters.AddWithValue("@Inicio", m.FechaInicio.Date);
        cmd.Parameters.AddWithValue("@Fin", m.FechaVencimiento.Date);
        cmd.Parameters.AddWithValue("@Precio", m.Precio);
        cmd.Parameters.AddWithValue("@Activa", m.Activa);
    }
}
