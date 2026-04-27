using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Application.Data.CompraPin.Wompi;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Entidades.Pago.Wompi;
using portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Response;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using portalAdministrativoSISEC.Entidades.Recaptcha;
using portalAdministrativoSISEC.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Application.Data.Pines.Cuotas;
using portalAdministrativoSISEC.Application.Data.Agendamiento;
using static portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Request.ReferenciaBancolombiaCdaWompi;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Entidades.Notificaciones;

namespace portalAdministrativoSISEC.Application.Contracts.MiLicencia
{
    public interface IMiLicenciaService
    {
        #region Public Methods

        Task ShowNotificacion(NotificationStatus status, string texto);

        Task<TokenModel> GetToken();

        Task<List<Data.CompraPin.Categoria>> ObtenerCategorias();

        Task<int> CalcularEdadAspirante(string fechaNacimiento);

        Task<CotizacionPin> GeneracionCosto(PagoPin pagoPin, int IdCategoria);
        Task<CotizacionPin> ObtenerPrecioPIN(ParametrosCostoPin parametrosCostoPin);

        Task<CotizacionPinCDA> GeneracionCostoCDA(PagoPinCDA pagoPin);

        Task<List<ConvenioCentro>> ObtenerConveniosCEA(ConsultaCentoId IdCentro);
        Task<List<ConvenioCentro>> ObtenerConveniosCRC(ConsultaCentoId IdCentro);

        Task<List<TipoDocumentoPtesaDTO>> ObtenerTipoDocumentos();

        Task<TokenAceptacion> ObtenerTokenAceptacion(int IdCliente);

        Task<ResponseReferenciaWompi> GenerarReferenciaBancolombiaWompi(ReferenciaBancolombiaWompi referenciaBancolombiaWompi);

        Task<EntidadMensaje> ConstruirCorreoWompi(CorreoWompi result);

        Task<ResponseDTO<List<ResponseInfoPinEstado>>> ConsultaInfoPinesPorEstado(ConsultaInfoPinEstado request);

        Task<ResponseDTO<List<ResponseConsultaDevolucion>>> ConsultaDevolucionesPines(ConsultaInfoPinEstado request);

        Task<ResponseDTO<List<ResponseConsultaDispersion>>> ConsultarDispersion(ConsultaDispersion request);

        Task<ResponseDTO<List<ResponseDetallePin>>> ConsultarDetallePin(ConsultaDetallePin request);

        Task<ResponseDTO<List<ResponsePinesAsociados>>> ConsultarPinesAsociados(string pin, string view = "");

        Task<List<Centro>> ObtenerTodosCentrosxComercio(ConsultaCentroPorComercio consulta);

        Task<List<AgenteDispersionResponseDTO>> ConsultarAgentesDispersion();

        Task<RespuestaRecaptchaDinamica<T>> DevolucionByPin<T>(ConsultaDevolucionPorPinRequest consultaDevolucionPorPin, bool IsAnulacion);

        Task<ResponseDTO<Entidad>> ConsultaInfoPin<T>(ConsultaDevolucionPorPinRequest consultaDevolucionPorPin);

        Task<ResponsePagoCuota<RespuestaGeneric<object>>> RecaudarCuotaPin(PagoCuotaRequest request);

        void LogMessage(string message);

        Task<List<Data.CompraPin.Categoria>> ConsultaCategoriasCentro(long idCentro);

        Task<ResponseDTO<EntidadCuotas>> ConsultarValorCuotaAliado(ConsultaValorAliado consultaValorAliado);

        Task<ResponseEnvioCorreoCuotas> ConstruirCorreoPagoCuotas(NotificacionPagoCuotas request);

        Task<CotizacionPinCDA> ObtenerCostoCDA(ConsultaCostoPinCda consultaCostoPinCda);

        Task<List<ResponseTipoVehiculos>> ObtenerTipoVehiculos(RequestTipoVehiculos tipoVehiculo);

        Task<ResponseCdaWompi> GenerarReferenciaPinCDA(RequestCreacionPinCDA referenciaBancolombiaWompi);

        Task<ResponseNotificacionCorreoCDA> ConstruirCorreoCDA(RequestNotificacionCDA result);

        Task<ResponseNotificacionCitas> ConstruirCorreoCitas(RequestNotificacionCitas result);

