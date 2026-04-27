using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.Articulos
{
    public partial class Articulos
    {
        #region Properties

        [Parameter]
        public string IdRunt { get; set; } = string.Empty;

        [Parameter]
        public string Negocio { get; set; } = string.Empty;

        [Parameter]
        public string UserName { get; set; } = string.Empty;

        [Parameter]
        public DatosArticulos CategoriasCEA { get; set; } = new();

        [Parameter]
        public EventCallback<object> UpdateModel { get; set; }

        #endregion Properties

        #region Inyección Dependencias

        [Inject]
        private IMiLicenciaService MiLicenciaService { get; set; }

        #endregion Inyección Dependencias

        #region Fields

        private ConfigurarArticulosModel ConfigurarArticulosModel = new();
        private bool IsLoading = true;
        private bool IsFormValid = false;
        private EditContext EditContext;
        private ValidationMessageStore MessageStore;
        private List<DetalleArticulo> CategoriasCEAInstructor = [];
        private List<DetalleArticulo> CategoriasCEAconduccion = [];

        #endregion Fields

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            ConfigurarArticulosModel = await CategoriasCEA.CargarDatosModelo();

            EditContext = new EditContext(ConfigurarArticulosModel);
            MessageStore = new ValidationMessageStore(EditContext);

            CategoriasCEAInstructor = [.. CategoriasCEA.DetalleArticulos.Where(x => x.NombreCategoria != null && x.NombreCategoria.Contains('I'))];
            CategoriasCEAconduccion = [.. CategoriasCEA.DetalleArticulos.Where(x => x.NombreCategoria != null && !x.NombreCategoria.Contains('I'))];

            await NotifyValidationStateChanged();

            IsLoading = false;
        }

        #endregion Protected Methods

        #region Private Methods

        private static async Task<EnumCategoriaBusquedaArticulos> ObternerTipoEnumCategoria(string nombreArticulo)
        {
            List<string> categorias = ["A1", "A2", "B1", "B2", "B3", "C1", "C2", "C3", "RC1"];
            List<string> categoriasInstructor = ["IA1", "IA2", "IB1", "IB2", "IB3", "IC1", "IC2", "IC3", "IRC1"];

            if (nombreArticulo == null)
                return await Task.FromResult(EnumCategoriaBusquedaArticulos.Otros);
            else if (categorias.Any(x => x.Contains(nombreArticulo)) && categoriasInstructor.Any(x => !x.Contains(nombreArticulo)))
                return await Task.FromResult(EnumCategoriaBusquedaArticulos.CategoriasConduccion);
            else if (categoriasInstructor.Any(x => x.Contains(nombreArticulo)) && categorias.Any(x => !x.Contains(nombreArticulo)))
                return await Task.FromResult(EnumCategoriaBusquedaArticulos.CategoriasInstructor);
            else
                return await Task.FromResult(EnumCategoriaBusquedaArticulos.Otros);
        }

        private List<string> ValidarColumnasTablaCategorias()
        {
            List<string> categorias = ["A1", "A2", "B1", "B2", "B3", "C1", "C2", "C3", "RC1"];

            HashSet<string> categoriasNormalizadas = CategoriasCEA.DetalleArticulos.Where(x => x.NombreCategoria != null)
                .Select(c => c.NombreCategoria.StartsWith("I", StringComparison.OrdinalIgnoreCase)
                    ? c.NombreCategoria[1..]
                    : c.NombreCategoria)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return [.. categorias.Where(c => categoriasNormalizadas.Contains(c))];
        }

        private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
        {
            await NotifyValidationStateChanged();
        }

        private async Task NotifyValidationStateChanged()
        {
            var isValid = EditContext.Validate();
            StateHasChanged();

            IsFormValid = isValid;

            await Task.FromResult(true);
        }

        private async Task GuardarDatos()
        {
            await NotifyValidationStateChanged();

            if (IsFormValid)
            {
                await CrearListaArticulos();

                DatosArticulos datosArticulos = new()
                {
                    AplicaConfiguracionEspecifica = ConfigurarArticulosModel.AplicaConfiguracionEspecifica,
                    IdRunt = IdRunt,
                    IdTipoPin = Negocio == "CRC" ? 1 : 2,
                    UsuarioPortal = UserName,
                    DetalleArticulos = CategoriasCEA.DetalleArticulos,
                };

                FacturacionResponse<object> responseArticulos = await MiLicenciaService.AlmacenarConfiguracionArticulos(datosArticulos);

                if (responseArticulos.SolicitudExitosa)
                {
                    await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Success, responseArticulos.Mensaje);
                    await UpdateModel.InvokeAsync(ConfigurarArticulosModel);
                }
                else
                    await responseArticulos.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
            }
            else
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Warning, "Hay validaciones en el formulario que no se están cumpliendo");
        }

        private async Task CrearListaArticulos()
        {
            PropertyInfo[] propiedades = ConfigurarArticulosModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            CategoriasCEA.DetalleArticulos.ForEach(async x =>
            {
                string codigoArticulo = await ObtenerCodigoArticuloModel(propiedades, x.NombreCategoria ?? x.NombreArticulo, await ObternerTipoEnumCategoria(x.NombreCategoria));

                if (!string.IsNullOrEmpty(codigoArticulo))
                {
                    x.CodigoArticulo = codigoArticulo;
                    x.EstadoArticulo = await ValidarEstadoArticulo(x.TipoConfiguracion);
                }
            });

            await Task.FromResult(true);
        }

        private async Task<string> ObtenerCodigoArticuloModel(PropertyInfo[] propiedades, string nombreCategoria, EnumCategoriaBusquedaArticulos tipoArticulo)
        {
            PropertyInfo nombrePropiedad = null;

            if (tipoArticulo == EnumCategoriaBusquedaArticulos.CategoriasConduccion)
                nombrePropiedad = propiedades.FirstOrDefault(p => p.Name.Contains(nombreCategoria, StringComparison.OrdinalIgnoreCase)
                            && !p.Name.Contains($"I{nombreCategoria}", StringComparison.OrdinalIgnoreCase));
            else if (tipoArticulo == EnumCategoriaBusquedaArticulos.CategoriasInstructor)
                nombrePropiedad = propiedades.FirstOrDefault(p => p.Name.Contains(nombreCategoria, StringComparison.OrdinalIgnoreCase));
            else if (tipoArticulo == EnumCategoriaBusquedaArticulos.Otros)
            {
                nombreCategoria = nombreCategoria switch
                {
                    "Curso de conducción" => "CursoConduccion",
                    "Curso de instructor en conducción" => "CursoConduccionInstructor",
                    "Impuesto ANSV" => "TarifaAnsv",
                    "Tarifa del SICOV" => "TarifaSicov",
                    "Tarifa del aliado de recaudo" => "TarifaAliado",
                    "Examen médico" => "ExamenMedico",
                    "Examen médico Sencillo" => "ExamenMedicoSencillo",
                    "Examen médico Combo" => "ExamenMedicoCombo",
                    _ => "",
                };

                nombrePropiedad = propiedades.FirstOrDefault(p => p.Name == nombreCategoria);
            }

            if (nombrePropiedad != null)
            {
                object codigoArticulo = nombrePropiedad.GetValue(ConfigurarArticulosModel);
                return await Task.FromResult(codigoArticulo?.ToString() ?? "NA-0");
            }

            return await Task.FromResult("NA-0");
        }

        private async Task<bool> ValidarEstadoArticulo(string tipoConfiguracion)
        {
            if (tipoConfiguracion == "Especifica" && ConfigurarArticulosModel.AplicaConfiguracionEspecifica)
                return await Task.FromResult(true);
            else if (tipoConfiguracion == "Especifica" && !ConfigurarArticulosModel.AplicaConfiguracionEspecifica)
                return await Task.FromResult(false);
            else if (tipoConfiguracion == "General" && ConfigurarArticulosModel.AplicaConfiguracionEspecifica)
                return await Task.FromResult(false);
            else if (tipoConfiguracion == "General" && !ConfigurarArticulosModel.AplicaConfiguracionEspecifica)
                return await Task.FromResult(true);
            else
                return await Task.FromResult(true);
        }

        #endregion Private Methods
    }
}
