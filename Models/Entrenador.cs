using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace gimnasio.Models;

/// <summary>Representa a un entrenador del gimnasio.</summary>
public class Entrenador
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El CI es obligatorio.")]
    [StringLength(15, ErrorMessage = "Máximo 15 caracteres.")]
    [Display(Name = "CI")]
    public string Ci { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^[0-9+\- ]{7,15}$", ErrorMessage = "Teléfono no válido (7 a 15 dígitos).")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [StringLength(40, ErrorMessage = "Máximo 40 caracteres.")]
    public string Especialidad { get; set; } = string.Empty;

    [Display(Name = "Fecha de contratación")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [FechaNoFutura]
    public DateTime FechaContratacion { get; set; } = DateTime.Today;

    [Display(Name = "Salario mensual (Bs)")]
    [Range(0, 1000000, ErrorMessage = "El salario debe estar entre 0 y 1,000,000.")]
    public decimal Salario { get; set; }

    public bool Activo { get; set; } = true;

    [JsonIgnore]
    public string NombreCompleto => $"{Nombre} {Apellido}";

    [JsonIgnore]
    public int AniosAntiguedad
    {
        get
        {
            var hoy = DateTime.Today;
            int anios = hoy.Year - FechaContratacion.Year;
            if (FechaContratacion.Date > hoy.AddYears(-anios)) anios--;
            return Math.Max(anios, 0);
        }
    }

    public Entrenador Clonar() => (Entrenador)MemberwiseClone();
}
