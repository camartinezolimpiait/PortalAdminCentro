using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.FacturacionElectronica
{
    public partial class DatosFactura
    {
        #region 
        private bool isLoading = true;
        [Parameter]
        public PagoPin pagoPin { get; set; }
        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }
        [Parameter]
        public EventCallback OnRetroceder { get; set; }
        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }
        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        private EditContext editContext;
        private DatosFacturacion DatosFacturaModel = new();
        private List<ConsultaGenericaTipos> TiposPersona = new();
        public List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = new();
        [Inject]
        private IMiLicenciaService _miLicenciaService { get; set; }
        private ValidationMessageStore messageStore;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            editContext = new EditContext(DatosFacturaModel);
            messageStore = new ValidationMessageStore(editContext);
            

            if (pagoPin?.DatosFacturacion != null)
            {
                DatosFacturaModel.RazonSocialFacturacion = pagoPin.DatosFacturacion.RazonSocialFacturacion;
                DatosFacturaModel.TipoPersonaFacturacion = pagoPin.DatosFacturacion.TipoPersonaFacturacion;
                DatosFacturaModel.TipoIdentificacionFacturacion = pagoPin.DatosFacturacion.TipoIdentificacionFacturacion;
                DatosFacturaModel.NumeroIdentificacionFacturacion = pagoPin.DatosFacturacion.NumeroIdentificacionFacturacion;
                DatosFacturaModel.NombresFacturacion = pagoPin.DatosFacturacion.NombresFacturacion;
                DatosFacturaModel.ApellidosFacturacion = pagoPin.DatosFacturacion.ApellidosFacturacion;
                DatosFacturaModel.NombreComercialFacturacion = pagoPin.DatosFacturacion.NombreComercialFacturacion;
                DatosFacturaModel.CorreoFacturacion = pagoPin.DatosFacturacion.CorreoFacturacion;
            }
            else
            {
                DatosFacturaModel.TipoPersonaFacturacion = 2;
            }

            // Inicio código generado por GitHub Copilot
            TiposPersona = (await MiLicenciaService.GetPersonTypes()).Datos;
            pagoPin.TiposPersonaFacturacion = TiposPersona;
            // Fin código generado por GitHub Copilot
            await ObtenerTiposDocumento();
            isLoading = false;
        }

        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        private async Task HandleValidSubmit()
        {
            if (DatosFacturaModel.TipoPersonaFacturacion == 1)
            {
                DatosFacturaModel.TipoIdentificacionFacturacion = 4;
            }
            pagoPin.DatosFacturacion = DatosFacturaModel;
            await PagoPinChanged.InvokeAsync(pagoPin);
            await OnFormValidChanged.InvokeAsync(true);
        }

        private async Task ObtenerTiposDocumento()
        {
            ListaDocumentos = await _miLicenciaService.ObtenerTipoDocumentos();
            ListaDocumentos = FilterDocumentsAviable.FilterDocumentsTypes(ListaDocumentos);
            // Inicio código generado por GitHub Copilot
            pagoPin.TiposDeDocumento = ListaDocumentos;
            // Fin código generado por GitHub Copilot
        }

        private void OnTipoPersonaChanged(ChangeEventArgs e)
        {
            DatosFacturaModel = new()
            {
                TipoPersonaFacturacion = int.Parse(e.Value.ToString()), 
                TipoIdentificacionFacturacion = 0
            };

            // Reiniciar el EditContext para limpiar mensajes
            editContext = new EditContext(DatosFacturaModel);

            StateHasChanged();
        }
    }
}


