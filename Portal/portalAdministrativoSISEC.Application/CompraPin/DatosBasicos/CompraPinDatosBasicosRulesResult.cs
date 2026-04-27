namespace portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;

public sealed record CompraPinDatosBasicosRulesResult(
    bool IsValid,
    int EdadAspirante = 0,
    bool MayorEdad = false,
    DateTime? FechaNacimiento = null,
    int? Genero = null,
    bool ResetCategorias = false,
    bool ResetTipoDocumento = false,
    bool ResetInstructorFlow = false,
    bool ResetRecategorizacionFlow = false);
