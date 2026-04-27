namespace portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;

public sealed class CompraPinDatosBasicosService : ICompraPinDatosBasicosService
{
    private const int RecategorizarTramite = 3;

    public CompraPinDatosBasicosValidationResult ValidateBirthDate(int? day, int? month, int? year, DateTime today)
    {
        if (!HasCompleteDate(day, month, year))
        {
            return new CompraPinDatosBasicosValidationResult(false, "La fecha de nacimiento no es válida");
        }

        if (!DateTime.TryParse($"{year}-{month}-{day}", out var birthDate))
        {
            return new CompraPinDatosBasicosValidationResult(false, "La fecha de nacimiento no es válida");
        }

        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age < 16
            ? new CompraPinDatosBasicosValidationResult(false, "Debe ser mayor de 16 años")
            : new CompraPinDatosBasicosValidationResult(true);
    }

    public bool HasCompleteDate(int? day, int? month, int? year)
    {
        return day.HasValue && day.Value != 0
            && month.HasValue && month.Value != 0
            && year.HasValue && year.Value != 0;
    }

    public CompraPinDatosBasicosRulesResult ApplyRules(CompraPinDatosBasicosRulesInput input, int edad, DateTime fechaNacimiento)
    {
        if (!HasCompleteDate(input.Dia, input.Mes, input.Anio))
        {
            return new CompraPinDatosBasicosRulesResult(false);
        }

        if (input.Dia > DateTime.DaysInMonth(input.Anio!.Value, input.Mes!.Value))
        {
            return new CompraPinDatosBasicosRulesResult(false);
        }

        if (edad < 16 || edad > 100)
        {
            return new CompraPinDatosBasicosRulesResult(false);
        }

        var resetCategorias = false;
        var resetTipoDocumento = false;

        if (input.EdadActual != edad)
        {
            if (edad < 18)
            {
                resetCategorias = true;
            }
            else
            {
                resetTipoDocumento = true;
            }
        }

        var resetInstructorFlow = edad < 18 && input.TramiteInstructor;
        var resetRecategorizacionFlow = edad < 18
            && input.OpcionTramite.HasValue
            && input.TipoTramite == RecategorizarTramite;

        return new CompraPinDatosBasicosRulesResult(
            true,
            EdadAspirante: edad,
            MayorEdad: edad >= 18,
            FechaNacimiento: fechaNacimiento,
            Genero: input.Sexo,
            ResetCategorias: resetCategorias,
            ResetTipoDocumento: resetTipoDocumento,
            ResetInstructorFlow: resetInstructorFlow,
            ResetRecategorizacionFlow: resetRecategorizacionFlow);
    }
}
