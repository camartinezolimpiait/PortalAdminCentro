using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion;

public partial class ConfigurarFacturacion
{
    #region Inyección Dependencias

    [Inject]
    private ProtectedSessionStorage ProtectedSessionStore { get; set; }

    [Inject]
    private IMiLicenciaService MiLicenciaService { get; set; }

    #endregion Inyección Dependencias

    #region Fields

    private ConfigurarCredencialesModel CredencialesModel = new();
    private ConfigurarEmisionModel EmisionModel = new();
    private ConfigurarNumeracionModel NumeracionModel = new();
    private ConfigurarArticulosModel ArticulosModel = new();
    private ConfigurarComportamientoModel ComportamientoModel = new();

    private string activeTab = "Tab1";
    private bool IsLoading = true;
    private bool FacturacionActiva;
    private string UserName = "";
    private string Plataforma = "";

    private List<ConsultaGenericaTipos> Departamentos = [];
    private List<ConsultaMunicipios> Municipios = [];
    private List<ConsultaGenericaTipos> TiposPersona = [];
    private List<ConsultaGenericaTipos> DisparadoresFacturacion = [];
    private List<ConsultaGenericaTipos> Regimen = [];
    private DatosArticulos CategoriasCEA = new();
    private ConfigurarFacturacionModel ConfigurarFacturacionModel = new();
    private GetDataResponseCentro GetCentroResponse = new();
    private EditContext EditContext;
    private ValidationMessageStore MessageStore; //Limpiar el store con las validaciones

    #endregion Fields

    #region Protected Methods

