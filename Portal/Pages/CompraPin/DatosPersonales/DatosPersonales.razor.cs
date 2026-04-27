using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using Blazored.Toast.Services;
using System.Collections.Generic;
using System.Linq;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Util.Extension;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System.Diagnostics;
using portalAdministrativoSISEC.Entidades.Common;
using portalAdministrativoSISEC.Application.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Entidades.Facturacion;

namespace portalAdministrativoSISEC.Pages.CompraPin.DatosPersonales
{
    public partial class DatosPersonales
    {
        #region Variables

        [Inject]
        private IMiLicenciaService _miLicenciaService { get; set; }
        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }
        [Parameter]
        public EventCallback OnRetroceder { get; set; }

        public List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = new();


        public int EdadAspirante { get; set; }

        public event Action OnRequestSubmit;

        private DatosPersonalesModel DatosPersonalesModel = new();
        private ValidationMessageStore MessageStore;
        private EditContext editContext;
        private string MensajeErrorDocumento;
        private bool isLoading = true;
        #endregion Variables

        #region Metodos

        protected override async Task OnInitializedAsync()
        {

            EdadAspirante = PagoPin.EdadAspirante;

            DatosPersonalesModel = new DatosPersonalesModel
            {
                Correo = "",
                Nombre = "",
                Apellidos = "",
                Documento = ""
            };
            editContext = new EditContext(DatosPersonalesModel);
            editContext.OnValidationRequested += ValidarDocumento;
            await ObtenerTiposDocumento();
            MessageStore = new ValidationMessageStore(editContext);
            await GetDataForm();
            isLoading = false;
        }

        private void ValidarDocumento(object sender, ValidationRequestedEventArgs args)
        {
            MensajeErrorDocumento = null;
            var fieldIdentifier = new FieldIdentifier(DatosPersonalesModel, nameof(DatosPersonalesModel.Documento));
            if (!object.Equals(MessageStore, null)) { MessageStore.Clear(fieldIdentifier); }

            string pattern = string.Empty;
            string mensaje = string.Empty;

            switch (DatosPersonalesModel.TipoDocumento)
            {
                case 1: // Cédula de Ciudadanía
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case 2: // Cédula de Extranjería
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Cédula de Extranjería debe tener  6 dígitos y solo acepta números.";
                    break;

                case 3: // Tarjeta de Identidad
                    pattern = @"^[0-9]{6,11}$"; // Hasta 11 dígitos
                    mensaje = "La Tarjeta de Identidad debe tener entre 6 y 11 dígitos y solo acepta números.";
                    break;

                case 4: // Nit
                    pattern = @"^[0-9]{6,10}$"; // 6 o más dígitos
                    mensaje = "El NIT debe tener al menos 6 dígitos y solo acepta números.";
                    break;

                case 5: // Pasaporte
                    pattern = @"^[0-9a-zA-Z]{1,24}$"; // 6 o más caracteres alfanuméricos
                    mensaje = "El pasaporte debe tener hasta 24 caracteres";
                    break;

                case 10: // Contraseña Cédula de Ciudadanía
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Contraseña de la Cédula de Ciudadanía debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case 11: // Contraseña Cédula de Extranjería
                    pattern = @"^[0-9]{6,10}$"; // 6 a 10 dígitos
                    mensaje = "La Contraseña de la Cédula de Extranjería debe tener entre 6 y 10 dígitos y solo acepta números.";
                    break;

                case 13: // Permiso Protección Temporal (PPT)
                    pattern = @"^[0-9]{6,7}$"; // 6 a 7 dígitos
                    mensaje = "El Permiso por Protección Temporal debe tener entre 6 y 7 dígitos y solo acepta números.";
                    break;

                default:
                    break;
            }

            if (!string.IsNullOrEmpty(pattern) && !System.Text.RegularExpressions.Regex.IsMatch(DatosPersonalesModel.Documento, pattern))
            {
                MensajeErrorDocumento = mensaje;
                MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
            }
        }

        private async Task HandleValidSubmit()

