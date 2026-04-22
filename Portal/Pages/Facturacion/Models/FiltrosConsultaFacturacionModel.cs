// Inicio código generado por GitHub Copilot
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models;

/// <summary>
/// Modelo para los filtros de consulta de facturación electrónica
/// Incluye validaciones con Data Annotations.
/// </summary>
public class FiltrosConsultaFacturacionModel : IValidatableObject
{
    #region Propiedades

    [Required(ErrorMessage = "La fecha inicial es obligatoria.")]
    public DateTime FechaDesde { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "La fecha final es obligatoria.")]
    public DateTime FechaHasta { get; set; } = DateTime.Today;

    public string TipoDocumento { get; set; }

    public string NumeroDocumento { get; set; }

    [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "El nombre solo debe contener letras y espacios.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string NombreCliente { get; set; }

    [RegularExpression(@"^\d{0,15}$", ErrorMessage = "El PIN debe contener solo números.")]
    public string PIN { get; set; }

    #endregion

    // Inicio código generado por GitHub Copilot
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Método generado por GitHub Copilot
        // Inicio código generado por GitHub Copilot
        if (FechaDesde > FechaHasta)
        {
            yield return new ValidationResult(
                "La fecha inicial debe ser menor que la fecha final.",
                [nameof(FechaDesde)]);
        }

        if ((FechaHasta - FechaDesde).TotalDays > 60)
        {
            yield return new ValidationResult(
                "El rango de fechas no puede superar 60 días.",
                [nameof(FechaDesde), nameof(FechaHasta)]);
        }
        // Fin código generado por GitHub Copilot

        // Inicio refactorización/optimización por GitHub Copilot
        // Validación de caracteres según tipo de documento; sin mínimo para soportar búsquedas parciales
        foreach (ValidationResult error in DocumentoFacturacionHelper.Validar(NumeroDocumento, TipoDocumento))
            yield return error;
        // Fin refactorización/optimización por GitHub Copilot
    }
    // Fin código generado por GitHub Copilot
}
// Fin código generado por GitHub Copilot
