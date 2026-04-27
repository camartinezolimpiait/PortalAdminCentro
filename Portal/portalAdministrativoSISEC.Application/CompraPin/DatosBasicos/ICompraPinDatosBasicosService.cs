namespace portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;

public interface ICompraPinDatosBasicosService
{
    CompraPinDatosBasicosValidationResult ValidateBirthDate(int? day, int? month, int? year, DateTime today);

    bool HasCompleteDate(int? day, int? month, int? year);

    CompraPinDatosBasicosRulesResult ApplyRules(CompraPinDatosBasicosRulesInput input, int edad, DateTime fechaNacimiento);
}
