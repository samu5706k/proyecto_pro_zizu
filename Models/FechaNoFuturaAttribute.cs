using System.ComponentModel.DataAnnotations;

namespace gimnasio.Models;

/// <summary>Valida que una fecha sea razonable y no esté en el futuro.</summary>
public class FechaNoFuturaAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime fecha)
        {
            if (fecha.Year < 1950)
                return new ValidationResult("Ingresa una fecha válida.");
            if (fecha.Date > DateTime.Today)
                return new ValidationResult("La fecha no puede ser futura.");
        }
        return ValidationResult.Success;
    }
}
