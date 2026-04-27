using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using portalAdministrativoSISEC.Application.Data;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;
using System;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class InfraestructuraCEA
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

        private InfraestructuraDtoCEA _infraestructuraDtoCea = new InfraestructuraDtoCEA();

        private AulaCEA _aula = new AulaCEA();
        private List<AulaCEA> _listAulas = new List<AulaCEA>();

        private PistaCEA _pista = new PistaCEA();

        private PistaClsResponseCEA _pistaResponseCls = new PistaClsResponseCEA();
        private List<PistaClsResponseCEA> _listPistasResponseCls = new List<PistaClsResponseCEA>();

        private PistaClsCEA _pistaCls = new PistaClsCEA();
        private List<PistaClsCEA> _listPistasCls = new List<PistaClsCEA>();

        private UnidadSanitariaCEA _unidad_sanitaria = new UnidadSanitariaCEA();
        private List<UnidadSanitariaCEA> _listUnidadesSanitarias = new List<UnidadSanitariaCEA>();

        private List<SelectSuperTransporteDTO> _selectUbicacion = new List<SelectSuperTransporteDTO>();
        private List<DepartamentosSuperTransporteDTO> _selectDepartamento = new List<DepartamentosSuperTransporteDTO>();

        private string error_ubicacion_oficina = "";
        private string error_ubicacion_aseo = "";
        private string error_ubicacion_recepcion = "";
        private string error_ubicaciones_und_sanitarias = "";
        private string error_ubicacion_sala_de_espera = "";

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            //applicationShared.IdCentro = 1;
            var campos = SelectSuperTransporteIndex.llenarCamposConCiudades();
            //_selectUbicacion = await _superTransporteService.GetListaMaestra("PlantaFisica", campos);
            var selectDepartamento = await _superTransporteService.GetListaMaestra("Departamentos", campos);
            var ciudades = await _superTransporteService.GetListaMaestra("Ciudades", campos);
            _selectUbicacion = await _superTransporteService.GetListaMaestraSuperT("plantasFisicas");
            //var selectDepartamento = await _superTransporteService.GetListaMaestraSuperT("territoriales");
            //var ciudades = await _superTransporteService.GetListaMaestraSuperT("municipios");
            _selectDepartamento = selectDepartamento.Select(s => new DepartamentosSuperTransporteDTO
            {
                id = s.id,
                codigoDep = s.codigoDep,
                nombre = s.nombre,
                ciudades = ciudades.Where(w => w.codigoDep == s.codigoDep).Select(sc => new CiudadesSuperTransporteDTO { codigo = sc.codigo, codigoDep = sc.codigoDep, nombre = sc.nombre }).ToList()
            }).ToList();

            var populates = new List<string>() {
                "infraestructura.aulas",
                "infraestructura.pistas",
                "infraestructura.pistas.contrato_vigente",
                "infraestructura.pistas.ctl",
                "infraestructura.lista_unidades_sanitarias",
            };
            var data = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (data != null && data.data.attributes.infraestructura != null)
            {
                _infraestructuraDtoCea.espacio_clases_practicas = data.data.attributes.infraestructura.espacio_clases_practicas;
                _infraestructuraDtoCea.oficina_admin = data.data.attributes.infraestructura.oficina_admin;
                _infraestructuraDtoCea.ubicacion_oficina = data.data.attributes.infraestructura.ubicacion_oficina;
                _infraestructuraDtoCea.recepcion = data.data.attributes.infraestructura.recepcion;
                _infraestructuraDtoCea.ubicacion_recepcion = data.data.attributes.infraestructura.ubicacion_recepcion;
                _infraestructuraDtoCea.sala_de_espera = data.data.attributes.infraestructura.sala_de_espera;
                _infraestructuraDtoCea.ubicacion_sala_de_espera = data.data.attributes.infraestructura.ubicacion_sala_de_espera;
                _infraestructuraDtoCea.servicios_aseo = data.data.attributes.infraestructura.servicios_aseo;
                _infraestructuraDtoCea.ubicacion_aseo = data.data.attributes.infraestructura.ubicacion_aseo;
                _infraestructuraDtoCea.unidades_sanitarias = data.data.attributes.infraestructura.unidades_sanitarias;

                if (data.data.attributes.infraestructura.aulas != null)
                    _listAulas = data.data.attributes.infraestructura.aulas;
                if (data.data.attributes.infraestructura.pistas != null)
                {
                    _listPistasResponseCls = data.data.attributes.infraestructura.pistas;
                }
                if (data.data.attributes.infraestructura.lista_unidades_sanitarias != null)
                    _listUnidadesSanitarias = data.data.attributes.infraestructura.lista_unidades_sanitarias;
            }
        }

        private async Task GuardarInformacion()
        {
            _loader.Show();
            if (validarMultiSeleccion())
            {
                foreach (var pista in _listPistasResponseCls)
                {
                    _pistaCls = new PistaClsCEA();
                    _pistaCls.direccion = pista.direccion;
                    _pistaCls.departamento = pista.departamento;
                    _pistaCls.ciudad = pista.ciudad;
                    _pistaCls.propio = pista.propio;
                    if (!String.IsNullOrEmpty(pista.contrato_vigente?.data?.attributes?.name))
                    {
                        _pistaCls.contrato_vigente = pista.contrato_vigente.data.id;
                    }
                    if (!String.IsNullOrEmpty(pista.ctl?.data?.attributes?.name))
                    {
                        _pistaCls.ctl = pista.ctl.data.id;
                    }
                    _listPistasCls.Add(_pistaCls);
                }

                var resultado = await _superTransporteService.PutCentro(new InfraestructuraDtoRequestCEA()
                {
                    infraestructura = new InfraestructuraClsCEA()
                    {
                        espacio_clases_practicas = _infraestructuraDtoCea.espacio_clases_practicas,
                        oficina_admin = _infraestructuraDtoCea.oficina_admin,
                        ubicacion_oficina = _infraestructuraDtoCea.ubicacion_oficina,
                        recepcion = _infraestructuraDtoCea.recepcion,
                        ubicacion_recepcion = _infraestructuraDtoCea.ubicacion_recepcion,
                        sala_de_espera = _infraestructuraDtoCea.sala_de_espera,
                        ubicacion_sala_de_espera = _infraestructuraDtoCea.ubicacion_sala_de_espera,
                        servicios_aseo = _infraestructuraDtoCea.servicios_aseo,
                        ubicacion_aseo = _infraestructuraDtoCea.ubicacion_aseo,
                        unidades_sanitarias = _infraestructuraDtoCea.unidades_sanitarias,
                        aulas = _listAulas,
                        pistas = _listPistasCls,
                        lista_unidades_sanitarias = _listUnidadesSanitarias
                    }
                }, applicationShared.IdCentroStrappi);

                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }
        private bool validarMultiSeleccion()
        {
            if (_infraestructuraDtoCea.oficina_admin && string.IsNullOrEmpty(_infraestructuraDtoCea.ubicacion_oficina)
            || _infraestructuraDtoCea.servicios_aseo && string.IsNullOrEmpty(_infraestructuraDtoCea.ubicacion_aseo)
            || _infraestructuraDtoCea.recepcion && string.IsNullOrEmpty(_infraestructuraDtoCea.ubicacion_recepcion)
            || _infraestructuraDtoCea.unidades_sanitarias && !_listUnidadesSanitarias.Any()
                || _infraestructuraDtoCea.sala_de_espera && string.IsNullOrEmpty(_infraestructuraDtoCea.ubicacion_sala_de_espera))
            {
                return false;
            }
            return true;
        }
        private async Task AddAula(EditContext context, AulaCEA nuevo)
        {
            if (context.Validate())
            {
                _listAulas.Add(nuevo);
                _aula = new AulaCEA();
            }
        }
        private async Task AddPista(EditContext context, PistaCEA nuevo)
        {
            if (context.Validate())
            {
                if (!_listPistasResponseCls.Where(w => w.direccion == nuevo.direccion).Any())
                {
                    PistaClsResponseCEA pistaNuevo = new PistaClsResponseCEA();
                    pistaNuevo.direccion = nuevo.direccion;
                    pistaNuevo.departamento = nuevo.departamento;
                    pistaNuevo.ciudad = nuevo.ciudad;
                    pistaNuevo.propio = nuevo.propio;
                    if (!String.IsNullOrEmpty(nuevo.contrato_vigente?.Name))
                    {
                        int idFile = await _superTransporteService.PostFile(nuevo.contrato_vigente);
                        if (idFile > 0)
                        {
                            pistaNuevo.contrato_vigente = new GetFile { data = new DataFile { id = idFile, attributes = new AttributesFile { name = nuevo.contrato_vigente.Name } } };
                        }
                        else
                        {
                            toastService.ShowError(@"Ha ocurrido un error al subir los archivos, intente nuevamente", "Información");
                            return;
                        }
                    }
                    if (!String.IsNullOrEmpty(nuevo.ctl?.Name))
                    {
                        int idFile = await _superTransporteService.PostFile(nuevo.ctl);
                        if (idFile > 0)
                        {
                            pistaNuevo.ctl = new GetFile { data = new DataFile { id = idFile, attributes = new AttributesFile { name = nuevo.ctl.Name } } }; ;
                        }
                        else
                        {
                            toastService.ShowError(@"Ha ocurrido un error al subir los archivos, intente nuevamente", "Información");
                            return;
                        }
                    }
                    _listPistasResponseCls.Add(pistaNuevo);
                    _pista = new PistaCEA();
                }
                else
                {
                    toastService.ShowError(@"La pista seleccionada ya se encuentra registrada. Para continuar quitar la registrada.", "Información");
                }
            }
        }
        private async Task AddUnidadSanitaria(EditContext context, UnidadSanitariaCEA nuevo)
        {
            if (context.Validate())
            {
                if (!_listUnidadesSanitarias.Where(w => w.ubicacion_unidad_sanitaria == nuevo.ubicacion_unidad_sanitaria).Any())
                {
                    _listUnidadesSanitarias.Add(nuevo);
                    _unidad_sanitaria = new UnidadSanitariaCEA();
                }
                else
                {
                    toastService.ShowError(@"La ubicación de unidad sanitaria seleccionada ya se encuentra registrada. Para continuar quitar la registrada.", "Información");
                }
            }
        }

        void RemoveAula(AulaCEA aula)
        {
            _listAulas.Remove(aula);
        }

        private async Task RemovePista(PistaClsResponseCEA pista)
        {
            try
            {
                if (!String.IsNullOrEmpty(pista.contrato_vigente?.data?.attributes?.name))
                {
                    if (!await _superTransporteService.DeleteFile(pista.contrato_vigente.data.id))
                    {
                        toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                        return;
                    }
                }
                if (!String.IsNullOrEmpty(pista.ctl?.data?.attributes?.name))
                {
                    if (!await _superTransporteService.DeleteFile(pista.ctl.data.id))
                    {
                        toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                        return;
                    }
                }
                _listPistasResponseCls.Remove(pista);
            }
            catch (Exception ex)
            {
                toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                return;
            }
        }

        void RemoveUnidadSanitaria(UnidadSanitariaCEA unidadSanitaria)
        {
            _listUnidadesSanitarias.Remove(unidadSanitaria);
        }
        private void HandleContratoPista(InputFileChangeEventArgs e)
        {
            _pista.contrato_vigente = e.File;
        }
        private void HandleCtlPista(InputFileChangeEventArgs e)
        {
            _pista.ctl = e.File;
        }

        private bool validarCidudadseleccionada()
        {
            if (_selectDepartamento.Find(w => w.codigoDep == _pista.departamento) != null)
                if (_selectDepartamento.Find(w => w.codigoDep == _pista.departamento).ciudades != null)
                    if (_selectDepartamento.Find(w => w.codigoDep == _pista.departamento).ciudades.Find(f => f.codigo == _pista.ciudad) == null)
                        _pista.ciudad = "";
            return true;
        }
        private void CambioSeleccion(ChangeEventArgs e, int id)
        {
            string mensaje = string.IsNullOrEmpty(e.Value.ToString()) ? "El campo no puede estar vacío." : "";
            cambioMensajeError(id == 4 ? "" : mensaje, id);
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
                    _infraestructuraDtoCea.ubicacion_oficina = "";
                    break;
                case 2:
                    _infraestructuraDtoCea.ubicacion_aseo = "";
                    break;
                case 3:
                    _infraestructuraDtoCea.ubicacion_recepcion = "";
                    break;
                case 4:
                    _listUnidadesSanitarias = new List<UnidadSanitariaCEA>();
                    break;
                case 5:
                    _infraestructuraDtoCea.ubicacion_sala_de_espera = "";
                    break;
            }
        }
    }
}