        Task<FileBase64> ConsultarComprobanteDevolucion(ConsultaComprobanteDevolucionRequest consultaComprobanteDevolucion);

        Task<ResponseDTO<List<ResponseConsultarPinesAsociados>>> ConsultarPinesAsociadosPinDirecto(PinesAsociadosRequest request);

        Task<long> SendCancelationNotification(RequestNotificationCancelation result);

        Task<long> SendRescheduleNotification(RequestNotificationCancelation result);

        Task<List<Departamentos>> ObtenerDepartamentos();

        Task<List<Municipios>> ObtenerMunicipios(int idDepartamento);

        Task<RespuestaAcciones> ConsultaEstadoDevolucion(ConsultaEstadoDevolucion consultaEstadoDevolucion, string googleCaptcha = "");

        Task<int> GetTipoCliente(string pin);

        Task<List<Data.CompraPin.Categoria>> ObtenerCategoriasCrc();

        #region Facturacion electronica

        Task<FacturacionResponse<ConfiguracionFacturacion>> ConsultarConfiguracionFacturacionElectronica(ConsultaEstadoFacturacion request);

        Task<FacturacionResponse<EstadoFacturacion>> ConsultarEstadoFacturacionElectronica(ConsultaEstadoFacturacion request);

        Task<FacturacionResponse<EstadoCredenciales>> ValidarEstadoCredencial(CredencialesProveedor request);

        Task<FacturacionResponse<List<ConsultaMunicipios>>> ConsultarMunicipiosDepartamentos();

        Task<FacturacionResponse<List<ConsultaGenericaTipos>>> ConsultarGenericaTipos(EnumTiposFacturacion tiposFacturacion);

        Task<FacturacionResponse<object>> AlmacenamientoDatosEmision(DatosEmision request);

        Task<FacturacionResponse<object>> AlmacenamientoDatosNumeracion(DatosNumeracion request);

        Task<FacturacionResponse<object>> AlmacenamientoComportamiento(DatosComportamiento request);

        Task<FacturacionResponse<DatosArticulos>> ConsultarListadoArticulos(ConsultaArticuloActivo request);

        Task<FacturacionResponse<RespuestaErrorFacturacion>> ConsultarEstadoSistema(ConsultaEstadoFacturacion request);

        Task<FacturacionResponse<object>> AlmacenarConfiguracionArticulos(DatosArticulos request);

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Obtiene las solicitudes de facturación electrónica según los criterios de búsqueda
        /// </summary>
        Task<FacturacionResponse<DataWrapper<PaginatedData>>> GetElectronicBillingRequests(
            RequestFilter consulta);

        /// <summary>
        /// Obtiene el historial de estados de una solicitud de facturación electrónica
        /// </summary>
        Task<FacturacionResponse<DataWrapper<HistoricData>>> GetElectronicBillingRequestDetail(int idPtesaPIN);
        // Fin código generado por GitHub Copilot

        /// Obtiene los tipos de documento habilitados para facturación electrónica
        /// Filtra por la propiedad VisualizarFacturacionElectronica
        /// </summary>
        Task<List<TipoDocumentoFacturacionElectronica>> ObtenerTiposDocumentoFacturacionElectronica();

        Task<FacturacionResponse<CancelBillingRequestResult>> CancelBillingRequest(UpdateElectronicBillingRequest request);

        #endregion Facturación electronica

        Task<FacturacionResponse<List<ConsultaGenericaTipos>>> GetPersonTypes();

        Task SendProcessNotificationPse(string pinGenerado);

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Valida un código de cupón de convenio
        /// </summary>
        /// <param name="request">Solicitud con el código del convenio y tipo de negocio</param>
        /// <returns>Respuesta con información de la empresa si es válido</returns>
        Task<ResponseDTO<ValidateCouponCompanyResponse>> ValidateCoupon(ValidateCouponRequest request);
        // Fin código generado por GitHub Copilot

        /// <summary>
        /// Obtiene la lista de empresas disponibles para el filtro de pines.
        /// </summary>
        /// <param name="codigoRunt">Código RUNT del centro.</param>
        /// <returns>ResponseDTO con la lista de nombres de empresa.</returns>
        Task<ResponseDTO<List<string>>> ObtenerEmpresasPines(string codigoRunt);

        #endregion Public Methods
    }
}

