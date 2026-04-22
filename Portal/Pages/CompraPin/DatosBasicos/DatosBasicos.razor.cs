using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.Pines;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Services.MiLicencia;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.DatosBasicos
{
    public partial class DatosBasicos
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

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

            if (PagoPin?.Usuario?.Genero != null && PagoPin?.Usuario?.Genero != 0)
                await SetGenero((int)PagoPin.Usuario.Genero);

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
            bool isValid = await HandleValid();

            await NotificarCambio(isValid);
            return isValid;
        }

        public async Task<bool> HandleValid()
        {
            PagoPin.Usuario ??= new Data.CompraPin.DatosBasicos();

            if (!TieneFechaCompleta() || !DatosBasicosModel.Sexo.HasValue || DatosBasicosModel.Sexo.Value == 0)
            {
                await PagoPinChanged.InvokeAsync(PagoPin);
                return false;
            }

            DateTime fechaNacimiento = new(
                DatosBasicosModel.Anio!.Value,
                DatosBasicosModel.Mes!.Value,
                DatosBasicosModel.Dia!.Value);

            int edad = await MiLicenciaService.CalcularEdadAspirante(fechaNacimiento.ToString("MM-dd-yyyy"));

            await SetGenero(DatosBasicosModel.Sexo.Value);
            DeterminarMayorEdad(edad);

            bool isValid = await AdministrarCambiosDatosBasicos(edad, fechaNacimiento);

            if (isValid)
                PagoPin.IsValidDatosBasicos = true;

            await PagoPinChanged.InvokeAsync(PagoPin);
            return isValid;
        }

        private async Task<bool> AdministrarCambiosDatosBasicos(int edad, DateTime fechaNacimiento)
        {
            if ((int)DatosBasicosModel.Dia <= DateTime.DaysInMonth((int)DatosBasicosModel.Anio, (int)DatosBasicosModel.Mes))
            {
                if (fechaNacimiento != new DateTime())
                {
                    if (edad < 16 || edad > 100)
                    {
                        return false;
                    }
                    else
                    {
                        if (PagoPin.EdadAspirante != edad)
                        {
                            if (edad < 18)
                            {
                                PagoPin.Categoria = "";
                                PagoPin.Categoria1 = "";
                                PagoPin.Categoria2 = "";
                                PagoPin.TipoTramite = null;
                                PagoPin.TipoTramite2 = null;
                                PagoPin.Usuario.TipoDocumentoDescpcion = string.Empty;
                            }
                            else
                            {
                                PagoPin.Usuario.TipoDocumento = 0;
                                PagoPin.Usuario.TipoDocumentoDescpcion = string.Empty;
                            }
                        }
                        PagoPin.EdadAspirante = edad;
                        PagoPin.Usuario.FechaNacimiento = fechaNacimiento;
                        if (edad < 18 && PagoPin.TramiteInstructor)
                        {
                            PagoPin.TramiteInstructor = false;
                            PagoPin.OpcionTramite = null;
                            PagoPin.TipoTramite = 0;                          
                        }
                        else if (edad < 18 && (PagoPin.OpcionTramite != null && PagoPin.TipoTramite == (int)EnumTramite.Recategorizar))
                        {
                            PagoPin.Categoria1 = PagoPin.Categoria2 = PagoPin.CategoriasActual = PagoPin.CategoriaSeleccionada = string.Empty;
                            PagoPin.OpcionTramite = null;
                            PagoPin.TipoTramite = 0;
                        }

                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        private void DeterminarMayorEdad(int edad)
        {
            if (edad < 18)
            {
                PagoPin.MayorEdad = false;
            }
            else
            {
                PagoPin.MayorEdad = true;
            }
        }

        private async Task SetGenero(int idSexo)
        {
            PagoPin.Usuario.Genero = idSexo;
            DatosBasicosModel.Sexo = idSexo;
            Sexos.Find(x => x.Id == idSexo).Selected = true;
            Sexos.Find(x => x.Id != idSexo).Selected = false;

            await Task.FromResult(true);
        }

        private void HandleValidationRequested(object sender, ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();
            if (!DateTime.TryParse($"{DatosBasicosModel.Anio}-{DatosBasicosModel.Mes}-{DatosBasicosModel.Dia}", out _))
            {
                messageStore?.Add(() => DatosBasicosModel.Dia, "La fecha de nacimiento no es válida");
            }
            else if (DateTime.TryParse($"{DatosBasicosModel.Anio}-{DatosBasicosModel.Mes}-{DatosBasicosModel.Dia}", out var birthDate))
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age)) age--;

                if (age < 16)
                {
                    messageStore?.Add(() => DatosBasicosModel.Dia, "Debe ser mayor de 16 años");
                }
            }
        }

        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        private bool TieneFechaCompleta()
        {
            return DatosBasicosModel.Dia != 0 &&
                   DatosBasicosModel.Mes != 0 &&
                   DatosBasicosModel.Anio != 0;
        }

        private async Task NotificarCambio(bool isValid)
        {
            await OnFormValidChanged.InvokeAsync(isValid);
        }

        #endregion Methods
    }
}