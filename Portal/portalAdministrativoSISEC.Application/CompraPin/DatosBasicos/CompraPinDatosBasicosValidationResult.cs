namespace portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;

public sealed record CompraPinDatosBasicosValidationResult(
    bool IsValid,
    string? ErrorMessage = null);
