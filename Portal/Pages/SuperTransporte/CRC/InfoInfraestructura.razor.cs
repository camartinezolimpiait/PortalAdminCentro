using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Services.SuperTransporte;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using System.Collections.Generic;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using System.Linq;
using portalAdministrativoSISEC.Entidades;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using PuppeteerSharp;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CRC
{
    public partial class InfoInfraestructura
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

        private List<SelectSuperTransporteDTO> _selectUbicaciones = new List<SelectSuperTransporteDTO>();

        private InfoInfraestructuraDto _infoInfraestructuraDto = new InfoInfraestructuraDto();

        private consultorios _consultorios_opto = new consultorios();
        private List<consultorios> _listConsultorios_opto = new List<consultorios>();

        private consultorios _consultorios_psico = new consultorios();
        private List<consultorios> _listConsultorios_psico = new List<consultorios>();

        private consultorios _consultorios_fono = new consultorios();
        private List<consultorios> _listConsultorios_fono = new List<consultorios>();

        private consultorios _consultorios_gene = new consultorios();
        private List<consultorios> _listConsultorios_gene = new List<consultorios>();

        private consultorios _consultorios_cert = new consultorios();
        private List<consultorios> _listConsultorios_cert = new List<consultorios>();

        private string error_ubicacion_oficina = "";
        private string error_ubicacion_aseo = "";
        private string error_ubicacion_recepcion = "";
        private string error_ubicaciones_und_sanitarias = "";
        private string error_ubicacion_sala_de_espera = "";

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("crcs");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //TODO:Se debe consultar las lista de profesiones
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_selectUbicaciones = await _superTransporteService.GetListaMaestra("PlantaFisica", campos);
            _selectUbicaciones = await _superTransporteService.GetListaMaestraSuperT("plantasFisicas");
            var populates = new List<string>() {
                "infraestructura.consultorios_opto",
                "infraestructura.consultorios_psico",
                "infraestructura.consultorios_fono",
                "infraestructura.consultorios_gene",
                "infraestructura.consultorios_cert",
            };
            var data = await _superTransporteService.GetCentroMultiPopulate(applicationShared.IdCentroStrappi, populates);
            if (data != null && data.data.attributes.infraestructura != null)
            {
                _infoInfraestructuraDto.oficina_admin = data.data.attributes.infraestructura.oficina_admin;
                _infoInfraestructuraDto.ubicacion_oficina = data.data.attributes.infraestructura.ubicacion_oficina;
                _infoInfraestructuraDto.servicios_aseo = data.data.attributes.infraestructura.servicios_aseo;
                _infoInfraestructuraDto.ubicacion_aseo = data.data.attributes.infraestructura.ubicacion_aseo;
                _infoInfraestructuraDto.recepcion = data.data.attributes.infraestructura.recepcion;
                _infoInfraestructuraDto.ubicacion_recepcion = data.data.attributes.infraestructura.ubicacion_recepcion;
                _infoInfraestructuraDto.unidades_sanitarias = data.data.attributes.infraestructura.unidades_sanitarias;
                _infoInfraestructuraDto.ubicaciones_und_sanitarias = data.data.attributes.infraestructura.ubicaciones_und_sanitarias;
                _infoInfraestructuraDto.sala_de_espera = data.data.attributes.infraestructura.sala_de_espera;
                _infoInfraestructuraDto.ubicacion_sala_de_espera = data.data.attributes.infraestructura.ubicacion_sala_de_espera;

                if (data.data.attributes.infraestructura.consultorios_opto != null)
                    _listConsultorios_opto = data.data.attributes.infraestructura.consultorios_opto;
                if (data.data.attributes.infraestructura.consultorios_psico != null)
                    _listConsultorios_psico = data.data.attributes.infraestructura.consultorios_psico;
                if (data.data.attributes.infraestructura.consultorios_fono != null)
                    _listConsultorios_fono = data.data.attributes.infraestructura.consultorios_fono;
                if (data.data.attributes.infraestructura.consultorios_gene != null)
                    _listConsultorios_gene = data.data.attributes.infraestructura.consultorios_gene;
                if (data.data.attributes.infraestructura.consultorios_cert != null)
                    _listConsultorios_cert = data.data.attributes.infraestructura.consultorios_cert;
            }
        }

        private async Task GuardarInformacion(EditContext context)
        {
            _loader.Show();
            if (context.Validate() && validarMultiSeleccion())
            {
                var resultado = await _superTransporteService.PutCentro(new InfoInfraestructuraDtoRequest()
                {
                    infraestructura = new InfoInfraestructuraCls()
                    {
                        oficina_admin = _infoInfraestructuraDto.oficina_admin,
                        ubicacion_oficina = _infoInfraestructuraDto.ubicacion_oficina,
                        servicios_aseo = _infoInfraestructuraDto.servicios_aseo,
                        ubicacion_aseo = _infoInfraestructuraDto.ubicacion_aseo,
                        recepcion = _infoInfraestructuraDto.recepcion,
                        ubicacion_recepcion = _infoInfraestructuraDto.ubicacion_recepcion,
                        unidades_sanitarias = _infoInfraestructuraDto.unidades_sanitarias,
                        ubicaciones_und_sanitarias = _infoInfraestructuraDto.ubicaciones_und_sanitarias,
                        sala_de_espera = _infoInfraestructuraDto.sala_de_espera,
                        ubicacion_sala_de_espera = _infoInfraestructuraDto.ubicacion_sala_de_espera,
                        consultorios_opto = _listConsultorios_opto,
                        consultorios_psico = _listConsultorios_psico,
                        consultorios_fono = _listConsultorios_fono,
                        consultorios_gene = _listConsultorios_gene,
                        consultorios_cert = _listConsultorios_cert
                    }
                }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro", false);
                }
            }
            _loader.Hide();
        }

        private bool validarMultiSeleccion()
        {
            if (_infoInfraestructuraDto.oficina_admin && string.IsNullOrEmpty(_infoInfraestructuraDto.ubicacion_oficina)
                || _infoInfraestructuraDto.servicios_aseo && string.IsNullOrEmpty(_infoInfraestructuraDto.ubicacion_aseo)
                || _infoInfraestructuraDto.recepcion && string.IsNullOrEmpty(_infoInfraestructuraDto.ubicacion_recepcion)
                || _infoInfraestructuraDto.unidades_sanitarias && string.IsNullOrEmpty(_infoInfraestructuraDto.ubicaciones_und_sanitarias)
                || _infoInfraestructuraDto.sala_de_espera && string.IsNullOrEmpty(_infoInfraestructuraDto.ubicacion_sala_de_espera))
            {
                return false;
            }
            return true;
        }
        private async Task addConsultorio(EditContext context, consultorios nuevo, int tipo)
        {
            if (context.Validate())
            {
                var consultorioOriginal = GetConsultorioByTipo(tipo);
                if (consultorioOriginal != null)
                {
                    consultorioOriginal.Add(nuevo);
                    SetConsultorioByTipo(tipo, consultorioOriginal);
                }
            }
        }

        private List<consultorios> GetConsultorioByTipo(int tipo)
        {

            switch (tipo)
            {
                case 1: return _listConsultorios_opto;
                case 2: return _listConsultorios_psico;
                case 3: return _listConsultorios_fono;
                case 4: return _listConsultorios_gene;
                case 5: return _listConsultorios_cert;
                default: return null;
            }
        }
        private void CambioSeleccion(ChangeEventArgs e, int id)
        {
            string mensaje = string.IsNullOrEmpty(e.Value.ToString()) ? "El campo no puede estar vacío." : "";
            cambioMensajeError(mensaje, id);
        }
        private void ActivarSeleccion(ChangeEventArgs e, int id)
        {
            if (e.Value.ToString().ToUpper() == "TRUE")
            {
                cambioMensajeError("El campo no puede estar vacío.", id);
            }
            else
            {
                resetearValor(id);
            }
        }
        private void cambioMensajeError(string mensaje, int id)
        {
            switch (id)
            {
                case 1:
                    error_ubicacion_oficina = mensaje;
                    break;
                case 2:
                    error_ubicacion_aseo = mensaje;
                    break;
                case 3:
                    error_ubicacion_recepcion = mensaje;
                    break;
                case 4:
                    error_ubicaciones_und_sanitarias = mensaje;
                    break;
                case 5:
                    error_ubicacion_sala_de_espera = mensaje;
                    break;
            }
        }
        private void resetearValor(int id)
        {
            switch (id)
            {
                case 1:
                    _infoInfraestructuraDto.ubicacion_oficina = "";
                    break;
                case 2:
                    _infoInfraestructuraDto.ubicacion_aseo = "";
                    break;
                case 3:
                    _infoInfraestructuraDto.ubicacion_recepcion = "";
                    break;
                case 4:
                    _infoInfraestructuraDto.ubicaciones_und_sanitarias = "";
                    break;
                case 5:
                    _infoInfraestructuraDto.ubicacion_sala_de_espera = "";
                    break;
            }
        }

        private void SetConsultorioByTipo(int tipo, List<consultorios> actualizado)
        {
            switch (tipo)
            {
                case 1:
                    _listConsultorios_opto = actualizado;
                    _consultorios_opto = new consultorios();
                    break;
                case 2:
                    _listConsultorios_psico = actualizado;
                    _consultorios_psico = new consultorios();
                    break;
                case 3:
                    _listConsultorios_fono = actualizado;
                    _consultorios_fono = new consultorios();
                    break;
                case 4:
                    _listConsultorios_gene = actualizado;
                    _consultorios_gene = new consultorios();
                    break;
                case 5:
                    _listConsultorios_cert = actualizado;
                    _consultorios_cert = new consultorios();
                    break;

            }
        }

        void ButtonOnClickQuitar(consultorios consultorio, int tipo)
        {
            var consultorioOriginal = GetConsultorioByTipo(tipo);
            if (consultorioOriginal != null)
            {
                consultorioOriginal.Remove(consultorio);
                SetConsultorioByTipo(tipo, consultorioOriginal);
            }
        }
    }
}
