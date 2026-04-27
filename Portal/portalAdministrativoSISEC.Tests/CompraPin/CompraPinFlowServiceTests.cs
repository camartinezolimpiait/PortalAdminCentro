using portalAdministrativoSISEC.Application.CompraPin;

namespace portalAdministrativoSISEC.Tests.CompraPin;

public class CompraPinFlowServiceTests
{
    private readonly CompraPinFlowService _service = new();

    [Fact]
    public void ActivarTipoTramite_sets_step_and_component()
    {
        var decision = _service.ActivarTipoTramite();

        Assert.Equal(CompraPinComponent.TipoTramite, decision.ActiveComponent);
        Assert.Equal(1, decision.PasoCotizacion);
    }

    [Fact]
    public void DatosPersonales_routes_to_invoice_when_issued_for_another_person()
    {
        var input = CreateInput(emisionOtraPersona: true);

        var decision = _service.ActivarSiguienteDespuesDeDatosPersonales(input);

        Assert.Equal(CompraPinComponent.FacturaElectronica, decision.ActiveComponent);
    }

    [Fact]
    public void DatosPersonales_routes_to_payment_when_invoice_is_not_required()
    {
        var input = CreateInput(emisionOtraPersona: false);

        var decision = _service.ActivarSiguienteDespuesDeDatosPersonales(input);

        Assert.Equal(CompraPinComponent.MediosPago, decision.ActiveComponent);
    }

    [Fact]
    public void MedioPago_for_cea_with_installments_routes_to_installments()
    {
        var input = CreateInput(
            clienteCompra: 8,
            tipoRecaudoCtrl: 10,
            mediosPago: [new MedioPagoSelection(10, true)]);

        var decision = _service.SeleccionarMedioPago(input);

        Assert.Equal(CompraPinComponent.CuotaCeas, decision.ActiveComponent);
        Assert.True(decision.RequiereObtenerCosto);
        Assert.False(decision.ObtenerPagoCrc);
    }

    [Fact]
    public void MedioPago_for_crc_routes_to_confirmation_and_marks_crc_payment()
    {
        var input = CreateInput(clienteCompra: 3);

        var decision = _service.SeleccionarMedioPago(input);

        Assert.Equal(CompraPinComponent.ConfirmarCompra, decision.ActiveComponent);
        Assert.True(decision.ObtenerPagoCrc);
        Assert.True(decision.RequiereObtenerCosto);
    }

    [Fact]
    public void Tramite_single_option_goes_to_personal_data()
    {
        var input = CreateInput(opcionTramite: 1, pasoCotizacion: 3);

        var decision = _service.SeleccionarTramite(input);

        Assert.Equal(CompraPinComponent.Categoria, decision.ActiveComponent);
        Assert.Equal(10, decision.PasoCotizacion);
    }

    [Fact]
    public void Categoria_valid_goes_to_personal_data_and_requires_cost()
    {
        var input = CreateInput();

        var decision = _service.SeleccionarCategoria(input, isValid: true);

        Assert.Equal(CompraPinComponent.DatosPersonales, decision.ActiveComponent);
        Assert.True(decision.RequiereObtenerCosto);
    }

    private static CompraPinFlowInput CreateInput(
        int clienteCompra = 8,
        int? opcionTramite = null,
        int pasoCotizacion = 1,
        bool emisionOtraPersona = false,
        int? tipoRecaudoCtrl = null,
        IReadOnlyCollection<MedioPagoSelection>? mediosPago = null)
    {
        return new CompraPinFlowInput(
            clienteCompra,
            opcionTramite,
            pasoCotizacion,
            emisionOtraPersona,
            tipoRecaudoCtrl,
            mediosPago ?? []);
    }
}
