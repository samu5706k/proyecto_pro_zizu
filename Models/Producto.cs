
using System.ComponentModel.DataAnnotations;

namespace gimnasio.Models;

public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(60)]
    [Display(Name = "Categoría")]
    public string Categoria { get; set; } = string.Empty;

    [Range(
        typeof(decimal),
        "0",
        "99999999.99",
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "El precio debe ser válido y no negativo.")]
    [Display(Name = "Precio (Bs)")]
    public decimal Precio { get; set; }

    [Range(0, 1000000,
        ErrorMessage = "El stock debe ser un número válido.")]
    [Display(Name = "Cantidad disponible")]
    public int Stock { get; set; }

    public bool Activo { get; set; } = true;
}