    // Inicio refactorización/optimización por GitHub Copilot
    protected override async Task OnInitializedAsync()
    {
        try
        {
            ConfigurarFacturacionModel = new();

            ProtectedBrowserStorageResult<GetCentroResponse> centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            ProtectedBrowserStorageResult<ApplicationSevice> applicationShared = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");

            if (!centroShared.Success || !applicationShared.Success)
            {
                await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, "Error al cargar los datos de sesión.");
                return;
            }

            GetCentroResponse = centroShared.Value.Respuesta;
            UserName = applicationShared.Value.UserName;
            Plataforma = applicationShared.Value.Plataforma;

            EditContext = new EditContext(ConfigurarFacturacionModel);
            MessageStore = new ValidationMessageStore(EditContext);

            FacturacionResponse<EstadoFacturacion> response = await MiLicenciaService.ConsultarEstadoFacturacionElectronica(new ConsultaEstadoFacturacion() { IdRunt = $"{GetCentroResponse.CodigoRUNT ?? 0}" });

            if (response.SolicitudExitosa)
            {
                FacturacionActiva = response.Datos.HabilitadoFacturaElectronica;

                // Ejecutar tareas en paralelo para mejorar rendimiento
                await Task.WhenAll(
                    ConsultaRegimenAsync(),
                    ConsultaDepartamentosAsync(),
                    ConsultaPersonasAsync(),
                    ConsultaCategoriasFacturacionAsync(),
                    ConsultaDisparadoresFacturacionAsync()
               );

                await CargarConfiguracionFacturacionElectronica();
            }
            else
            {
                await response.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
            }
        }
        catch (Exception ex)
        {
            await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Error al inicializar la configuración de facturación: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Fin refactorización/optimización por GitHub Copilot

    #endregion Protected Methods

    #region Private Methods

    // Inicio código generado por GitHub Copilot
    private async Task ConsultaRegimenAsync()
    {
        FacturacionResponse<List<ConsultaGenericaTipos>> responseTiposregimen = await MiLicenciaService.ConsultarGenericaTipos(EnumTiposFacturacion.TiposRegimen);

        if (responseTiposregimen.SolicitudExitosa)
            Regimen = responseTiposregimen.Datos;
        else
            await responseTiposregimen.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
    }

    private async Task ConsultaDepartamentosAsync()
    {
        FacturacionResponse<List<ConsultaGenericaTipos>> responseDepartamentos = await MiLicenciaService.ConsultarGenericaTipos(EnumTiposFacturacion.Departamentos);
        FacturacionResponse<List<ConsultaMunicipios>> responseMunicipios = await MiLicenciaService.ConsultarMunicipiosDepartamentos();

        if (responseDepartamentos.SolicitudExitosa)
            Departamentos = responseDepartamentos.Datos;
        else
            await responseDepartamentos.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");

        if (responseMunicipios.SolicitudExitosa)
            Municipios = responseMunicipios.Datos;
        else
            await responseMunicipios.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
    }

    private async Task ConsultaPersonasAsync()
    {
        FacturacionResponse<List<ConsultaGenericaTipos>> responseTiposPersona = await MiLicenciaService.ConsultarGenericaTipos(EnumTiposFacturacion.TiposPersona);

        if (responseTiposPersona.SolicitudExitosa)
            TiposPersona = responseTiposPersona.Datos;
        else
            await responseTiposPersona.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
    }

    private async Task ConsultaCategoriasFacturacionAsync()
    {
        FacturacionResponse<DatosArticulos> responseCategorias = await MiLicenciaService.ConsultarListadoArticulos(new()
        {
            IdRunt = $"{GetCentroResponse.CodigoRUNT}",
            IdTipoPin = Plataforma == "CEA" ? "2" : "1",
        });

        if (responseCategorias.SolicitudExitosa)
            CategoriasCEA = responseCategorias.Datos;
        else
            await responseCategorias.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
    }

    private async Task ConsultaDisparadoresFacturacionAsync()
    {
        FacturacionResponse<List<ConsultaGenericaTipos>> responseDisparadores = await MiLicenciaService.ConsultarGenericaTipos(EnumTiposFacturacion.TiposDisparadores);

        if (responseDisparadores.SolicitudExitosa)
            DisparadoresFacturacion = responseDisparadores.Datos;
        else
            await responseDisparadores.ShowErrorMessageCollection(MiLicenciaService, "Recaudo");
    }

    // Fin código generado por GitHub Copilot

    // Inicio refactorización/optimización por GitHub Copilot
    private async Task CargarConfiguracionFacturacionElectronica()
    {
        try
        {
            IsLoading = true;
            FacturacionResponse<ConfiguracionFacturacion> responseConfiguracion = await MiLicenciaService.ConsultarConfiguracionFacturacionElectronica(new ConsultaEstadoFacturacion() { IdRunt = $"{GetCentroResponse.CodigoRUNT ?? 0}" });

            if (responseConfiguracion.SolicitudExitosa)
            {
                CredencialesModel.Usuario = responseConfiguracion?.Datos?.Usuario ?? "";
                CredencialesModel.Clave = responseConfiguracion?.Datos?.Contraseña ?? "";

                EmisionModel.RazonSocial = responseConfiguracion?.Datos?.RazonSocial ?? "";
                EmisionModel.NIT = responseConfiguracion?.Datos?.NIT ?? "";
                EmisionModel.Dv = responseConfiguracion?.Datos?.DV ?? "";
                EmisionModel.Celular = responseConfiguracion?.Datos?.Celular ?? "";
                EmisionModel.Correo = responseConfiguracion?.Datos?.CorreoElectronico ?? "";
                EmisionModel.RegimenContributivo = responseConfiguracion?.Datos?.RegimenContributivo ?? "";
                EmisionModel.NombreComercial = responseConfiguracion?.Datos?.NombreCentro ?? "";
                EmisionModel.Direccion = responseConfiguracion?.Datos?.DireccionCentro ?? "";
                EmisionModel.Departamento = int.Parse(responseConfiguracion?.Datos?.DepartamentoCentro ?? "0");
                EmisionModel.Ciudad = int.Parse(responseConfiguracion?.Datos?.CiudadCentro ?? "0");
                EmisionModel.Observaciones = responseConfiguracion?.Datos?.Nota ?? "";
                EmisionModel.TipoPersona = responseConfiguracion?.Datos?.TipoPersona ?? 0;

                NumeracionModel.NumeroResolucion = responseConfiguracion?.Datos?.NumeroResolucion ?? "";
                NumeracionModel.FechaInicio = DateOnly.ParseExact(responseConfiguracion?.Datos?.InicioResolucion ?? DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                NumeracionModel.Fechafin = DateOnly.ParseExact(responseConfiguracion?.Datos?.FinResolucion ?? DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                NumeracionModel.Prefijo = responseConfiguracion?.Datos?.PrefijoResolucion ?? "";
                NumeracionModel.Desde = responseConfiguracion?.Datos?.Desde ?? 0;
                NumeracionModel.Hasta = responseConfiguracion?.Datos?.Hasta ?? 0;
                NumeracionModel.ConsecutivoEspecifico = responseConfiguracion?.Datos?.CheckConsecutivo ?? false;
                NumeracionModel.EmpezarDesde = responseConfiguracion?.Datos?.EmpezarDesde ?? 0;

                ArticulosModel = await CategoriasCEA.CargarDatosModelo();

                ComportamientoModel.FacturacionActiva = responseConfiguracion?.Datos?.ActivarFacturacion ?? false;
                ComportamientoModel.EventoFacturacion = $"{responseConfiguracion?.Datos?.MomentoFacturacion ?? 0}";
            }
        }
        catch (Exception ex)
        {
            await MiLicenciaService.ShowNotificacion(Enum.NotificationStatus.Error, $"Error al cargar la configuración de facturación: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Fin refactorización/optimización por GitHub Copilot

    private async Task ChangeTab(string tab)
    {
        if (tab == "Tab1")
            activeTab = tab;
        else if ((tab == "Tab2" && ValidarCredencialesModelo()) ||
            (tab == "Tab3" && ValidarEmisionModelo()) ||
            (tab == "Tab4" && ValidarNumeracionModelo()) ||
            (tab == "Tab5" && ValidarArticulosModelo() && ValidarNumeracionModelo()))
            activeTab = tab;

        await Task.FromResult(true);
    }

    private bool ValidarCredencialesModelo()
    {
        if (!string.IsNullOrEmpty(CredencialesModel.Usuario) &&
            !string.IsNullOrEmpty(CredencialesModel.Clave))
            return true;

        return false;
    }

    private bool ValidarEmisionModelo()
    {
        if (!string.IsNullOrEmpty(EmisionModel.RazonSocial) &&
            !string.IsNullOrEmpty(EmisionModel.NIT) &&
            !string.IsNullOrEmpty(EmisionModel.Dv) &&
            !string.IsNullOrEmpty(EmisionModel.Celular) &&
            !string.IsNullOrEmpty(EmisionModel.Correo) &&
            !string.IsNullOrEmpty(EmisionModel.RegimenContributivo) &&
            !string.IsNullOrEmpty(EmisionModel.NombreComercial) &&
            !string.IsNullOrEmpty(EmisionModel.Direccion) &&
            (EmisionModel.Departamento != null && EmisionModel.Departamento > 0) &&
            (EmisionModel.Ciudad != null && EmisionModel.Ciudad > 0) &&
            !string.IsNullOrEmpty(EmisionModel.Observaciones) &&
            (EmisionModel.TipoPersona != null && EmisionModel.TipoPersona > 0))
            return true;

        return false;
    }

    private bool ValidarNumeracionModelo()
    {
        if (!string.IsNullOrEmpty(NumeracionModel.NumeroResolucion) &&
            !string.IsNullOrEmpty(NumeracionModel.Prefijo) &&
            NumeracionModel.Desde > 0 &&
            NumeracionModel.Hasta > 0)
            return true;

        return false;
    }

    private bool ValidarArticulosModelo()
    {
        if (!string.IsNullOrEmpty(ArticulosModel.TarifaAnsv) &&
            !string.IsNullOrEmpty(ArticulosModel.TarifaSicov) &&
            !string.IsNullOrEmpty(ArticulosModel.TarifaAliado) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccion) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionA1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionA2) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionB1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionB2) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionB3) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionC1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionC2) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionC3) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionRC1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionInstructor) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIA1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIA2) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIB1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIB2) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIB3) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIC1) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIC2) &&
            !string.IsNullOrEmpty(ArticulosModel.CursoConduccionIC3) &&
            !string.IsNullOrEmpty(ArticulosModel.ExamenMedico) &&
            !string.IsNullOrEmpty(ArticulosModel.ExamenMedicoSencillo) &&
            !string.IsNullOrEmpty(ArticulosModel.ExamenMedicoCombo))
            return true;

        return false;
    }

    private string GetTabClass(string tabName)
    {
        return activeTab == tabName ? "active" : "";
    }

    private async Task UpdateModel(object value)
    {
        if (value.GetType() == typeof(ConfigurarCredencialesModel))
            CredencialesModel = (ConfigurarCredencialesModel)value;
        else if (value.GetType() == typeof(ConfigurarEmisionModel))
            EmisionModel = (ConfigurarEmisionModel)value;
        else if (value.GetType() == typeof(ConfigurarNumeracionModel))
            NumeracionModel = (ConfigurarNumeracionModel)value;
        else if (value.GetType() == typeof(ConfigurarArticulosModel))
            ArticulosModel = (ConfigurarArticulosModel)value;
        else if (value.GetType() == typeof(ConfigurarComportamientoModel))
            ComportamientoModel = (ConfigurarComportamientoModel)value;

        await Task.FromResult(true);
    }

    #endregion Private Methods
}