using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.CompraPin.Models;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.ResumenCompra
{
	public partial class ResumenCompra
	{

		#region Variables

		[Inject]
		private NavigationManager Navigation { get; set; }
        [Parameter]
        public PagoPin PagoPin { get; set; }
        [Parameter]
        public TransactionInfo transactionInfo { get; set; }

        public bool btnCopied { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        private bool abrirModalIrInicio;
        private bool isLoading = false;

        [Inject]
        private IMiLicenciaService _miLicenciaService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private Centro CentroSeleccionado = new();

        private Entidad infoPin;

        #endregion Variables

        #region Metodos

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await ConsultarInformacionAsync();
            isLoading = false;
        }

        private void AbrirConfirmacionIrInicio()
        {
            abrirModalIrInicio = true;
        }

        private void ConfirmarIrInicio()
        {
            abrirModalIrInicio = false; // cierra el modal
        }

        private async Task ConsultarInformacionAsync()
        {
            await InfoCentro();

            if (!string.IsNullOrEmpty(PagoPin.PinGenerado))
            {
                ResponseDTO<Entidad> response = await _miLicenciaService.ConsultaInfoPin<ResponseDTO<Entidad>>(new Entidades.Devolucion.ConsultaDevolucionPorPinRequest()
                {
                    IdRunt = PagoPin.CentroSeleccionado.CodigoRUNT.ToString(),
                    NumeroIdentificacion = PagoPin.Usuario.NumDocumento,
                    Pin = PagoPin.PinGenerado,
                    TipoIdentificacion = PagoPin.Usuario.TipoDocumento.Value
                });

                infoPin = response.Entidad;
            }
        }   

        private void Salir()
		{
			Navigation.NavigateTo("/compradepin", forceLoad: true);
		}

		private void ComprarOtroPin()
		{
			Navigation.NavigateTo("/compradepin?modo=crear", forceLoad:true);
		}

        private string ObtenerDescripcion(int plataforma)
        {
            return plataforma switch
            {
                (int)EnumTipoCliente.CEA => "Curso de conducción",
                (int)EnumTipoCliente.CRC => "Examen médico",
                (int)EnumTipoCliente.CDA => "Revisión técnico-mecánica", // opcional
                _ => ""
            };
        }

        private string ObtenerTipoCentro()
        {
            return PagoPin?.ClienteCompra switch
            {
                (int)EnumTipoCliente.CEA => "Centro de enseñanza",
                (int)EnumTipoCliente.CRC => "Centro de reconocimiento",
                _ => "Centro"
            };
        }

        private string ObtenerIdentificacionFactura()
        {
            if (PagoPin?.EmisionOtraPersona == true && PagoPin.DatosFacturacion != null)
            {
                string tipoDocumento = ObtenerTipoDocumentoFacturacion(PagoPin.DatosFacturacion.TipoIdentificacionFacturacion);
                string numeroDocumento = PagoPin.DatosFacturacion.NumeroIdentificacionFacturacion;

                return string.IsNullOrWhiteSpace(tipoDocumento)
                    ? numeroDocumento
                    : $"{tipoDocumento} {numeroDocumento}".Trim();
            }

            string tipoDocumentoUsuario = PagoPin?.Usuario?.TipoDocumentoDescpcion ?? string.Empty;
            string numeroDocumentoUsuario = PagoPin?.Usuario?.NumDocumento ?? string.Empty;

            return string.IsNullOrWhiteSpace(tipoDocumentoUsuario)
                ? numeroDocumentoUsuario
                : $"{tipoDocumentoUsuario} {numeroDocumentoUsuario}".Trim();
        }

        private string ObtenerTipoDocumentoFacturacion(int tipoDocumento)
        {
            return PagoPin?.TiposDeDocumento?
                .FirstOrDefault(doc => doc.IdTipoSisec == tipoDocumento.ToString())
                ?.CodigoACH ?? string.Empty;
        }

        private async Task CopiarPin()
        {
            await JS.InvokeVoidAsync("copiarAlPortapapeles", infoPin.Pin);
            btnCopied = true;
            StateHasChanged();
            await Task.Delay(1500);
            btnCopied = false;
            StateHasChanged();
        }

        private async Task InfoCentro()
        {
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            var response = centroShared.Value.Respuesta;

            CentroSeleccionado.IdCentro = response.IdCentro;
            CentroSeleccionado.Nombre = response.Nombre;
            CentroSeleccionado.CodigoRUNT = response.CodigoRUNT;

        }

        public int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;

            // Si aún no ha cumplido años este año, restar uno
            if (fechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;

            return edad;
        }

        #endregion

    }
}
