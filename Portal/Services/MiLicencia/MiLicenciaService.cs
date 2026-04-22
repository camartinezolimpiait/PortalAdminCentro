using Blazored.Toast.Services;
using Newtonsoft.Json;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.Agendamiento;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Data.CompraPin.Wompi;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Data.Pines.Cuotas;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Entidades.Notificaciones;
using portalAdministrativoSISEC.Entidades.Pago.Wompi;
using portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Response;
using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using portalAdministrativoSISEC.Entidades.Recaptcha;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Const;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using portalAdministrativoSISEC.Util.Helpers;
using portalAdministrativoSISEC.Util.LogAuditoria;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Request.ReferenciaBancolombiaCdaWompi;

namespace portalAdministrativoSISEC.Services.MiLicencia
{
    public class MiLicenciaService : IMiLicenciaService
    {
        #region Fields

        private string _filePath;

        #endregion Fields

        #region Inyeccion Dependencias

        private readonly IPortalAdministrativoService _portalAdministrativoService;
        private readonly IApiMilicenciaService _apiMilicenciaService;
        private readonly IToastService _toastService;
        private readonly IAesEncryptionHelper _aesEncryptionHelper;

        #endregion Inyeccion Dependencias

        #region Constructor

        public MiLicenciaService(IPortalAdministrativoService portalAdministrativoService, IToastService toastService, IApiMilicenciaService apiMilicenciaService, IAesEncryptionHelper aesEncryptionHelper)
        {
            _portalAdministrativoService = portalAdministrativoService;
            _apiMilicenciaService = apiMilicenciaService;
            _toastService = toastService;
            _aesEncryptionHelper = aesEncryptionHelper;
            _filePath = @"C:\SISEC\log.txt";
        }

        #endregion Constructor

        #region Notificaciones

        public async Task ShowNotificacion(NotificationStatus status, string texto)
        {
            switch ((int)status)
            {
                case 1:
                    _toastService.ShowSuccess(texto, "CORRECTO");
                    break;

                case 2:
                    _toastService.ShowInfo(texto, "INFORMACIÓN");
                    break;

                case 3:
                    _toastService.ShowWarning(texto, "ADVERTENCIA");
                    break;

                case 4:
                    _toastService.ShowError(texto, "ERROR");
                    break;
            }

            await Task.FromResult(true);
        }

        #endregion Notificaciones

        #region Servicios

        /// <summary>
        /// Llama en metodo GetTokenML
        /// </summary>
        /// <returns></returns>
        public async Task<TokenModel> GetToken()
        {
            TokenModel token = await _portalAdministrativoService.ConsumirServicioGET<TokenModel>(MetodosApiPortalAdmin.GET_TOKEN_ML, false);
            return token;
        }

        /// <summary>
        /// Llama el metodo Categorias de ApiFrontMilicencia
        /// Obtiene el listado completo de las categorias configuradas para los CEAs
        /// </summary>
        /// <returns></returns>
        public async Task<List<Data.CompraPin.Categoria>> ObtenerCategorias()
        {
            List<Data.CompraPin.Categoria> categorias = [];
            try
            {
                categorias = await _apiMilicenciaService.ConsumirServicioGET<List<Data.CompraPin.Categoria>>(MetodosApiFrontMiLicencia.GET_CATEGORIAS, true);
                return categorias;
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                // Pendiente captura de errores
                return categorias;
            }
        }

        /// <summary>
        /// Llama el metodo Categorias de ApiFrontMilicencia
        /// Obtiene todo el listado de las categorias configuradas para los CRC
        /// </summary>
        /// <returns></returns>
        public async Task<List<Data.CompraPin.Categoria>> ObtenerCategoriasCrc()
        {
            List<Data.CompraPin.Categoria> categorias = new();
            try
            {
                categorias = await _apiMilicenciaService.ConsumirServicioGET<List<Data.CompraPin.Categoria>>(MetodosApiFrontMiLicencia.OBTENER_CATEGORIAS, true);
                return categorias;
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                // Pendiente captura de errores
                return categorias;
            }
        }

