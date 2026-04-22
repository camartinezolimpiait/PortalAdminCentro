using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.CompraPin.Models;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin.Tramite.SeleccionCategorias
{
    public partial class SeleccionCategorias
    {
        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        [Parameter]
        public PagoPin pagoPin { get; set; }


        [Parameter]
        public EventCallback<PagoPin> PagoPinChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnFormValidChanged { get; set; }
        [Parameter]
        public EventCallback OnRetroceder { get; set; }


        #region variables
        private int tempTipoTramite;
        private bool vistaCategoriasCarro = false;
        private bool vistaCategoriasMoto = false;

        private bool mostrarRecategorizacionCarro = true;
        private bool mostrarRecategorizacionMoto = true;
        private bool mostrarRecategorizacionCombo = true;
        private CompraPinFormModel compraPinForm = new CompraPinFormModel();

        private List<Categoria> categoriasCarro = new();
        private List<Categoria> categoriasMoto = new();
        private List<Categoria> categoriasParticular = new();
        private List<Categoria> categoriasPublico = new();
        public string? CategoriasActual { get; set; }

        public string? CategoriaActual { get; set; }
        public string? CategoriaFinal { get; set; }

        private List<Categoria> categoriasARecategorizar = new();
        private List<Categoria> categoriasInstructor = new();
        private bool isLoading = false;
        public List<string>? ListaCategoriasCentro { get; set; }
        public List<Categoria>? CategoriasRecategorizacion { get; set; }

        private EditContext editContext;

        public Dictionary<int, string> DescripcionOpcionTramite { get; set; } =
    new()
    {
            { 1, "Un solo trámite" },
            { 2, "Varios trámites (Carro y moto)" }
    };
        #endregion
        protected override void OnInitialized()
        {
            isLoading = true;
            inicializarFormulario();
            procesarCategorias();
            CalcularRecategorizacion();

            editContext = new EditContext(compraPinForm);
            isLoading = false;
        }

        public void inicializarFormulario()
        {
            if (pagoPin.TramiteInstructor)
            {
                switch (pagoPin.TipoTramite)
                {
                    case (int)EnumTramite.PrimeraVez:
                        pagoPin.TipoTramite = (int)EnumTramite.PrimeraVezInstructor;
                        break;
                    case (int)EnumTramite.Recategorizar:
                        pagoPin.TipoTramite = (int)EnumTramite.RecategorizarInstructor;
                        break;
                }
            }

            tempTipoTramite = (int)pagoPin.TipoTramite;
            if (pagoPin.OpcionTramite == DescripcionOpcionTramite.FirstOrDefault(x => x.Key == 2).Key)
            {
                if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
                {
                    if (pagoPin.TipoTramite == (int)EnumTramite.PrimeraVez)
                    {
                        vistaCategoriasCarro = !vistaCategoriasCarro;
                    }
                    else if (pagoPin.TipoTramite == (int)EnumTramite.Renovar || pagoPin.TipoTramite == (int)EnumTramite.Recategorizar)
                    {
                        mostrarRecategorizacionCarro = true;
                        mostrarRecategorizacionMoto = false;
                    }
                    compraPinForm.CategoriaPrincipal =
                        !string.IsNullOrEmpty(pagoPin.Categoria1) ? pagoPin.Categoria1 : null;

                    compraPinForm.CategoriaSecundaria = "";
                }
                else
                {
                    tempTipoTramite = (int)pagoPin.TipoTramite2;
                    if (pagoPin.TipoTramite2 == (int)EnumTramite.PrimeraVez)
                    {
                        vistaCategoriasMoto = !vistaCategoriasMoto;
                        compraPinForm.CategoriaPrincipal = null;

                        compraPinForm.CategoriaSecundaria =
                            !string.IsNullOrEmpty(pagoPin.Categoria2) ? pagoPin.Categoria2 : "";
                    }
                    else if (pagoPin.TipoTramite2 == (int)EnumTramite.Recategorizar)
                    {
                        mostrarRecategorizacionCarro = false;
                        mostrarRecategorizacionMoto = false;
                        compraPinForm.CategoriaPrincipal = !string.IsNullOrEmpty(pagoPin.Categoria1) ? pagoPin.Categoria1 : "";
                        compraPinForm.CategoriaSecundaria = !string.IsNullOrEmpty(pagoPin.Categoria2) ? pagoPin.Categoria2 : "";

                    }
                    else
                    {
                        mostrarRecategorizacionCarro = false;
                        mostrarRecategorizacionMoto = true;
                        compraPinForm.CategoriaPrincipal = !string.IsNullOrEmpty(pagoPin.Categoria2) ? pagoPin.Categoria2 : "";
                        compraPinForm.CategoriaSecundaria = !string.IsNullOrEmpty(pagoPin.Categoria2) ? pagoPin.Categoria : "";
                    }
                }
            }
            else
            {
                if (pagoPin.TipoTramite == (int)EnumTramite.Recategorizar || pagoPin.TipoTramite2 == (int)EnumTramite.Recategorizar)
                {
                    mostrarRecategorizacionMoto = false;
                }
                compraPinForm.CategoriaPrincipal = !string.IsNullOrEmpty(pagoPin.Categoria1) ? pagoPin.Categoria1 : null;
                compraPinForm.CategoriaSecundaria = "";
            }
        }

        private void procesarCategorias()
        {
            if (pagoPin.ClienteCompra == (int)EnumTipoCliente.CRC)
            {
                MapearDescripcionesCortas(pagoPin.CategoriasCrc);
                FiltrarCategoriasPorEdadyServicio(pagoPin.CategoriasCrc);
                categoriasARecategorizar = pagoPin.CategoriasCrc
                    .Where(x => !new[] { "A2", "B3", "C3" }.Contains(x.Codigo))
                    .ToList();
                categoriasInstructor = new List<Categoria>();
            }
            else
            {
                this.MapearDescripcionesCortas(this.pagoPin.CategoriasCea);
                var listaCategorias = new List<string> { "A1", "A2", "B1", "B2", "B3", "C1", "C2", "C3", "RC1" };
                List<Categoria> categorias;
                int tipoTramiteTemp = 0;

                if (pagoPin.TramiteInstructor)
                {
                    switch (pagoPin.TipoTramite)
                    {
                        case (int)EnumTramite.PrimeraVezInstructor:
                            tipoTramiteTemp = (int)(int)EnumTramite.PrimeraVez;
                            break;

                        case (int)EnumTramite.RecategorizarInstructor:
                            tipoTramiteTemp = (int)EnumTramite.Recategorizar;
                            break;

                        default:
                            tipoTramiteTemp = (int)pagoPin.TipoTramite;
                            break;
                    }
                }
                else
                {
                    tipoTramiteTemp = (int)pagoPin.TipoTramite;
                }

                categorias = pagoPin.CategoriasCea?
                    .Where(x => listaCategorias.Contains(x.Codigo) && x.IdTramite == tipoTramiteTemp)
                    .ToList();
                categoriasInstructor = pagoPin.CategoriasCea?
                    .Where(x => x.Codigo.Contains("I") && x.IdTramite == tipoTramiteTemp)
                    .ToList();

                pagoPin.CategoriasCentroIp =
                    ((pagoPin.TipoTramite == (int)EnumTramite.PrimeraVezInstructor && pagoPin.TramiteInstructor) ||
                     (pagoPin.TipoTramite == (int)EnumTramite.RecategorizarInstructor && pagoPin.TramiteInstructor))
                        ? categoriasInstructor
                        : categorias;

                if (pagoPin.CentroIpValidacion)
                {
                    CargarCategoriasIpCentro(() =>
                    {
                        categorias = pagoPin.CategoriasCea?
                            .Where(x => pagoPin.CentroSeleccionado?.Categorias?.Contains(x.Codigo) == true)
                            .ToList();

                        categoriasInstructor = categorias?
                            .Where(x => x.Codigo.Contains("I") && x.IdTramite == tipoTramiteTemp)
                            .ToList();
                    });
                }
                if (!pagoPin.TramiteInstructor)
                {
                    FiltrarCategoriasPorEdadyServicio(categorias);
                }
            }
        }

        public void MapearDescripcionesCortas(List<Categoria> catArray)
        {
            foreach (var element in catArray)
            {
                if (CategoriasTexto.Valores.TryGetValue(element.Codigo, out var nombre))
                {
                    element.Nombre = nombre;
                }
            }
        }

        private void FiltrarCategoriasPorEdadyServicio(List<Categoria> categorias)
        {
            if (pagoPin.EdadAspirante < 18)
            {
                pagoPin.Categorias = categorias
                    .Where(x => new[] { "A1", "A2", "B1", "I" }.Contains(x.Codigo))
                    .ToList();
            }
            else
            {
                pagoPin.Categorias = categorias;
            }

            var categoriasCarroEdad = pagoPin.EdadAspirante < 18
                ? new[] { "B1" }
                : new[] { "B1", "C1" };

            categoriasCarro = categorias
                .Where(x => categoriasCarroEdad.Contains(x.Codigo))
                .ToList();

            categoriasMoto = categorias
                .Where(x => x.Codigo.Contains("A"))
                .ToList();

            categoriasParticular = categorias
                .Where(x => x.Codigo.Contains("B"))
                .ToList();

            categoriasPublico = categorias
                .Where(x => x.Codigo.Contains("C"))
                .ToList();
        }

        private async Task CargarCategoriasIpCentro(Action fn)
        {

            // Llamada al servicio (supongo que tu API devuelve algo tipo IEnumerable<Categoria>)
            var centroCategoria = await MiLicenciaService.ConsultaCategoriasCentro(pagoPin.CentroSeleccionado.IdCentro);

            // Procesar categorías
            CargarCategoriasCentro(centroCategoria);

            if (pagoPin.CentroSeleccionado != null)
                pagoPin.CentroSeleccionado.Categorias = ListaCategoriasCentro;

            // Ejecutar callback
            fn?.Invoke();
        }

        private void CargarCategoriasCentro(List<Categoria> listadoCategorias)
        {
            if (pagoPin.CategoriasCea != null)
            {
                var idCategoriasCentros = listadoCategorias?
                    .Select(x => x.IdCategoria)
                    .ToList();

                ListaCategoriasCentro = pagoPin.CategoriasCea
                    .Where(x => idCategoriasCentros.Contains(x.IdCategoria))
                    .Select(x => x.Codigo)
                    .ToList();
            }
        }

        public void CalcularRecategorizacion()
        {
            // "actual" sería la categoría seleccionada en este momento
            var actual = CategoriasActual;

            if (!string.IsNullOrEmpty(actual))
            {
                var recategorizarCategorias = GetRecategorizacionByCategoria(actual);

                CategoriasRecategorizacion = pagoPin.Categorias?
                    .Where(x => recategorizarCategorias.Contains(x.Codigo))
                    .ToList();
            }
        }


        private List<string> GetRecategorizacionByCategoria(string categoria)
        {
            switch (categoria)
            {
                case "A1":
                    return new List<string> { "A2" };

                case "B1":
                    return new List<string> { "B2", "C1" };

                case "B2":
                    return new List<string> { "B1", "B3", "C1", "C2" };

                case "B3":
                    return new List<string> { "B1", "B2", "C3" };

                case "C1":
                    return new List<string> { "B1", "B2", "C2" };

                case "C2":
                    return new List<string> { "B1", "B2", "C1", "C3" };

                case "C3":
                    return new List<string> { "B1", "B2", "C1", "C2" };

                default:
                    return new List<string>();
            }
        }

        private async Task CalcularPin(bool esSubmit = false)
        {
            if (pagoPin.OpcionTramite == 1)
            {
                pagoPin.PasoCotizacion = PasosCompraPin.DatosPersonales;
                pagoPin.Categoria = compraPinForm.CategoriaPrincipal;
                pagoPin.Categoria1 = compraPinForm.CategoriaPrincipal;
                pagoPin.Categoria2 = compraPinForm.CategoriaSecundaria;
                pagoPin.CategoriasActual = CategoriasActual;

                if (pagoPin.Categoria1 != null && pagoPin.Categoria1.Contains("I"))
                {
                    switch (pagoPin.TipoTramite)
                    {
                        case (int)EnumTramite.PrimeraVezInstructor:
                            pagoPin.TipoTramite = (int)EnumTramite.PrimeraVez;
                            break;

                        case (int)EnumTramite.RecategorizarInstructor:
                            pagoPin.TipoTramite = (int)EnumTramite.Recategorizar;
                            break;
                    }
                }

                if (pagoPin.TipoTramite == (int)EnumTramite.Recategorizar && !string.IsNullOrEmpty(pagoPin.Categoria1)
                    && !(pagoPin.Categoria1?.Contains("I") ?? false))
                {
                    var recategorizarCategorias = GetRecategorizacionByCategoria(pagoPin.CategoriasActual ?? string.Empty);

                    if (CategoriasActual.Equals(pagoPin.Categoria1))
                    {
                        pagoPin.Categoria1 = string.Empty;
                        return;
                    }

                    if (!recategorizarCategorias.Contains(pagoPin.Categoria1))
                    {
                        await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, $"No se puede recategorizar de {pagoPin.CategoriasActual} a {pagoPin.Categoria1}");
                        pagoPin.Categoria1 = string.Empty;
                        return;
                    }

                    if (pagoPin.Categoria1 == "C1")
                    {
                        pagoPin.Categoria = "RC1";
                        pagoPin.Categoria1 = "RC1";
                    }

                }

                await PagoPinChanged.InvokeAsync(pagoPin);

                if (esSubmit) // ✅ solo en submit
                    await OnFormValidChanged.InvokeAsync(true);
            }
            else if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
            {
                pagoPin.Categoria1 = compraPinForm.CategoriaPrincipal;
                pagoPin.Categoria = compraPinForm.CategoriaPrincipal;
                pagoPin.CategoriasActual = CategoriasActual;

                await PagoPinChanged.InvokeAsync(pagoPin);

                if (esSubmit)
                {
                    // ✅ solo en submit
                    await OnFormValidChanged.InvokeAsync(false);
                    vistaCategoriasCarro = false;
                    vistaCategoriasMoto = true;
                } 
            }
            else
            {
                pagoPin.PasoCotizacion = PasosCompraPin.DatosPersonales;

                pagoPin.Categoria2 = pagoPin.TipoTramite2 == (int)EnumTramite.PrimeraVez
                    ? compraPinForm.CategoriaSecundaria
                    : compraPinForm.CategoriaPrincipal;

                await PagoPinChanged.InvokeAsync(pagoPin);

                if (esSubmit) 
                {
                    // ✅ solo en submit
                    await OnFormValidChanged.InvokeAsync(true);
                    vistaCategoriasMoto = false;
                }
                   
            }
        }


        private void MostrarCategoriasCarro()
        {
            this.vistaCategoriasCarro = !this.vistaCategoriasCarro;
        }

        private void MostrarCategoriasMoto()
        {
            this.vistaCategoriasMoto = !this.vistaCategoriasMoto;
        }

        public void ValidarCategoriaCombo(Categoria ctg)
        {
            // Mantengo referencias locales si las necesitas
            var categoria1 = this.pagoPin.Categoria1;
            var categoria2 = this.pagoPin.Categoria2;

            // Si es trámite simple
            if (this.pagoPin.OpcionTramite == DescripcionOpcionTramite.FirstOrDefault(x => x.Key == 1).Key)
            {
                var x = this.compraPinForm.CategoriaPrincipal;
                this.pagoPin.CategoriasActual = this.CategoriasActual;
            }

            // Si está en paso de combo Moto
            if (this.pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto)
            {
                this.pagoPin.Categoria2 = this.compraPinForm.CategoriaPrincipal;
                this.pagoPin.CategoriasActual = this.CategoriasActual;
            }
            // Si está en paso de combo Carro
            else if (this.pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
            {
                this.pagoPin.Categoria1 = this.compraPinForm.CategoriaPrincipal;
                this.pagoPin.Categoria = this.compraPinForm.CategoriaPrincipal;
                this.pagoPin.CategoriasActual = this.CategoriasActual;
            }

            // Actualizo siempre
            this.pagoPin.CategoriasActual = this.CategoriasActual;
        }

        public bool MostrarBotonSiguienteCombo()
        {
            if (this.pagoPin.OpcionTramite == DescripcionOpcionTramite.FirstOrDefault(x => x.Key == 2).Key)
            {
                if (this.pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro &&
                    !string.IsNullOrEmpty(this.pagoPin.Categoria1))
                {
                    return true;
                }
                else if (this.pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto &&
                         !string.IsNullOrEmpty(this.pagoPin.Categoria1) &&
                         !string.IsNullOrEmpty(this.pagoPin.Categoria2))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private async Task Retroceder()
        {
            //if (pagoPin.PasoCotizacion == PasosCompraPin.DatosPersonales || pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
            //{
            //    await OnRetroceder.InvokeAsync();
            //}
            if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto)
            {
                pagoPin.PasoCotizacion = PasosCompraPin.TipoTramiteComboMoto;
            }
            await OnRetroceder.InvokeAsync();

        }

        public List<Categoria> CategoriasInstructorPorCodigo(string letra)
        {
            return this.categoriasInstructor
                       .Where(x => x.Codigo.Contains(letra))
                       .ToList();
        }

    }
}
