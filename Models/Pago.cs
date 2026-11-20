using System.ComponentModel.DataAnnotations;

namespace gimnasio.Models;

public class Pago
{
    public int Id { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona una membresía.")]
    [Display(Name = "Membresía")]
    public int MembresiaId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de pago")]
    public DateTime FechaPago { get; set; } = DateTime.Today;

    // CORREGIDO: validación decimal
    [Range(typeof(decimal), "0.01", "99999999.99",
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "El monto debe ser mayor que cero.")]
    [Display(Name = "Monto (Bs)")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "Selecciona un método de pago.")]
    [StringLength(30)]
    [Display(Name = "Método de pago")]
    public string Metodo { get; set; } = "Efectivo";

    [Required(ErrorMessage = "Debes ingresar la referencia o comprobante del pago.")]
    [StringLength(80, ErrorMessage = "La referencia no puede superar los 80 caracteres.")]
    [Display(Name = "Referencia / Comprobante")]
    public string Referencia { get; set; } = "";

    public string ClienteNombre { get; set; } = "";

    public string Plan { get; set; } = "";
}
