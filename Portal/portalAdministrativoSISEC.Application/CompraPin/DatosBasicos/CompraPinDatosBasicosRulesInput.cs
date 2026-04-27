namespace portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;

public sealed record CompraPinDatosBasicosRulesInput(
    int? Dia,
    int? Mes,
    int? Anio,
    int? Sexo,
    int EdadActual,
    bool TramiteInstructor,
    int? OpcionTramite,
    int? TipoTramite);
