using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPinCDA.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.CategoriasCDA
{
    public partial class CategoriasCDA
    {
        #region Inyeccion Dependencias

        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        [Parameter]
        public PagoPinCDA PagoPinCda { get; set; }

        [Parameter]
        public EventCallback<PagoPinCDA> PagoPinCdaChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        [Parameter]
        public EventCallback CostoHasChanged { get; set; }

        private readonly List<TipoVehiculo> tipoVehiculos = new();
        private DatosBasicosCdaModel datosBasicosModel = new();
        private DatosBasicosMoto datosBasicosMoto = new();
        private EditContext editContext;
        private EditContext editContextM;
        private bool isLoading = false;
       // private int? seleccionTipoVehiculo;
        private bool placaValidaMoto=false;
        private bool isValidCheckBox = false;
        private bool isTouched = false;
        private Dictionary<string, bool> touchedFields = new Dictionary<string, bool>();


        #endregion Variables

        #region Methods

        protected override async Task OnInitializedAsync()
        {
            datosBasicosModel = new DatosBasicosCdaModel
            {
                Anio = null,
                Mes = null,
                Dia = null,
                nombrePlaca = "",
            };
            datosBasicosMoto = new DatosBasicosMoto
            {
                nombrePlacaMoto = ""
            };
            editContext = new EditContext(datosBasicosModel);
            editContextM = new EditContext(datosBasicosMoto);
            await ObtenerListaVehiculos();

            PagoPinCda.TipoRecaudoCtrl = (int)EnumTipoPago.PinDirecto;                                                                       ///(int)EnumTipoPago.BancolombiaWompi;//(int)EnumTipoPago.PinDirecto;  SE COLOCA EL DE WOMPI PARA PODER PROBAR
            PagoPinCda.Cuotas = 0;
            PagoPinCda.ClienteCompra = (int)EnumTipoCliente.CDA;

            if (PagoPinCda?.FechaMatriculaVehiculo != null)
            {
                datosBasicosModel.seleccionTipoVehiculo = this.PagoPinCda.CategoriaVehiculo;//this.tipoVehiculos.Find(x => x.Id == ).Seleccionado = true;
                datosBasicosModel.Dia =PagoPinCda.FechaMatriculaVehiculo.Value.Day;
                datosBasicosModel.Mes = PagoPinCda.FechaMatriculaVehiculo.Value.Month;
                datosBasicosModel.Anio = PagoPinCda.FechaMatriculaVehiculo.Value.Year;
                datosBasicosModel.nombrePlaca = PagoPinCda.PlacaVehiculo.ToString();
                datosBasicosMoto.nombrePlacaMoto = PagoPinCda.PlacaVehiculo.ToString();
				isTouched = true;
                isValidCheckBox = true;
			}


            if (datosBasicosModel.Dia != null && datosBasicosModel.Mes != null && datosBasicosModel.Anio != null)
            {
                await NotifyValidationStateChanged();
            }
            await PagoPinCdaChanged.InvokeAsync(PagoPinCda);
        }

        private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
        {
            string inputValue = e.Value?.ToString();
            switch (fieldName)
            {
                case "Dia":
                    isTouched = true;
                    if (int.TryParse(inputValue, out int dia)){
                        datosBasicosModel.Dia = dia;
                    }
                    else{
                        datosBasicosModel.Dia = null;
                    }
                    
                    break;

                case "Mes":
                    isTouched = true;
                    if (int.TryParse(inputValue, out int Mes)){
                        datosBasicosModel.Mes = Mes;
                    }
                    else{
                        datosBasicosModel.Mes = null;
                    }
                    break;

                case "Anio":
                    isTouched = true;
                    if (int.TryParse(inputValue, out int Anio)){
                        datosBasicosModel.Anio = Anio;
                    }
                    else{
                        datosBasicosModel.Anio = null;
                    }
                    break;
                case "nombrePlaca":
                    isTouched = true;
                    datosBasicosModel.nombrePlaca = e.Value.ToString();
                    datosBasicosMoto.nombrePlacaMoto = null;
                    break;
                case "nombrePlacaMoto":
                    isTouched=true;
                    datosBasicosMoto.nombrePlacaMoto = e.Value.ToString();
                    datosBasicosModel.nombrePlaca = null;
                    break;

            }
            await NotifyValidationStateChanged();
        }

        private async Task ObtenerListaVehiculos()
        {

            List<ResponseTipoVehiculos> listaVehiculos = new();
            RequestTipoVehiculos request = new()
            {
                Aplication = new string[] { $"{EnumTipoCliente.CDA}" },
                IdCda = (int)PagoPinCda.CentroSeleccionado.IdCentro
            };
            isLoading = true;
            this.PagoPinCda.IsLoadingTemp = true;
            listaVehiculos = await MiLicenciaService.ObtenerTipoVehiculos(request);
            isLoading = false;
            this.PagoPinCda.IsLoadingTemp = false;
            if (listaVehiculos.Count > 0)
            {
                foreach (var item in listaVehiculos)
                {
                    tipoVehiculos.Add(new TipoVehiculo { Id = item.idTipoVehiculo, Nombre = item.nombre, Seleccionado = false });
                }

            }
        }

        private async Task NotifyValidationStateChanged()
        {
            bool valid = false;
            bool isValid = editContext.Validate(); // Esto valida el contexto de edición y devuelve true si es válido.
            placaValidaMoto = IsValidPlaca(datosBasicosMoto.nombrePlacaMoto);
            

            if (isValid || placaValidaMoto)
            {
                if (isValid && isValidCheckBox == true)
                {
                    valid = await HandleValidSubmit();
                    await GeneracionCosto();
                    PagoPinCda.IsValidDatosBasicos = true;
                }
                if (placaValidaMoto)
                {
                    if (datosBasicosModel.Dia != null &&
                        datosBasicosModel.Mes != null &&
                        datosBasicosModel.Anio != null &&
						isValidCheckBox ==true)
                    // && datosBasicosModel.nombrePlaca == null)
                    {
                        valid = await HandleValidSubmit();
                        await GeneracionCosto();
                        PagoPinCda.IsValidDatosBasicos = true;
                    }
                    else 
                    {
                        PagoPinCda.IsValidDatosBasicos = false;
                    }
                }
            }
            else
            {

                PagoPinCda.IsValidDatosBasicos = false;

            }
            await OnFormValidChanged.InvokeAsync(valid);
            StateHasChanged();
        }

        private async Task<bool> HandleValidSubmit()
        {
            bool isValid = false;
            if (datosBasicosModel.Dia != 0 && datosBasicosModel.Mes != 0 && datosBasicosModel.Anio != 0)
                isValid = await AdministrarCambiosDatosBasicos();
            await PagoPinCdaChanged.InvokeAsync(PagoPinCda);
            return isValid;
        }

        private async Task<bool> AdministrarCambiosDatosBasicos()
        {
            if (datosBasicosModel.seleccionTipoVehiculo != null && isTouched == true)
            {
                this.isValidCheckBox = true;
                PagoPinCda.TipoVehiculo = this.PagoPinCda.TipoVehiculo = tipoVehiculos.Find(x => x.Id == datosBasicosModel.seleccionTipoVehiculo).Nombre;
            }
            else 
            {
                this.isValidCheckBox = false;
            }

            if (datosBasicosModel.nombrePlaca != null)
            {

                PagoPinCda.PlacaVehiculo = datosBasicosModel.nombrePlaca.ToUpper();
            }
            else if (datosBasicosMoto.nombrePlacaMoto != null)
            {

                PagoPinCda.PlacaVehiculo = datosBasicosMoto.nombrePlacaMoto.ToUpper();
            }
            if ((int)datosBasicosModel.Dia <= DateTime.DaysInMonth((int)datosBasicosModel.Anio, (int)datosBasicosModel.Mes))
            {
                DateTime edadVehiculo = new((int)datosBasicosModel.Anio, (int)datosBasicosModel.Mes, (int)datosBasicosModel.Dia);
                if (edadVehiculo != new DateTime())
                {
                    if (!ValidarEdadVehiculo(edadVehiculo))
                    {
                        await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "La fecha ingresada no es válida");
                        return false;
                    }
                    int edad = await MiLicenciaService.CalcularEdadAspirante(edadVehiculo.ToString("MM-dd-yyyy"));
                    if (edad < 0)
                    {
                        await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "La edad del Vehiculo no es válida");

                        return false;
                    }
                        PagoPinCda.FechaMatriculaVehiculo = edadVehiculo;
                        PagoPinCda.EdadVehiculo = edad;
                }
                return true;
            }
            else
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Info, "La fecha ingresada no es válida");
                return false;
            }


        }
        private bool IsValidPlaca(string? placa)
        {
            if (string.IsNullOrEmpty(placa))
                return false;

            var regex = new Regex(@"^[A-Za-z]{3}\d{2}[A-Za-z]?$");
            return regex.IsMatch(placa);
        }

        public bool ValidarEdadVehiculo(DateTime fechaNacimiento)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - fechaNacimiento.Year;

            if (today.Month < fechaNacimiento.Month || (today.Month == fechaNacimiento.Month && today.Day < fechaNacimiento.Day))
            {
                age--;
            }

            return age >= 0;
        }

        private async Task RadioSelectCategoria(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value.ToString(), out int idCat))
                datosBasicosModel.seleccionTipoVehiculo = idCat;
            this.isTouched = true;
            this.isValidCheckBox = true;
            this.tipoVehiculos.Find(x => x.Id == idCat).Seleccionado = true;
            this.PagoPinCda.TipoVehiculo = tipoVehiculos.Find(x => x.Id == idCat).Nombre;
            this.PagoPinCda.CategoriaVehiculo = idCat;
            cambioInputPlaca(this.PagoPinCda.TipoVehiculo);
            await HandleInputChange(this.PagoPinCda.TipoVehiculo,args);
            await GeneracionCosto();
            await PagoPinCdaChanged.InvokeAsync(PagoPinCda);
            await Task.FromResult(true);
            await NotifyValidationStateChanged();
        }

        private void cambioInputPlaca(string nombreInput) 
        {
            if (nombreInput.Trim() == "Motocarros" || nombreInput.Trim() == "Motocicletas")
            {
                if (!string.IsNullOrEmpty(datosBasicosModel.nombrePlaca))
                {
                    datosBasicosMoto.nombrePlacaMoto = datosBasicosModel.nombrePlaca.ToUpper();
                    datosBasicosModel.nombrePlaca = null;
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(datosBasicosMoto.nombrePlacaMoto))
                {
                    datosBasicosModel.nombrePlaca = datosBasicosMoto.nombrePlacaMoto.ToUpper();
                    datosBasicosMoto.nombrePlacaMoto = null;
                }
            }
        }

        private async Task GeneracionCosto()
        {
            if (PagoPinCda.TipoVehiculo != null && PagoPinCda.EdadVehiculo > 0 && PagoPinCda.PlacaVehiculo != null)
            {
                await CostoHasChanged.InvokeAsync();
            }

        }

        //marcar campos tocados 
        private void OnBlur(string fieldName)
        {
            if (!touchedFields.ContainsKey(fieldName))
            {
                touchedFields[fieldName] = true;
            }
        }
        //verificar si un campo ha sido tocado
        private bool isFieldTouched(string fieldName)
        {
            return touchedFields.ContainsKey(fieldName) && touchedFields[fieldName];
        }

        #endregion Methods
    }

}

