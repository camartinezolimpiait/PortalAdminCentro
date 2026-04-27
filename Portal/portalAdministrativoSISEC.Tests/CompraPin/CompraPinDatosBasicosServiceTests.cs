using portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;

namespace portalAdministrativoSISEC.Tests.CompraPin;

public class CompraPinDatosBasicosServiceTests
{
    private readonly CompraPinDatosBasicosService _service = new();

    [Fact]
    public void ValidateBirthDate_returns_error_when_date_is_invalid()
    {
        var result = _service.ValidateBirthDate(31, 2, 2010, new DateTime(2026, 4, 26));

        Assert.False(result.IsValid);
        Assert.Equal("La fecha de nacimiento no es válida", result.ErrorMessage);
    }

    [Fact]
    public void ValidateBirthDate_returns_error_when_age_is_less_than_16()
    {
        var result = _service.ValidateBirthDate(1, 1, 2012, new DateTime(2026, 4, 26));

        Assert.False(result.IsValid);
        Assert.Equal("Debe ser mayor de 16 años", result.ErrorMessage);
    }

    [Fact]
    public void ApplyRules_resets_categories_for_minor_when_age_changes()
    {
        var input = new CompraPinDatosBasicosRulesInput(
            Dia: 1,
            Mes: 1,
            Anio: 2010,
            Sexo: 1,
            EdadActual: 20,
            TramiteInstructor: false,
            OpcionTramite: null,
            TipoTramite: null);

        var result = _service.ApplyRules(input, edad: 17, fechaNacimiento: new DateTime(2010, 1, 1));

        Assert.True(result.IsValid);
        Assert.False(result.MayorEdad);
        Assert.True(result.ResetCategorias);
        Assert.False(result.ResetTipoDocumento);
    }

    [Fact]
    public void ApplyRules_resets_instructor_flow_for_minor()
    {
        var input = new CompraPinDatosBasicosRulesInput(
            Dia: 1,
            Mes: 1,
            Anio: 2009,
            Sexo: 1,
            EdadActual: 18,
            TramiteInstructor: true,
            OpcionTramite: 1,
            TipoTramite: 1);

        var result = _service.ApplyRules(input, edad: 17, fechaNacimiento: new DateTime(2009, 1, 1));

        Assert.True(result.IsValid);
        Assert.True(result.ResetInstructorFlow);
    }

    [Fact]
    public void ApplyRules_resets_recategorization_flow_for_minor()
    {
        var input = new CompraPinDatosBasicosRulesInput(
            Dia: 1,
            Mes: 1,
            Anio: 2009,
            Sexo: 1,
            EdadActual: 18,
            TramiteInstructor: false,
            OpcionTramite: 2,
            TipoTramite: 3);

        var result = _service.ApplyRules(input, edad: 17, fechaNacimiento: new DateTime(2009, 1, 1));

        Assert.True(result.IsValid);
        Assert.True(result.ResetRecategorizacionFlow);
    }
}
