namespace portalAdministrativoSISEC.Application.CompraPin;

public interface ICompraPinFlowService
{
    CompraPinFlowDecision ActivarTipoTramite();

    CompraPinFlowDecision ActivarSiguienteDespuesDeDatosPersonales(CompraPinFlowInput input);

    CompraPinFlowDecision ActivarMediosPago();

    CompraPinFlowDecision SeleccionarMedioPago(CompraPinFlowInput input);

    CompraPinFlowDecision SeleccionarTramite(CompraPinFlowInput input);

    CompraPinFlowDecision SeleccionarCategoria(CompraPinFlowInput input, bool isValid);
}
