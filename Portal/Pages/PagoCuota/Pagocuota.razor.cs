using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Data.Pines.Cuotas;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Entidades.Recaptcha;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.Devoluciones;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.PagoCuota.ResumenPagoCuota;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
namespace portalAdministrativoSISEC.Pages.PagoCuota
{
    public partial class PagoCuota
    {

        #region Inyeccion Dependencias

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }


        #endregion Inyeccion Dependencias

        #region Variables

        private GetDataResponseCentro GetCentroResponse = new();
        private readonly Centro CentroSeleccionado = new();

        private bool isLoading = false;
        public List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = [];

        [Parameter]
        public ResponseDTO<Entidad> InfoPin { get; set; } 

        //public ResponseDTO<object> PagoRealizado = new();

        public ResponsePagoCuota<RespuestaGeneric<object>> PagoRealizado = new();
        public bool NuevaConsulta = false;


        private decimal ValorTotal;
        private decimal OtroValorInputDecimal { get; set; } = 0;

        private bool ConfirmacionCompra { get; set; } = false;
        public decimal ValorCompra { get; set; } = 0;
        public string Pin { get; set; } = "";
        public bool abrirModalCuotas { get; set; } = false;


        private string opcionPago = "Cuotas"; // Valor por defecto

        private bool EsPagoPorCuotas => opcionPago == "Cuotas";

        private EditContext editContext;

        private PaymentOptionModel paymentOptionModel = new();
        private ValidationMessageStore? validationMessageStore = default!;

        public ResponseDTO<Entidad> ConsultaPago { get; set; }

        ResponsePagoCuota<RespuestaGeneric<object>> pago;

        #endregion Variables

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            decimal primerAbono = InfoPin.Entidad.ValorTransaccion - InfoPin.Entidad.ValorAliadoCuota;
            decimal cuotaPactada = InfoPin.Entidad.CuotasPendientes; 

            ValorTotal = InfoPin.Entidad.SaldoPendiente;
            paymentOptionModel.ValorTotal = ValorTotal;
            paymentOptionModel.CuotasRestantes = InfoPin.Entidad.CuotasPendientes;
            paymentOptionModel.ValorCuota = InfoPin.Entidad.ValorCuota;


            editContext = new EditContext(paymentOptionModel);
            validationMessageStore = new ValidationMessageStore(editContext);
            editContext.OnFieldChanged += (sender, e) =>
            {
                validationMessageStore.Clear();
            };

