using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Services.MiLicencia;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Categorias
{
    public partial class Categorias
    {
        #region Variables

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public PagoPin PagoPin { get; set; }

        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback CostoHasChanged { get; set; }

        public List<Categoria> listadoCategorias = new List<Categoria>();
        public List<Categoria> listadoCategoriasMotos = new List<Categoria>();
        public List<Categoria> listadoCategoriasAutoParticular = new List<Categoria>();
        public List<Categoria> listadoCategoriasAutoPublico = new List<Categoria>();
        public List<Categoria> listadoCategoriasRecategorizacion = new List<Categoria>();
        public bool SeleccionCategoriaInicial = false;
        public bool SeleccionRecategorizacionFinal = false;
        public Categoria CategoriaSeleccionada = new Categoria();
        public Categoria RecategorizacionSeleccionada = new Categoria();

        private bool isLoading = false;

        #endregion Variables

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await ObtenerCategorias();
            isLoading = false;
        }

        private async Task ObtenerCategorias()
        {
            if (!PagoPin.CategoriasCea.Any())
            {
                listadoCategorias = await MiLicenciaService.ConsultaCategoriasCentro(this.PagoPin.CentroSeleccionado.IdCentro);
                PagoPin.CategoriasCea = listadoCategorias;
                await PagoPinChanged.InvokeAsync(PagoPin);
            }
            else
            {
                listadoCategorias = PagoPin.CategoriasCea;
                if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                {
                    var args = new ChangeEventArgs { Value = PagoPin.Categoria1 };
                    ClearSelectedCheck(listadoCategorias);
                    await SeleccionCategoria(args);
                }
            }
            TratamientoListados();
        }

        private void ClearSelectedCheck(List<Categoria> listadoCategorias)
        {
            foreach (var categoria in listadoCategorias)
            {
                categoria.Seleccionada = false;
            }
        }

        private async void TratamientoListados()
        {
            if (PagoPin.TramiteInstructor)
            {
                listadoCategoriasMotos = listadoCategorias.FindAll(x => x.Codigo.Contains("IA") && x.IdTramite == PagoPin.TipoTramite);
                listadoCategoriasAutoParticular = listadoCategorias.FindAll(x => x.Codigo.Contains("IB") && x.IdTramite == PagoPin.TipoTramite);
                listadoCategoriasAutoPublico = listadoCategorias.FindAll(x => x.Codigo.Contains("IC") && x.IdTramite == PagoPin.TipoTramite);
            }
            else
            {
                listadoCategoriasMotos = listadoCategorias.FindAll(x => x.Codigo.Contains("A") && !x.Codigo.Contains("IA") && x.IdTramite == PagoPin.TipoTramite);
                listadoCategoriasAutoParticular = listadoCategorias.FindAll(x => x.Codigo.Contains("B") && !x.Codigo.Contains("IB") && x.IdTramite == PagoPin.TipoTramite);
                listadoCategoriasAutoPublico = listadoCategorias.FindAll(x => x.Codigo.Contains("C") && !x.Codigo.Contains("IC") && !x.Codigo.Contains("Curso") && !x.Codigo.Contains("RC1") && x.IdTramite == PagoPin.TipoTramite);
            }
            FiltradoDeCategorias();
        }

        private void FiltradoDeCategorias()
        {
            switch (PagoPin.TipoTramite)
            {
                case 1:
                    if (PagoPin.TramiteInstructor)
                    {
                        listadoCategoriasMotos = listadoCategoriasMotos.FindAll(x => (x.Codigo == "IA1" || x.Codigo == "IA2") && x.IdTramite == PagoPin.TipoTramite);
                        listadoCategoriasAutoParticular = listadoCategoriasAutoParticular.FindAll(x => (x.Codigo.ToUpper() == "IB1" || x.Codigo.ToUpper() == "IB2" || x.Codigo.ToUpper() == "IB3") && x.IdTramite == PagoPin.TipoTramite);
                    }
                    else
                    {
                        listadoCategoriasAutoParticular = listadoCategoriasAutoParticular.FindAll(x => x.Codigo == "B1" && x.IdTramite == PagoPin.TipoTramite);
                        listadoCategoriasAutoPublico = listadoCategoriasAutoPublico.FindAll(x => x.Codigo == "C1" && x.IdTramite == PagoPin.TipoTramite);
                    }

                    break;

                case 2:
                    // Todas
                    break;

                case 3:
                    if (PagoPin.TramiteInstructor)
                    {
                        listadoCategoriasMotos = listadoCategoriasMotos.FindAll(x => x.Codigo == "IA1" || x.Codigo == "IA2" && x.IdTramite == PagoPin.TipoTramite);
                        listadoCategoriasAutoParticular = listadoCategoriasAutoParticular.FindAll(x => (x.Codigo.ToUpper() == "IB1" || x.Codigo.ToUpper() == "IB2" || x.Codigo.ToUpper() == "IB3") && x.IdTramite == PagoPin.TipoTramite);
                    }
                    else
                    {
                        listadoCategoriasMotos = listadoCategoriasMotos.FindAll(x => x.Codigo == "A1" && x.IdTramite == PagoPin.TipoTramite);
                        listadoCategoriasAutoParticular = listadoCategoriasAutoParticular.FindAll(x => (x.Codigo.ToUpper() == "B1" || x.Codigo.ToUpper() == "B2") && x.IdTramite == PagoPin.TipoTramite);
                    }

                    // Recategorizacion
                    // A1
                    //B1 B2
                    //C1 C2 C3
                    //Panel de categoria a la que deseas cambiar Categoria a la que deseas cambiar
                    // A1 -> A2
                    // B1 -> C1 B2
                    // B2 -> B3
                    // C1 -> B1 B2 C2
                    // C2 -> B2 B3 C3
                    // C3 -> B3

                    break;
            }
        }

        public async Task SeleccionCategoria(ChangeEventArgs args)
        {
            var SeleccionTramite = args.Value.ToString();
            if (!string.IsNullOrEmpty(SeleccionTramite))
                switch (SeleccionTramite)
                {
                    case "A1":
                        if (PagoPin.TipoTramite == 3)
                        {
                            await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "A2" && x.IdTramite == PagoPin.TipoTramite);
                            listadoCategoriasRecategorizacion[0].Seleccionada = false;
                            SeleccionCategoriaInicial = true;
                            SeleccionRecategorizacionFinal = false;
                            PagoPin.Categoria = "";
                            PagoPin.Categoria1 = "";
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, @"Seleccione la siguiente categoría.");
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);

                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }

                        break;

                    case "A2":
                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                            }
                            // Emision de seleccion de categoria
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);

                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            //await GeneracionCosto();
                        }
                        break;

                    case "B1":
                        if (PagoPin.TipoTramite == 3)
                        {
                            if (!PagoPin.PreSeleccionado)
                            {
                                PagoPin.Categoria1 = string.Empty;
                            }
                            // Emision de seleccion de categoria
                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                                listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => (x.Codigo == "B2" || x.Codigo == "C1") && x.IdTramite == PagoPin.TipoTramite);
                                //CategoriaSeleccionada.Seleccionada = false;
                                SeleccionCategoriaInicial = true;
                                RecategorizacionSeleccionada.Seleccionada = false;
                                SeleccionRecategorizacionFinal = false;
                                PagoPin.Categoria = "";
                                PagoPin.Categoria1 = "";
                                PagoPin.CategoriaSeleccionada = SeleccionTramite;

                                await PagoPinChanged.InvokeAsync(PagoPin);
                            }
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }

                        break;

                    case "B2":
                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            if (!PagoPin.PreSeleccionado)
                            {
                                PagoPin.Categoria1 = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                                listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "B3" && x.IdTramite == PagoPin.TipoTramite);
                                SeleccionCategoriaInicial = true;
                                SeleccionRecategorizacionFinal = false;
                                PagoPin.Categoria = "";
                                PagoPin.Categoria1 = "";
                                PagoPin.CategoriaSeleccionada = SeleccionTramite;
                                await PagoPinChanged.InvokeAsync(PagoPin);
                                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, @"Seleccione la siguiente categoría");
                            }
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }
                        break;

                    case "B3":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }
                        break;

                    case "C1":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            if (!PagoPin.PreSeleccionado)
                            {
                                PagoPin.Categoria1 = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                                listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => (x.Codigo == "B1" || x.Codigo == "B2" || x.Codigo == "C2") && x.IdTramite == PagoPin.TipoTramite);
                                SeleccionCategoriaInicial = true;
                                SeleccionRecategorizacionFinal = false;
                                PagoPin.Categoria = "";
                                PagoPin.Categoria1 = "";
                                PagoPin.CategoriaSeleccionada = SeleccionTramite;
                                RecategorizacionSeleccionada.Seleccionada = false;
                                await PagoPinChanged.InvokeAsync(PagoPin);
                                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, @"Seleccione la siguiente categoría");
                            }
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);

                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }
                        break;

                    case "C2":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            if (!PagoPin.PreSeleccionado)
                            {
                                PagoPin.Categoria1 = string.Empty;
                            }

                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                                listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => (x.Codigo == "B2" || x.Codigo == "B3" || x.Codigo == "C3") && x.IdTramite == PagoPin.TipoTramite);
                                SeleccionCategoriaInicial = true;
                                SeleccionRecategorizacionFinal = false;
                                PagoPin.Categoria = "";
                                PagoPin.Categoria1 = "";
                                PagoPin.CategoriaSeleccionada = SeleccionTramite;
                                RecategorizacionSeleccionada.Seleccionada = false;
                                await PagoPinChanged.InvokeAsync(PagoPin);
                                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, @"Seleccione la siguiente categoría");
                            }
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }
                        break;

                    case "C3":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            if (!PagoPin.PreSeleccionado)
                            {
                                PagoPin.Categoria1 = string.Empty;
                            }

                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                //Carga las categorias iniciales
                                await SeleccionCategoria(PagoPin.CategoriaSeleccionada);
                                //Crea una instancia independiente de categoria para marca la que está preseleccionada
                                CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                                RecategorizacionSeleccionada = GuardarRecategorizacionSeleccionada(PagoPin.Categoria1);
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                                listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "B3" && x.IdTramite == PagoPin.TipoTramite);
                                SeleccionCategoriaInicial = true;
                                SeleccionRecategorizacionFinal = false;
                                PagoPin.Categoria = "";
                                PagoPin.Categoria1 = "";
                                PagoPin.CategoriaSeleccionada = SeleccionTramite;
                                await PagoPinChanged.InvokeAsync(PagoPin);
                                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, @"Seleccione la siguiente categoría");
                            }
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            PagoPin.CategoriaSeleccionada = SeleccionTramite;
                            CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                            await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, @"Seleccione la siguiente categoría");
                        }
                        break;

                    case "IA1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IA2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IB1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IB2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IB3":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IC1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IC2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IC3":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        PagoPin.CategoriaSeleccionada = SeleccionTramite;
                        CategoriaSeleccionada = GuardarCategoriaSeleccionada(PagoPin.CategoriaSeleccionada);
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;
                }
        }

        public async Task SeleccionCategoria(string categoria)
        {
            TratamientoListados();
            PagoPin.PreSeleccionado = false;
            var SeleccionTramite = categoria;
            if (!string.IsNullOrEmpty(SeleccionTramite))
                switch (SeleccionTramite)
                {
                    case "A1":
                        if (PagoPin.TipoTramite == 3)
                        {
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "A2" && x.IdTramite == PagoPin.TipoTramite);
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;

                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }

                        break;

                    case "A2":
                        if (PagoPin.TipoTramite == 3)
                        {
                            if (!string.IsNullOrEmpty(PagoPin.Categoria1))
                            {
                                listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "A2" && x.IdTramite == PagoPin.TipoTramite);
                                CategoriaSeleccionada = listadoCategorias.FirstOrDefault(x => x.Codigo == "A2");
                                CategoriaSeleccionada.Seleccionada = true;
                                listadoCategoriasMotos = listadoCategorias.FindAll(x => x.Codigo == PagoPin.CategoriaSeleccionada && x.IdTramite == PagoPin.TipoTramite);
                                listadoCategoriasMotos[0].Seleccionada = true;
                            }
                            else
                            {
                                await JsRuntime.InvokeVoidAsync("clearRadios", "categoriaRecategorizacion");
                            }

                            // Emision de seleccion de categoria
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;

                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }
                        break;

                    case "B1":
                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => (x.Codigo == "B2" || x.Codigo == "C1") && x.IdTramite == PagoPin.TipoTramite);
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }

                        break;

                    case "B2":
                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "B3" && x.IdTramite == PagoPin.TipoTramite);
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }
                        break;

                    case "B3":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            listadoCategoriasRecategorizacion.Clear();
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }
                        break;

                    case "C1":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => (x.Codigo == "B1" || x.Codigo == "B2" || x.Codigo == "C2") && x.IdTramite == PagoPin.TipoTramite);
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }
                        break;

                    case "C2":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => (x.Codigo == "B2" || x.Codigo == "B3" || x.Codigo == "C3") && x.IdTramite == PagoPin.TipoTramite);
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }
                        break;

                    case "C3":

                        if (PagoPin.TipoTramite == 3)
                        {
                            // Emision de seleccion de categoria
                            listadoCategoriasRecategorizacion = listadoCategorias.FindAll(x => x.Codigo == "B3" && x.IdTramite == PagoPin.TipoTramite);
                        }
                        else
                        {
                            PagoPin.Categoria = SeleccionTramite;
                            PagoPin.Categoria1 = SeleccionTramite;
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }
                        break;

                    case "IA1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IA2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IB1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IB2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IB3":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IC1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IC2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;

                    case "IC3":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        await GeneracionCosto();
                        await PagoPinChanged.InvokeAsync(PagoPin);

                        break;
                }
        }

        public async Task SeleccionCategoriaRecategorizacion(ChangeEventArgs args)
        {
            var SeleccionTramite = args.Value.ToString();
            if (!string.IsNullOrEmpty(SeleccionTramite))
                switch (SeleccionTramite)
                {
                    case "A2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;
                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial)
                        {
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }

                        break;

                    case "B1":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;
                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial)
                        {
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }

                        break;

                    case "B2":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;
                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial)
                        {
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }

                        break;

                    case "B3":

                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;
                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial)
                        {
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }

                        break;

                    case "C1":

                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;
                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial && PagoPin.CategoriaSeleccionada != "B1")
                        {
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }
                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial && PagoPin.CategoriaSeleccionada == "B1")
                        {
                            PagoPin.Categoria = "RC1";
                            PagoPin.Categoria1 = "RC1";
                            SeleccionRecategorizacionFinal = true;
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }

                        break;

                    case "C2":

                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;

                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial)
                        {
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                        }
                        break;

                    case "C3":
                        PagoPin.Categoria = SeleccionTramite;
                        PagoPin.Categoria1 = SeleccionTramite;
                        SeleccionRecategorizacionFinal = true;

                        if (SeleccionRecategorizacionFinal && SeleccionCategoriaInicial)
                        {
                            await GeneracionCosto();
                            await PagoPinChanged.InvokeAsync(PagoPin);
                            await GeneracionCosto();
                        }

                        break;
                }
        }

        private async Task GeneracionCosto()
        {
            await CostoHasChanged.InvokeAsync();
        }

        private Categoria GuardarCategoriaSeleccionada(string categoria)
        {
            CategoriaSeleccionada = listadoCategorias
                                                            .Where(x => x.Codigo == categoria)
                                                            .Select(c => new Categoria
                                                            {
                                                                IdCategoria = c.IdCategoria,
                                                                Nombre = c.Nombre,
                                                                Codigo = c.Codigo,
                                                                IdGrupo = c.IdGrupo,
                                                                Seleccionada = c.Codigo == categoria
                                                            })
                                                            .FirstOrDefault();

            CategoriaSeleccionada.Seleccionada = true;
            SeleccionCategoriaInicial = true;

            return CategoriaSeleccionada;
        }

        private Categoria GuardarRecategorizacionSeleccionada(string categoria)
        {
            RecategorizacionSeleccionada = listadoCategorias
                                                                    .Where(x => x.Codigo == categoria)
                                                                    .Select(c => new Categoria
                                                                    {
                                                                        IdCategoria = c.IdCategoria,
                                                                        Nombre = c.Nombre,
                                                                        Codigo = c.Codigo,
                                                                        IdGrupo = c.IdGrupo,
                                                                        Seleccionada = c.Codigo == categoria
                                                                    })
                                                                    .FirstOrDefault();

            RecategorizacionSeleccionada.Seleccionada = true;

            return RecategorizacionSeleccionada;
        }
    }
}