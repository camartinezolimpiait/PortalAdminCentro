using System.Collections.Generic;

namespace portalAdministrativoSISEC.Application.CompraPin;

public sealed record CompraPinFlowInput(
    int ClienteCompra,
    int? OpcionTramite,
    int PasoCotizacion,
    bool EmisionOtraPersona,
    int? TipoRecaudoCtrl,
    IReadOnlyCollection<MedioPagoSelection> MediosPago);
