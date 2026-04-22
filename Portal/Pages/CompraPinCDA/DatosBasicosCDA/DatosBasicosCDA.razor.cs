using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPinCDA.Models;
using portalAdministrativoSISEC.Services.MiLicencia;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace portalAdministrativoSISEC.Pages.CompraPinCDA.DatosBasicosCDA
{
	public partial class DatosBasicosCDA
	{
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        [Parameter]
        public PagoPinCDA PagoPinCda { get; set; }

        [Parameter]
        public EventCallback<PagoPinCDA> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        private readonly List<Sexo> Sexos = new();
        private DatosBasicosCdaModel DatosBasicosModel = new();
        private EditContext editContext;

        #endregion Variables

        #region Methods

        protected override async Task OnInitializedAsync()
        {
			
			editContext = new EditContext(DatosBasicosModel);

            Sexos.Add(new Sexo { Id = (int)EnumSexo.Hombre, Genero = nameof(EnumSexo.Hombre) });
            Sexos.Add(new Sexo { Id = (int)EnumSexo.Mujer, Genero = nameof(EnumSexo.Mujer) });

            PagoPinCda.TipoRecaudoCtrl = (int)EnumTipoPago.BancolombiaWompi;//(int)EnumTipoPago.PinDirecto;  SE COLOCA EL DE WOMPI PARA PODER PROBAR

			PagoPinCda.Cuotas = 0;
            PagoPinCda.ClienteCompra = (int)EnumTipoCliente.CEA;

            if (PagoPinCda?.Usuario?.FechaNacimiento != null)
            {
                DatosBasicosModel.Dia = PagoPinCda.Usuario.FechaNacimiento.Value.Day;
                DatosBasicosModel.Mes = PagoPinCda.Usuario.FechaNacimiento.Value.Month;
                DatosBasicosModel.Anio = PagoPinCda.Usuario.FechaNacimiento.Value.Year;
            }

            if (PagoPinCda?.Usuario?.Genero != null && PagoPinCda?.Usuario?.Genero != 0)
                await SetGenero((int)PagoPinCda.Usuario.Genero);

            if (DatosBasicosModel.Dia != null && DatosBasicosModel.Mes != null && DatosBasicosModel.Anio != null && DatosBasicosModel.Sexo != 0)
                await NotifyValidationStateChanged();

            await PagoPinChanged.InvokeAsync(PagoPinCda);
        }

        private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
        {
            if (int.TryParse(e.Value.ToString(), out int result))
            {
                switch (fieldName)
                {
                    case "Dia":
                        DatosBasicosModel.Dia = result;
                        break;

                    case "Mes":
                        DatosBasicosModel.Mes = result;
                        break;

                    case "Anio":
                        DatosBasicosModel.Anio = result;
                        break;
                }
            }

            await NotifyValidationStateChanged();
        }

        private async Task NotifyValidationStateChanged()
        {
            bool isValid = editContext.Validate(); // Esto valida el contexto de edición y devuelve true si es válido.

            if (isValid)
            {
                isValid = await HandleValidSubmit();
                PagoPinCda.IsValidDatosBasicos = true;
            }
            else
            {
                PagoPinCda.IsValidDatosBasicos = false;
            }
            await OnFormValidChanged.InvokeAsync(isValid);
			StateHasChanged();
        }

        private async Task<bool> HandleValidSubmit()
        {
            bool isValid = false;
            if (DatosBasicosModel.Dia != 0 && DatosBasicosModel.Mes != 0 && DatosBasicosModel.Anio != 0)
                isValid = await AdministrarCambiosDatosBasicos();

            if (DatosBasicosModel.Sexo == 0 || PagoPinCda.Usuario.Genero == 0)
            {
                isValid = false;
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "Se debe seleccionar un género para continuar");
            }

            await PagoPinChanged.InvokeAsync(PagoPinCda);
            return isValid;
        }

        private async Task<bool> AdministrarCambiosDatosBasicos()
        {
            if ((int)DatosBasicosModel.Dia <= DateTime.DaysInMonth((int)DatosBasicosModel.Anio, (int)DatosBasicosModel.Mes))
            {
                DateTime fechaNacimiento = new((int)DatosBasicosModel.Anio, (int)DatosBasicosModel.Mes, (int)DatosBasicosModel.Dia);
                if (fechaNacimiento != new DateTime())
                {
                    int edad = await CalcularEdad(fechaNacimiento);
                    if (edad < 16 || edad > 100)
                    {
                        await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "La edad del aspirante no es válida");

                        return false;
                    }
                    else
                    {
                        PagoPinCda.EdadVehiculo = edad;
                        PagoPinCda.Usuario.FechaNacimiento = fechaNacimiento;
                        if (edad < 18 && PagoPinCda.TramiteInstructor)
                        {
                            PagoPinCda.TramiteInstructor = false;
                            PagoPinCda.OpcionTramite = null;
                            PagoPinCda.TipoTramite = 0;
                        }
                        if (edad < 18)
                        {
                            PagoPinCda.MayorEdad = false;
                        }
                        else
                        {
                            PagoPinCda.MayorEdad = true;
                        }

                        return true;
                    }
                }
                return false;
            }
            else
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "La fecha ingresada no es válida");
                return false;
            }
        }

        private async Task RadioSelection(ChangeEventArgs args)
        {
            if (PagoPinCda.Usuario == null)
                PagoPinCda.Usuario = new Data.CompraPin.CDA.DatosBasicosCDA();

            if (int.TryParse(args.Value.ToString(), out int idSexo))
                await SetGenero(idSexo);

            await NotifyValidationStateChanged();
        }

        private async Task SetGenero(int idSexo)
        {
            PagoPinCda.Usuario.Genero = idSexo;
            DatosBasicosModel.Sexo = (EnumSexo)idSexo;
            Sexos.Find(x => x.Id == idSexo).Selected = true;
            Sexos.Find(x => x.Id != idSexo).Selected = false;

            await Task.FromResult(true);
        }

        public async Task<int> CalcularEdad(DateTime fechaNacimiento)
        {
            int edad = DateTime.Today.Year - fechaNacimiento.Year;

            return await Task.FromResult(fechaNacimiento.Date > DateTime.Today.AddYears(-edad) ? edad-- : edad);
        }

        #endregion Methods
    }
}
