// Inicio código generado por GitHub Copilot
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.CouponCode
{
    /// <summary>
    /// Componente para gestionar códigos de convenio/referido en la compra de PIN
    /// </summary>
    public partial class CouponCode
    {
        #region Inyección de Dependencias

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion

        #region Parámetros

        /// <summary>
        /// Objeto que contiene la información de la compra del PIN
        /// </summary>
        [Parameter]
        public PagoPin PagoPin { get; set; }

        /// <summary>
        /// Evento que se dispara cuando se aplica un cupón exitosamente
        /// </summary>
        [Parameter]
        public EventCallback<string> CouponAplicadoEvent { get; set; }

        #endregion

        #region Variables Privadas

        private CouponCodeModel CouponModel { get; set; } = new();
        private EditContext EditContext { get; set; }
        private bool IsLoading { get; set; }
        private string MensajeRespuesta { get; set; } = string.Empty;
        private bool EsExitoso { get; set; }
        private bool CodigoAplicado { get; set; }
        private string NombreEmpresa { get; set; } = string.Empty;

        // Inicio código generado por GitHub Copilot
        /// <summary>
        /// Diccionario de mensajes de error personalizados según el código de respuesta del enum
        /// </summary>
        private static readonly Dictionary<EnumCodigoErrorCupon, string> CouponErrorMessages = new()
        {
            [EnumCodigoErrorCupon.CodigoInvalido] = "Código inválido",
            [EnumCodigoErrorCupon.ConvenioExpirado] = "Este convenio ha expirado",
            [EnumCodigoErrorCupon.ConvenioInactivo] = "Este convenio está inactivo",
            [EnumCodigoErrorCupon.ConvenioNoAplicaServicio] = "El convenio no aplica para este servicio",
            [EnumCodigoErrorCupon.ConvenioNoDisponible] = "Este convenio no está disponible"
        };
        // Fin código generado por GitHub Copilot

        #endregion

        #region Métodos del Ciclo de Vida

        // Método generado por GitHub Copilot
        protected override void OnInitialized()
        {
            InicializarFormulario();
        }

        #endregion

        #region Métodos Privados

        // Método generado por GitHub Copilot
        /// <summary>
        /// Inicializa el formulario y carga datos previos si existen
        /// </summary>
        private void InicializarFormulario()
        {
            EditContext = new EditContext(CouponModel);

            if (!string.IsNullOrEmpty(PagoPin?.CodigoConvenio))
            {
                CouponModel.ReferralCode = PagoPin.CodigoConvenio;
                CodigoAplicado = true;
                NombreEmpresa = PagoPin.EmpresaConvenio ?? PagoPin.CodigoConvenio;
                MensajeRespuesta = string.Empty;
                EsExitoso = true;
            }
        }

        // Método generado por GitHub Copilot
        /// <summary>
        /// Maneja el evento de cambio de input, convirtiendo a mayúsculas y validando caracteres
        /// </summary>
        private async Task OnInputChangeAsync(ChangeEventArgs e)
        {
            var value = e.Value?.ToString() ?? string.Empty;
            // Usar Regex para remover caracteres no alfanuméricos
            var upperValue = System.Text.RegularExpressions.Regex.Replace(value.ToUpper(), "[^A-Z0-9]", string.Empty);

            // Actualizar modelo
            CouponModel.ReferralCode = upperValue;

            // Limpiar mensajes
            MensajeRespuesta = string.Empty;
            EsExitoso = false;
            CodigoAplicado = false;

            // Notificar cambios al formulario
            EditContext.NotifyFieldChanged(EditContext.Field(nameof(CouponModel.ReferralCode)));

            await Task.CompletedTask;
        }

        // Método generado por GitHub Copilot
        /// <summary>
        /// Aplica el código de cupón validándolo contra el servicio
        /// </summary>
        private async Task OnApplyAsync()
        {
            if (!EditContext.Validate())
            {
                return;
            }

            var codigo = CouponModel.ReferralCode?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(codigo))
            {
                MensajeRespuesta = "Debe ingresar un código válido";
                EsExitoso = false;
                return;
            }

            IsLoading = true;
            MensajeRespuesta = string.Empty;
            StateHasChanged();

            try
            {
                // Inicio código generado por GitHub Copilot
                var request = new ValidateCouponRequest
                {
                    NombreConvenio = codigo,
                    TipoNegocio = PagoPin.ClienteCompra == (int)EnumTipoCliente.CEA
                  ? (int)EnumTipoPin.CEA
                     : (int)EnumTipoPin.CRC
                };

                var respuesta = await MiLicenciaService.ValidateCoupon(request);

                if (respuesta != null && respuesta.Codigo == 0)
                {
                    // Éxito
                    PagoPin.CodigoConvenio = codigo;
                    PagoPin.EmpresaConvenio = respuesta.Entidad?.Empresa ?? codigo;
                    NombreEmpresa = PagoPin.EmpresaConvenio;
                    CodigoAplicado = true;
                    MensajeRespuesta = string.Empty;
                    EsExitoso = true;

                    await CouponAplicadoEvent.InvokeAsync(codigo);
                }
                else
                {
                    // Error de validación - Usar el enum para mapear el mensaje
                    MensajeRespuesta = ObtenerMensajeError(respuesta?.Codigo ?? -1);
                    EsExitoso = false;
                }
                // Fin código generado por GitHub Copilot
            }
            catch (Exception)
            {
                MensajeRespuesta = CouponErrorMessages[EnumCodigoErrorCupon.CodigoInvalido];
                EsExitoso = false;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        // Método generado por GitHub Copilot
        /// <summary>
        /// Obtiene el mensaje de error correspondiente al código recibido
        /// </summary>
        /// <param name="codigo">Código de error recibido del servicio</param>
        /// <returns>Mensaje de error localizado</returns>
        private string ObtenerMensajeError(int codigo)
        {
            // Intentar convertir el código a enum
            if (System.Enum.IsDefined(typeof(EnumCodigoErrorCupon), codigo))
            {
                var errorEnum = (EnumCodigoErrorCupon)codigo;
                return CouponErrorMessages.TryGetValue(errorEnum, out var mensaje)
                     ? mensaje
                    : CouponErrorMessages[EnumCodigoErrorCupon.CodigoInvalido];
            }

            // Si el código no está en el enum, usar el mensaje por defecto
            return CouponErrorMessages[EnumCodigoErrorCupon.CodigoInvalido];
        }

        // Método generado por GitHub Copilot
        /// <summary>
        /// Limpia el código de cupón aplicado
        /// </summary>
        public void OnClear()
        {
            CouponModel.ReferralCode = string.Empty;
            PagoPin.CodigoConvenio = null;
            PagoPin.EmpresaConvenio = null;
            NombreEmpresa = string.Empty;
            MensajeRespuesta = string.Empty;
            EsExitoso = false;
            CodigoAplicado = false;
            EditContext.NotifyFieldChanged(EditContext.Field(nameof(CouponModel.ReferralCode)));
        }

        #endregion
    }
}
// Fin código generado por GitHub Copilot