        /// <summary>
        /// Obtiene el calculo de la edad del aspirante
        /// </summary>
        /// <param name="fechaNacimiento">fecha de nacimiento MM-DD-YYYY</param>
        /// <returns></returns>
        public async Task<int> CalcularEdadAspirante(string fechaNacimiento)
        {
            try
            {
                int edad = await _apiMilicenciaService.ConsumirServicioGET<int>(MetodosApiFrontMiLicencia.OBTENER_EDAD + fechaNacimiento);
                return edad;
            }
            catch (System.Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                // Pendiente captura de errores
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el TokenAceptacion requerido para el metodo de pago Wompi
        /// </summary>
        /// </summary>
        /// <param name="IdCliente"></param>
        /// <returns></returns>
        public async Task<TokenAceptacion> ObtenerTokenAceptacion(int IdCliente)
        {
            try
            {
                TokenAceptacion tokenAceptacion = await _apiMilicenciaService
                    .ConsumirServicioGET<TokenAceptacion>($"{MetodosApiFrontMiLicencia.OBTENER_TOKEN_ACEPTACION}/{IdCliente}");
                return tokenAceptacion;
            }
            catch (System.Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new TokenAceptacion();
            }
        }

        /// <summary>
        /// Construye el objeto ConsultaCostoPinCea para consumir el metodo ObtenerCosto
        /// </summary>
        /// <param name="pagoPin"></param>
        /// <param name="IdCategoria"></param>
        /// <returns></returns>
        public async Task<CotizacionPin> GeneracionCosto(PagoPin pagoPin, int IdCategoria)
        {
            try
            {
                ConsultaCostoPinCea consulta = new()
                {
                    IdCentro = int.Parse(pagoPin.CentroSeleccionado.IdCentro.ToString()),
                    IdTramite = (int)pagoPin.TipoTramite,
                    IdCategoria = IdCategoria,
                    Edad = pagoPin.EdadAspirante != 0 && pagoPin.EdadAspirante >= 16 ? pagoPin.EdadAspirante : 0,
                    Genero = int.Parse(pagoPin.Usuario.Genero.ToString()),
                    IsPindirecto =
                        pagoPin?.TipoRecaudoCtrl.HasValue == true &&
                        pagoPin.TipoRecaudoCtrl.Value > 0 &&
                        pagoPin.TipoRecaudoCtrl.Value == (int)EnumTipoPago.PinDirecto,
                    ValorAliado = pagoPin?.TipoRecaudoCtrl.HasValue == true &&
                        pagoPin.TipoRecaudoCtrl.Value > 0 &&
                        pagoPin.TipoRecaudoCtrl.Value == (int)EnumTipoPago.PinDirecto
                    ? pagoPin?.ConfiguracionCuotas?.ValorAliado.ToString()!
                    : "0"
                };

                return await ObtenerCosto(consulta);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new CotizacionPin();
            }
        }

        /// <summary>
        /// Llama el metodo ObtenerConveniosCentroCEAPtlAdmin para obtener los datos del convenio PinDirecto
        /// para el idCentro que pasa por el parametro
        /// </summary>
        /// <param name="IdCentro"></param>
        /// <returns>convenioCentroPredeterminado </returns>
        ///

        public async Task<List<ConvenioCentro>> ObtenerConveniosCEA(ConsultaCentoId IdCentro)
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioPOST<List<ConvenioCentro>>(MetodosApiFrontMiLicencia.OBTENER_CONVENIOS_CEA, IdCentro, true);
            }
            catch (Exception ex)
            {
                await SaveLog(IdCentro, ex, new StackFrame(1).GetMethod()?.Name);

                var convenioCentroPredeterminado = new ConvenioCentro
                {
                    IdOrigenPin = 0,
                    ConvenioNombre = null,
                    VentaConvenio = null,
                    NombreInterno = null,
                    GeneraPin = false,
                };
                return [convenioCentroPredeterminado];
            }
        }

