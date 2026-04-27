using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
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

        private bool MostrarDetalle = false;

        private ApplicationSevice menuService = new();

        [Parameter]
        public PagoPin PagoPin { get; set; }

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

        public bool datosPersonales { get; set; } = false;

        #endregion Variables

        #region Metodos

        public async Task cambiarAVista(PasosCotizacion vista)
        {
            await OnNavegarResumen.InvokeAsync(vista);
        }

        protected override void OnInitialized()
        {
            if (!string.IsNullOrEmpty(PagoPin.Usuario.Nombre) && !string.IsNullOrEmpty(PagoPin.Usuario.Apellido)
                && PagoPin.Usuario.Celular != null
                && !string.IsNullOrEmpty(PagoPin.Usuario.Correo)
                && !string.IsNullOrEmpty(PagoPin.Usuario.NumDocumento)
                && PagoPin.Usuario.TipoDocumento != null
            )
            {
                datosPersonales = true;
            }
        }

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(PagoPin.Usuario.Nombre) && !string.IsNullOrEmpty(PagoPin.Usuario.Apellido)
                && PagoPin.Usuario.Celular != null
                && !string.IsNullOrEmpty(PagoPin.Usuario.Correo)
                && !string.IsNullOrEmpty(PagoPin.Usuario.NumDocumento)
                && PagoPin.Usuario.TipoDocumento != null
)
            {
                datosPersonales = true;
            }
        }

        private string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        private string ObtenerTipoCentro()
        {
            return PagoPin.ClienteCompra switch
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
            return PagoPin?.FacturacionActiva == true
                && PagoPin.EmisionOtraPersona
                && PagoPin.DatosFacturacion != null
                && !string.IsNullOrWhiteSpace(ObtenerNombreFacturaElectronica())
                && !string.IsNullOrWhiteSpace(PagoPin.DatosFacturacion.CorreoFacturacion);
        }

        private string ObtenerTipoPersonaFacturaElectronica()
        {
            return PagoPin?.TiposPersonaFacturacion?
                .FirstOrDefault(tipo => tipo.Codigo == PagoPin?.DatosFacturacion?.TipoPersonaFacturacion.ToString())
                ?.Nombre ?? string.Empty;
        }

        private string ObtenerNombreFacturaElectronica()
        {
            if (PagoPin?.DatosFacturacion == null)
            {
                return string.Empty;
            }

            return PagoPin.DatosFacturacion.TipoPersonaFacturacion == 1
                ? (!string.IsNullOrWhiteSpace(PagoPin.DatosFacturacion.RazonSocialFacturacion)
                    ? PagoPin.DatosFacturacion.RazonSocialFacturacion
                    : PagoPin.DatosFacturacion.NombreComercialFacturacion)
                : $"{ToTitleCase(PagoPin.DatosFacturacion.NombresFacturacion)} {ToTitleCase(PagoPin.DatosFacturacion.ApellidosFacturacion)}".Trim();
        }

        private string ObtenerDocumentoFacturaElectronica()
        {
            if (PagoPin?.DatosFacturacion == null)
            {
                return string.Empty;
            }

            string tipoDocumento = ObtenerTipoDocumentoFacturacion(PagoPin.DatosFacturacion.TipoIdentificacionFacturacion);
            return string.IsNullOrWhiteSpace(tipoDocumento)
                ? PagoPin.DatosFacturacion.NumeroIdentificacionFacturacion
                : $"{tipoDocumento} {PagoPin.DatosFacturacion.NumeroIdentificacionFacturacion}".Trim();
        }

        private string ObtenerTipoDocumentoFacturacion(int tipoDocumento)
        {
            return PagoPin?.TiposDeDocumento?
                .FirstOrDefault(doc => doc.IdTipoSisec == tipoDocumento.ToString())
                ?.CodigoACH ?? string.Empty;
        }

        // Fin código generado por GitHub Copilot

        private void RemoveConvenio()
        {
            PagoPin.EmpresaConvenio = null;
            PagoPin.CodigoConvenio = null;
        }

        #endregion Metodos
    }
}
