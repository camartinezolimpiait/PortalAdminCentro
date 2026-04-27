using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.CompraPin.Models;
using portalAdministrativoSISEC.Application.Data.CompraPin.Wompi;
using portalAdministrativoSISEC.Entidades;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Entidades.Pago.Wompi;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.Common;
using portalAdministrativoSISEC.Pages.CompraPin.ProcesoDePago;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.ComfirmacionCompra
{
    public partial class ConfirmacionCompra
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback OnRetroceder { get; set; }

        [Parameter]
        public EventCallback<int> PasosCompraPinChanged { get; set; }

        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public int PasosCompraPin { get; set; }

        [Parameter]
        public string PinGenerado { get; set; }

        private string AcceptanceToken { get; set; } = "";
        private int IdOrigenPin { get; set; }
        private int TipoReferenciaActual { get; set; }
        private string PdfWompi { get; set; } = "";

        public bool isTransactionInProgress { get; set; } = false;
        public bool proceedToPayment { get; set; } = false;

        public bool TransactionDone { get; set; } = false;

        private CostoCuota CuotaSeleccionada = new();
        private bool IsLoadingData = false;

        public int pasosCompraPin = 0;
        private bool abrirModal;
        public TransactionInfo transactionInfo { get; set; }

        #endregion Variables

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            switch (PagoPin.TipoRecaudoCtrl)
            {
                case 4:
                    TipoReferenciaActual = (int)EnumTipoReferencia.PinDirecto;
                    await PagoElectronico();
                    break;
                //Bancolombia
                case 13:
                    TipoReferenciaActual = (int)EnumTipoReferencia.BancolombiaWompi;

                    await CargarTycWompi();
                    await PagoElectronico();
                    break;

                case 14:
                    await PagoElectronico();
                    break;

                case 15:
                    await PagoElectronico();
                    break;

                default:
                    await PagoElectronico();
                    break;
            }
        }

        private async Task CargarTycWompi()
        {
            TokenAceptacion tokenAceptacion = await MiLicenciaService.ObtenerTokenAceptacion(PagoPin.ClienteCompra);

            PdfWompi = tokenAceptacion?.Permalink;
            AcceptanceToken = tokenAceptacion?.Acceptance_token;
        }

        private async Task PagoElectronico()
        {
            IdOrigenPin = (int)EnumOrigenPin.PinesOlimpia;
            if (PagoPin.TipoRecaudoCtrl == (int)EnumTipoPago.PSE && PagoPin.TipoBancoPSE != null)
            {
                TipoReferenciaActual = (int)EnumTipoReferencia.Electronica;
            }

            await Task.FromResult(true);
        }

        public async Task PagarReferenciaPinDirecto()
        {
            var cuotas = PagoPin.MediosPago
            .FirstOrDefault(x => x.Id == PagoPin.TipoRecaudoCtrl).ComprasCuotas;

            Entidades.Pago.Wompi.Data data = new()
            {
                AcceptanceToken = "",
                Apellidos = PagoPin.Usuario.Apellido,
                Categoria = PagoPin.Categoria1,
                CorreoElectronico = PagoPin.Usuario.Correo,
                CostoExtraRecaudo = 0,
                Cuotas = await SetCoutas(),
                DispersionAliado = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Banco),
                DispersionAns = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Ansv),
                DispersionCrc = await PagoPin.ConfiguracionCuotas.CalcularValorCea(PagoPin.ValorDiscriminadoCotizacion, cuotas),
                DispersionSicov = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Sicov),
                EstadoWompi = (int)EnumEstadoSandboxWompi.APPROVED,
                FechaNacimiento = ((DateTime)PagoPin.Usuario.FechaNacimiento).ToString("yyyy-MM-dd"),
                IdConvenio = PagoPin.TipoRecaudoCtrl.Value, //await ObtenerConvenioMedioPago(),
                IdOrigenCotizacion = (int)EnumOrigenCotizacion.PortalAdministrativo,
                IdRunt = $"{PagoPin.CentroSeleccionado.CodigoRUNT}", //"666555444", TEMPORAL MIENTRAS SE AJUSTA EL CONTRO
                Nombres = PagoPin.Usuario.Nombre,
                NumeroIdentificacion = PagoPin.Usuario.NumDocumento,
                Sexo = PagoPin.Usuario.Genero ?? 0,
                TelefonoContacto = $"{PagoPin.Usuario.Celular}",
                TipoIdentificacion = PagoPin.Usuario.TipoDocumento ?? 0,
                Tramite = await SetTramite(),
                UrlRedireccion = "",
                ValorTransaccion = PagoPin.ConfiguracionCuotas.PermiteCuotas && cuotas
                ? (int)Math.Round(PagoPin.ConfiguracionCuotas.CuotaSeleccionada.PrimeraCuota)
                : (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.ValorTotal)
            };

            if (PagoPin.EmisionOtraPersona && PagoPin.FacturacionActiva)
            {
                data.TipoIdentificacionFacturacion = PagoPin.DatosFacturacion.TipoIdentificacionFacturacion;
                data.TipoPersonaFacturacion = PagoPin.DatosFacturacion.TipoPersonaFacturacion;
                data.RazonSocialFacturacion = PagoPin.DatosFacturacion.RazonSocialFacturacion;
                data.NombresFacturacion = PagoPin.DatosFacturacion.NombresFacturacion;
                data.ApellidosFacturacion = PagoPin.DatosFacturacion.ApellidosFacturacion;
                data.NombreComercialFacturacion = PagoPin.DatosFacturacion.NombreComercialFacturacion;
                data.CorreoFacturacion = PagoPin.DatosFacturacion.CorreoFacturacion;
                data.NumeroIdentificacionFacturacion = PagoPin.DatosFacturacion.NumeroIdentificacionFacturacion;

            }else if (PagoPin.FacturacionActiva)
            {
                data.TipoIdentificacionFacturacion = PagoPin.Usuario.TipoDocumento.Value;
                data.TipoPersonaFacturacion = 2; // natural
                data.NombresFacturacion = PagoPin.Usuario.Nombre;
                data.ApellidosFacturacion = PagoPin.Usuario.Apellido;
                data.CorreoFacturacion = PagoPin.Usuario.Correo;
                data.NumeroIdentificacionFacturacion = PagoPin.Usuario.NumDocumento;
            }

            ReferenciaBancolombiaWompi referenciaWompi = new()
            {
                IdCliente = PagoPin.ClienteCompra,
                Data = data,
            };

            IsLoadingData = true;
            ShowDialog();

            string json = JsonSerializer.Serialize(referenciaWompi);

            try
            {
                ResponseReferenciaWompi responseReferenciaWompi = await MiLicenciaService.GenerarReferenciaBancolombiaWompi(referenciaWompi);
                
                CloseDialog();

                if ((responseReferenciaWompi?.Respuesta?.MensajeRespuestaField == "Ok"))
                {

                    PagoPin.Nut = responseReferenciaWompi?.Nut ?? CreateNutCode();
                    PagoPin.PinGenerado = responseReferenciaWompi?.Respuesta?.PinField ?? "";
                    PagoPin.FechaPago = responseReferenciaWompi?.FechaPago ?? DateTime.Now.ToString("dd/MM/yyyy");
                    PagoPin.UrlRedireccion = responseReferenciaWompi?.UrlRedireccion ?? "";

                    await PagoPinChanged.InvokeAsync(PagoPin);

                    switch (PagoPin.TipoRecaudoCtrl.Value)
                    {
                        case (int)EnumTipoPago.PinDirecto:
                            IrAResumenCompra(true, responseReferenciaWompi?.Respuesta?.PinField);

                            await MiLicenciaService.ConstruirCorreoWompi(new()
                            {
                                PinField = PagoPin.PinGenerado,
                                TotalTransaccion = data.ValorTransaccion.ToString("F0"),
                                Cuotas = data.Cuotas.Count + 1,
                                DispersionCentro = $"{data.DispersionCrc}"
                            });
                            break;

                        case (int)EnumTipoPago.PSEColpatria:
                            if (PagoPin.ClienteCompra == (int)EnumTipoCliente.CEA)
                            {
                                proceedToPayment = true;
                            }
                            else
                            {
                                await JS.InvokeVoidAsync("abrirEnNuevaPestana", PagoPin.UrlRedireccion);

                                isTransactionInProgress = true;
                                transactionInfo = new TransactionInfo
                                {
                                    NUT = PagoPin.Nut,
                                    Descripcion = ObtenerDescripcion(((EnumTipoCliente)PagoPin.ClienteCompra).ToString()),
                                    Empresa = "PSE Davibank",
                                    FechaTransaccion = responseReferenciaWompi?.FechaPago,
                                    ValorPagado = data.ValorTransaccion.ToString("F0")
                                };
                            }
                            _ = Task.Run(async () =>
                            {
                                await MiLicenciaService.SendProcessNotificationPse(PagoPin.PinGenerado);
                            });
                            break;

                        default:
                            throw new Exception("Tipo de recaudo no configurado.");
                    }

                    IsLoadingData = false;
                }
                else
                {
                    IrAResumenCompra(false, "");
                    IsLoadingData = false;
                }

               
            }
            catch (Exception ex)
            {
                IsLoadingData = false;
                CloseDialog();
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Error,
                        $"La referencia no se generó correctamente. Ha ocurrido un error intentelo nuevamente.");
                IrAResumenCompra(false, "");
            }
        }

        public async Task<List<Cuota>> SetCoutas()
        {
            List<Cuota> totalCuotas = [];

            if (PagoPin.TipoRecaudoCtrl == (int)EnumTipoPago.PSEColpatria)
            {
                return [];
            }

            if (PagoPin.ConfiguracionCuotas.PermiteCuotas)
            {
                // Inicio refactorización/optimización por GitHub Copilot
                if (PagoPin.Cuotas != 0)
                {
                    CuotaSeleccionada = PagoPin.ConfiguracionCuotas.CuotaSeleccionada;

                    for (int i = 0; i < CuotaSeleccionada.NumeroCuotas - 1; i++)
                    {
                        totalCuotas.Add(new Cuota()
                        {
                            DispersionAliado = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Banco),
                            ValorTransaccion = (int)Math.Round((decimal)CuotaSeleccionada?.ValorCuota)
                        });
                    }
                }
                // Fin refactorización/optimización por GitHub Copilot
            }

            return await Task.FromResult(totalCuotas);
        }

        public async Task<int> ObtenerConvenioMedioPago()
        {
            if (PagoPin.ClienteCompra == (int)EnumTipoCliente.CEA)
            {
                if (!string.IsNullOrEmpty(PagoPin.TipoBancoPSE))
                {
                    //PSE por defecto origen Olimpia = 9
                    return await Task.FromResult((int)EnumOrigenPin.PinesOlimpia);
                }
                else if (PagoPin.TipoPagoEfectivo != null)
                {
                    //Punto Pago CEA = 8
                    return await Task.FromResult((int)EnumOrigenPin.PuntoPagoCrc);
                }
                else if (TipoReferenciaActual == (int)EnumTipoReferencia.BancolombiaWompi)
                {
                    return await Task.FromResult((int)EnumOrigenPin.BancolombiaWompi);
                }
                else if (TipoReferenciaActual == (int)EnumTipoReferencia.PinDirecto)
                {
                    return await Task.FromResult((int)EnumOrigenPin.PinDirecto);
                }
            }

            if (PagoPin.TipoPagoEfectivo != null)
            {
                //Medio pago efectivo, para corresponsal bancario bancolombia de deja el origen Olimpia = 9
                return await Task.FromResult(
                    PagoPin.TipoPagoEfectivo == (int)EnumOrigenPin.CorresponsalBancolombia
                        ? (int)EnumOrigenPin.PinesOlimpia
                        : (int)PagoPin.TipoPagoEfectivo);
            }
            else if (TipoReferenciaActual == (int)EnumTipoReferencia.BancolombiaWompi)
            {
                return await Task.FromResult((int)EnumOrigenPin.BancolombiaWompi);
            }
            else
            {
                //PSE por defecto origen Olimpia = 9
                return await Task.FromResult((int)EnumOrigenPin.PinesOlimpia);
            }
        }

        public async Task<Entidades.Pago.Wompi.Tramite> SetTramite()
        {
            Entidades.Pago.Wompi.Tramite tramite = new()
            {
                Categoria1 = 0,
                Categoria2 = 0,
                CodigoCategoria1 = PagoPin.Categoria1,
                CodigoCategoria2 = PagoPin.Categoria2 ?? "",
                IdTramite1 = PagoPin.TipoTramite ?? 0,
                IdTramite2 = PagoPin.TipoTramite2 ?? 0
            };

            if (PagoPin.ClienteCompra == (int)EnumTipoCliente.CEA)
            {
                tramite.Categoria1 = PagoPin.CategoriasCea.FirstOrDefault(x => x.Codigo == PagoPin.Categoria1)?.IdCategoria ?? 0;

                tramite.Categoria2 = PagoPin.CategoriasCea.FirstOrDefault(x => x.Codigo == PagoPin.Categoria2)?.IdCategoria ?? 0;
            }
            else
            {
                tramite.Categoria1 = PagoPin.CategoriasCrc.FirstOrDefault(x => x.Codigo == PagoPin.Categoria1)?.IdCategoria ?? 0;

                tramite.Categoria2 = PagoPin.CategoriasCrc.FirstOrDefault(x => x.Codigo == PagoPin.Categoria2)?.IdCategoria ?? 0;
            }

            return await Task.FromResult(tramite);
        }

        public async Task Volver()
        {
            await OnRetroceder.InvokeAsync();
        }

        private void IrAResumenCompra(bool parametro, string Pin)
        {
            TransactionDone = true;
        }

        public void CambiarAVista(PasosCotizacion paso)
        {
            PasosCompraPinChanged.InvokeAsync((int)paso);
        }

        public async Task copiaNut()
        {
            IsLoadingData = true;
            proceedToPayment = false;
            transactionInfo = new TransactionInfo
            {
                NUT = PagoPin.Nut,
                Descripcion = ObtenerDescripcion(((EnumTipoCliente)PagoPin.ClienteCompra).ToString()),
                Empresa = "PSE Davibank",
                FechaTransaccion = PagoPin?.FechaPago,
                ValorPagado = PagoPin.MediosPago.Where(x => x.Id == PagoPin.TipoRecaudoCtrl).FirstOrDefault().ComprasCuotas && PagoPin.ConfiguracionCuotas.PermiteCuotas
                ? ((int)Math.Round(PagoPin.ConfiguracionCuotas.CuotaSeleccionada.PrimeraCuota)).ToString("F0")
                : ((int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.ValorTotal)).ToString("F0")
            };
            IsLoadingData = false;
            isTransactionInProgress = true;

            StateHasChanged();

            await JS.InvokeVoidAsync("abrirEnNuevaPestana", PagoPin.UrlRedireccion);
        }

        public async Task TransactionSuccess()
        {
            isTransactionInProgress = false;
            proceedToPayment = false;
            TransactionDone = true;
        }

        
        #endregion Methods

        #region Private Mothods

        /// <summary>
        /// Muestra el modal
        /// </summary>
        /// <returns></returns>
        public void ShowDialog()
        {
            abrirModal = true;
        }

        /// <summary>
        /// Oculta el modal
        /// </summary>
        /// <returns></returns>
        public void CloseDialog()
        {
            abrirModal = false;
        }

        public string obtenerFecha()
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            return date;
        }

        private static string CreateNutCode()
        {
            Random random = new();

            return $"{random.Next(11111111, 99999999)}{random.Next(11111111, 99999999)}";
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

        #endregion Private Mothods
    }
}

