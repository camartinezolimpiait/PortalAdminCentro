namespace portalAdministrativoSISEC.Util.Const
{
	public static class MetodosApiFrontMiLicencia
	{

		///  Metodos del Global 
		/// </summary>
		public const string GET_TOKEN_ML = "servicios/Global/Autenticate";
		public const string OBTENER_EDAD = "servicios/Global/ObtenerEdad/";

		/// <summary>
		/// Metodo de Principal
		/// </summary>
		public const string OBTENER_CONVENIOS_CEA_APIGATEWAY = "Principal/ObtenerConveniosCentroCEAPtlAdmin";

		/// <summary>
		/// Metodo de Centro
		/// </summary>
		public const string OBTENER_CONVENIOS_CEA = "servicios/Centro/ObtenerConveniosCentroCEAPtlAdmin";


        /// <summary>
        /// Metodo de CompraPin
        /// </summary>
        public const string POST_COSTOPINCEA = "servicios/CompraPin/CostoPinCEA";
		public const string GET_CATEGORIAS = "servicios/CompraPin/Categorias";


		/// <summary>
		/// Metodos Cotizador
		/// </summary>
		public const string OBTENER_TIPOS_DOCUMENTO = "servicios/Cotizador/obtenerTipoDocumentos";
		public const string OBTENER_TOKEN_ACEPTACION = "servicios/Cotizador/ObtenerTokenAceptacion";
		public const string GENERAR_REFERENCIA_BANCOLOMBIA_WOMPI = "servicios/Cotizador/GenerarReferenciaBancolombiaWompi";
		public const string GENERAR_CORREO_WOMPI = "servicios/Cotizador/ConstruirCorreoWompi";

        public const string OBTENER_CATEGORIAS = "servicios/Cotizador/ObtenerCategorias";


        public const string CONSULTA_INFO_PINES_POR_ESTADO = "servicios/Cotizador/ConsultaInfoPinesPorEstado";
		public const string CONSULTA_DEVOLUCIONES_PINES = "servicios/Cotizador/ConsultaDevolucionesPines";
		public const string CONSULTA_DISPERSION = "servicios/Cotizador/ConsultarDispersion";
		public const string CONSULTAR_DETALLE_PIN = "servicios/Cotizador/ConsultarDetallePin";
		public const string CONSULTAR_PINES_ASOCIADOS = "servicios/Cotizador/ConsultarPinesAsociados";
		public const string OBTENER_TODOS_CENTROS = "servicios/comprapin/ObtenerTodosCentros";
		public const string OBTENER_TODOS_CENTROS_X_COMERCIO = "servicios/comprapin/ObtenerTodosCentrosxComercio";
		public const string OBTENER_AGENTES_DISPERSION = "servicios/Cotizador/ConsultarAgentesDispersion";
        public const string OBTENER_CATEGORIAS_CENTROS = "servicios/Cotizador/ConsultaCategoriasCentro";
        public const string ENVIO_NOTIFICACION_CUOTA = "servicios/Cotizador/ConstruirCorreoPagoCuotas";
        
		public const string OBTENER_DEPARTAMENTOS = "servicios/Cotizador/obtenerDepartamentos";
        public const string OBTENER_MINUCIPIOS = "servicios/Cotizador/obtenerMunicipios/{0}";
		public const string CONSULTA_ESTADO_DEVOLUCION = "servicios/Cotizador/ConsultaEstadoDevolucion";
		public const string OBTENER_TIPO_CLIENTE = "servicios/Cotizador/ObtenerTipoCliente";
		

        public const string OBTENER_VALOR_PIN = "servicios/Cotizador/Costo";
        public const string OBTENER_CONVENIOS = "servicios/Cotizador/Convenio";

		public const string GET_PERSON_TYPES = "servicios/Cotizador/GetPersonType";
		public const string SEND_NOTIFICATION_PSE = "servicios/EmailNotifications/SendNotificationPse";
    }
}
