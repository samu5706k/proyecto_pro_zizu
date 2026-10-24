namespace gimnasio.Models;

public class ReporteViewModel
{
    public int Total { get; set; }
    public int Activos { get; set; }
    public int Inactivos { get; set; }
    public decimal PlanillaMensual { get; set; }
    public decimal SalarioPromedio { get; set; }
    public Entrenador? MasAntiguo { get; set; }
    public Entrenador? MayorSalario { get; set; }
    public List<EspecialidadConteo> PorEspecialidad { get; set; } = new();
}

public record EspecialidadConteo(string Especialidad, int Cantidad);
