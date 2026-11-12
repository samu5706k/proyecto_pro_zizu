
namespace gimnasio.Models;

public class EstadisticasViewModel
{
    public int TotalClientes { get; set; }
    public int ClientesActivos { get; set; }
    public int ClientesInactivos { get; set; }
    public int ClientesRegistradosMes { get; set; }

    public int TotalEntrenadores { get; set; }
    public int EntrenadoresActivos { get; set; }
    public int EntrenadoresInactivos { get; set; }

    public int TotalAdministradores { get; set; }
    public int AdministradoresActivos { get; set; }
    public int AdministradoresInactivos { get; set; }

    public int TotalPersonas { get; set; }
    public int TotalActivos { get; set; }

    public List<EstadisticaItem> PorEspecialidad { get; set; } = new();
}

public class EstadisticaItem
{
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}