            isLoading = false;
        }


        private void OnCancelar()
        {
            NavManager.NavigateTo("/compradepin/gestionarPin", true);
        }

        private void VolverCompraPin()
        {
            NavManager.NavigateTo("/compradepin", false);
        }

        private string FormattedOtroValorInput
        {
            get => OtroValorInputDecimal.ToString("N0", CultureInfo.CreateSpecificCulture("es-CO"));
            set
            {
                if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("es-CO"), out decimal result))
                {
                    OtroValorInputDecimal = result;
                }
            }
        }

        private void AgregarCuotas()
        {
            if (paymentOptionModel.Cuotas < InfoPin.Entidad.CuotasPendientes)
                paymentOptionModel.Cuotas++;
        }

        private void RestarCuotas()
        {
            if (paymentOptionModel.Cuotas > 1)
                paymentOptionModel.Cuotas--;
        }

        private async Task submitCuotas()
        {
            abrirModalCuotas = true;

            var pagoCuotaRequest = new PagoCuotaRequest
            {
                IdRunt = InfoPin.Entidad.IdRunt,
                IdTipoDoc = InfoPin.Entidad.TipoDocumento,
                NumDocumento = InfoPin.Entidad.NumeroDocumento,
                NumPin = InfoPin.Entidad.Pin,
                ValorAbono = paymentOptionModel.MontoAPagar,
                ValorAliado = InfoPin.Entidad.ValorAliadoCuota
            };

            pago = await MiLicenciaService.RecaudarCuotaPin(pagoCuotaRequest);

            if (pago != null)
            {
                if (pago.Respuesta.Codigo == 0)
                {
                    ConsultaPago = await MiLicenciaService.ConsultaInfoPin<ResponseDTO<Entidad>>(new ConsultaDevolucionPorPinRequest()
                    {
                        IdRunt = InfoPin.Entidad.IdRunt,
                        TipoIdentificacion = InfoPin.Entidad.TipoDocumento,
                        NumeroIdentificacion = InfoPin.Entidad.NumeroDocumento,
                        Pin = InfoPin.Entidad.Pin,
                    });

                    if (InfoPin != null && ConsultaPago != null)
                    {
                        if (InfoPin.Codigo == 0 && ConsultaPago.Codigo == 0)
                            await GenerarNotificacionAsync(InfoPin.Entidad, ConsultaPago.Entidad, pagoCuotaRequest.ValorAbono);
                    }
                }
            }
            abrirModalCuotas = false;
            ConfirmacionCompra = true;
        }

        public async Task OnPaymentOptionChanged()
        {
            // Resetear validaciones y valores no relevantes
            paymentOptionModel.ValorParcial = null;
            paymentOptionModel.Cuotas = 1;

            // Limpiar mensajes de validación
            editContext = new EditContext(paymentOptionModel);
            StateHasChanged();
        }

        private async Task GenerarNotificacionAsync(Entidad InfoPago, Entidad ConsultaPago, decimal valorAbono)
        {
            try
            {
                NotificacionPagoCuotas notificacion = new();

                notificacion.NumPin = InfoPago.Pin;
                notificacion.NumCuotas = InfoPago.CuotasPactadas.ToString();
                notificacion.FechaCompra = InfoPago.FechaReferencia.ToString("dd-MM-yyyy");
                notificacion.FechaPago = ConsultaPago.Pagos.Last().FechaRecaudo.ToString("dd-MM-yyyy");
                notificacion.ValorAbono = valorAbono.ToString();
                notificacion.ValorAliado = InfoPago.ValorAliadoCuota.ToString();
                notificacion.SaldoPendienteCuota = valorAbono < InfoPago.ValorCuota ? (InfoPago.ValorCuota - valorAbono).ToString() : "0";
                notificacion.SaldoPendienteDeuda = ConsultaPago.SaldoPendiente.ToString();
                notificacion.CuotasPendientes = ConsultaPago.CuotasPendientes.ToString();
                notificacion.ValorTotal = (ConsultaPago.ValorTransaccion + (ConsultaPago.CuotasPactadas * ConsultaPago.ValorCuota)).ToString();
                //Calculo de cuota inicial 
                int cuotaInicial = ConsultaPago.CuotasPendientes == 0 ? 0 : CalcularCuotaInicial(InfoPago, ConsultaPago);
                int cantidadCuotas = InfoPago.CuotasPactadas;
                //Validacion del tipo de pago de cuota por valor de abono y diferencia entre objeto antes del pago y despues del pago 
                int tipoEnvio = await ValidacionTipoDePagoCuota(InfoPago, ConsultaPago, valorAbono);
                notificacion.Correo = InfoPago.Email;

                // Armado encabezado
                notificacion = ArmadoEncabezado(notificacion, tipoEnvio, cuotaInicial, cantidadCuotas, valorAbono, ConsultaPago.ValorCuota);
                // Pendiente armado del request hacia api front mi licencia 
                ResponseEnvioCorreoCuotas envioNotificacion = await MiLicenciaService.ConstruirCorreoPagoCuotas(notificacion);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public int CalcularCuotaInicial(Entidad estadoInicial, Entidad estadoFinal)
        {
            int cuotasTotales = estadoInicial.CuotasPactadas;
            int cuotasPendientesInicial = estadoInicial.CuotasPendientes;
            int cuotasPendientesFinal = estadoFinal.CuotasPendientes;

            // La cuota inicial será la primera cuota que se pagó en este intervalo
            int cuotaInicial = (cuotasTotales - cuotasPendientesInicial) + 1;

            return cuotaInicial;
        }

        private async Task<int> ValidacionTipoDePagoCuota(Entidad InfoPago, Entidad ConsultaPago, decimal valorAbono)
        {
            if (ConsultaPago.SaldoPendiente == 0)
            {
                return 1;
            }
            if (valorAbono == ConsultaPago.ValorCuota || valorAbono > ConsultaPago.ValorCuota)
            {
                return 2;
            }
            if (valorAbono < ConsultaPago.ValorCuota)
            {
                return 3;
            }
            return 0;
        }

        private NotificacionPagoCuotas ArmadoEncabezado(NotificacionPagoCuotas notificacion, int casoPago, int cuotaInicial, int cantidadCuotas, decimal valorAbono, decimal valorCuota)
        {
            switch (casoPago)
            {
                case 1: // Pago Total de la deuda
                    notificacion.Encabezado = "Hemos recibido el pago total del curso de conducción, que has adquirido, relacionado con el número de pin";
                    break;
                case 2: // Pago de Cuotas Completas
                    int cuotasPagadas = (int)(valorAbono / valorCuota);
                    string numeroCuotas = FormatearNumeroCuotas(cuotaInicial, cuotaInicial + cuotasPagadas - 1);
                    if (!string.IsNullOrEmpty(numeroCuotas))
                    {
                        notificacion.Encabezado = $"Hemos recibido el <strong>pago de las cuotas N° {numeroCuotas}</strong> para el curso de conducción, que has adquirido, relacionado con el número de pin";
                    }
                    else
                    {
                        notificacion.Encabezado = "Hemos recibido un pago para el curso de conducción, que has adquirido, relacionado con el número de pin";
                    }
                    break;
                case 3: // Abono parcial
                    notificacion.Encabezado = "Hemos recibido un abono al curso de conducción, que has adquirido, relacionado con el número de pin";
                    break;
            }
            return notificacion;
        }

        private string FormatearNumeroCuotas(int cuotaInicial, int cantidadCuotas)
        {
            if (cuotaInicial == cantidadCuotas)
            {
                return cuotaInicial.ToString();
            }

            List<string> cuotas = new List<string>();
            for (int i = cuotaInicial; i <= cantidadCuotas; i++)
            {
                cuotas.Add(i.ToString());
            }

            if (cuotas.Count == 2)
            {
                return string.Join(" y ", cuotas);
            }
            else
            {
                string ultimaCuota = cuotas[cuotas.Count - 1];
                cuotas.RemoveAt(cuotas.Count - 1);
                return string.Join(", ", cuotas) + " y " + ultimaCuota;
            }
        }
    }
}
