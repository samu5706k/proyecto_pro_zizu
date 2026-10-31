
using System.ComponentModel.DataAnnotations;

namespace gimnasio.Models;

public class Administrador
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El CI es obligatorio.")]
    [StringLength(15)]
    public string Ci { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^[0-9+\- ]{7,15}$", ErrorMessage = "Teléfono no válido.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo no válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cargo es obligatorio.")]
    public string Cargo { get; set; } = "Administrador";

    [Required(ErrorMessage = "Seleccione un nivel de acceso.")]
    public string NivelAcceso { get; set; } = "Limitado";

    public bool Activo { get; set; } = true;

    public string NombreCompleto => $"{Nombre} {Apellido}";

    public Administrador Clonar() => (Administrador)MemberwiseClone();
}
