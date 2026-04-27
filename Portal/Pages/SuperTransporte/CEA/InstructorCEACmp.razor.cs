using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Contracts.SuperTransporte;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Pages.SuperTransporte.CRC;

namespace portalAdministrativoSISEC.Pages.SuperTransporte.CEA
{
    public partial class InstructorCEACmp
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
        /// <summary>
        /// Entidad con la información del centro
        /// </summary>
        private ApplicationShared applicationShared = new ApplicationShared();

        private InstructorDtoCEA<IBrowserFile> _instructorCeaForm = new InstructorDtoCEA<IBrowserFile>();
        private List<InstructorDtoCEA<GetFile>> _instructoresCeaResponseDto = new List<InstructorDtoCEA<GetFile>>();
        private List<InstructorDtoCEA<int>> _instructoresCeaDtoReq = new List<InstructorDtoCEA<int>>();
        private List<SelectSuperTransporteDTO> _select = new List<SelectSuperTransporteDTO>();
        private bool CategoriasAutorizadasSeleccionadas = false;
        private bool _submitClicked = false;

        protected override async Task OnInitializedAsync()
        {
            _superTransporteService.SetPlataforma("ceas");
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            var campos = SelectSuperTransporteIndex.llenarCamposgenericos();
            //_select = await _superTransporteService.GetListaMaestra("TipoIdentificacion", campos);
            _select = await _superTransporteService.GetListaMaestraSuperT("tipoDocumentos");
            //TODO: BORRAR Y ENVIAR EL IDENTIFICADOR CORRECTO 
            //applicationShared.IdCentroStrappi = 4; // OJO Prueba Eliminar
            var populates = new List<string>() {
                "instructores.categorias_autorizadas",
                "instructores.licencia_instructor"
            };
            var data = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (data != null && data.data.attributes.instructores != null)
            {
                _instructoresCeaResponseDto = data.data.attributes.instructores;
                
                SetStringCategoria();
            }
        }

        private async Task GuardarInformacion()
        {
            _loader.Show();
            
            if (_instructoresCeaResponseDto != null && _instructoresCeaResponseDto.Any())
            {
                _instructoresCeaDtoReq.Clear();
                foreach (var instructor in _instructoresCeaResponseDto)
                {
                    InstructorDtoCEA<int> instructorReq = new InstructorDtoCEA<int>
                    {
                        tipo_doc = instructor.tipo_doc,
                        num_doc = instructor.num_doc,
                        primer_nombre = instructor.primer_nombre,
                        segundo_nombre = instructor.segundo_nombre,
                        primer_apellido = instructor.primer_apellido,
                        segundo_apellido = instructor.segundo_apellido,
                        num_licencia_instructor = instructor.num_licencia_instructor,
                        categorias_autorizadas = instructor.categorias_autorizadas
                    };
                    if (!String.IsNullOrEmpty(instructor.licencia_instructor?.data?.attributes?.name))
                    {
                        instructorReq.licencia_instructor = instructor.licencia_instructor.data.id;
                    }
                    _instructoresCeaDtoReq.Add(instructorReq);
                }
                var resultado = await _superTransporteService.PutCentro(new { instructores = _instructoresCeaDtoReq }, applicationShared.IdCentroStrappi);
                if (resultado != null)
                {
                    toastService.ShowSuccess(@"Se ha guardado la información correctamente.", "Información");
                    //Navigation.NavigateTo("/supertransporte/centro-cea", false);
                }
            }
            _loader.Hide();
        }

        private void HandleArchivoInstructor(InputFileChangeEventArgs e)
        {
            _instructorCeaForm.licencia_instructor = e.File;
        }

