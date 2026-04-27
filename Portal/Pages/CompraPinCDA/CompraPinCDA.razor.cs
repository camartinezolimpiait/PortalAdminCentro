using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Collections.Generic;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Enum;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA
{
    public partial class CompraPinCDA
    {
        #region Constructor

        [Inject]
        private IPermisoService PermisoService { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Constructor

        #region Variables

        private ApplicationSevice menuService = new();
        private GetDataResponseCentro getCentroResponse = new();
        private PagoPinCDA pagoPinCda = new();
        private int pasosCompraPin = 0;
        private bool isLoading = false;
        private bool isFormValid = false;
        private readonly HashSet<int> PasosVisitados = new();

        #endregion Variables


        #region Metodos

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");
            menuService = protectedSessionStore.Value;

            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            await ValidarPermisos();
            AsignacionCentro();
            isLoading = false;

            pagoPinCda.CentroSeleccionado.IdCentro = getCentroResponse.IdCentro;
            pagoPinCda.CentroSeleccionado.Nombre = getCentroResponse.Nombre;
            pagoPinCda.CentroSeleccionado.IdComercio = getCentroResponse.IdComercio;
            pagoPinCda.CentroSeleccionado.IdDepartamento = getCentroResponse.IdDepartamento;
            pagoPinCda.CentroSeleccionado.IdMunicipio = getCentroResponse.IdMunicipio;
            pagoPinCda.CentroSeleccionado.IdZona = getCentroResponse.IdZona;
            pagoPinCda.CentroSeleccionado.Direccion = getCentroResponse.Direccion;
            pagoPinCda.CentroSeleccionado.Email = getCentroResponse.Email;
            pagoPinCda.CentroSeleccionado.Fijo = getCentroResponse.Fijo;
            pagoPinCda.CentroSeleccionado.Movil = getCentroResponse.Movil;
            pagoPinCda.CentroSeleccionado.Latitud = getCentroResponse.Latitud;
            pagoPinCda.CentroSeleccionado.Longitud = getCentroResponse.Longitud;
            pagoPinCda.CentroSeleccionado.CodigoRUNT = getCentroResponse.CodigoRUNT;
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }

        private void GetIsCliked(int number)
        {
            switch ((PasosCompraPinCda)number)
            {
                case PasosCompraPinCda.configuracion:
                    NavigateToCompraPin();
                    pasosCompraPin = 0;
                    break;

                case PasosCompraPinCda.DatosBasicos:
                    pasosCompraPin = 1;
                    break;

                case PasosCompraPinCda.DatosPersonales:
                    pasosCompraPin = 2;
                    pagoPinCda.PreSeleccionado = true;
                    break;

                case PasosCompraPinCda.TipoTramiteComboCarro:
                    pasosCompraPin = 3;
                    pagoPinCda.PreSeleccionado = true;
                    break;

                case PasosCompraPinCda.CategoriaComboCarro:
                    pasosCompraPin = 4;
                    break;

                case PasosCompraPinCda.TipoTramiteComboMoto:

                    pasosCompraPin = 5;
                    break;

                case PasosCompraPinCda.CategoriaComboMoto:
                    pasosCompraPin = 6;
                    break;

                case PasosCompraPinCda.TipoTramiteSimple:
                    pasosCompraPin = 7;
                    break;

                case PasosCompraPinCda.CategoriasSimple:
                    pasosCompraPin = 8;
                    break;

                case PasosCompraPinCda.SeleccionCentro:
                    pasosCompraPin = 9;
                    break;

                case PasosCompraPinCda.MedioPago:
                    pasosCompraPin = 11;
                    break;

                case PasosCompraPinCda.CuotasCeas:
                    pasosCompraPin = 12;
                    break;

                case PasosCompraPinCda.ConfirmarCompra:
                    pasosCompraPin = 13;
                    break;

                default:
                    break;
            }
            isFormValid = false;
        }

        private void HandleFormValidChanged(bool isValid)
        {
            isFormValid = isValid;
            StateHasChanged();
        }

        private async Task OnSiguienteClicked()
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Success, "Información Guardada con éxito");
            GetIsCliked(13);
        }

        private async Task PinHasChanged(PagoPinCDA value)
        {
            // Maneja el evento y recibe el valor del componente hijo
            pagoPinCda = value;
            await Task.FromResult(true);
        }

        private async Task PasosCompraPinChanged(int value)
        {
            // Maneja el evento y recibe el valor del componente hijo

            NavegarResumen(value);
            await Task.FromResult(true);
        }

        private void AsignacionCentro()
        {
            pagoPinCda.CentroSeleccionado = new Centro()
            {
                IdCentro = getCentroResponse != null ? getCentroResponse.IdCentro : 0,
                Nombre = getCentroResponse != null ? getCentroResponse.Nombre : ""
            };
        }

        private async Task CostoHasChanged()
        {
            // Maneja el evento y recibe el valor del componente hijo

            await ObtenerCosto();

        }

        private async Task ObtenerCosto()  //ajustar para compra de pin cda
        {
            
            CotizacionPinCDA costo = await MiLicenciaService.GeneracionCostoCDA(pagoPinCda);
            await AsignarCosto(costo);
        
        }

        private async Task AsignarCosto(CotizacionPinCDA costo)
        {
            if (costo != null && await ValidarCosto(costo))
            {
                pagoPinCda.ValorDiscriminadoCotizacion.ValorTotal = costo.ValorTotal;
                pagoPinCda.ValorDiscriminadoCotizacion.CDA = (double)costo.ValorCDA;
                pagoPinCda.ValorDiscriminadoCotizacion.Sicov = (double)costo.ValorSicov;
                pagoPinCda.ValorDiscriminadoCotizacion.Banco = (double)costo.ValorAliado;
                pagoPinCda.ValorDiscriminadoCotizacion.Ansv = (double)costo.ValorANSV;
                pagoPinCda.CostoCuotas = costo.CalculoCoutas;

                if (costo.CalculoCoutas.Any())
                    pagoPinCda.ValorDiscriminadoCotizacion.CalculoCoutas = costo.CalculoCoutas;
            }
        }

        private async Task<bool> ValidarCosto(CotizacionPinCDA cotizacionPin)
        {
            if (cotizacionPin.ValorANSV == 0 || cotizacionPin.ValorTotal == 0 || cotizacionPin.ValorAliado == 0 )
            {
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "No se encontró información relacionada para calcular " +
                    "el costo del PIN para la categoría seleccionada ");
                return false;
            }
            return true;
        }

        private async Task ValidarPermisos()
        {
            int idCentro = getCentroResponse.IdCentro;
            int platataforma = menuService.ClienteId;
            bool tienePermiso = await PermisoService.TienePermisoParaCompraPin(idCentro, platataforma);
            tienePermiso = true;
            if (!tienePermiso)
            {
                Navigation.NavigateTo("/configuracion/PerfilMilicencia");
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "No se puede acceder a la ruta especificada");
            }
        }

        public void NavegarResumen(int paso)
        {
            pasosCompraPin = paso;
            PasosVisitados.Add(paso);
            StateHasChanged();
        }

        private void VolverDatosPersonales()
        {
            GetIsCliked((int)PasosCompraPinCda.DatosPersonales);
        }


        private void NavigateToCompraPin()
        {
            //Navigation.NavigateTo("/compradepin");
            var rutaActual = Navigation.Uri;

            // Recargar la ruta actual
            Navigation.NavigateTo(rutaActual, forceLoad: true);
        }
        #endregion Metodos
    }
}



