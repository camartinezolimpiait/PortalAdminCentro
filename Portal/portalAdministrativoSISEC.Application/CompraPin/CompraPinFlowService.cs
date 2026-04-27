using System.Linq;

namespace portalAdministrativoSISEC.Application.CompraPin;

public sealed class CompraPinFlowService : ICompraPinFlowService
{
    private const int ClienteCea = 8;
    private const int PasoDatosBasicos = 1;
    private const int PasoCategoriaComboCarro = 4;
    private const int PasoTipoTramiteComboMoto = 5;
    private const int PasoCategoriaComboMoto = 6;
    private const int PasoDatosPersonales = 10;

    public CompraPinFlowDecision ActivarTipoTramite()
    {
        return new CompraPinFlowDecision(CompraPinComponent.TipoTramite, PasoDatosBasicos);
    }

    public CompraPinFlowDecision ActivarSiguienteDespuesDeDatosPersonales(CompraPinFlowInput input)
    {
        return input.EmisionOtraPersona
            ? new CompraPinFlowDecision(CompraPinComponent.FacturaElectronica)
            : new CompraPinFlowDecision(CompraPinComponent.MediosPago);
    }

    public CompraPinFlowDecision ActivarMediosPago()
    {
        return new CompraPinFlowDecision(CompraPinComponent.MediosPago);
    }

    public CompraPinFlowDecision SeleccionarMedioPago(CompraPinFlowInput input)
    {
        var medioSeleccionado = input.MediosPago?
            .FirstOrDefault(x => x.Id == input.TipoRecaudoCtrl);

        if (input.ClienteCompra == ClienteCea)
        {
            return medioSeleccionado?.ComprasCuotas == true
                ? new CompraPinFlowDecision(CompraPinComponent.CuotaCeas, RequiereObtenerCosto: true)
                : new CompraPinFlowDecision(CompraPinComponent.ConfirmarCompra, RequiereObtenerCosto: true);
        }

        return new CompraPinFlowDecision(
            CompraPinComponent.ConfirmarCompra,
            ObtenerPagoCrc: true,
            RequiereObtenerCosto: true);
    }

    public CompraPinFlowDecision SeleccionarTramite(CompraPinFlowInput input)
    {
        var paso = input.PasoCotizacion;

        if (input.OpcionTramite == 1)
        {
            paso = PasoDatosPersonales;
        }
        else if (input.PasoCotizacion == PasoTipoTramiteComboMoto)
        {
            paso = PasoCategoriaComboMoto;
        }
        else if (input.PasoCotizacion != PasoCategoriaComboCarro
              && input.PasoCotizacion != PasoCategoriaComboMoto)
        {
            paso = PasoCategoriaComboCarro;
        }

        return new CompraPinFlowDecision(CompraPinComponent.Categoria, paso);
    }

    public CompraPinFlowDecision SeleccionarCategoria(CompraPinFlowInput input, bool isValid)
    {
        return isValid
            ? new CompraPinFlowDecision(CompraPinComponent.DatosPersonales, RequiereObtenerCosto: true)
            : new CompraPinFlowDecision(CompraPinComponent.TipoTramite, PasoCategoriaComboMoto);
    }
}
