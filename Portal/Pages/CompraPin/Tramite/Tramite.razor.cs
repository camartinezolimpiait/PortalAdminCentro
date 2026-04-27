using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Tramite
{
    public partial class Tramite
    {
        #region Variables

        [Inject]
        private IMiLicenciaService _miLicenciaService { get; set; }

        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback OnRetroceder { get; set; }
        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }

        public List<SeleccionTramites> ListadoTramites { get; set; } = new();
        public List<int> IdsCategoriasAceptadas { get; set; } = new();
        public List<int> IdsCategoriasInstructorAceptadas { get; set; } = new();

        private IEnumerable<string> codigosInstructor = new[] { "IA", "IB", "IC" };

        public TiposTramitesHabilitados TiposTramitesHabilitados { get; set; } = new();
        public IEnumerable<string> CodigosInstructor { get => codigosInstructor; set => codigosInstructor = value; }
        private EditContext editContext;
        public OpcionCantidadModel cantidadModel { get; set; }

        private bool isLoading = false;

        #endregion Variables

        #region Metodos
        /// <summary>
        /// Inicializa el componente Tramite, obteniendo las categorias disponibles y los tramites habilitados para el usuario, tanto para instructor como para no instructor.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            cantidadModel ??= new();
            editContext = new EditContext(cantidadModel);
            isLoading = true;
            await ObtenerCategorias();
            await InicializacionTramites();

            if (PagoPin.TipoTramite != null && PagoPin.TipoTramite != 0)
            {
                cantidadModel.Cantidad = PagoPin.TipoTramite;
            }

            isLoading = false;
        }
        /// <summary>
        /// Inicializa los tramites disponibles para el usuario, tanto para instructor como para no instructor.
        /// </summary>
        /// <returns></returns>
        private async Task InicializacionTramites()
        {
            if (TiposTramitesHabilitados.PrimeraVez) { await AddSelectionTramite(EnumTramite.PrimeraVez, false); }
            if (TiposTramitesHabilitados.Recategorizacion) { 
                if (PagoPin.MayorEdad) { await AddSelectionTramite(EnumTramite.Recategorizar, false); }
                else if (PagoPin.TipoTramite != (int)EnumTramite.PrimeraVez) { PagoPin.TipoTramite = 0; } }
            if (TiposTramitesHabilitados.PrimeraVezInstructor) { await AddSelectionTramite(EnumTramite.PrimeraVezInstructor, true); }
            if (TiposTramitesHabilitados.RecategorizacionInstructor) { await AddSelectionTramite(EnumTramite.RecategorizarInstructor, true); }


            if (PagoPin.OpcionTramite != null && PagoPin.TipoTramite != 0)
            {
                int tipoTramite = 0;
                if (!PagoPin.TramiteInstructor && PagoPin.TipoTramite == (int)EnumTramite.PrimeraVez)
                    tipoTramite = (int)EnumTramite.PrimeraVez;

                if (!PagoPin.TramiteInstructor && PagoPin.TipoTramite == (int)EnumTramite.Recategorizar)
                    tipoTramite = (int)EnumTramite.Recategorizar;

                //Se cambia el tramite a 1 (Primera vez) porque en bd esta configurado como tipo de tramite 1 para instructor
                if (PagoPin.TramiteInstructor && PagoPin.TipoTramite == (int)EnumTramite.PrimeraVez)
                    tipoTramite = (int)EnumTramite.PrimeraVezInstructor;

                //Se cambia el tramite a 3 (Recategorizar) porque en bd esta configurado como tipo de tramite 3 para instructor
                if (PagoPin.TramiteInstructor && PagoPin.TipoTramite == (int)EnumTramite.Recategorizar)
                    tipoTramite = (int)EnumTramite.RecategorizarInstructor;

                foreach (var tramite in ListadoTramites)
                    tramite.Selected = tramite.Id == tipoTramite;
            }
        }
        /// <summary>
        /// Obtiene las categorias disponibles para el centro seleccionado y las categorias generales, tanto para instructor como para no instructor.
        /// </summary>
        /// <returns></returns>
        private async Task ObtenerCategorias()
        {
            if (!PagoPin.CategoriasCea.Any())
            {

                PagoPin.CategoriasCea = await _miLicenciaService.ConsultaCategoriasCentro(this.PagoPin.CentroSeleccionado.IdCentro);
                await PagoPinChanged.InvokeAsync(PagoPin);

            }
            if (!PagoPin.Categorias.Any())
            {

                PagoPin.Categorias = await _miLicenciaService.ObtenerCategorias();
                await PagoPinChanged.InvokeAsync(PagoPin);

            }
            await CargarListadosIdsAceptados();
        }
        /// <summary>
        /// Maneja el evento de cambio del radio button para seleccionar el tramite, tanto para instructor como para no instructor.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task RadioSelection()
        {
            if (cantidadModel.Cantidad != 0)
                await SetPagoPinTramiteCategoria(cantidadModel.Cantidad.Value, cantidadModel.Cantidad == 4 || cantidadModel.Cantidad == 5);
        }
        /// <summary>
        /// Establece el tramite y la categoria seleccionada en el objeto PagoPin, tanto para instructor como para no instructor.
        /// </summary>
        /// <param name="seleccionTramite"></param>
        /// <param name="isInstructor"></param>
        /// <returns></returns>
        private async Task SetPagoPinTramiteCategoria(int seleccionTramite, bool isInstructor)
        {

            foreach (var tramite in ListadoTramites)
                tramite.Selected = tramite.Id == seleccionTramite;

            //Se cambia el tramite a 1 (Primera vez) porque en bd esta configurado como tipo de tramite 1 para instructor
            if (isInstructor && seleccionTramite == (int)EnumTramite.PrimeraVezInstructor)
                seleccionTramite = (int)EnumTramite.PrimeraVez;

            //Se cambia el tramite a 3 (Recategorizar) porque en bd esta configurado como tipo de tramite 3 para instructor
            if (isInstructor && seleccionTramite == (int)EnumTramite.RecategorizarInstructor)
                seleccionTramite = (int)EnumTramite.Recategorizar;

            PagoPin.OpcionTramite = 1; //Por defecto para pin directo siempre sera Tramite simple
            PagoPin.TipoTramite = seleccionTramite;
            PagoPin.TramiteInstructor = isInstructor;
            PagoPin.CategoriaSeleccionada = string.Empty;
            PagoPin.Categoria1 = string.Empty;
            PagoPin.Categoria = string.Empty;
            PagoPin.PreSeleccionado = false;

            await PagoPinChanged.InvokeAsync(PagoPin);
            await OnFormValidChanged.InvokeAsync(true);
        }
        /// <summary>
        /// Agrega un tramite al listado de tramites disponibles para el usuario, tanto para instructor como para no instructor.
        /// </summary>
        /// <param name="enumTramite"></param>
        /// <param name="isInstructor"></param>
        /// <returns></returns>
        private async Task AddSelectionTramite(EnumTramite enumTramite, bool isInstructor)
        {
            ListadoTramites.Add(new SeleccionTramites()
            {
                Id = (int)enumTramite,
                Nombre = PagoPinConst.DescripcionTramite[enumTramite],
                Instructor = isInstructor,
                Selected = false
            });

            await Task.FromResult(true);
        }


        private async Task Retroceder()
        {
            await OnRetroceder.InvokeAsync();
        }

        /// <summary>
        /// Carga los listados de categorias aceptadas para el tramite, tanto para instructor como para no instructor.
        /// </summary>
        /// <returns></returns>
        private async Task CargarListadosIdsAceptados()
        {

            IdsCategoriasAceptadas = CargaDinamicaListados(false, this.CodigosInstructor);
            IdsCategoriasInstructorAceptadas = CargaDinamicaListados(true, this.CodigosInstructor);
            TratarCategoriasHabilitadas();


        }
        /// <summary>
        /// Verifica las categorias habilitadas para los tramites, tanto para instructor como para no instructor.
        /// </summary>
        /// <returns></returns>
        private async Task TratarCategoriasHabilitadas()
        {
            TiposTramitesHabilitados.PrimeraVez = PagoPin.CategoriasCea.FindAll(x => x.IdTramite == (int)EnumTramite.PrimeraVez && (IdsCategoriasAceptadas.Contains(x.IdCategoria))).Count > 0 ? true : false;
            TiposTramitesHabilitados.Recategorizacion = PagoPin.CategoriasCea.FindAll(x => x.IdTramite == (int)EnumTramite.Recategorizar && (IdsCategoriasAceptadas.Contains(x.IdCategoria))).Count > 0 ? true : false;
            TiposTramitesHabilitados.PrimeraVezInstructor = PagoPin.CategoriasCea.FindAll(x => x.IdTramite == (int)EnumTramite.PrimeraVez && (IdsCategoriasInstructorAceptadas.Contains(x.IdCategoria))).Count > 0 ? true : false;
            TiposTramitesHabilitados.RecategorizacionInstructor = PagoPin.CategoriasCea.FindAll(x => x.IdTramite == (int)EnumTramite.Recategorizar && (IdsCategoriasInstructorAceptadas.Contains(x.IdCategoria))).Count > 0 ? true : false;
            TiposTramitesHabilitados.TramiteNormal = TiposTramitesHabilitados.PrimeraVez || TiposTramitesHabilitados.Recategorizacion;
            TiposTramitesHabilitados.TramiteInstructor = TiposTramitesHabilitados.PrimeraVezInstructor || TiposTramitesHabilitados.RecategorizacionInstructor;
        }
        /// <summary>
        /// Carga los listados de categorias aceptadas para el tramite, tanto para instructor como para no instructor.
        /// </summary>
        /// <param name="instructor"></param>
        /// <param name="codigosInstructor"></param>
        /// <returns></returns>
        private List<int> CargaDinamicaListados(bool instructor, IEnumerable<string> codigosInstructor)
        {
            var categorias = PagoPin.Categorias;

            var listaValores = categorias
                .Where(c =>
                    instructor
                        ? codigosInstructor.Any(code => c.Codigo.Contains(code))
                        : (c.Codigo.Contains("A") && !c.Codigo.Contains("IA")) ||
                          (c.Codigo.Contains("B") && !c.Codigo.Contains("IB")) ||
                          (c.Codigo.Contains("C") &&
                           !c.Codigo.Contains("IC") &&
                           !c.Codigo.Contains("Curso") &&
                           !c.Codigo.Contains("RC1")))
                .Select(c => c.IdCategoria)
                .ToList();

            return listaValores;
        }

        private void OnSubmit()
        {
           
            // Aquí iría tu lógica de negocio (guardar, navegar, etc.)
        }

        /// <summary>
        /// Carga los listados de categorias aceptadas para el tramite, tanto para instructor como para no instructor.
        /// </summary>
        /// <param name="instructor"></param>
        /// <returns></returns>
        private List<int> CargaDinamicaListados(bool instructor)
        {
            List<int> listaValoresEnum = new List<int>();
            Array valoresEnum;
            // Obtenemos un array con todos los valores del enumerador
            if (!instructor)
            {
                listaValoresEnum = PagoPin.Categorias
                .Where(obj => (obj.Codigo.Contains("A") && !obj.Codigo.Contains("IA")) || (obj.Codigo.Contains("B") && !obj.Codigo.Contains("IB")) || (obj.Codigo.Contains("C") && !obj.Codigo.Contains("IC") && !obj.Codigo.Contains("Curso") && !obj.Codigo.Contains("RC1")))
                .Select(obj => obj.IdCategoria)
                .ToList();
            }
            else
            {
                listaValoresEnum = PagoPin.Categorias
                .Where(obj => obj.Codigo.Contains("IA") || obj.Codigo.Contains("IB") || obj.Codigo.Contains("IC"))
                .Select(obj => obj.IdCategoria)
                .ToList();
            }
            // Iteramos sobre el array y agregamos cada valor como entero a la lista
            return listaValoresEnum;
        }
        #endregion Metodos
    }
}

