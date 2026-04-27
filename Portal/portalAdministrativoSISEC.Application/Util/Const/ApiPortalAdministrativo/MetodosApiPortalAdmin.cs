namespace portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo
{
    public static class MetodosApiPortalAdmin
    {
        #region Fields

        public const string GET_TOKEN_ML = "Principal/GetTokenML";
        public const string GET_CATEGORIAS = "Principal/ObtenerCategorias";
        public const string POST_COSTOPINCEA = "Principal/CostoPinCEA";
        public const string POST_COSTOPINCDA = "Principal/CostoPinCdaAsync";
        public const string POST_OBTENER_CATEGORIAVEHICULO = "Principal/ObtenerVehiculoCdaAsync";
        public const string OBTENER_EDAD = "Principal/ObtenerEdad/";
        public const string OBTENER_CONVEVIOS_CEA = "Principal/ObtenerConveniosCentroCEA";
        public const string OBTENER_TIPOS_DOCUMENTO = "Principal/obtenerTipoDocumentos";
        public const string OBTENER_TOKEN_ACEPTACION = "Principal/ObtenerTokenAceptacion";
        public const string GENERAR_REFERENCIA_BANCOLOMBIA_WOMPI = "Principal/GenerarReferenciaBancolombiaWompi";
        public const string GENERAR_CORREO_WOMPI = "Principal/ConstruirCorreoWompi";
        public const string CONSULTA_INFO_PINES_POR_ESTADO = "Principal/ConsultaInfoPinesPorEstado";
        public const string CONSULTA_INFO_PINES_POR_ESTADO_PIN_DIRECTO = "Principal/ConsultarPinesAsociadosPinDirecto";
        public const string CONSULTA_DEVOLUCIONES_PINES = "Principal/ConsultaDevolucionesPines";
        public const string OBTENER_EMPRESAS_PINES = "Principal/GetCompaniesAgreement";
        public const string CONSULTA_DISPERSION = "Principal/ConsultarDispersion";
        public const string CONSULTAR_DETALLE_PIN = "Principal/ConsultarDetallePin";
        public const string CONSULTAR_PINES_ASOCIADOS = "Principal/ConsultarPinesAsociados";
        public const string OBTENER_TODOS_CENTROS = "Principal/ObtenerTodosCentros";
        public const string OBTENER_TODOS_CENTROS_X_COMERCIO = "Principal/ObtenerTodosCentrosxComercio";
        public const string OBTENER_AGENTES_DISPERSION = "Principal/ConsultarAgentesDispersion";
        public const string CONSULTA_INFO_PIN = "Principal/ConsultarInfoPin";

        public const string DEVOLUCION_BY_PIN = "Principal/DevolucionByPin";
        public const string REALIZAR_DEVOLUCION_BY_PIN = "Principal/RealizarDevolucionByPin";
        public const string REGISTRAR_LOG_COTIZADOR = "Principal/RegistrarLogCotizador";
        public const string REGISTER_EXCEPTION_LOG = "Principal/RegisterExceptionLog";
        public const string REGISTER_INFO_LOG = "Principal/RegisterInfoLog";

        public const string CONSULTAR_COMPROBANTE_DEVOLUCION = "Principal/ConsultarComprobanteDevolucion";

        public const string RECAUDAR_CUOTA_PIN = "Principal/RecaudarCuotaPin";
        public const string CONSULTAR_VALOR_CUOTA_ALIADO = "Principal/ConsultarValorCuotaAliado";

        public const string GENERAR_REFERENCIA_PIN_CDA = "Principal/CrearPinCdaAsync";
        public const string GENERAR_CORREO_NOTIFICACION_CDA = "Principal/EnviarNotificacionCdaAsync";
        public const string GENERAR_CORREO_NOTIFICACION_AGENDA = "PrincipalNotificador/NotificarCita";
        public const string GENERAR_CORREO_NOTIFICACION_CANCELACION = "Principal/CancellationNotification";
        public const string GENERAR_CORREO_NOTIFICACION_REAGENDAMIENTO = "Principal/RescheduleNotification";

        #endregion Fields

        #region Facturación electronica

        public const string CONSULTAR_CONFIGURACION_FACTURACION_ELECTRONICA = "Principal/ConsultarConfiguracionFacturacionElectronica";
        public const string CONSULTAR_ESTADO_FACTURACION_ELECTRONICA = "Principal/ConsultarEstadoFacturacionElectronica";
        public const string VALIDAR_ESTADO_CREDENCIAL = "Principal/ValidarEstadoCredencial";
        public const string CONSULTAR_DEPARTAMENTOS = "Principal/ConsultaDepartamentos";
        public const string CONSULTAR_MUNICIPIOS_DEPARTAMENTOS = "Principal/ConsultarMunicipiosDepartamentos";
        public const string CONSULTAR_TIPO_PERSONA = "Principal/ConsultarTipoPersona";
        public const string AMLACENAMIENTO_DATOS_EMISION = "Principal/AlmacenamientoDatosEmision";
        public const string AMLACENAMIENTO_DATOS_NUMERACION = "Principal/AlmacenamientoDatosNumeracion";
        public const string CONSULTAR_DISPARADORES_FACTURACION = "Principal/ConsultarDisparadoresFacturacion";
        public const string ALMACENAMIENTO_DATOS_COMPORTAMIENTO = "Principal/AlmacenamientoComportamiento";
        public const string CONSULTAR_REGIMEN_CONTRIBUTIVO = "Principal/ConsultarRegimenContributivo";
        public const string CONSULTAR_LISTADO_ARTICULOS = "Principal/ConsultarListadoArticulos";
        public const string CONSULTAR_ESTADO_SISTEMA = "Principal/ConsultarEstadoSistema";
        public const string AMLACENAMIENTO_CONFIGURACION_ARTICULOS = "Principal/AlmacenarConfiguracionArticulos";
        public const string CANCEL_BILLING_REQUEST = "Principal/CancelBillingRequest";

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Endpoint para obtener solicitudes de facturación electrónica
        /// </summary>
        public const string GET_ELECTRONIC_BILLING_REQUESTS = "Principal/GetElectronicBillingRequests";

        /// <summary>
        /// Endpoint para obtener el detalle de una solicitud de facturación
        /// </summary>
        public const string GET_ELECTRONIC_BILLING_REQUEST_DETAIL = "Principal/GetElectronicBillingRequestDetail";

        // Fin código generado por GitHub Copilot

        #endregion Facturación electronica
    }
}