        /// <summary>
        /// Método para agregar un instructor al listado
        /// </summary>
        /// <param name="context"></param>
        private async Task AddInstructorCea(EditContext context, InstructorDtoCEA<IBrowserFile> instructor)
        {
            _submitClicked = true;
            if (context.Validate())
            {
                if (!_instructorCeaForm.categorias_autorizadas.A1 &&
                    !_instructorCeaForm.categorias_autorizadas.A2 &&
                    !_instructorCeaForm.categorias_autorizadas.B1 &&
                    !_instructorCeaForm.categorias_autorizadas.B2 &&
                    !_instructorCeaForm.categorias_autorizadas.B3 &&
                    !_instructorCeaForm.categorias_autorizadas.C1 &&
                    !_instructorCeaForm.categorias_autorizadas.C2 &&
                    !_instructorCeaForm.categorias_autorizadas.C3)
                {
                    CategoriasAutorizadasSeleccionadas = false;
                    
                }
                else
                {
                    var existe = _instructoresCeaResponseDto.Where(x => x.tipo_doc.Equals(instructor.tipo_doc) && x.num_doc.Equals(instructor.num_doc));
                    if (!existe.Any())
                    {
                        InstructorDtoCEA<GetFile> instructorNuevo = new InstructorDtoCEA<GetFile>();
                        instructorNuevo.tipo_doc = instructor.tipo_doc;
                        instructorNuevo.num_doc = instructor.num_doc;
                        instructorNuevo.primer_nombre = instructor.primer_nombre;
                        instructorNuevo.segundo_nombre = instructor.segundo_nombre;
                        instructorNuevo.primer_apellido = instructor.primer_apellido;
                        instructorNuevo.segundo_apellido = instructor.segundo_apellido;
                        instructorNuevo.categorias_autorizadas = instructor.categorias_autorizadas;
                        instructorNuevo.num_licencia_instructor = instructor.num_licencia_instructor;
                        if (!String.IsNullOrEmpty(instructor.licencia_instructor?.Name))
                        {
                            int idFile = await _superTransporteService.PostFile(instructor.licencia_instructor);
                            if (idFile > 0)
                            {
                                instructorNuevo.licencia_instructor = new GetFile { data = new DataFile { id = idFile, attributes = new AttributesFile { name = instructor.licencia_instructor.Name } } };
                            }
                            else
                            {
                                toastService.ShowError(@"Ha ocurrido un error al subir los archivos, intente nuevamente", "Información");
                                return;
                            }
                        }
                        _instructoresCeaResponseDto.Add(instructorNuevo);
                        SetStringCategoria();
                        _instructorCeaForm = new InstructorDtoCEA<IBrowserFile>();
                        CategoriasAutorizadasSeleccionadas = true;
                        await GuardarInformacion();
                    }
                    else
                    {
                        toastService.ShowError(@"El instructor ya se encuentra en la lista. Para continuar quite al registrado.", "Información");
                    }
                }

            }
        }

        /// <summary>
        /// Método para quitar del listado un instructor
        /// </summary>
        /// <param name="instructor"></param>
        private async Task RemoveInstructor(InstructorDtoCEA<GetFile> instructor)
        {
            try
            {
               // await CargaInstructores();
                
               _instructoresCeaResponseDto.Remove(instructor);
                
                    if (!String.IsNullOrEmpty(instructor.licencia_instructor?.data?.attributes?.name))
                    {
                        if (!await _superTransporteService.DeleteFile(instructor.licencia_instructor.data.id))
                        {
                            toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                            return;
                        }
                    }
                if (_instructoresCeaResponseDto.Count > 0)
                {
                    await GuardarInformacion();
                }
                else
                {
                    
                    var resultado = await _superTransporteService.PutCentro(new { instructores = Array.Empty<List<InstructorDtoCEA<int>>>() }, applicationShared.IdCentroStrappi);
                    if (resultado != null)
                    {
                        toastService.ShowSuccess(@"Se ha guardado la información correctamente.", "Información");
                    }
                }
               
            }
            catch (Exception ex)
            {
                toastService.ShowError(@"Ha ocurrido un error al eliminar el registro, intente nuevamente", "Información");
                return;
            }
        }

        void SetStringCategoria()
        {
            if (_instructoresCeaResponseDto != null && _instructoresCeaResponseDto.Any())
            {
                foreach (var instructor in _instructoresCeaResponseDto)
                {
                    if (instructor.categorias_autorizadas != null)
                    {
                        var categoriasAutorizadas = string.Empty;
                        if (instructor.categorias_autorizadas.A1)
                            categoriasAutorizadas += "A1 ";
                        if (instructor.categorias_autorizadas.A2)
                            categoriasAutorizadas += "A2 ";
                        if (instructor.categorias_autorizadas.B1)
                            categoriasAutorizadas += "B1 ";
                        if (instructor.categorias_autorizadas.B2)
                            categoriasAutorizadas += "B2 ";
                        if (instructor.categorias_autorizadas.B3)
                            categoriasAutorizadas += "B3 ";
                        if (instructor.categorias_autorizadas.C1)
                            categoriasAutorizadas += "C1 ";
                        if (instructor.categorias_autorizadas.C2)
                            categoriasAutorizadas += "C2 ";
                        if (instructor.categorias_autorizadas.C3)
                            categoriasAutorizadas += "C3 ";

                        instructor.categoriasAutorizadas = categoriasAutorizadas;
                    }
                }
            }
        }

        private async Task DownloadFileLicenciaInstructor(InstructorDtoCEA<GetFile> instructor)
        {

            try
            {
                if (instructor.licencia_instructor.data.attributes.url != null)
                {
                    string fileUrl = instructor.licencia_instructor.data.attributes.url.ToString();
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

        private async Task CargaInstructores()
        {
            var populates = new List<string>() {
                "instructores.categorias_autorizadas",
                "instructores.licencia_instructor"
            };
            var data = await _superTransporteService.GetCentroMultiPopulateCEA(applicationShared.IdCentroStrappi, populates);
            if (data != null && data.data.attributes.instructores != null)
            {
                _instructoresCeaResponseDto = data.data.attributes.instructores;

                SetStringCategoria();
            }
        }
    }
}


