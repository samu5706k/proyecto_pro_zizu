
using gimnasio.Models;
using MySqlConnector;

namespace gimnasio.Services;

public class ProductoRepositorio
{
    private readonly string _conexion;

    public ProductoRepositorio(IConfiguration configuration)
    {
        _conexion = configuration
            .GetConnectionString("Gimnasio")
            ?? throw new InvalidOperationException(
                "Falta configurar la conexión Gimnasio.");
    }

    // ABRIR CONEXIÓN
    private MySqlConnection Abrir()
    {
        var cn = new MySqlConnection(_conexion);
        cn.Open();
        return cn;
    }

    // CONVERTIR REGISTRO A PRODUCTO
    private static Producto Mapear(MySqlDataReader r)
    {
        return new Producto
        {
            Id = r.GetInt32("Id"),
            Nombre = r.GetString("Nombre"),
            Descripcion = r.GetString("Descripcion"),
            Categoria = r.GetString("Categoria"),
            Precio = r.GetDecimal("Precio"),
            Stock = r.GetInt32("Stock"),
            Activo = r.GetBoolean("Activo")
        };
    }

    // OBTENER TODOS LOS PRODUCTOS
    public IReadOnlyList<Producto> ObtenerTodos()
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            "SELECT * FROM productos ORDER BY Id DESC", cn);

        using var reader = cmd.ExecuteReader();

        var lista = new List<Producto>();

        while (reader.Read())
            lista.Add(Mapear(reader));

        return lista;
    }

    // BUSCAR PRODUCTOS
    public IReadOnlyList<Producto> Buscar(string texto)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            SELECT * FROM productos
            WHERE Nombre LIKE @Buscar
               OR Categoria LIKE @Buscar
               OR Descripcion LIKE @Buscar
            ORDER BY Id DESC;", cn);

        cmd.Parameters.AddWithValue(
            "@Buscar", $"%{texto.Trim()}%");

        using var reader = cmd.ExecuteReader();

        var lista = new List<Producto>();

        while (reader.Read())
            lista.Add(Mapear(reader));

        return lista;
    }

    // OBTENER PRODUCTO POR ID
    public Producto? ObtenerPorId(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            "SELECT * FROM productos WHERE Id = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        using var reader = cmd.ExecuteReader();

        return reader.Read() ? Mapear(reader) : null;
    }

    // REGISTRAR PRODUCTO
    public Producto Agregar(Producto producto)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            INSERT INTO productos
            (Nombre, Descripcion, Categoria,
             Precio, Stock, Activo)
            VALUES
            (@Nombre, @Descripcion, @Categoria,
             @Precio, @Stock, @Activo);", cn);

        Parametros(cmd, producto);

        cmd.ExecuteNonQuery();

        producto.Id = Convert.ToInt32(cmd.LastInsertedId);

        return producto;
    }

    // ACTUALIZAR PRODUCTO
    public bool Actualizar(Producto producto)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(@"
            UPDATE productos
            SET Nombre = @Nombre,
                Descripcion = @Descripcion,
                Categoria = @Categoria,
                Precio = @Precio,
                Stock = @Stock,
                Activo = @Activo
            WHERE Id = @Id;", cn);

        Parametros(cmd, producto);
        cmd.Parameters.AddWithValue("@Id", producto.Id);

        return cmd.ExecuteNonQuery() > 0;
    }

    // ELIMINAR PRODUCTO
    public bool Eliminar(int id)
    {
        using var cn = Abrir();

        using var cmd = new MySqlCommand(
            "DELETE FROM productos WHERE Id = @Id", cn);

        cmd.Parameters.AddWithValue("@Id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    // PARÁMETROS DE MARIADB
    private static void Parametros(
        MySqlCommand cmd, Producto p)
    {
        cmd.Parameters.AddWithValue("@Nombre", p.Nombre);

        cmd.Parameters.AddWithValue(
            "@Descripcion", p.Descripcion ?? "");

        cmd.Parameters.AddWithValue(
            "@Categoria", p.Categoria);

        cmd.Parameters.AddWithValue("@Precio", p.Precio);
        cmd.Parameters.AddWithValue("@Stock", p.Stock);
        cmd.Parameters.AddWithValue("@Activo", p.Activo);
    }
}
