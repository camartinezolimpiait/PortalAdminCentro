using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Devolucion;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Entidades.Recaptcha;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.GestionarPin
{
    public partial class GestionarPin
    {
        #region Inyeccion Dependencias

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Parameter]
        public List<ConsultaDevolucion> ConsultaDevolucion { get; set; }

        #endregion Inyeccion Dependencias

        private int pasosCompraPin;
        private bool isLoading = true;
        private bool showMessageError = false;

        private GridItemsProvider<RowList> Data;
        //private IQueryable<List<RowList>> Data;

        private Paginas Paginas = new();

        private List<ColumnList> Columnas;

        private readonly Centro CentroSeleccionado = new();
        private readonly List<Entidad> Listado = [];

        private GetDataResponseCentro GetCentroResponse = new();
        private RespuestaRecaptchaDinamica<ConsultaInfoPinDto> ConsultaInfoPin = new();
        private List<RowList> HistorialPagos { get; set; } = new();

        private ConsultaDevolucionPorPinRequest DevolucionPorPinRequest { get; set; } = new();
        private bool HabilitarBotonConsulta = false;
        private bool abrirModalIrInicio;
        private bool abrirModalConfirmacion;
        private bool abrirModalProceso = false;
        private bool pinProcesado = false;
        private string plataforma;
        private bool mostrarBloque = false;
        private bool isLoadingConsult = false;
        ResponseDTO<Entidad> infoPin = new ResponseDTO<Entidad>();
        private string messageError;
        private bool DevolucionRealizada = false;
        private string ButtonClass = "flex items-center text-white text-sm font-medium bg-azul-600 hover:bg-azul-700 rounded-md px-3 py-1 transition-colors duration-150 cursor-pointer";
        private bool IsUpdating = false;
        private string ButtonText = "Actualizar estado";
        private int RemainingSeconds = 0;
        private System.Timers.Timer? CountdownTimer;
        private string CountdownDisplay = string.Empty;
        private string LastUpdateDisplay = "0 min";
        private DateTime LastUpdateTime;
        private System.Timers.Timer TimeAgoTimer;
        private bool showStepCuotas = false;
        public List<MedioPago> MediosPago { get; set; }
        List<TipoDocumentoPtesaDTO> ListaDocumentos;
        private static readonly HashSet<int> OrigenesPermitidos = new()
        {
            (int)EnumTipoPago.PinDirecto,
            (int)EnumTipoPago.PSEColpatria
        };

        private static readonly Dictionary<EnumTipoPago, string> NombresPorTipoPago = new()
        {
            [EnumTipoPago.PinDirecto] = "PIN Directo Davibank",
            [EnumTipoPago.PSEColpatria] = "PSE Davibank",
            // agregar más mapeos si se habilitan más orígenes
        };


        protected override async Task OnInitializedAsync()
        {
            plataforma = (await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService")).Value?.Plataforma;

            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            GetCentroResponse = centroShared.Value.Respuesta;

            CentroSeleccionado.IdCentro = GetCentroResponse.IdCentro;
            CentroSeleccionado.Nombre = GetCentroResponse.Nombre;
            CentroSeleccionado.IdComercio = GetCentroResponse.IdComercio;
            CentroSeleccionado.IdDepartamento = GetCentroResponse.IdDepartamento;
            CentroSeleccionado.IdMunicipio = GetCentroResponse.IdMunicipio;
            CentroSeleccionado.IdZona = GetCentroResponse.IdZona;
            CentroSeleccionado.Direccion = GetCentroResponse.Direccion;
            CentroSeleccionado.Email = GetCentroResponse.Email;
            CentroSeleccionado.Fijo = GetCentroResponse.Fijo;
            CentroSeleccionado.Movil = GetCentroResponse.Movil;
            CentroSeleccionado.Latitud = GetCentroResponse.Latitud;
            CentroSeleccionado.Longitud = GetCentroResponse.Longitud;
            CentroSeleccionado.CodigoRUNT = GetCentroResponse.CodigoRUNT;

            Columnas = new List<ColumnList>
            {
                new() { NameColumn = "Cuota" },
                new() { NameColumn = "Valor de la cuota" },
                new() { NameColumn = "Valor pagado" },
                new() { NameColumn = "Saldo restante" },
                new() { NameColumn = "Fecha de abono" },
                new() { NameColumn = "Estado" }
            };

            ListaDocumentos = await MiLicenciaService.ObtenerTipoDocumentos();

            isLoading = false;
        }

        private async Task ConsultaDevolucionesChanged(ConsultaDevolucionPorPinRequest devolucionPorPinRequest)
        {
            mostrarBloque = true;
            isLoadingConsult = true;
            DevolucionPorPinRequest = devolucionPorPinRequest;
            await ConsultaDevolucionPin();
        }
        
        private async Task ConsultaDevolucionPin()
        {
            infoPin = await MiLicenciaService.ConsultaInfoPin<ResponseDTO<Entidad>>(DevolucionPorPinRequest);
            //showMessageError = !string.IsNullOrEmpty(ConsultaInfoPin.Mensaje) && !string.IsNullOrEmpty(ConsultaInfoPin?.Data?.Entidad?.IdMedioRecaudo) && ConsultaInfoPin.Data.Entidad.IdMedioRecaudo != $"{(int)EnumOrigenPin.PinDirecto}";

            ValidarConsultaInfoPin();

            if (infoPin != null && infoPin.Entidad != null && infoPin.Entidad.CuotasPactadas > 1)
            {
                ConstruirHistorialAsync();
            }
            if (infoPin?.Entidad != null)
            {
                await ObtenerConvenios();
            }
           
            isLoadingConsult = false;
        }

        private void ValidarConsultaInfoPin()
        {
            if (!EsInfoPinValido(infoPin))
            {
                AsignarError($"Es posible que la información ingresada esté errada o que el PIN no corresponda a su {plataforma}");
                return;
            }

            if (!EsMedioRecaudoValido(infoPin.Entidad.IdMedioRecaudo))
            {
                AsignarError(
                    "Este PIN no corresponde a un pago realizado por medio de PIN Directo Colpatria. " +
                    "Dirígete a centro.milicencia.co o milicencia.co para gestionar este PIN."
                );
                return;
            }

            if (!EsRuntValido(long.Parse(infoPin.Entidad.IdRunt)))
            {
                AsignarError("Es posible que el PIN ingresado no corresponda a su CEA.");
                return;
            }

            if (!EsOrigenCotizacionValido(infoPin.Entidad.IdOrigenCotizacion))
            {
                AsignarError(
                    "Este formulario solo permite actuar sobre pines recaudados con " +
                    "PIN DIRECTO Colpatria. Diríjase a centro.milicencia.co para gestionar este PIN."
                );
                return;
            }

            StateHasChanged();
        }

        private void AbrirConfirmacionIrInicio()
        {
            abrirModalIrInicio = true;
        }
        // Método generado por GitHub Copilot
        private void ConfirmarIrInicio()
        {
            abrirModalIrInicio = false; // cierra el modal
            Navigation.NavigateTo("/compradepin", true);
        }

        private void AbrirConfirmacionDevolucion()
        {
            abrirModalConfirmacion = true;
        }
        private async Task ConfirmarDevolucion()
        {
            abrirModalConfirmacion = false; // cierra el modal
            abrirModalProceso = true;
            await RealizarDevolucion();
            abrirModalProceso = false;

            await ConsultaDevolucionPin();
            StateHasChanged();
        }

        private string ObtenerDescripcion(string plataforma)
        {
            return plataforma switch
            {
                "CEA" => "Curso de conducción",
                "CRC" => "Examen médico",
                "CDA" => "Revisión técnico-mecánica", // opcional
                _ => ""
            };
        }

        private bool EsInfoPinValido(ResponseDTO<Entidad> infoPin)
        {
            return infoPin != null && infoPin.Codigo != -1 && infoPin.Entidad != null;
        }

        private bool EsMedioRecaudoValido(string idMedioRecaudo)
        {
            var origenesValidos = new[]
            {
        ((int)EnumOrigenPin.PinDirecto).ToString(),
        ((int)EnumOrigenPin.BancolombiaWompi).ToString(),
        ((int)EnumOrigenPin.PseColpatria).ToString(),
    };

            return origenesValidos.Contains(idMedioRecaudo);
        }

        private bool EsRuntValido(long idRunt)
        {
            return idRunt == CentroSeleccionado.CodigoRUNT;
        }

        private bool EsOrigenCotizacionValido(int? idOrigenCotizacion)
        {
            return idOrigenCotizacion == (int)EnumOrigenCotizacion.PortalAdministrativo || showMessageError;
        }

        private void AsignarError(string mensaje)
        {
            messageError = mensaje;
            mostrarBloque = false;
        }

        private async Task RealizarDevolucion()
        {
            if (infoPin.Entidad.IdOrigenCotizacion == (int)EnumOrigenCotizacion.PortalAdministrativo)
            {
                RespuestaRecaptchaDinamica<ResponseAnulacion> anulacion = await MiLicenciaService.DevolucionByPin<ResponseAnulacion>(DevolucionPorPinRequest, true);

                if (anulacion.Data?.CodigoRespuestaField != 0 || anulacion.Data?.MensajeRespuestaField != "OK")
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, anulacion?.Data?.MensajeRespuestaField);
                    return;
                }

                DevolucionRealizada = true;
            }
            else
            {
                DevolucionRealizada = false;
            }
        }

        private string GetEstadoPinClass(int estadoPin)
        {
            var baseClasses = "px-3 py-1 text-white text-sm font-semibold rounded ";

            return estadoPin switch
            {
                (int)EnumEstadoPin.Activo => baseClasses + "bg-verde-500",  // Activo
                (int)EnumEstadoPin.Anulado => baseClasses + "bg-rojo-500",   // Anulado
                (int)EnumEstadoPin.Referencia => baseClasses + "bg-azul-500",   // Referencia
                (int)EnumEstadoPin.Usado => baseClasses + "bg-gris-500",   // Usado
                _ => baseClasses + "bg-gray-400"    // Por defecto
            };
        }

        private async Task ActualizarManualAsync()
        {
            if (IsUpdating)
                return;

            await IniciarProcesoAsync();
        }

        private async Task IniciarProcesoAsync()
        {
            // Cambiar el estado del botón a "actualizando"
            IsUpdating = true;
            ButtonText = "Actualizando...";
            ButtonClass = "flex items-center text-white text-sm font-medium bg-gris-400 rounded-md px-3 py-1 transition-colors duration-150 cursor-not-allowed";

            //  Llama tu método principal de consulta
            await ConsultaDevolucionPin();

            // Inicia el contador regresivo (120 segundos)
            RemainingSeconds = 120;
            StartCountdown();

            // Inicia el temporizador de 2 minutos
            _ = HabilitarBotonDespuesDeEsperaAsync();
        }

        private void StartCountdown()
        {
            CountdownTimer?.Dispose();
            CountdownTimer = new System.Timers.Timer(1000);
            CountdownTimer.Elapsed += async (s, e) =>
            {
                if (RemainingSeconds > 0)
                {
                    RemainingSeconds--;
                    CountdownDisplay = $"{RemainingSeconds / 60:D2}:{RemainingSeconds % 60:D2}";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    CountdownTimer?.Stop();
                    IsUpdating = false;
                    ButtonText = "Actualizar estado";
                    ButtonClass = "flex items-center text-white text-sm font-medium bg-azul-600 hover:bg-azul-700 rounded-md px-3 py-1 transition-colors duration-150 cursor-pointer";

                    // Guardar la hora de la última actualización
                    LastUpdateTime = DateTime.Now;
                    LastUpdateDisplay = "Hace unos segundos";

                    // ?? Iniciar el actualizador automático del texto
                    StartTimeAgoUpdater();

                    await InvokeAsync(StateHasChanged);
                }
            };
            CountdownTimer.Start();
        }

        private async Task HabilitarBotonDespuesDeEsperaAsync()
        {
            await Task.Delay(120000); // 2 minutos

            IsUpdating = false;
            ButtonText = "Actualizar estado";
            ButtonClass = "flex items-center text-white text-sm font-medium bg-azul-600 hover:bg-azul-700 rounded-md px-3 py-1 transition-colors duration-150 cursor-pointer";

            await InvokeAsync(StateHasChanged); // Forzar renderizado al finalizar el contador
        }

        private string GetTimeAgo(DateTime time)
        {
            var diff = DateTime.Now - time;

            if (diff.TotalSeconds < 60)
                return $"{Math.Floor(diff.TotalSeconds)} seg";
            else if (diff.TotalMinutes < 60)
                return $"{Math.Floor(diff.TotalMinutes)} min";
            else if (diff.TotalHours < 24)
                return $"{Math.Floor(diff.TotalHours)} h";
            else
                return $"{Math.Floor(diff.TotalDays)} día{(diff.TotalDays >= 2 ? "s" : "")}";
        }

        private void StartTimeAgoUpdater()
        {
            TimeAgoTimer?.Dispose();
            TimeAgoTimer = new System.Timers.Timer(30000); // cada 30 segundos
            TimeAgoTimer.Elapsed += async (s, e) =>
            {
                if (LastUpdateTime != default)
                {
                    LastUpdateDisplay = GetTimeAgo(LastUpdateTime);
                    await InvokeAsync(StateHasChanged);
                }
            };
            TimeAgoTimer.Start();
        }

        public void Dispose()
        {
            CountdownTimer?.Dispose();
            TimeAgoTimer?.Dispose();
        }

        private void ConstruirHistorialAsync()
        {
            List<RowList> rows = new();

            infoPin.Entidad.Pagos ??= new List<Pago>();

            int referenciaCuota = 0;
            int cuotaActual = 1;
            decimal saldoPendienteEnCuota = infoPin.Entidad.ValorCuota;
            var ProximaCuota = 1;
            var SaldoProximaCuota = infoPin.Entidad.ValorCuota;

            int cuotasPactadas = infoPin.Entidad.CuotasPactadas;
            int cuotasPendientes = infoPin.Entidad.CuotasPendientes;
            int cuotasPagadas = cuotasPactadas - cuotasPendientes;

            bool PagoConAbono = false;
            bool PagoSuperaCuota = false;

            var primerPago = infoPin.Entidad.Pagos.FirstOrDefault();
            if (primerPago != null)
            {
                decimal valorRestado = primerPago.ValorTransaccion - infoPin.Entidad.ValorAliadoCuota;

                rows.Add(new RowList
                {
                    Row = new List<ColumnList>
                    {
                        new ColumnList { NameColumn = "Cuota", Value = "1" },
                        new ColumnList { NameColumn = "Valor de la cuota", Value = valorRestado.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) },
                        new ColumnList { NameColumn = "Valor pagado", Value = $"{valorRestado.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))}" },
                        new ColumnList { NameColumn = "Saldo restante", Value = $"{0.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))}" },
                        new ColumnList { NameColumn = "Fecha de abono", Value = $"{primerPago.FechaRecaudo:yyyy-MM-dd}" },
                        new ColumnList { NameColumn = "Estado", Value = "Pagada *" }
                    }
                });


                cuotaActual = 2;
            }

            var pagos = infoPin.Entidad.Pagos.Skip(1).ToList();

            foreach (var pago in pagos)
            {
                decimal valorNeto = pago.ValorTransaccion - infoPin.Entidad.ValorAliadoCuota;

                while (valorNeto > 0 && cuotaActual <= infoPin.Entidad.CuotasPactadas)
                {
                    if (valorNeto >= saldoPendienteEnCuota)
                    {
                        decimal epsilon = 5M;
                        bool dentroDelMargen = Math.Abs(valorNeto - infoPin.Entidad.ValorCuota) <= epsilon;
                        bool saldoIgualCuota = saldoPendienteEnCuota == infoPin.Entidad.ValorCuota;

                        PagoConAbono = !dentroDelMargen && valorNeto != infoPin.Entidad.ValorCuota && !saldoIgualCuota;
                        PagoSuperaCuota = !dentroDelMargen && valorNeto > infoPin.Entidad.ValorCuota;

                        rows.Add(new RowList
                        {
                            Row = new List<ColumnList>
                                {
                                    new ColumnList { NameColumn = "Cuota", Value = PagoConAbono ? "\u00A0" : cuotaActual.ToString() },
                                    new ColumnList { NameColumn = "Valor de la cuota", Value = PagoConAbono ? "\u00A0" : infoPin.Entidad.ValorCuota.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) },
                                    new ColumnList
                                    {
                                        NameColumn = "Valor pagado",
                                        Value = PagoConAbono
                                            ? $"{saldoPendienteEnCuota.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))} | Abono"
                                            : $"{saldoPendienteEnCuota.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))}"
                                    },
                                    new ColumnList { NameColumn = "Saldo restante", Value = $"{0.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))}" },
                                    new ColumnList { NameColumn = "Fecha de abono", Value = $"{pago.FechaRecaudo:yyyy-MM-dd}" },
                                    new ColumnList { NameColumn = "Estado", Value = "Pagada" }
                                }
                        });


                        valorNeto -= saldoPendienteEnCuota;
                        saldoPendienteEnCuota = infoPin.Entidad.ValorCuota;
                        cuotaActual++;
                    }
                    else
                    {
                        // Ejemplo simplificado de abonos parciales (puedes mantener tu lógica completa)
                        rows.Add(new RowList
                        {
                            Row = new List<ColumnList>
                                {
                                    new ColumnList { NameColumn = "Cuota", Value = cuotaActual.ToString() },
                                    new ColumnList { NameColumn = "Valor de la cuota", Value = infoPin.Entidad.ValorCuota.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) },
                                    new ColumnList { NameColumn = "Valor pagado", Value = $"{valorNeto.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))} | Abono" },
                                    new ColumnList { NameColumn = "Saldo restante", Value = (saldoPendienteEnCuota - valorNeto).ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) },
                                    new ColumnList { NameColumn = "Fecha de abono", Value = $"{pago.FechaRecaudo:yyyy-MM-dd}" },
                                    new ColumnList { NameColumn = "Estado", Value = "Pago Parcial" }
                                }
                        });


                        saldoPendienteEnCuota -= valorNeto;
                        valorNeto = 0;
                        referenciaCuota = cuotaActual;
                    }
                }
            }

            ProximaCuota = cuotaActual;
            SaldoProximaCuota = saldoPendienteEnCuota;

            // Agregar cuotas pendientes
            while (cuotaActual <= infoPin.Entidad.CuotasPactadas)
            {
                if (saldoPendienteEnCuota == infoPin.Entidad.ValorCuota)
                {
                    rows.Add(new RowList
                    {
                        Row = new List<ColumnList>
                            {
                                new ColumnList { NameColumn = "Cuota", Value = cuotaActual.ToString() },
                                new ColumnList { NameColumn = "Valor de la cuota", Value = infoPin.Entidad.ValorCuota.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) },
                                new ColumnList { NameColumn = "Valor pagado", Value = "-" },
                                new ColumnList { NameColumn = "Saldo restante", Value = "-" },
                                new ColumnList { NameColumn = "Fecha de abono", Value = "-" },
                                new ColumnList { NameColumn = "Estado", Value = "Pendiente" }
                            }
                    });

                }
                cuotaActual++;
                saldoPendienteEnCuota = infoPin.Entidad.ValorCuota;
            }

            HistorialPagos = rows;
        }

        public void showCuotaStep()
        {
            showStepCuotas = true;
        }

        private async Task ObtenerConvenios()
        {
            ConsultaCentoId consulta = new ConsultaCentoId
            {
                IdCentro = (int)CentroSeleccionado.IdCentro,
                CodigoDestino = infoPin.Entidad.IdRunt?.ToString()
            };

            List<ConvenioCentro> convenios = plataforma switch
            {
                "CEA" => await MiLicenciaService.ObtenerConveniosCEA(consulta),
                "CRC" => await MiLicenciaService.ObtenerConveniosCRC(consulta),
                _ => new List<ConvenioCentro>()
            };

            MediosPago = convenios
                .Where(c => OrigenesPermitidos.Contains(c.IdOrigenPin))
                .Select(c => new MedioPago
                {
                    Id = c.IdOrigenPin,
                    Nombre = NombresPorTipoPago.TryGetValue((EnumTipoPago)c.IdOrigenPin, out var nombre)
                        ? nombre
                        : c.ConvenioNombre, // fallback si llega un id no mapeado
                    IconoUrl = "",
                    ComprasCuotas = c.ComprasCuotas
                })
                .ToList();
        }

            // Inicio código generado por GitHub Copilot
            [SupplyParameterFromQuery(Name = "pin")] public string? Pin { get; set; }
            [SupplyParameterFromQuery(Name = "tipoId")] public int? TipoId { get; set; }
            [SupplyParameterFromQuery(Name = "numeroId")] public string? NumeroId { get; set; }
            // Fin código generado por GitHub Copilot

            // Inicio código generado por GitHub Copilot
            private decimal ValorTotal
            {
                get
                {
                    var cuotasExtras = Math.Max(infoPin.Entidad.CuotasPactadas - 1, 0);
                    var totalCuotas = (infoPin.Entidad.ValorCuota + infoPin.Entidad.ValorAliadoCuota) * cuotasExtras
                                      + infoPin.Entidad.ValorTransaccion;
                    return infoPin.Entidad.CuotasPactadas > 1 ? totalCuotas : infoPin.Entidad.ValorTransaccion;
                }
            }
            // Fin código generado por GitHub Copilot

        }
}

