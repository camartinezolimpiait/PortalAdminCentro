using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace portalAdministrativoSISEC.Pages.Facturacion.Models;

/// <summary>
/// Centraliza reglas de validación y restricciones de entrada del campo Número de Documento
/// para el módulo de consulta de facturación electrónica.
/// No se impone longitud mínima para soportar búsquedas parciales.
/// </summary>
public static class DocumentoFacturacionHelper
{
    // Método generado por GitHub Copilot
    // Inicio codigo generado por GitHub Copilot
    private sealed record Constraints(int MaxLength, bool SoloNumeros, string MensajeCaracteres);

    // Keyed by IdTipoSisec (entero). Los IDs corresponden a EnumTipoDocumento / catálogo SISEC.
    private static readonly Dictionary<int, Constraints> Reglas = new()
    {
        { 1,  new(10, true,  "La Cédula de Ciudadanía solo acepta números (máx. 10 dígitos).") },
        { 2,  new(10, true,  "La Cédula de Extranjería solo acepta números (máx. 10 dígitos).") },
        { 3,  new(11, true,  "La Tarjeta de Identidad solo acepta números (máx. 11 dígitos).") },
        { 4,  new(10, true,  "El NIT solo acepta números (máx. 10 dígitos).") },
        { 5,  new(24, false, "El Pasaporte acepta letras y números (máx. 24 caracteres).") },
        { 10, new(10, true,  "La Contraseña de Cédula de Ciudadanía solo acepta números (máx. 10 dígitos).") },
        { 11, new(10, true,  "La Contraseña de Cédula de Extranjería solo acepta números (máx. 10 dígitos).") },
        { 13, new(10, true,  "El PPT solo acepta números (máx. 10 dígitos).") },
        { 15, new(10, true,  "El PEP solo acepta números (máx. 10 dígitos).") },
        { 16, new(10, true,  "El NUIP solo acepta números (máx. 10 dígitos).") },
    };

    /// <summary>
    /// Máximo de caracteres permitidos para el tipo dado.
    /// Retorna <see cref="int.MaxValue"/> cuando no aplica restricción (caso "Todos").
    /// </summary>
    public static int GetMaxLength(string idTipoSisec)
        => TryGet(idTipoSisec, out var c) ? c.MaxLength : int.MaxValue;

    /// <summary>
    /// inputmode HTML adecuado para el tipo de documento ("numeric" | "text").
    /// </summary>
    public static string GetInputMode(string idTipoSisec)
        => TryGet(idTipoSisec, out var c) && c.SoloNumeros ? "numeric" : "text";

    /// <summary>
    /// Valida el número de documento contra las restricciones de caracteres del tipo seleccionado.
    /// No genera errores si el campo está vacío (búsqueda parcial permitida).
    /// </summary>
    public static IEnumerable<ValidationResult> Validar(string numero, string idTipoSisec)
    {
        if (string.IsNullOrWhiteSpace(numero)) yield break;
        if (!TryGet(idTipoSisec, out var c)) yield break;

        string patron = c.SoloNumeros ? @"^\d*$" : @"^[0-9a-zA-Z]*$";
        if (!Regex.IsMatch(numero, patron))
            yield return new ValidationResult(c.MensajeCaracteres, new[] { "NumeroDocumento" });
    }

    /// <summary>
    /// Elimina caracteres no permitidos para el tipo dado.
    /// Se invoca en tiempo real (oninput) para evitar entradas inválidas.
    /// </summary>
    public static string FiltrarCaracteres(string valor, string idTipoSisec)
    {
        if (string.IsNullOrEmpty(valor)) return valor;
        if (!TryGet(idTipoSisec, out var c)) return valor;

        string patron = c.SoloNumeros ? @"[^\d]" : @"[^0-9a-zA-Z]";
        return Regex.Replace(valor, patron, string.Empty);
    }

    private static bool TryGet(string idTipoSisec, out Constraints constraints)
    {
        constraints = null;
        return int.TryParse(idTipoSisec, out int id) && Reglas.TryGetValue(id, out constraints);
    }
}
// Fin codigo generado por GitHub Copilot