        public async Task<List<ConvenioCentro>> ObtenerConveniosCRC(ConsultaCentoId IdCentro)
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioPOST<List<ConvenioCentro>>(MetodosApiFrontMiLicencia.OBTENER_CONVENIOS, IdCentro, true);
            }
            catch (Exception ex)
            {
                var convenioCentroPredeterminado = new ConvenioCentro
                {
                    IdOrigenPin = 0,
                    ConvenioNombre = null,
                    VentaConvenio = null,
                    NombreInterno = null,
                    GeneraPin = false,
                };
                return new List<ConvenioCentro> { convenioCentroPredeterminado };
            }
        }

        /// <summary>
        /// Hace llamado al metodo obtenerTipoDocumentos del APIFrontMiLicencia para obtener el listado de tipos de documento como
        /// el nombre del metodo lo indica
        /// </summary>
        /// <returns>List<TipoDocumentoPtesaDTO></returns>
        public async Task<List<TipoDocumentoPtesaDTO>> ObtenerTipoDocumentos()
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioGET<List<TipoDocumentoPtesaDTO>>(MetodosApiFrontMiLicencia.OBTENER_TIPOS_DOCUMENTO, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return [];
            }
        }

        /// <summary>
        /// Hace la petición a GenerarReferenciaBancolombiaWompi para generar el PIN
        /// </summary>
        /// <param name="referenciaBancolombiaWompi"></param>
        /// <returns>responseReferenciaWompi</returns>
        public async Task<ResponseReferenciaWompi> GenerarReferenciaBancolombiaWompi(ReferenciaBancolombiaWompi referenciaBancolombiaWompi)
        {
            try
            {
                ResponseReferenciaWompi responseReferenciaWompi = await _apiMilicenciaService
                    .ConsumirServicioPOST<ResponseReferenciaWompi>(MetodosApiFrontMiLicencia.GENERAR_REFERENCIA_BANCOLOMBIA_WOMPI, referenciaBancolombiaWompi);
                return responseReferenciaWompi;
            }
            catch (System.Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseReferenciaWompi();
            }
        }

        /// <summary>
        /// llama el metodo ConstruirCorreoWompi del APIFrontMiLicencia para generar la notificación de compra de PIN
        /// </summary>
        /// <param name="result"></param>
        /// <returns>EntidadMensaje</returns>
        public async Task<EntidadMensaje> ConstruirCorreoWompi(CorreoWompi result)
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioPOST<EntidadMensaje>(MetodosApiFrontMiLicencia.GENERAR_CORREO_WOMPI, result);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new EntidadMensaje();
            }
        }

        public async Task<ResponseDTO<List<ResponseInfoPinEstado>>> ConsultaInfoPinesPorEstado(ConsultaInfoPinEstado request)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<List<ResponseInfoPinEstado>>>(MetodosApiPortalAdmin.CONSULTA_INFO_PINES_POR_ESTADO, request, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<List<ResponseInfoPinEstado>>();
            }
        }

        public async Task<ResponseDTO<List<ResponseConsultarPinesAsociados>>> ConsultarPinesAsociadosPinDirecto(PinesAsociadosRequest request)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<List<ResponseConsultarPinesAsociados>>>(MetodosApiPortalAdmin.CONSULTA_INFO_PINES_POR_ESTADO_PIN_DIRECTO, request, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<List<ResponseConsultarPinesAsociados>>();
            }
        }

        public async Task<ResponseDTO<List<ResponseConsultaDevolucion>>> ConsultaDevolucionesPines(ConsultaInfoPinEstado request)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<List<ResponseConsultaDevolucion>>>(MetodosApiPortalAdmin.CONSULTA_DEVOLUCIONES_PINES, request, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<List<ResponseConsultaDevolucion>>();
            }
        }

        public async Task<ResponseDTO<List<ResponseConsultaDispersion>>> ConsultarDispersion(ConsultaDispersion request)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<List<ResponseConsultaDispersion>>>(MetodosApiPortalAdmin.CONSULTA_DISPERSION, request, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<List<ResponseConsultaDispersion>>();
            }
        }

        public async Task<ResponseDTO<List<ResponseDetallePin>>> ConsultarDetallePin(ConsultaDetallePin request)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<List<ResponseDetallePin>>>(MetodosApiPortalAdmin.CONSULTAR_DETALLE_PIN, request, true);
            }
            catch (Exception ex)
            {
                String methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                String componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<List<ResponseDetallePin>>();
            }
        }

        public async Task<ResponseDTO<List<ResponsePinesAsociados>>> ConsultarPinesAsociados(string pin, string view = "")
        {
            try
            {
                ResponseDTO<List<ResponsePinesAsociados>> pinesAsociados = await _portalAdministrativoService.ConsumirServicioGET<ResponseDTO<List<ResponsePinesAsociados>>>($"{MetodosApiPortalAdmin.CONSULTAR_PINES_ASOCIADOS}/{pin}", true);

                if (view == "Usados")
                {
                    List<ResponsePinesAsociados> pinesAsociadosPinDirecto = (await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<List<ResponseConsultarPinesAsociados>>>
                    ($"{MetodosApiPortalAdmin.CONSULTA_INFO_PINES_POR_ESTADO_PIN_DIRECTO}", new { Pin = pin }, true)).Entidad?.Select(x => new ResponsePinesAsociados()
                    {
                        CanalVenta = x.CanalVenta,
                        Estado = x.Estado,
                        FechaDispersion = x.FechaDispersion != null ? DateTime.Parse(x.FechaDispersion) : null,
                        FechaRegistro = x.FechaRegistro,
                        NumeroIdentificacion = x.NumeroIdentificacion,
                        Pin = x.Pin,
                        RazonSocial = x.RazonSocial,
                        TipoIdentificacion = x.TipoIdentificacion,
                        TipoPin = x.TipoPin,
                        ValorActor = x.ValorActor,
                        ValorAliado = x.ValorAliado,
                        ValorAns = x.ValorAns,
                        ValorTransaccion = x.ValorTransaccion,
                        ValosSicov = x.ValosSicov,
                        TotalRegistros = 1,
                        Banco = x.Banco,
                        CtaDispersion = x.CtaDispersion,
                        ValorDispersado = x.ValorDispersado,
                        AgenteDispersion = x.AgenteDispersion,
                    }).ToList();

                    pinesAsociados = await _portalAdministrativoService.ConsumirServicioGET<ResponseDTO<List<ResponsePinesAsociados>>>($"{MetodosApiPortalAdmin.CONSULTAR_PINES_ASOCIADOS}/{pin}", true);

                    pinesAsociados.Entidad = [.. pinesAsociadosPinDirecto.UnionBy(pinesAsociados?.Entidad, x => x.Pin)];
                }

                return pinesAsociados;
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<List<ResponsePinesAsociados>>();
            }
        }

        public async Task<List<Centro>> ObtenerTodosCentrosxComercio(ConsultaCentroPorComercio consulta)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<List<Centro>>(MetodosApiPortalAdmin.OBTENER_TODOS_CENTROS_X_COMERCIO, consulta, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return [];
            }
        }

        public async Task<List<AgenteDispersionResponseDTO>> ConsultarAgentesDispersion()
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioGET<List<AgenteDispersionResponseDTO>>(MetodosApiPortalAdmin.OBTENER_AGENTES_DISPERSION, true);
            }
            catch (Exception ex)
            {
                await SaveLog("", ex, new StackFrame(1).GetMethod()?.Name);
                return [];
            }
        }

        public async Task<RespuestaRecaptchaDinamica<T>> DevolucionByPin<T>(ConsultaDevolucionPorPinRequest consultaDevolucionPorPin, bool IsAnulacion)
        {
            try
            {
                RespuestaRecaptchaDinamica<T> consultaInfoPin = await _portalAdministrativoService
                    .ConsumirServicioPOST<RespuestaRecaptchaDinamica<T>>(
                    IsAnulacion
                            ? MetodosApiPortalAdmin.REALIZAR_DEVOLUCION_BY_PIN
                            : MetodosApiPortalAdmin.DEVOLUCION_BY_PIN,
                        consultaDevolucionPorPin);
                return consultaInfoPin;
            }
            catch (Exception ex)
            {
                await SaveLog(consultaDevolucionPorPin, ex, new StackFrame(1).GetMethod()?.Name);
                return new RespuestaRecaptchaDinamica<T>();
            }
        }

        public async Task<FileBase64> ConsultarComprobanteDevolucion(ConsultaComprobanteDevolucionRequest consultaComprobanteDevolucion)
        {
            try
            {
                FileBase64 base64String = await _portalAdministrativoService
                    .ConsumirServicioPOST<FileBase64>(MetodosApiPortalAdmin.CONSULTAR_COMPROBANTE_DEVOLUCION, consultaComprobanteDevolucion);
                return base64String;
            }
            catch (Exception ex)
            {
                await SaveLog(consultaComprobanteDevolucion, ex, new StackFrame(1).GetMethod()?.Name);
                return new FileBase64();
            }
        }

        /// <summary>
        /// Metodo para insertar un log de seguimiento en un archivo local
        /// solo para ambiente de desarrollo o pruebas para hacer seguimiento a una petición
        /// </summary>
        /// <param name="message"></param>
        public void LogMessage(string message)
        {
            using StreamWriter writer = new(_filePath, true);
            writer.WriteLine($"{DateTime.Now}: {message}");
        }

        /// <summary>
        /// Metodo para obtener las categorias relacionadas con el centro configuradas
        /// en Sisec Soporte.
        /// </summary>
        /// <param name="idCentro"></param>
        /// <returns></returns>
        public async Task<List<Data.CompraPin.Categoria>> ConsultaCategoriasCentro(long idCentro)
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioGET<List<Data.CompraPin.Categoria>>($"{MetodosApiFrontMiLicencia.OBTENER_CATEGORIAS_CENTROS}/{idCentro}", true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return [];
            }
        }

        public async Task<ResponseDTO<Entidad>> ConsultaInfoPin<T>(ConsultaDevolucionPorPinRequest consultaDevolucionPorPin)
        {
            try
            {
                ResponseDTO<Entidad> consultaInfoPin = await _portalAdministrativoService
                    .ConsumirServicioPOST<ResponseDTO<Entidad>>(
                     MetodosApiPortalAdmin.CONSULTA_INFO_PIN,
                        consultaDevolucionPorPin);
                return consultaInfoPin;
            }
            catch (Exception ex)
            {
                await SaveLog(consultaDevolucionPorPin, ex, new StackFrame(1).GetMethod()?.Name);
                return new ResponseDTO<Entidad>();
            }
        }

        public async Task<ResponsePagoCuota<RespuestaGeneric<object>>> RecaudarCuotaPin(PagoCuotaRequest request)
        {
            try
            {
                ResponsePagoCuota<RespuestaGeneric<object>> response = await _portalAdministrativoService
                    .ConsumirServicioPOST<ResponsePagoCuota<RespuestaGeneric<object>>>(
                     MetodosApiPortalAdmin.RECAUDAR_CUOTA_PIN,
                        request);
                return response;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new ResponsePagoCuota<RespuestaGeneric<object>>();
            }
        }

        /// <summary>
        /// Envio de notificacion de pago cuotas
        /// </summary>
        /// <param name="request">Parametros para envio de correo de pago cuotas</param>
        /// <returns></returns>
        public async Task<ResponseEnvioCorreoCuotas> ConstruirCorreoPagoCuotas(NotificacionPagoCuotas request)
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioPOST<ResponseEnvioCorreoCuotas>(MetodosApiFrontMiLicencia.ENVIO_NOTIFICACION_CUOTA, request);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseEnvioCorreoCuotas() { Enviado = false };
            }
        }

        /// <summary>
        /// Metodo para Obtener el ValorAliado de recaudo para el calculo de cuotas
        /// en Sisec Soporte.
        /// </summary>
        /// <param name="ConsultaValorAliado"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<EntidadCuotas>> ConsultarValorCuotaAliado(ConsultaValorAliado consultaValorAliado)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseDTO<EntidadCuotas>>(MetodosApiPortalAdmin.CONSULTAR_VALOR_CUOTA_ALIADO,
                    consultaValorAliado, true);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseDTO<EntidadCuotas>();
            }
        }

        /// <summary>
        /// Metodo para obtener todos los departamentos relacionadas con el centro configuradas
        /// en Sisec Soporte.
        /// </summary>
        /// <param name="idCentro"></param>
        /// <returns></returns>
        public async Task<List<Departamentos>> ObtenerDepartamentos()
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioGET<List<Departamentos>>(MetodosApiFrontMiLicencia.OBTENER_DEPARTAMENTOS, true);
            }
            catch (Exception ex)
            {
                await SaveLog(new { Data = "" }, ex, new StackFrame(1).GetMethod()?.Name);
                return [];
            }
        }

        /// <summary>
        /// Metodo para obtener todos los municios relacionadas con el departamento seleccionado
        /// en Sisec Soporte.
        /// </summary>
        /// <param name="idCentro"></param>
        /// <returns></returns>
        public async Task<List<Municipios>> ObtenerMunicipios(int idDepartamento)
        {
            try
            {
                return await _apiMilicenciaService.ConsumirServicioGET<List<Municipios>>($"{string.Format(MetodosApiFrontMiLicencia.OBTENER_MINUCIPIOS, idDepartamento)}", true);
            }
            catch (Exception ex)
            {
                await SaveLog(new { Data = idDepartamento }, ex, new StackFrame(1).GetMethod()?.Name);
                return [];
            }
        }

        public async Task<FacturacionResponse<List<ConsultaGenericaTipos>>> GetPersonTypes()
        {
            try
            {
                FacturacionResponse<List<ConsultaGenericaTipos>> municipios = await _apiMilicenciaService
                    .ConsumirServicioGET<FacturacionResponse<List<ConsultaGenericaTipos>>>(MetodosApiFrontMiLicencia.GET_PERSON_TYPES);
                return municipios;
            }
            catch (Exception ex)
            {
                await SaveLog("", ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<List<ConsultaGenericaTipos>>();
            }
        }

        #endregion Servicios

        #region Facturacion electronica

        public async Task<FacturacionResponse<EstadoFacturacion>> ConsultarEstadoFacturacionElectronica(ConsultaEstadoFacturacion request)
        {
            try
            {
                FacturacionResponse<EstadoFacturacion> estadoFacturacion = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<EstadoFacturacion>>(MetodosApiPortalAdmin.CONSULTAR_ESTADO_FACTURACION_ELECTRONICA,
                        request);
                return estadoFacturacion;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<EstadoFacturacion>();
            }
        }

        public async Task<FacturacionResponse<ConfiguracionFacturacion>> ConsultarConfiguracionFacturacionElectronica(ConsultaEstadoFacturacion request)
        {
            try
            {
                FacturacionResponse<ConfiguracionFacturacion> configuracionFacturacion = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<ConfiguracionFacturacion>>(MetodosApiPortalAdmin.CONSULTAR_CONFIGURACION_FACTURACION_ELECTRONICA,
                        request);
                if (configuracionFacturacion.SolicitudExitosa)
                {
                    configuracionFacturacion.Datos.Usuario = await _aesEncryptionHelper.Decrypt(configuracionFacturacion?.Datos?.Usuario ?? "");
                    configuracionFacturacion.Datos.Contraseña = await _aesEncryptionHelper.Decrypt(configuracionFacturacion?.Datos?.Contraseña ?? "");
                }

                return configuracionFacturacion;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<ConfiguracionFacturacion>();
            }
        }

        public async Task<FacturacionResponse<EstadoCredenciales>> ValidarEstadoCredencial(CredencialesProveedor request)
        {
            request.Usuario = await _aesEncryptionHelper.Encrypt(request.Usuario);
            request.Clave = await _aesEncryptionHelper.Encrypt(request.Clave);

            EncryptData data = new()
            {
                Data = await _aesEncryptionHelper.Encrypt(JsonConvert.SerializeObject(request))
            };

            try
            {
                FacturacionResponse<EstadoCredenciales> estadoCredenciales = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<EstadoCredenciales>>(MetodosApiPortalAdmin.VALIDAR_ESTADO_CREDENCIAL, data);

                return estadoCredenciales;
            }
            catch (Exception ex)
            {
                await SaveLog(data, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<EstadoCredenciales>();
            }
        }

        public async Task<FacturacionResponse<List<ConsultaMunicipios>>> ConsultarMunicipiosDepartamentos()
        {
            try
            {
                FacturacionResponse<List<ConsultaMunicipios>> municipios = await _portalAdministrativoService
                    .ConsumirServicioGET<FacturacionResponse<List<ConsultaMunicipios>>>(MetodosApiPortalAdmin.CONSULTAR_MUNICIPIOS_DEPARTAMENTOS);
                return municipios;
            }
            catch (Exception ex)
            {
                await SaveLog("", ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<List<ConsultaMunicipios>>();
            }
        }

        public async Task<FacturacionResponse<List<ConsultaGenericaTipos>>> ConsultarGenericaTipos(EnumTiposFacturacion tiposFacturacion)
        {
            try
            {
                FacturacionResponse<List<ConsultaGenericaTipos>> municipios = await _portalAdministrativoService
                    .ConsumirServicioGET<FacturacionResponse<List<ConsultaGenericaTipos>>>(
                        tiposFacturacion switch
                        {
                            EnumTiposFacturacion.TiposPersona => MetodosApiPortalAdmin.CONSULTAR_TIPO_PERSONA,
                            EnumTiposFacturacion.TiposDisparadores => MetodosApiPortalAdmin.CONSULTAR_DISPARADORES_FACTURACION,
                            EnumTiposFacturacion.Departamentos => MetodosApiPortalAdmin.CONSULTAR_DEPARTAMENTOS,
                            EnumTiposFacturacion.TiposRegimen => MetodosApiPortalAdmin.CONSULTAR_REGIMEN_CONTRIBUTIVO,
                            _ => "PortalAdmin: Tipo de facturacion Not Found"
                        });
                return municipios;
            }
            catch (Exception ex)
            {
                await SaveLog("", ex, $"{new StackFrame(1).GetMethod()?.Name} {nameof(tiposFacturacion)}");
                return new FacturacionResponse<List<ConsultaGenericaTipos>>();
            }
        }

        public async Task<FacturacionResponse<object>> AlmacenamientoDatosEmision(DatosEmision request)
        {
            try
            {
                FacturacionResponse<object> datosEmision = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<object>>(MetodosApiPortalAdmin.AMLACENAMIENTO_DATOS_EMISION, request);

                return datosEmision;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<object>();
            }
        }

        public async Task<FacturacionResponse<object>> AlmacenamientoDatosNumeracion(DatosNumeracion request)
        {
            try
            {
                FacturacionResponse<object> datosNumeracion = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<object>>(MetodosApiPortalAdmin.AMLACENAMIENTO_DATOS_NUMERACION, request);

                return datosNumeracion;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<object>();
            }
        }

        public async Task<FacturacionResponse<object>> AlmacenamientoComportamiento(DatosComportamiento request)
        {
            try
            {
                FacturacionResponse<object> datosComportamiento = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<object>>(MetodosApiPortalAdmin.ALMACENAMIENTO_DATOS_COMPORTAMIENTO, request);

                return datosComportamiento;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<object>();
            }
        }

        public async Task<FacturacionResponse<DatosArticulos>> ConsultarListadoArticulos(ConsultaArticuloActivo request)
        {
            try
            {
                FacturacionResponse<DatosArticulos> datosComportamiento = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<DatosArticulos>>(MetodosApiPortalAdmin.CONSULTAR_LISTADO_ARTICULOS, request);

                return datosComportamiento;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<DatosArticulos>();
            }
        }

        public async Task<FacturacionResponse<RespuestaErrorFacturacion>> ConsultarEstadoSistema(ConsultaEstadoFacturacion request)
        {
            try
            {
                FacturacionResponse<RespuestaErrorFacturacion> datosComportamiento = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<RespuestaErrorFacturacion>>(MetodosApiPortalAdmin.CONSULTAR_ESTADO_SISTEMA, request);

                return datosComportamiento;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<RespuestaErrorFacturacion>();
            }
        }

        public async Task<FacturacionResponse<object>> AlmacenarConfiguracionArticulos(DatosArticulos request)
        {
            try
            {
                FacturacionResponse<object> datosNumeracion = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<object>>(MetodosApiPortalAdmin.AMLACENAMIENTO_CONFIGURACION_ARTICULOS, request);

                return datosNumeracion;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<object>();
            }
        }

        public async Task<FacturacionResponse<CancelBillingRequestResult>> CancelBillingRequest(UpdateElectronicBillingRequest request)
        {
            try
            {
                FacturacionResponse<CancelBillingRequestResult> resultAnulacion = await _portalAdministrativoService
                    .ConsumirServicioPOST<FacturacionResponse<CancelBillingRequestResult>>(MetodosApiPortalAdmin.CANCEL_BILLING_REQUEST, request);

                return resultAnulacion;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<CancelBillingRequestResult>();
            }
        }

        /// <summary>
        /// Obtiene los tipos de documento habilitados para facturación electrónica
        /// Llama al endpoint GetIdDocumentType y filtra por VisualizarFacturacionElectronica
        /// </summary>
        /// <returns>Lista de tipos de documento habilitados para facturación electrónica</returns>
        public async Task<List<TipoDocumentoFacturacionElectronica>> ObtenerTiposDocumentoFacturacionElectronica()
        {
            // Inicio refactorización/optimización por GitHub Copilot
            try
            {
                // Llamar al endpoint GetIdDocumentType
                var respuesta = await _apiMilicenciaService.ConsumirServicioGET<RespuestaTipoDocumento>(
                    "servicios/Cotizador/GetIdDocumentType",
                    true
                );

                // Filtrar solo los tipos que:
                // 1. Tienen habilitada la facturación electrónica
                // 2. Tienen un código ACH válido
                if (respuesta?.Entidad != null)
                {
                    return respuesta.Entidad
                        .Where(doc =>
                            doc.VisualizarFacturacionElectronica &&
                            !string.IsNullOrWhiteSpace(doc.CodigoACH)
                        )
                        .ToList();
                }

                return new List<TipoDocumentoFacturacionElectronica>();
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new List<TipoDocumentoFacturacionElectronica>();
            }
            // Fin refactorización/optimización por GitHub Copilot
        }

        #endregion Facturacion electronica

        #region Public Methods

        public async Task<RespuestaAcciones> ConsultaEstadoDevolucion(ConsultaEstadoDevolucion consultaEstadoDevolucion, string googleCaptcha = "")
        {
            try
            {
                RespuestaAcciones responseAcciones = await _apiMilicenciaService
                    .ConsumirServicioPOST<RespuestaAcciones>(MetodosApiFrontMiLicencia.CONSULTA_ESTADO_DEVOLUCION,
                        consultaEstadoDevolucion, true, googleCaptcha);
                return responseAcciones;
            }
            catch (Exception ex)
            {
                await SaveLog(consultaEstadoDevolucion, ex, new StackFrame(1).GetMethod()?.Name);
                return new() { Aprobado = false, Texto = "Pin no encontrado." };
            }
        }

        public async Task<int> GetTipoCliente(string pin)
        {
            try
            {
                int tipocliente = await _apiMilicenciaService
                    .ConsumirServicioPOST<int>(MetodosApiFrontMiLicencia.OBTENER_TIPO_CLIENTE + "?Pin=" + pin, "",
                        true);
                return tipocliente;
            }
            catch (Exception ex)
            {
                await SaveLog(new { Pin = pin }, ex, new StackFrame(1).GetMethod()?.Name);
                return 0;
            }
        }

        #endregion Public Methods

        #region Private Methods

        public async Task<CotizacionPinCDA> GeneracionCostoCDA(PagoPinCDA pagoPin)
        {
            try
            {
                ConsultaCostoPinCda consulta = new()
                {
                    IdCentro = (int)pagoPin.CentroSeleccionado.IdCentro,
                    IdRunt = (long)pagoPin.CentroSeleccionado.CodigoRUNT,
                    IdCategoria = pagoPin.CategoriaVehiculo,
                    Edad = pagoPin.EdadVehiculo,
                };
                if (consulta.IdCentro == 0 || consulta.IdRunt == 0 || consulta.IdCategoria == 0)
                {
                    return new CotizacionPinCDA();
                }
                return await ObtenerCostoCDA(consulta);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new CotizacionPinCDA();
            }
        }

        public async Task<CotizacionPinCDA> ObtenerCostoCDA(ConsultaCostoPinCda consultaCostoPinCda)
        {
            CotizacionPinCDA cotizacionPin = new();
            try
            {
                cotizacionPin = await _portalAdministrativoService.ConsumirServicioPOST<CotizacionPinCDA>(MetodosApiPortalAdmin.POST_COSTOPINCDA, consultaCostoPinCda, true);
                return cotizacionPin;
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return cotizacionPin;
            }
        }

        public async Task<List<ResponseTipoVehiculos>> ObtenerTipoVehiculos(RequestTipoVehiculos tipoVehiculo)
        {
            List<ResponseTipoVehiculos> Vehiculos = [];
            try
            {
                Vehiculos = await _portalAdministrativoService.ConsumirServicioPOST<List<ResponseTipoVehiculos>>(MetodosApiPortalAdmin.POST_OBTENER_CATEGORIAVEHICULO, tipoVehiculo, true);
                return Vehiculos;
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return Vehiculos;
            }
        }

        public async Task<ResponseCdaWompi> GenerarReferenciaPinCDA(RequestCreacionPinCDA referenciaBancolombiaWompi)
        {
            try
            {
                ResponseCdaWompi responseReferenciaWompi = await _portalAdministrativoService.ConsumirServicioPOST<ResponseCdaWompi>(MetodosApiPortalAdmin.GENERAR_REFERENCIA_PIN_CDA, referenciaBancolombiaWompi);

                return responseReferenciaWompi;
            }
            catch (System.Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseCdaWompi();
            }
        }

        public async Task<ResponseNotificacionCorreoCDA> ConstruirCorreoCDA(RequestNotificacionCDA result)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseNotificacionCorreoCDA>(MetodosApiPortalAdmin.GENERAR_CORREO_NOTIFICACION_CDA, result);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseNotificacionCorreoCDA();
            }
        }

        public async Task<ResponseNotificacionCitas> ConstruirCorreoCitas(RequestNotificacionCitas result)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<ResponseNotificacionCitas>(MetodosApiPortalAdmin.GENERAR_CORREO_NOTIFICACION_AGENDA, result);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return new ResponseNotificacionCitas();
            }
        }

        public async Task<long> SendCancelationNotification(RequestNotificationCancelation result)
        {
            try
            {
                return await _portalAdministrativoService.ConsumirServicioPOST<long>(MetodosApiPortalAdmin.GENERAR_CORREO_NOTIFICACION_CANCELACION, result);
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name ?? "Default";
                string componentName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name ?? "Default";
                await _portalAdministrativoService.RegisterExceptionLog(ExtensionLog.GenerateExceptionLog(ex, methodName, componentName, null, null, null));
                return -1;
            }
        }

        public async Task<long> SendRescheduleNotification(RequestNotificationCancelation result)
        {
            try
            {
                var res = await _portalAdministrativoService.ConsumirServicioPOST<long>(MetodosApiPortalAdmin.GENERAR_CORREO_NOTIFICACION_REAGENDAMIENTO, result);
                if (res == null)
                {
                    throw new Exception("ES nulo");
                }
                Console.WriteLine(res);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return -1;
            }
        }

        /// <summary>
        /// Consume el metodo CostoPinCEA del APIFrontMiLicencia para obtener el costo del PIN seguib para los parametros
        /// IdCentro, IdTramite, IdCategoria, Edad, Genero en el objeto ConsultaCostoPinCea
        /// </summary>
        /// <param name="consultaCostoPinCea"></param>
        /// <returns>cotizacionPin</returns>
        private async Task<CotizacionPin> ObtenerCosto(ConsultaCostoPinCea consultaCostoPinCea)
        {
            CotizacionPin cotizacionPin = new();
            try
            {
                cotizacionPin = await _apiMilicenciaService.ConsumirServicioPOST<CotizacionPin>(MetodosApiFrontMiLicencia.POST_COSTOPINCEA, consultaCostoPinCea, true);
                return cotizacionPin;
            }
            catch (Exception ex)
            {
                await SaveLog(consultaCostoPinCea, ex, new StackFrame(1).GetMethod()?.Name);
                return cotizacionPin;
            }
        }

        /// <summary>
        /// Guarda el log en pasarela a traves del cotizador
        /// </summary>
        /// <param name="json">Objeto del request</param>
        /// <param name="ex">Exception</param>
        /// <param name="method">Nombre del metodo que se estaba ejecutando</param>
        /// <returns></returns>
        private async Task<bool> SaveLog(object json, Exception ex, string method = "SaveLog")
        {
            return await _portalAdministrativoService.RegistrarLogCotizador(new DetalleLog()
            {
                Metodo = method,
                Request = JsonConvert.SerializeObject(json),
                Response = ex.ToString(),
                Mensaje = ex.Message,
                MensajeCompleto = ex.InnerException?.Message
            });
        }

        public async Task<CotizacionPin> ObtenerPrecioPIN(ParametrosCostoPin parametrosCostoPin)
        {
            CotizacionPin cotizacionPin = new();
            try
            {
                cotizacionPin = await _apiMilicenciaService.ConsumirServicioPOST<CotizacionPin>(MetodosApiFrontMiLicencia.OBTENER_VALOR_PIN, parametrosCostoPin, true);
                return cotizacionPin;
            }
            catch (Exception ex)
            {
                await SaveLog(parametrosCostoPin, ex, new StackFrame(1).GetMethod()?.Name);
                return cotizacionPin;
            }
        }

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Envía una notificación por correo electrónico a través de PSE y retorna la respuesta detallada
        /// </summary>
        /// <param name="request">Objeto con la información de la notificación</param>
        /// <returns>Respuesta detallada de la operación</returns>
        public async Task<SendNotificationPseResponse> SendNotificationPse(NotificationByPin request)
        {
            try
            {
                var res = await _apiMilicenciaService.ConsumirServicioPOST<SendNotificationPseResponse>(MetodosApiFrontMiLicencia.SEND_NOTIFICATION_PSE, request);
                return res;
            }
            catch (Exception ex)
            {
                await SaveLog(request, ex, new StackFrame(1).GetMethod()?.Name);
                return new SendNotificationPseResponse
                {
                    Codigo = -1,
                    Respuesta = ex.Message,
                    Entidad = new EntidadData { CitizenCode = 0, CenterCode = 0 }
                };
            }
        }

        private async Task SendProcessNotificationPse(string pinGenerado)
        {
            const int maxRetries = 3;
            int attempt = 0;
            bool success = false;
            var notification = new portalAdministrativoSISEC.Entidades.Notificaciones.NotificationByPin
            {
                Pin = pinGenerado,
                NotificationType = 1,
                Recipient = 0 // Ambos: 0
            };

            while (attempt < maxRetries && !success)
            {
                try
                {
                    var response = await SendNotificationPse(notification);
                    // Reintentar si CitizenCode o CenterCode es 0 (no enviado correctamente)
                    if ((response?.Entidad?.CitizenCode ?? 0) > 0 && (response?.Entidad?.CenterCode ?? 0) > 0)
                    {
                        success = true;
                    }
                }
                catch (Exception)
                {
                    // Loguea el error si es necesario
                }
                attempt++;
            }
        }

        Task IMiLicenciaService.SendProcessNotificationPse(string pinGenerado)
        {
            return SendProcessNotificationPse(pinGenerado);
        }

        // Fin código generado por GitHub Copilot

        #endregion Private Methods

        #region Consultar Facturación Electrónica

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Obtiene las solicitudes de facturación electrónica con paginación
        /// </summary>
        /// <param name="consulta">Criterios de búsqueda</param>
        /// <returns>Respuesta con lista paginada de solicitudes</returns>
        public async Task<FacturacionResponse<DataWrapper<PaginatedData>>> GetElectronicBillingRequests(
            RequestFilter consulta)
        {
            try
            {
                // Construir query string
                var queryString = BuildElectronicBillingRequestsQueryString(consulta);

                // Usar el servicio existente para consumir el endpoint
                FacturacionResponse<DataWrapper<PaginatedData>> result = await _portalAdministrativoService
                    .ConsumirServicioGET<FacturacionResponse<DataWrapper<PaginatedData>>>(
                        $"{MetodosApiPortalAdmin.GET_ELECTRONIC_BILLING_REQUESTS}?{queryString}",
                        true
                    );

                return result ?? new FacturacionResponse<DataWrapper<PaginatedData>>
                {
                    SolicitudExitosa = false,
                    Mensaje = "La respuesta del servidor fue vacía"
                };
            }
            catch (Exception ex)
            {
                await SaveLog(consulta, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<DataWrapper<PaginatedData>>
                {
                    SolicitudExitosa = false,
                    Mensaje = "Error al consultar solicitudes de facturación",
                    Errores = new List<string> { ex.Message }
                };
            }
        }

        /// <summary>
        /// Obtiene el historial de estados de una solicitud de facturación electrónica
        /// </summary>
        /// <param name="idPtesaPIN">ID de la petición del PIN</param>
        /// <returns>Historial de estados y resumen de la petición</returns>
        public async Task<FacturacionResponse<DataWrapper<HistoricData>>> GetElectronicBillingRequestDetail(int idPtesaPIN)
        {
            try
            {
                FacturacionResponse<DataWrapper<HistoricData>> result = await _portalAdministrativoService
                    .ConsumirServicioGET<FacturacionResponse<DataWrapper<HistoricData>>>(
                        $"{MetodosApiPortalAdmin.GET_ELECTRONIC_BILLING_REQUEST_DETAIL}?idPtesaPin={idPtesaPIN}",
                        true
                    );

                return result ?? new FacturacionResponse<DataWrapper<HistoricData>>
                {
                    SolicitudExitosa = false,
                    Mensaje = "Respuesta vacía del servidor"
                };
            }
            catch (Exception ex)
            {
                await SaveLog(new { idPtesaPIN }, ex, new StackFrame(1).GetMethod()?.Name);
                return new FacturacionResponse<DataWrapper<HistoricData>>
                {
                    SolicitudExitosa = false,
                    Mensaje = "Error al obtener el historial de la solicitud",
                    Errores = new List<string> { ex.Message }
                };
            }
        }

        /// <summary>
        /// Construye el query string para la consulta de solicitudes de facturación
        /// </summary>
        /// <param name="consulta">Criterios de búsqueda</param>
        /// <returns>Query string formateado</returns>
        private string BuildElectronicBillingRequestsQueryString(RequestFilter request)
        {
            // Inicio refactorización/optimización por GitHub Copilot
            var queryParameters = new List<string>();

            if (request.FechaInicio.HasValue)
            {
                // Enviar solo la fecha en formato yyyy-MM-dd sin componente de hora
                var fechaInicio = request.FechaInicio.Value.ToString("yyyy-MM-dd");
                queryParameters.Add($"FechaInicio={Uri.EscapeDataString(fechaInicio)}");
            }

            if (request.FechaFin.HasValue)
            {
                // Enviar solo la fecha en formato yyyy-MM-dd sin componente de hora
                var fechaFin = request.FechaFin.Value.ToString("yyyy-MM-dd");
                queryParameters.Add($"FechaFin={Uri.EscapeDataString(fechaFin)}");
            }

            if (!string.IsNullOrWhiteSpace(request.TipoDocumento))
            {
                queryParameters.Add($"TipoDocumento={Uri.EscapeDataString(request.TipoDocumento)}");
            }

            if (!string.IsNullOrWhiteSpace(request.NumeroDocumento))
            {
                queryParameters.Add($"NumeroDocumento={Uri.EscapeDataString(request.NumeroDocumento)}");
            }

            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                queryParameters.Add($"Nombre={Uri.EscapeDataString(request.Nombre)}");
            }

            if (!string.IsNullOrWhiteSpace(request.Apellido))
            {
                queryParameters.Add($"Apellido={Uri.EscapeDataString(request.Apellido)}");
            }

            if (!string.IsNullOrWhiteSpace(request.Pin))
            {
                queryParameters.Add($"Pin={Uri.EscapeDataString(request.Pin)}");
            }

            if (!string.IsNullOrWhiteSpace(request.IdRunt))
            {
                queryParameters.Add($"IdRunt={Uri.EscapeDataString(request.IdRunt)}");
            }

            if (!string.IsNullOrWhiteSpace(request.EstadoML))
            {
                queryParameters.Add($"EstadoML={Uri.EscapeDataString(request.EstadoML)}");
            }

            if (request.PageNumber.HasValue)
            {
                queryParameters.Add($"PageNumber={request.PageNumber.Value}");
            }

            if (request.PageSize.HasValue)
            {
                queryParameters.Add($"PageSize={request.PageSize.Value}");
            }

            return string.Join("&", queryParameters);
            // Fin refactorización/optimización por GitHub Copilot
        }

        // Fin código generado por GitHub Copilot

        #endregion Consultar Facturación Electrónica
    }
}