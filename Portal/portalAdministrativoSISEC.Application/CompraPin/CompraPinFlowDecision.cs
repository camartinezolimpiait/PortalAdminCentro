namespace portalAdministrativoSISEC.Application.CompraPin;

public sealed record CompraPinFlowDecision(
    CompraPinComponent ActiveComponent,
    int? PasoCotizacion = null,
    bool ObtenerPagoCrc = false,
    bool RequiereObtenerCosto = false);
