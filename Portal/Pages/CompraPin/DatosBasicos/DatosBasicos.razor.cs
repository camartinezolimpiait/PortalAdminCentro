using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.CompraPin.DatosBasicos;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.DatosBasicos
{
    public partial class DatosBasicos
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        public ICompraPinDatosBasicosService DatosBasicosService { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        [Parameter]
        public EventCallback CostoHasChanged { get; set; }

        [Parameter]
        public EventCallback OnRetroceder { get; set; }

        private readonly List<Sexo> Sexos = new()
        {
            new Sexo { Id = (int)EnumSexo.Hombre, Genero = nameof(EnumSexo.Hombre) },
            new Sexo { Id = (int)EnumSexo.Mujer, Genero = nameof(EnumSexo.Mujer) }
        };

        [SupplyParameterFromForm]
        private DatosBasicosModel DatosBasicosModel { get; set; }

        private EditContext editContext;
        private ValidationMessageStore messageStore;

        #endregion Variables

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            DatosBasicosModel ??= new();

            editContext = new EditContext(DatosBasicosModel);
            editContext.OnValidationRequested += HandleValidationRequested;
            messageStore = new(editContext);

            if (PagoPin?.Usuario?.FechaNacimiento != null)
            {
                DatosBasicosModel.Dia = PagoPin.Usuario.FechaNacimiento.Value.Day;
                DatosBasicosModel.Mes = PagoPin.Usuario.FechaNacimiento.Value.Month;
                DatosBasicosModel.Anio = PagoPin.Usuario.FechaNacimiento.Value.Year;
            }

            if (PagoPin?.Usuario?.Genero != null && PagoPin.Usuario.Genero != 0)
            {
                await SetGenero((int)PagoPin.Usuario.Genero);
            }

            if (PagoPin.ClienteCompra == (int)EnumTipoCliente.CEA)
            {
                await ValidarCuotasCea();
            }

            await PagoPinChanged.InvokeAsync(PagoPin);
        }

        private async Task ValidarCuotasCea()
        {
            var idRunt = PagoPin?.CentroSeleccionado?.CodigoRUNT?.ToString() ?? string.Empty;

            var result = await MiLicenciaService
                .ConsultarValorCuotaAliado(new ConsultaValorAliado { IdRunt = idRunt });

            PagoPin.ConfiguracionCuotas.PermiteCuotas = result?.Entidad?.PermiteCuotas ?? false;
            PagoPin.ConfiguracionCuotas.ValorAliado = result?.Entidad?.ValorAliado ?? 0;
        }

        public async Task<bool> HandleValidSubmit()
        {
            var isValid = await HandleValid();
            await NotificarCambio(isValid);
            return isValid;
        }

        public async Task<bool> HandleValid()
        {
            PagoPin.Usuario ??= new portalAdministrativoSISEC.Application.Data.CompraPin.DatosBasicos();

            if (!DatosBasicosService.HasCompleteDate(DatosBasicosModel.Dia, DatosBasicosModel.Mes, DatosBasicosModel.Anio)
                || !DatosBasicosModel.Sexo.HasValue
                || DatosBasicosModel.Sexo.Value == 0)
            {
                await PagoPinChanged.InvokeAsync(PagoPin);
                return false;
            }

            var fechaNacimiento = new DateTime(
                DatosBasicosModel.Anio!.Value,
                DatosBasicosModel.Mes!.Value,
                DatosBasicosModel.Dia!.Value);

            var edad = await MiLicenciaService.CalcularEdadAspirante(fechaNacimiento.ToString("MM-dd-yyyy"));
            var isValid = await AdministrarCambiosDatosBasicos(edad, fechaNacimiento);

            if (isValid)
            {
                PagoPin.IsValidDatosBasicos = true;
            }

            await PagoPinChanged.InvokeAsync(PagoPin);
            return isValid;
        }

        private async Task<bool> AdministrarCambiosDatosBasicos(int edad, DateTime fechaNacimiento)
        {
            var result = DatosBasicosService.ApplyRules(
                new CompraPinDatosBasicosRulesInput(
                    DatosBasicosModel.Dia,
                    DatosBasicosModel.Mes,
                    DatosBasicosModel.Anio,
                    DatosBasicosModel.Sexo,
                    PagoPin.EdadAspirante,
                    PagoPin.TramiteInstructor,
                    PagoPin.OpcionTramite,
                    PagoPin.TipoTramite),
                edad,
                fechaNacimiento);

            if (!result.IsValid)
            {
                return false;
            }

            if (result.Genero.HasValue)
            {
                await SetGenero(result.Genero.Value);
            }

            if (result.ResetCategorias)
            {
                PagoPin.Categoria = string.Empty;
                PagoPin.Categoria1 = string.Empty;
                PagoPin.Categoria2 = string.Empty;
                PagoPin.TipoTramite = null;
                PagoPin.TipoTramite2 = null;
                PagoPin.Usuario.TipoDocumentoDescpcion = string.Empty;
            }
            else if (result.ResetTipoDocumento)
            {
                PagoPin.Usuario.TipoDocumento = 0;
                PagoPin.Usuario.TipoDocumentoDescpcion = string.Empty;
            }

            PagoPin.EdadAspirante = result.EdadAspirante;
            PagoPin.Usuario.FechaNacimiento = result.FechaNacimiento;
            PagoPin.MayorEdad = result.MayorEdad;

            if (result.ResetInstructorFlow)
            {
                PagoPin.TramiteInstructor = false;
                PagoPin.OpcionTramite = null;
                PagoPin.TipoTramite = 0;
            }
            else if (result.ResetRecategorizacionFlow)
            {
                PagoPin.Categoria1 = string.Empty;
                PagoPin.Categoria2 = string.Empty;
                PagoPin.CategoriasActual = string.Empty;
                PagoPin.CategoriaSeleccionada = string.Empty;
                PagoPin.OpcionTramite = null;
                PagoPin.TipoTramite = 0;
            }

            return true;
        }

        private async Task SetGenero(int idSexo)
        {
            PagoPin.Usuario.Genero = idSexo;
            DatosBasicosModel.Sexo = idSexo;
            Sexos.Find(x => x.Id == idSexo).Selected = true;
            Sexos.Find(x => x.Id != idSexo).Selected = false;

            await Task.CompletedTask;
        }

        private void HandleValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();

            var validationResult = DatosBasicosService.ValidateBirthDate(
                DatosBasicosModel.Dia is 0 ? null : DatosBasicosModel.Dia,
                DatosBasicosModel.Mes is 0 ? null : DatosBasicosModel.Mes,
                DatosBasicosModel.Anio is 0 ? null : DatosBasicosModel.Anio,
                DateTime.Today);

            if (!validationResult.IsValid && !string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
            {
                messageStore?.Add(() => DatosBasicosModel.Dia, validationResult.ErrorMessage);
            }
        }

        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        private async Task NotificarCambio(bool isValid)
        {
            await OnFormValidChanged.InvokeAsync(isValid);
        }

        #endregion Methods
    }
}