        {
            if (PagoPin.Usuario == null)
            {
                PagoPin.Usuario = new portalAdministrativoSISEC.Application.Data.CompraPin.DatosBasicos();
            }

            await obtenerDescripcionTipoDoc();
            PagoPin.Usuario.Nombre = DatosPersonalesModel.Nombre;
            PagoPin.Usuario.Apellido = DatosPersonalesModel.Apellidos;
            PagoPin.Usuario.Correo = DatosPersonalesModel.Correo;
            PagoPin.Usuario.TipoDocumento = DatosPersonalesModel.TipoDocumento;
            PagoPin.EmisionOtraPersona = DatosPersonalesModel.EmisionOtraPersona;

            //PagoPin.Usuario.TipoDocumentoDescpcion =
            PagoPin.Usuario.NumDocumento = DatosPersonalesModel.Documento;
            PagoPin.Usuario.Celular = long.Parse(DatosPersonalesModel.Celular);

            await PagoPinChanged.InvokeAsync(PagoPin);
            await OnFormValidChanged.InvokeAsync(true);
        }

        private async Task obtenerDescripcionTipoDoc()
        {
            if (ListaDocumentos != null && ListaDocumentos.Any())
            {

                string tipoDocumentoString = DatosPersonalesModel.TipoDocumento?.ToString();

                var descripciondocumento = ListaDocumentos.FirstOrDefault(doc => doc.IdTipoSisec == tipoDocumentoString);

                if (descripciondocumento != null)
                {
                    PagoPin.Usuario.TipoDocumentoDescpcion = descripciondocumento.CodigoACH;
                }
                else
                {
                    PagoPin.Usuario.TipoDocumentoDescpcion = "CC";
                }
            }
        }

        private async Task ObtenerTiposDocumento()
        {
            ListaDocumentos = await _miLicenciaService.ObtenerTipoDocumentos();
            ListaDocumentos = ListaDocumentos.Where(doc => EdadAspirante >= doc.EdadMinima && EdadAspirante <= doc.EdadMaxima && doc.VisualizarCea == 1).ToList();
            
            // Codigo para filtrar los documentos no aceptados por colpatria
           ListaDocumentos= FilterDocumentsAviable.FilterDocumentsTypes(ListaDocumentos);
            // Inicio código generado por GitHub Copilot
            PagoPin.TiposDeDocumento = ListaDocumentos;
            // Fin código generado por GitHub Copilot

            var tiposPermitidos = ListaDocumentos.Select(doc => doc.IdTipoSisec).ToList();
            if (!tiposPermitidos.Contains(PagoPin.Usuario.TipoDocumento?.ToString()) && PagoPin.Usuario.TipoDocumento != 0)
            {
                PagoPin.Usuario.TipoDocumento = null;
                DatosPersonalesModel.TipoDocumento = null;
                MessageStore?.Clear();

                //await NotifyValidationStateChanged();
                await _miLicenciaService.ShowNotificacion(NotificationStatus.Warning, "Seleccione nuevamente el tipo de documento, esto debido al cambio de edad realizado.");
            }
        }

        private async Task GetDataForm()
        {
            if (PagoPin.Usuario.Apellido != "" && PagoPin.Usuario.Nombre != "" && PagoPin.Usuario.Correo != "" && PagoPin.Usuario.NumDocumento != "" && PagoPin.Usuario.Celular != 0)
            {
                DatosPersonalesModel.Correo = PagoPin.Usuario.Correo;
                DatosPersonalesModel.Nombre = PagoPin.Usuario.Nombre;
                DatosPersonalesModel.Apellidos = PagoPin.Usuario.Apellido;
                DatosPersonalesModel.TipoDocumento = PagoPin.Usuario.TipoDocumento == 0 ? null : PagoPin.Usuario.TipoDocumento;
                DatosPersonalesModel.Documento = PagoPin.Usuario.NumDocumento;
                DatosPersonalesModel.Celular = PagoPin.Usuario.Celular.ToString();
                DatosPersonalesModel.EmisionOtraPersona = PagoPin.EmisionOtraPersona;
                //await NotifyValidationStateChanged();
            }
        }

        // Inicio código generado por GitHub Copilot
        private string ObtenerMomentoFacturacionMensaje()
        {
            return PagoPin.MomentoFacturacion == (int)EnumEventoFacturacion.RecaudoPIN
                ? "se recaude"
                : "se enrole";
        }
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        private void SetEmisionOtraPersona(bool value)
        {
            DatosPersonalesModel.EmisionOtraPersona = value;
        }
        // Fin código generado por GitHub Copilot

        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        #endregion Metodos
    }
}


