using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Enum.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;
using System.Collections.Generic;
// Inicio código generado por GitHub Copilot
using System.Linq;
// Fin código generado por GitHub Copilot
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Resumen
{
	public partial class Resumen
	{
		#region Variables

		[Parameter]
		public PagoPin pagoPin { get; set; }

        [Parameter]
        public bool EsConfirmacion { get; set; }

        [Parameter]
        public bool EsModificacion { get; set; }

        // Inicio código generado por GitHub Copilot
        [Parameter]
        public bool MostrarNotaRecaudo { get; set; }
        // Fin código generado por GitHub Copilot

        [Parameter]
		public EventCallback<PasosCotizacion> OnNavegarResumen { get; set; }


        private bool MostrarDetalle = false; 
        public bool datosPersonales { get; set; } = false;

        private ApplicationSevice menuService = new();

        #endregion Variables

        #region Metodos

        protected override void OnInitialized()
        {
            if (!string.IsNullOrEmpty(pagoPin.Usuario.Nombre) && !string.IsNullOrEmpty(pagoPin.Usuario.Apellido)
                && pagoPin.Usuario.Celular != null
                && !string.IsNullOrEmpty(pagoPin.Usuario.Correo)
                && !string.IsNullOrEmpty(pagoPin.Usuario.NumDocumento)
                && pagoPin.Usuario.TipoDocumento != null
            )
            {
                datosPersonales = true;
            }
        }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(pagoPin.Usuario.Nombre) && !string.IsNullOrEmpty(pagoPin.Usuario.Apellido)
                && pagoPin.Usuario.Celular != null
                && !string.IsNullOrEmpty(pagoPin.Usuario.Correo)
                && !string.IsNullOrEmpty(pagoPin.Usuario.NumDocumento)
                && pagoPin.Usuario.TipoDocumento != null
)
            {
                datosPersonales = true;
            }
        }

        public async Task cambiarAVista(PasosCotizacion vista)
        {
            await OnNavegarResumen.InvokeAsync(vista);
        }

        private string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        private string ObtenerTipoCentro()
        {
            return pagoPin.ClienteCompra switch
            {
                (int)EnumTipoCliente.CEA => "Centro de enseñanza",
                (int)EnumTipoCliente.CRC => "Centro de reconocimiento",
                (int)EnumTipoCliente.CDA => "Centro de diagnóstico", 
                _ => "Centro"
            };
        }

        // Inicio código generado por GitHub Copilot
        private bool MostrarDatosFacturaElectronica()
        {
            return pagoPin?.FacturacionActiva == true
                && pagoPin.EmisionOtraPersona
                && pagoPin.DatosFacturacion != null
                && !string.IsNullOrWhiteSpace(ObtenerNombreFacturaElectronica())
                && !string.IsNullOrWhiteSpace(pagoPin.DatosFacturacion.CorreoFacturacion);
        }

        private string ObtenerTipoPersonaFacturaElectronica()
        {
            return pagoPin?.TiposPersonaFacturacion?
                .FirstOrDefault(tipo => tipo.Codigo == pagoPin?.DatosFacturacion?.TipoPersonaFacturacion.ToString())
                ?.Nombre ?? string.Empty;
        }

        private string ObtenerNombreFacturaElectronica()
        {
            if (pagoPin?.DatosFacturacion == null)
            {
                return string.Empty;
            }

            return pagoPin.DatosFacturacion.TipoPersonaFacturacion == 1
                ? (!string.IsNullOrWhiteSpace(pagoPin.DatosFacturacion.RazonSocialFacturacion)
                    ? pagoPin.DatosFacturacion.RazonSocialFacturacion
                    : pagoPin.DatosFacturacion.NombreComercialFacturacion)
                : $"{ToTitleCase(pagoPin.DatosFacturacion.NombresFacturacion)} {ToTitleCase(pagoPin.DatosFacturacion.ApellidosFacturacion)}".Trim();
        }

        private string ObtenerDocumentoFacturaElectronica()
        {
            if (pagoPin?.DatosFacturacion == null)
            {
                return string.Empty;
            }

            string tipoDocumento = ObtenerTipoDocumentoFacturacion(pagoPin.DatosFacturacion.TipoIdentificacionFacturacion);
            return string.IsNullOrWhiteSpace(tipoDocumento)
                ? pagoPin.DatosFacturacion.NumeroIdentificacionFacturacion
                : $"{tipoDocumento} {pagoPin.DatosFacturacion.NumeroIdentificacionFacturacion}".Trim();
        }

        private string ObtenerTipoDocumentoFacturacion(int tipoDocumento)
        {
            return pagoPin?.TiposDeDocumento?
                .FirstOrDefault(doc => doc.IdTipoSisec == tipoDocumento.ToString())
                ?.CodigoACH ?? string.Empty;
        }
        // Fin código generado por GitHub Copilot


        #endregion Metodos
    }
}
