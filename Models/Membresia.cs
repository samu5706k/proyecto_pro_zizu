using System.ComponentModel.DataAnnotations;

namespace gimnasio.Models;

public class Membresia
{
    public int Id { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona un cliente.")]
    [Display(Name = "Cliente / Socio")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El plan es obligatorio.")]
    [StringLength(40)]
    public string Plan { get; set; } = "Mensual";

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de vencimiento")]
    public DateTime FechaVencimiento { get; set; }
        = DateTime.Today.AddMonths(1);

    // CORREGIDO: lectura del rango decimal
    [Range(typeof(decimal), "0.01", "99999999.99",
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "El precio debe ser mayor que cero.")]
    [Display(Name = "Precio (Bs)")]
    public decimal Precio { get; set; }

    public bool Activa { get; set; } = true;

    public string ClienteNombre { get; set; } = "";

    public decimal TotalPagado { get; set; }

    public decimal SaldoPendiente =>
        Math.Max(0m, Precio - TotalPagado);

    public string Estado
    {
        get
        {
            if (!Activa)
                return "Inactiva";

            if (FechaVencimiento.Date < DateTime.Today)
                return "Vencida";

            if (FechaInicio.Date > DateTime.Today)
                return "Pendiente";

            return "Vigente";
        }
    }

    public string Etiqueta =>
        $"#{Id} - {ClienteNombre} - {Plan}";
}
