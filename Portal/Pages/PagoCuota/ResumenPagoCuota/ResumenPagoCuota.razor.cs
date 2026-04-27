using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Routing.Constraints;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Application.Data.Pines.Cuotas;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.PagoCuota.ResumenPagoCuota
{

    public partial class ResumenPagoCuota
    {

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        [Parameter]
        #region variables

        public ResponseDTO<Entidad> InfoPin { get; set; }

        private decimal primerAbono => InfoPin.Entidad.ValorTransaccion - InfoPin.Entidad.ValorAliadoCuota;

        private decimal cuotaPactada => InfoPin.Entidad.CuotasPactadas - 1;
        private decimal ValorTotal => (InfoPin.Entidad.ValorCuota * cuotaPactada) + primerAbono;
        private int CuotasPagadas => InfoPin.Entidad.CuotasPactadas - InfoPin.Entidad.CuotasPendientes;
        private decimal SaldoPendiente => InfoPin.Entidad.SaldoPendiente;
        private decimal ValorRecaudo => InfoPin.Entidad.ValorAliadoCuota;

        private decimal ValorMinimo = 50000;
        private decimal abonosRealizados { get;set; } 

        public bool IsLoadingDataResumen = false;


        [Parameter]
        public EventCallback<bool> Historial { get; set; }
        public bool resumen = true;
        public ResponseDTO<Entidad> ConsultaPago { get; set; }
        public bool PinEnrolado = true;

        [Parameter]
        public EventCallback<(bool ConfirmacionCompra, ResponsePagoCuota<RespuestaGeneric<object>> Pago)> peticionCompra { get; set; }
        private async Task OnPayOrCreditClicked()
        {

            this.IsLoadingDataResumen = true;
            ShowDialog();
            await Historial.InvokeAsync(false);
            resumen = false;
            this.IsLoadingDataResumen = true;
            CloseDialog();

        }

        private async Task Volver()
        {
            await Historial.InvokeAsync(true);
            resumen = true;
        }
        [Parameter]
        public int ProximaCuota { get; set; }

        [Parameter]
        public decimal SaldoProximaCuota { get; set; }

        [Parameter]
        public decimal EnvioValor { get; set; }

        private decimal TotalPagar => ValorRecaudo + EnvioValor;
        private decimal SaldoRestante => SaldoPendiente - EnvioValor;

        private PagoCuotaRequest pagoCuotaRequest { get; set; } = new();

        private ResponsePagoCuota<RespuestaGeneric<object>> Pago { get; set; } = new();

        public bool saldoPendiente = true;
        public int estadoRecaudado { get; set; } = 4;

        #endregion   variables

        protected override async Task OnInitializedAsync()
        {
            if (CuotasPagadas == InfoPin.Entidad.CuotasPactadas)
            {
                saldoPendiente = false;
            }

            if (InfoPin.Entidad.EstadoPin != estadoRecaudado)
            {
                PinEnrolado = false;
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "El PIN debe estar en estado usado (enrolado) para poder realizar los pagos correspondientes.");
            }
            await CalcularAbono();

        }



        private async Task CalcularAbono()
        {

            if (InfoPin.Entidad.Pagos != null)
            {
                foreach (var pago in InfoPin.Entidad.Pagos)
                {

                    decimal valorRestante = pago.ValorTransaccion - InfoPin.Entidad.ValorAliadoCuota;

                    valorRestante = Math.Max(0, valorRestante);

                    abonosRealizados += valorRestante;
                }
            }
        }

        private bool IsButtonEnabled()
        {
            if (EnvioValor > ValorMinimo)
            {
                pagoCuotaRequest = new PagoCuotaRequest
                {
                    IdRunt = InfoPin.Entidad.IdRunt,
                    IdTipoDoc = InfoPin.Entidad.TipoDocumento,
                    NumDocumento = InfoPin.Entidad.NumeroDocumento,
                    NumPin = InfoPin.Entidad.Pin,
                    ValorAbono = EnvioValor,
                    ValorAliado = ValorRecaudo
                };
            }
            return EnvioValor > 50000;
        }

        private async Task RealiarPago()
        {
            if (EnvioValor >= ValorMinimo)
            {

                pagoCuotaRequest = new PagoCuotaRequest
                {
                    IdRunt = InfoPin.Entidad.IdRunt,
                    IdTipoDoc = InfoPin.Entidad.TipoDocumento,
                    NumDocumento = InfoPin.Entidad.NumeroDocumento,
                    NumPin = InfoPin.Entidad.Pin,
                    ValorAbono = EnvioValor,
                    ValorAliado = ValorRecaudo
                };
                this.IsLoadingDataResumen = true;
                ShowDialog();
                Pago = await MiLicenciaService.RecaudarCuotaPin(pagoCuotaRequest);
                this.IsLoadingDataResumen = false;
                CloseDialog();
                if (Pago != null)
                {
                    if (Pago.Respuesta.Codigo == 0)
                    {

                        // Realizamos la consulta del estado actual del pin
                        ConsultaPago = await MiLicenciaService.ConsultaInfoPin<ResponseDTO<Entidad>>(new ConsultaDevolucionPorPinRequest()
                        {
                            IdRunt = InfoPin.Entidad.IdRunt,
                            TipoIdentificacion = InfoPin.Entidad.TipoDocumento,
                            NumeroIdentificacion = InfoPin.Entidad.NumeroDocumento,
                            Pin = InfoPin.Entidad.Pin,
                        });

                        //Envio de notificacion de cuotas
                        if (InfoPin != null && ConsultaPago != null)
                        {
                            if (InfoPin.Codigo == 0 && ConsultaPago.Codigo == 0)
                                await GenerarNotificacionAsync(InfoPin.Entidad, ConsultaPago.Entidad, pagoCuotaRequest.ValorAbono);

                        }
                    }
                    await RedireccionPago();
                }
                else
                {
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Algo salio mal, por favor vuelva a intentarlo.");
                }
                

            }

        }

        private async Task RedireccionPago()
        {
            await peticionCompra.InvokeAsync((true, Pago));
        }
        /// <summary>
        /// Metodo de generacion de notificación de pago efectivo de cuota
        /// </summary>
        /// <param name="InfoPago"></param>
        /// <param name="ConsultaPago"></param>
        /// <param name="valorAbono"></param>
        /// <returns></returns>
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
        public int CalcularCuotaInicial(Entidad estadoInicial, Entidad estadoFinal)
        {
            int cuotasTotales = estadoInicial.CuotasPactadas;
            int cuotasPendientesInicial = estadoInicial.CuotasPendientes;
            int cuotasPendientesFinal = estadoFinal.CuotasPendientes;

            // La cuota inicial será la primera cuota que se pagó en este intervalo
            int cuotaInicial = (cuotasTotales - cuotasPendientesInicial) + 1;

            return cuotaInicial;
        }
        /// <summary>
        /// Metodo para inicializar variables en el envio de correo a cuotas para validacion de informacion y trabajo entre cuotas
        /// </summary>
        /// <returns></returns>
        private async Task InicializacionVariablesPrueba()
        {
            EnvioValor = 40000;
            InfoPin = new ResponseDTO<Entidad>
            {
                Codigo = 0,
                Respuesta = "Información del PIN encontrada",
                Entidad = new Entidad
                {
                    Pin = "813702043485486",
                    FechaNacimiento = new DateTime(2004, 2, 24),
                    TipoDocumento = 1,
                    NumeroDocumento = "1115081441",
                    Email = "antonio.garcia@olimpiait.com",
                    Telefono = "3207654321",
                    TipoTramite = 1,
                    Categoria = "B1",
                    Genero = "M",
                    NombreCompleto = "Alejandro Prueba Cuotas",
                    IdRunt = "666555444",
                    EstadoPin = 4,
                    IdPtesaPin = 190014,
                    FechaReferencia = new DateTime(2024, 8, 5, 12, 52, 21, 897),
                    FechaRecaudo = new DateTime(2024, 8, 5, 12, 52, 34, 387),
                    FechaUso = new DateTime(2024, 8, 5, 12, 55, 11, 100),
                    ValorTransaccion = 90000,
                    IdMedioRecaudo = "4",
                    IdOrigenCotizacion = 4,
                    FechaAnulacion = new DateTime(1, 1, 1),
                    FechaDevolucion = new DateTime(1, 1, 1),
                    FechaVencimientoReferencia = new DateTime(1, 1, 1),
                    IdTipoPin = 2,
                    IdCliente = 8,
                    CuotasPactadas = 5,
                    CuotasPendientes = 5,
                    ValorCuota = 50000,
                    ValorAliadoCuota = 10000,
                    SaldoPendiente = 0,
                    Pagos = new List<Pago>
                {
                    new Pago { Pin = "813702043485486", IdPtesaPin = 190014, ValorTransaccion = 90000, FechaRecaudo = new DateTime(2024, 8, 5, 12, 52, 34, 387) },
                    new Pago { Pin = "829501001781079", IdPtesaPin = 190015, ValorTransaccion = 60000, FechaRecaudo = new DateTime(2024, 8, 5, 12, 55, 52, 323) },
                    new Pago { Pin = "818901032514398", IdPtesaPin = 200311, ValorTransaccion = 60000, FechaRecaudo = new DateTime(2024, 8, 20, 10, 7, 43, 290) },
                    new Pago { Pin = "874400841737893", IdPtesaPin = 200312, ValorTransaccion = 70000, FechaRecaudo = new DateTime(2024, 8, 20, 10, 19, 35, 627) },
                    new Pago { Pin = "839300854355283", IdPtesaPin = 200316, ValorTransaccion = 30000, FechaRecaudo = new DateTime(2024, 8, 20, 14, 37, 16, 677) },
                    new Pago { Pin = "888000420519541", IdPtesaPin = 200456, ValorTransaccion = 80000, FechaRecaudo = new DateTime(2024, 8, 26, 14, 43, 49, 577) },
                    new Pago { Pin = "874800601998358", IdPtesaPin = 200458, ValorTransaccion = 80000, FechaRecaudo = new DateTime(2024, 8, 26, 14, 43, 49, 577) },
                    new Pago { Pin = "897301642792380", IdPtesaPin = 200455, ValorTransaccion = 80000, FechaRecaudo = new DateTime(2024, 8, 26, 14, 43, 49, 723) }
                }
                }
            };

            ConsultaPago = new ResponseDTO<Entidad>
            {
                Codigo = 0,
                Respuesta = "Información del PIN encontrada",
                Entidad = new Entidad
                {
                    Pin = "813702043485486",
                    FechaNacimiento = new DateTime(2004, 2, 24),
                    TipoDocumento = 1,
                    NumeroDocumento = "1115081441",
                    Email = "antonio.garcia@olimpiait.com",
                    Telefono = "3207654321",
                    TipoTramite = 1,
                    Categoria = "B1",
                    Genero = "M",
                    NombreCompleto = "Alejandro Prueba Cuotas",
                    IdRunt = "666555444",
                    EstadoPin = 4,
                    IdPtesaPin = 190014,
                    FechaReferencia = new DateTime(2024, 8, 5, 12, 52, 21, 897),
                    FechaRecaudo = new DateTime(2024, 8, 5, 12, 52, 34, 387),
                    FechaUso = new DateTime(2024, 8, 5, 12, 55, 11, 100),
                    ValorTransaccion = 90000,
                    IdMedioRecaudo = "4",
                    IdOrigenCotizacion = 4,
                    FechaAnulacion = new DateTime(1, 1, 1),
                    FechaDevolucion = new DateTime(1, 1, 1),
                    FechaVencimientoReferencia = new DateTime(1, 1, 1),
                    IdTipoPin = 2,
                    IdCliente = 8,
                    CuotasPactadas = 5,
                    CuotasPendientes = 4,
                    ValorCuota = 50000,
                    ValorAliadoCuota = 10000,
                    SaldoPendiente = 200000,
                    Pagos = new List<Pago>
                    {
                        new Pago { Pin = "813702043485486", IdPtesaPin = 190014, ValorTransaccion = 90000, FechaRecaudo = new DateTime(2024, 8, 5, 12, 52, 34, 387) },
                        new Pago { Pin = "829501001781079", IdPtesaPin = 190015, ValorTransaccion = 60000, FechaRecaudo = new DateTime(2024, 8, 5, 12, 55, 52, 323) },
                        new Pago { Pin = "818901032514398", IdPtesaPin = 200311, ValorTransaccion = 60000, FechaRecaudo = new DateTime(2024, 8, 20, 10, 7, 43, 290) },
                        new Pago { Pin = "874400841737893", IdPtesaPin = 200312, ValorTransaccion = 70000, FechaRecaudo = new DateTime(2024, 8, 20, 10, 19, 35, 627) },
                        new Pago { Pin = "839300854355283", IdPtesaPin = 200316, ValorTransaccion = 30000, FechaRecaudo = new DateTime(2024, 8, 20, 14, 37, 16, 677) },
                        new Pago { Pin = "888000420519541", IdPtesaPin = 200456, ValorTransaccion = 80000, FechaRecaudo = new DateTime(2024, 8, 26, 14, 43, 49, 577) },
                        new Pago { Pin = "874800601998358", IdPtesaPin = 200458, ValorTransaccion = 80000, FechaRecaudo = new DateTime(2024, 8, 26, 14, 43, 49, 577) },
                        new Pago { Pin = "897301642792380", IdPtesaPin = 200455, ValorTransaccion = 80000, FechaRecaudo = new DateTime(2024, 8, 26, 14, 43, 49, 723) }
                    }
                }
            };
        }

        public async Task ShowDialog()
        {
            await JS.InvokeVoidAsync("window.dialogFunctions.showDialog", "procesandoCompra");
        }
        public async Task CloseDialog()
        {
            await JS.InvokeVoidAsync("window.dialogFunctions.closeDialog", "procesandoCompra");
        }
    }
}


