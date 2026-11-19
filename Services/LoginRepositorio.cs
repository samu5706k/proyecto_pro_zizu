
using gimnasio.Models;
using Microsoft.AspNetCore.Identity;
using MySqlConnector;

namespace gimnasio.Services;

public class LoginRepositorio
{
    private readonly string _conexion;
    private readonly PasswordHasher<Administrador> _hasher = new();

    public LoginRepositorio(IConfiguration configuration)
    {
        _conexion = configuration.GetConnectionString("Gimnasio")
            ?? throw new InvalidOperationException(
                "Falta la conexión Gimnasio.");
    }

    public Administrador? Validar(string email, string password)
    {
        using var cn = new MySqlConnection(_conexion);
        cn.Open();

        using var cmd = new MySqlCommand(@"
            SELECT a.Id, a.Nombre, a.Apellido, a.Ci,
                   a.Telefono, a.Email, a.Cargo,
                   a.NivelAcceso, a.Activo,
                   c.PasswordHash
            FROM administradores a
            INNER JOIN credenciales_admin c
                ON c.AdministradorId = a.Id
            WHERE a.Email = @Email
              AND a.Activo = 1
            LIMIT 1;", cn);

        cmd.Parameters.AddWithValue("@Email", email.Trim());

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        var admin = new Administrador
        {
            Id = reader.GetInt32("Id"),
            Nombre = reader.GetString("Nombre"),
            Apellido = reader.GetString("Apellido"),
            Ci = reader.GetString("Ci"),
            Telefono = reader.GetString("Telefono"),
            Email = reader.GetString("Email"),
            Cargo = reader.GetString("Cargo"),
            NivelAcceso = reader.GetString("NivelAcceso"),
            Activo = reader.GetBoolean("Activo")
        };

        var hash = reader.GetString("PasswordHash");

        var resultado = _hasher.VerifyHashedPassword(
            admin, hash, password);

        return resultado == PasswordVerificationResult.Failed
            ? null
            : admin;
    }
}
