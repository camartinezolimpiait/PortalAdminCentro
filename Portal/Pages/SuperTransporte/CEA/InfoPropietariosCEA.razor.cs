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
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class InfoPropietariosCEA
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
		private InfoPropietariosDtoCEA _infoPropietariosDto = new InfoPropietariosDtoCEA();
        private List<InfoPropietariosDtoCEA> _lista_infoPropietariosDto = new List<InfoPropietariosDtoCEA>();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();
        private List<DepartamentosSuperTransporteDTO> _selectDepartamento = new List<DepartamentosSuperTransporteDTO>();

        protected override async Task OnInitializedAsync()
		{
			_superTransporteService.SetPlataforma("ceas");
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
           // applicationShared.IdCentro = 1; // OJO Prueba Eliminar
            var campos = SelectSuperTransporteIndex.llenarCamposConCiudades();
            //_select = await _superTransporteService.GetListaMaestra("TipoIdentificacion", campos);
            var selectDepartamento = await _superTransporteService.GetListaMaestra("Departamentos", campos);
            var ciudades = await _superTransporteService.GetListaMaestra("Ciudades", campos);

            _select = await _superTransporteService.GetListaMaestraSuperT("tipoDocumentos");
            //var selectDepartamento = await _superTransporteService.GetListaMaestraSuperT("territoriales");
            //var ciudades = await _superTransporteService.GetListaMaestraSuperT("municipios");
            _selectDepartamento = selectDepartamento.Select(s => new DepartamentosSuperTransporteDTO
            {
                id = s.id,
                codigoDep = s.codigoDep,
                nombre = s.nombre,
                ciudades = ciudades.Where(w => w.codigoDep == s.codigoDep).Select(sc => new CiudadesSuperTransporteDTO { codigo = sc.codigo, codigoDep = sc.codigoDep, nombre = sc.nombre }).ToList()
            }).ToList();

            var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "propietarios");
			if (data.data.attributes.propietarios != null)
                _lista_infoPropietariosDto = data.data.attributes.propietarios;
        }

		private async Task GuardarInformacion(EditContext context)
		{
			    _loader.Show();
                var resultado = await _superTransporteService.PutCentro(new InfoPropietariosDtoRequestCEA()
                {
                    propietarios = _lista_infoPropietariosDto
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    
                    var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "propietarios");
                    if (data.data.attributes.propietarios != null)
                    {
                        toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    }
                    else
                    {
                        toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                        Navigation.NavigateTo("/supertransporte/centro-cea", false);
                    }   
                }
            _loader.Hide();
        }
		private async Task addPropietario(EditContext context) 
		{
			if (context.Validate()) {
                var existe = _lista_infoPropietariosDto.Where(w => w.num_id == _infoPropietariosDto.num_id);
                if (!existe.Any())
                {
                    _lista_infoPropietariosDto.Add(_infoPropietariosDto);
                    _infoPropietariosDto = new InfoPropietariosDtoCEA();
                    var resultado = await _superTransporteService.PutCentro(new InfoPropietariosDtoRequestCEA()
                    {
                        propietarios = _lista_infoPropietariosDto
                    }, applicationShared.IdCentroStrappi);
                    if (resultado != null)
                    {

                        var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "propietarios");
                        if (data.data.attributes.propietarios != null)
                        {
                            toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                        }
                        else
                        {
                            toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                            Navigation.NavigateTo("/supertransporte/centro-cea", false);
                        }
                    }
                }
                else {
                    toastService.ShowError(@"El propietario seleccionado ya se encuentra registrado.", "Información");
                }
				
			}
		}
        private async void ButtonOnClickQuitar(InfoPropietariosDtoCEA propietario)
		{
            _lista_infoPropietariosDto.Remove(propietario);

            _loader.Show();
            var resultado = await _superTransporteService.PutCentro(new InfoPropietariosDtoRequestCEA()
            {
                propietarios = _lista_infoPropietariosDto
            }, applicationShared.IdCentroStrappi);
            if (resultado != null)
            {

                var data = await _superTransporteService.GetCentroCEA(applicationShared.IdCentroStrappi, "propietarios");
                if (data.data.attributes.propietarios != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                }
                else
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }

        private bool validarCidudadseleccionada()
        {
            if (_selectDepartamento.Find(w => w.codigoDep == _infoPropietariosDto.departamento) != null)
                if (_selectDepartamento.Find(w => w.codigoDep == _infoPropietariosDto.departamento).ciudades != null)
                    if (_selectDepartamento.Find(w => w.codigoDep == _infoPropietariosDto.departamento).ciudades.Find(f => f.codigo == _infoPropietariosDto.ciudad) == null)
                        _infoPropietariosDto.ciudad = "";
            return true;
        }
    }
}


