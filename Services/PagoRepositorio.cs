
using gimnasio.Models;
using MySqlConnector;

namespace gimnasio.Services;

public class PagoRepositorio
{
    private readonly string _conexion;

    public PagoRepositorio(IConfiguration config)
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
        SELECT p.Id, p.MembresiaId, p.FechaPago,
               p.Monto, p.Metodo, p.Referencia,
               CONCAT(c.Nombre, ' ', c.Apellido) AS ClienteNombre,
               m.Plan
        FROM pagos p
        INNER JOIN membresias m ON m.Id = p.MembresiaId
        INNER JOIN clientes c ON c.Id = m.ClienteId";

    private static Pago Mapear(MySqlDataReader r)
    {
        return new Pago
        {
            Id = r.GetInt32("Id"),
            MembresiaId = r.GetInt32("MembresiaId"),
            FechaPago = r.GetDateTime("FechaPago"),
            Monto = r.GetDecimal("Monto"),
            Metodo = r.GetString("Metodo"),
            Referencia = r.GetString("Referencia"),
            ClienteNombre = r.GetString("ClienteNombre"),
            Plan = r.GetString("Plan")
        };
    }

    public IReadOnlyList<Pago> ObtenerTodos(
        string? buscar = null, int? membresiaId = null)
    {
        using var cn = Abrir();

        var sql = ConsultaBase + " WHERE 1=1";

        if (membresiaId.HasValue)
            sql += " AND p.MembresiaId = @MembresiaId";

        if (!string.IsNullOrWhiteSpace(buscar))
            sql += @" AND (
                c.Nombre LIKE @Buscar
                OR c.Apellido LIKE @Buscar
                OR m.Plan LIKE @Buscar
                OR p.Metodo LIKE @Buscar)";

        sql += " ORDER BY p.FechaPago DESC, p.Id DESC";

        using var cmd = new MySqlCommand(sql, cn);

        if (membresiaId.HasValue)
            cmd.Parameters.AddWithValue(
                "@MembresiaId", membresiaId.Value);

        if (!string.IsNullOrWhiteSpace(buscar))
            cmd.Parameters.AddWithValue(
                "@Buscar", $"%{buscar.Trim()}%");

        using var r = cmd.ExecuteReader();
        var lista = new List<Pago>();

        while (r.Read())
            lista.Add(Mapear(r));

        return lista;
    }

    public Pago? ObtenerPorId(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            ConsultaBase + " WHERE p.Id = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        using var r = cmd.ExecuteReader();

        return r.Read() ? Mapear(r) : null;
    }

    public Pago Agregar(Pago p)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            INSERT INTO pagos
            (MembresiaId, FechaPago, Monto, Metodo, Referencia)
            VALUES
            (@MembresiaId, @FechaPago, @Monto, @Metodo, @Referencia)",
            cn);

        Parametros(cmd, p);
        cmd.ExecuteNonQuery();

        p.Id = Convert.ToInt32(cmd.LastInsertedId);
        return p;
    }

    public bool Actualizar(Pago p)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            UPDATE pagos
            SET MembresiaId = @MembresiaId,
                FechaPago = @FechaPago,
                Monto = @Monto,
                Metodo = @Metodo,
                Referencia = @Referencia
            WHERE Id = @Id", cn);

        Parametros(cmd, p);
        cmd.Parameters.AddWithValue("@Id", p.Id);

        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            "DELETE FROM pagos WHERE Id = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    private static void Parametros(MySqlCommand cmd, Pago p)
    {
        cmd.Parameters.AddWithValue(
            "@MembresiaId", p.MembresiaId);

        cmd.Parameters.AddWithValue(
            "@FechaPago", p.FechaPago.Date);

        cmd.Parameters.AddWithValue("@Monto", p.Monto);
        cmd.Parameters.AddWithValue("@Metodo", p.Metodo);

        cmd.Parameters.AddWithValue(
            "@Referencia", p.Referencia ?? "");
    }
}
