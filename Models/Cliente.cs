using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace gimnasio.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El CI es obligatorio.")]
    [Display(Name = "CI")]
    public string Ci { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Display(Name = "Teléfono")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no es válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime? FechaNacimiento { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Today;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    [JsonIgnore]
    public string NombreCompleto => $"{Nombre} {Apellido}";

    public Cliente Clonar() => (Cliente)MemberwiseClone();
}