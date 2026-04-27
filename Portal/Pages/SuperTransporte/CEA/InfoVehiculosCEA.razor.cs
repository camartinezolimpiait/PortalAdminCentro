using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Application.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System;
using Microsoft.JSInterop;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class InfoVehiculosCEA
    {
		[Inject]
		public NavigationManager Navigation { get; set; }
		[Inject]
		public ISuperTransporteService _superTransporteService { get; set; }
		[Inject]
		private IToastService toastService { get; set; }
		[Inject]
		ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private LoaderEventSubmit _loader = new LoaderEventSubmit();
        private ApplicationShared applicationShared = new ApplicationShared();

        private InfoVehiculosDtoCEA<IBrowserFile> _vehiculosDtoForm = new InfoVehiculosDtoCEA<IBrowserFile>();
        private List<InfoVehiculosDtoCEA<GetFile>> _vehiculosDtoResponse = new List<InfoVehiculosDtoCEA<GetFile>>();
        private List<InfoVehiculosDtoCEA<int?>> _vehiculosDtoRequest = new List<InfoVehiculosDtoCEA<int?>>();

        private InfoVehiculosDtoCEA<GetFile> _VehiculoDto = new InfoVehiculosDtoCEA<GetFile>();
        private SiniestroClases _siniestro = new SiniestroClases();
        private List<SiniestroClases> _siniestros = new List<SiniestroClases>();

        private List<SelectSuperTransporteDTO> _selectClaseVehiculo = new List<SelectSuperTransporteDTO>();
        private List<SelectSuperTransporteDTO> _selectTerritoriales = new List<SelectSuperTransporteDTO>();

        private bool CategoriasAutorizadasSeleccionadas = false;
        private bool _submitClicked = false;


        protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("ceas");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
            // applicationShared.IdCentro = 1; // OJO Prueba Eliminar
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_selectClaseVehiculo = await _superTransporteService.GetListaMaestra("ClaseVehiculo", campos);
            //_selectTerritoriales = await _superTransporteService.GetListaMaestra("Territoriales", campos);
            _selectClaseVehiculo = await _superTransporteService.GetListaMaestraSuperT("clasesVehiculos");
            _selectTerritoriales = await _superTransporteService.GetListaMaestraSuperT("territoriales");

            var populates = new List<string>() {
                "vehiculos.categorias_autorizadas",
                "vehiculos.detalle_siniestros",
                "vehiculos.licencia_de_transito",
                "vehiculos.copia_tarjeta_servicio",
            };
            var data = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (data != null && data.data.attributes.vehiculos != null)
            {
                _vehiculosDtoResponse = data.data.attributes.vehiculos;
                SetStringCategoria();
                SetSiniestros();
            }
        }

        private void SetSiniestros()
        {
            if (_vehiculosDtoResponse != null && _vehiculosDtoResponse.Any())
            {
                foreach (var vehiculo in _vehiculosDtoResponse)
                {
                    if (vehiculo.ha_estado_en_siniestro && vehiculo.detalle_siniestros != null && vehiculo.detalle_siniestros.Any())
                    {
                        string detalleSiniestros = string.Empty;
                        int countDuranteClase = 0;
                        int countSiniestros = 1;
                        foreach (var siniestro in vehiculo.detalle_siniestros)
                        {
                            if (siniestro.fue_durante_clase)
                                countDuranteClase++;
                            siniestro.id_siniestro = countSiniestros.ToString();
                            countSiniestros++;
                        }
                        detalleSiniestros = $@"Durante Clase:{countDuranteClase}, Fuera de Clase:{vehiculo.detalle_siniestros.Count - countDuranteClase}";
                        vehiculo.detalleSiniestros = detalleSiniestros;
                    }
                }
            }
        }

        private async Task GuardarInformacion()
		{
            _loader.Show();
            _vehiculosDtoRequest.Clear();
            if (_vehiculosDtoResponse != null && _vehiculosDtoResponse.Any())
            {
                foreach (var vehiculo in _vehiculosDtoResponse)
                {
                    InfoVehiculosDtoCEA<int?> vehiculoReq = new InfoVehiculosDtoCEA<int?>
                    {
                        placa = vehiculo.placa,
                        categorias_autorizadas = vehiculo.categorias_autorizadas,
                        modelo = vehiculo.modelo,
                        clase_de_vehiculo = vehiculo.clase_de_vehiculo,
                        num_tarjeta_servicio = vehiculo.num_tarjeta_servicio,
                        dt_que_expide_ts = vehiculo.dt_que_expide_ts,
                        ha_estado_en_siniestro = vehiculo.ha_estado_en_siniestro,
                        detalle_siniestros = vehiculo.detalle_siniestros,
                        fecha_expedicion_ts = vehiculo.fecha_expedicion_ts,
                        fecha_vencimiento_ts = vehiculo.fecha_vencimiento_ts,
                    };
                    if (!String.IsNullOrEmpty(vehiculo.licencia_de_transito?.data?.attributes?.name))
                    {
                        vehiculoReq.licencia_de_transito = vehiculo.licencia_de_transito.data.id;
                    }
                    if (!String.IsNullOrEmpty(vehiculo.copia_tarjeta_servicio?.data?.attributes?.name))
                    {
                        vehiculoReq.copia_tarjeta_servicio = vehiculo.copia_tarjeta_servicio.data.id;
                    }
                    _vehiculosDtoRequest.Add(vehiculoReq);
                }
                var resultado = await _superTransporteService.PutCentro(new { vehiculos = _vehiculosDtoRequest }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la información correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }

        private void AddSiniestro(EditContext context, SiniestroClases siniestro)
        {
            
            if (context.Validate())
            {
                
                    siniestro.id_siniestro = (_siniestros.Count + 1).ToString();
                    _siniestros.Add(siniestro);
                    _siniestro = new SiniestroClases();

            }
        }

        private async Task addVehiculo(EditContext context, InfoVehiculosDtoCEA<IBrowserFile> vehiculo) 
		{
            _submitClicked = true;
            if (context.Validate())
            {
                if (!_vehiculosDtoForm.categorias_autorizadas.A1 &&
                    !_vehiculosDtoForm.categorias_autorizadas.A2 &&
                    !_vehiculosDtoForm.categorias_autorizadas.B1 &&
                    !_vehiculosDtoForm.categorias_autorizadas.B2 &&
                    !_vehiculosDtoForm.categorias_autorizadas.B3 &&
                    !_vehiculosDtoForm.categorias_autorizadas.C1 &&
                    !_vehiculosDtoForm.categorias_autorizadas.C2 &&
                    !_vehiculosDtoForm.categorias_autorizadas.C3)
                {
                    CategoriasAutorizadasSeleccionadas = false;
                }
                else
                {
                    var existe = _vehiculosDtoResponse.Where(x => x.placa.Equals(vehiculo.placa));
                    if (!existe.Any())
                    {
                        InfoVehiculosDtoCEA<GetFile> vehiculoNuevo = new InfoVehiculosDtoCEA<GetFile>();
                        vehiculoNuevo.placa = vehiculo.placa;
                        vehiculoNuevo.categorias_autorizadas = vehiculo.categorias_autorizadas;
                        vehiculoNuevo.modelo = vehiculo.modelo;
                        if (!String.IsNullOrEmpty(vehiculo.licencia_de_transito?.Name))
                        {
                            int idFile = await _superTransporteService.PostFile(vehiculo.licencia_de_transito);
                            if (idFile > 0)
                            {
                                vehiculoNuevo.licencia_de_transito = new GetFile { data = new DataFile { id = idFile, attributes = new AttributesFile { name = vehiculo.licencia_de_transito.Name } } };
                            }
                            else
                            {
                                toastService.ShowError(@"Ha ocurrido un error al subir los archivos, intente nuevamente", "Información");
                                return;
                            }
                        }
                        vehiculoNuevo.clase_de_vehiculo = vehiculo.clase_de_vehiculo;
                        vehiculoNuevo.num_tarjeta_servicio = vehiculo.num_tarjeta_servicio;
                        vehiculoNuevo.dt_que_expide_ts = vehiculo.dt_que_expide_ts;
                        if (!String.IsNullOrEmpty(vehiculo.copia_tarjeta_servicio?.Name))
                        {
                            int idFile = await _superTransporteService.PostFile(vehiculo.copia_tarjeta_servicio);
                            if (idFile > 0)
                            {
                                vehiculoNuevo.copia_tarjeta_servicio = new GetFile { data = new DataFile { id = idFile, attributes = new AttributesFile { name = vehiculo.copia_tarjeta_servicio.Name } } };
                            }
                            else
                            {
                                toastService.ShowError(@"Ha ocurrido un error al subir los archivos, intente nuevamente", "Información");
                                return;
                            }
                        }
                        vehiculoNuevo.ha_estado_en_siniestro = vehiculo.ha_estado_en_siniestro;
                        vehiculoNuevo.detalle_siniestros = _siniestros;
                        vehiculoNuevo.fecha_expedicion_ts = vehiculo.getFechaExpedicionTs();
                        vehiculoNuevo.fecha_vencimiento_ts = vehiculo.getFechaVencimientoTs();

                        _vehiculosDtoResponse.Add(vehiculoNuevo);
                        SetStringCategoria();
                        SetSiniestros();
                        _vehiculosDtoForm = new InfoVehiculosDtoCEA<IBrowserFile>();
                        _siniestros = new List<SiniestroClases>();
                        CategoriasAutorizadasSeleccionadas = true;
                    }
                    else
                    {
                        toastService.ShowError(@"El vehiculo ya se encuentra en la lista. Para continuar quite al registrado.", "Información");
                    }
                }
                    
            }
        }

        void RemoveSiniestro(SiniestroClases siniestro)
        {
            _siniestros.Remove(siniestro);
            ActualizarSiniestros();
        }
        void ActualizarSiniestros()
        {
            int countSiniestros = 1;
            foreach (var siniestro in _siniestros)
            {
                siniestro.id_siniestro = countSiniestros.ToString();
                countSiniestros++;
            }
        }
        private async Task RemoveVehiculo(InfoVehiculosDtoCEA<GetFile> vehiculo)
		{
            try
            {
                if (!String.IsNullOrEmpty(vehiculo.licencia_de_transito?.data?.attributes?.name))
                {
                    if (!await _superTransporteService.DeleteFile(vehiculo.licencia_de_transito.data.id))
                    {
                        toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                        return;
                    }
                }
                if (!String.IsNullOrEmpty(vehiculo.copia_tarjeta_servicio?.data?.attributes?.name))
                {
                    if (!await _superTransporteService.DeleteFile(vehiculo.copia_tarjeta_servicio.data.id))
                    {
                        toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                        return;
                    }
                }
                _vehiculosDtoResponse.Remove(vehiculo);
            }
            catch (Exception ex)
            {
                toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                return;
            }
        }

        private void HandleArchivoTarjetaServicio(InputFileChangeEventArgs e)
        {
            _vehiculosDtoForm.copia_tarjeta_servicio = e.File;
        }
        private void HandleArchivoLicenciaTransito(InputFileChangeEventArgs e)
        {
            _vehiculosDtoForm.licencia_de_transito = e.File;
        }

        void SetStringCategoria()
        {
            if (_vehiculosDtoResponse != null && _vehiculosDtoResponse.Any())
            {
                foreach (var vehiculo in _vehiculosDtoResponse)
                {
                    if (vehiculo.categorias_autorizadas != null)
                    {
                        var categoriasAutorizadas = string.Empty;
                        if (vehiculo.categorias_autorizadas.A1)
                            categoriasAutorizadas += "A1 ";
                        if (vehiculo.categorias_autorizadas.A2)
                            categoriasAutorizadas += "A2 ";
                        if (vehiculo.categorias_autorizadas.B1)
                            categoriasAutorizadas += "B1 ";
                        if (vehiculo.categorias_autorizadas.B2)
                            categoriasAutorizadas += "B2 ";
                        if (vehiculo.categorias_autorizadas.B3)
                            categoriasAutorizadas += "B3 ";
                        if (vehiculo.categorias_autorizadas.C1)
                            categoriasAutorizadas += "C1 ";
                        if (vehiculo.categorias_autorizadas.C2)
                            categoriasAutorizadas += "C2 ";
                        if (vehiculo.categorias_autorizadas.C3)
                            categoriasAutorizadas += "C3 ";

                        vehiculo.categoriasAutorizadas = categoriasAutorizadas;
                    }
                }
            }
        }
        private async Task DownloadFileTarjetaServicio(InfoVehiculosDtoCEA<GetFile> vehiculo)
        {

            try
            {
                if (vehiculo.copia_tarjeta_servicio.data.attributes.url != null)
                {
                    string fileUrl = vehiculo.copia_tarjeta_servicio.data.attributes.url.ToString();
                    string sasToken = await _superTransporteService.GetBlobSasToken();
                    string fileDownload = fileUrl + sasToken;

                    await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
                }
            }
            catch (Exception ex)
            {
                toastService.ShowError(@"Ha ocurrido un error al descargar el adjunto, intente nuevamente", "Información");
                return;
            }
        }
        private async Task DownloadFileLicenciaTransito(InfoVehiculosDtoCEA<GetFile> vehiculo)
        {

            try
            {
                if (vehiculo.licencia_de_transito.data.attributes.url != null)
                {
                    string fileUrl = vehiculo.licencia_de_transito.data.attributes.url.ToString();
                    string sasToken = await _superTransporteService.GetBlobSasToken();
                    string fileDownload = fileUrl + sasToken;

                    await jsRuntime.InvokeVoidAsync("open", fileDownload, "_blank");
                }
            }
            catch (Exception ex)
            {
                toastService.ShowError(@"Ha ocurrido un error al descargar el adjunto, intente nuevamente", "Información");
                return;
            }
        }
    }
}


