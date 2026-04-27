using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Pages.Facturacion.Common;
using portalAdministrativoSISEC.Pages.Facturacion.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion.EstadosConsultarFacturacion;

/// <summary>
/// Componente hijo que contiene la lógica de filtrado y visualización de solicitudes de facturación
/// Recibe el estado como parámetro del componente padre
/// </summary>
public partial class EstadosConsultarFacturacion : ComponentBase
{
    // Inicio código generado por GitHub Copilot

    #region Inyección de Dependencias

    /// <summary>
    /// Servicio principal para consumir APIs de MiLicencia y Portal Administrativo
    /// </summary>
    [Inject]
    public IMiLicenciaService MiLicenciaService { get; set; }

    /// <summary>
    /// Almacenamiento protegido en sesión del navegador
    /// </summary>
    [Inject]
    private ProtectedSessionStorage ProtectedSessionStore { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    #endregion Inyección de Dependencias

    #region Parámetros del Componente

    /// <summary>
    /// Estado recibido del componente padre (PorRevisar, Facturadas, Anuladas, EnCola)
    /// </summary>
    [Parameter]
    public string Estado { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<bool> OnFormCompleteChanged { get; set; }

    #endregion Parámetros del Componente

    #region Variables de QuickGrid

    /// <summary>
    /// Estado de paginación del grid (página actual, items por página, etc.)
    /// </summary>
    private readonly PaginationState Pagination = new() { ItemsPerPage = 10 };

    /// <summary>
    /// Referencia al componente QuickGrid para operaciones de refresh
    /// </summary>
    private QuickGrid<PINDetail>? Grid;

    /// <summary>
    /// Delegado que provee los datos al QuickGrid de forma asíncrona
    /// </summary>
    private GridItemsProvider<PINDetail> FacturacionProvider;

    /// <summary>
    /// Indica si el componente ya completó su primera renderización
    /// </summary>
    private bool _firstRenderComplete = false;

    /// <summary>
    /// Indica si hubo cambios en los parámetros que requieren refrescar el grid
    /// </summary>
    private bool _parametersChanged = false;

    /// <summary>
    /// Lista en memoria de la página actual para poder trabajar con selección múltiple
    /// </summary>
    private List<PINDetail> DatosPaginaActual { get; set; } = [];

    /// <summary>
    /// Indica si el checkbox de seleccionar todos en el encabezado está activo
    /// </summary>
    private bool SeleccionarTodos { get; set; }

    /// <summary>
    /// Indica si hay al menos un registro seleccionado
    /// </summary>
    private bool HaySeleccionados => DatosPaginaActual?.Any(p => p.Seleccionado) == true;

    /// <summary>
    /// Cantidad de registros seleccionados en la página actual.
    /// </summary>
    private int CantidadSeleccionados => DatosPaginaActual?.Count(p => p.Seleccionado) ?? 0;

    /// <summary>
    /// Texto a mostrar en el botón de exportación de selección.
    /// </summary>
    private string TextoBotonExportarSeleccion
     => CantidadSeleccionados ==1 ? "Exportar1 solicitud" : $"Exportar {CantidadSeleccionados} solicitudes";

    /// <summary>
    /// Descripción mostrada en el modal al finalizar la exportación.
    /// </summary>
    private string DescripcionModal { get; set; } = "Datos exportados exitosamente";

    #endregion

    #region Variables de Estado y Configuración

    /// <summary>
    /// Datos compartidos de la aplicación (usuario, centro, etc.)
    /// </summary>
    private ApplicationShared ApplicationShared = new();

    /// <summary>
    /// Datos del centro actual obtenidos de la sesión
    /// </summary>
    private GetDataResponseCentro GetCentroResponse = new();

    /// <summary>
    /// Validación de formularios para la configuración de facturación electrónica
    /// </summary>
    private EditContext EditContext;

    /// <summary>
    /// Indica si el componente está cargando datos
    /// </summary>
    private bool IsLoading = true;

    /// <summary>
    /// Indica si la consulta no retornó resultados (para mostrar mensaje de estado vacío)
    /// </summary>
    private bool SinResultados { get; set; }

    /// <summary>
    /// Indica si el usuario ha realizado al menos una búsqueda manual
    /// Se usa para diferenciar la carga inicial de búsquedas posteriores
    /// </summary>
    private bool BusquedaManualRealizada { get; set; } = false;

    /// <summary>
    /// Diccionario para almacenar los contadores de registros por estado/pestaña
    /// </summary>
    private Dictionary<int, int> Contadores { get; set; } = new Dictionary<int, int>
    {
        {1,0 }, // Por Revisar
        {2,0 }, // Facturadas
        {3,0 }, // Anuladas
        {4,0 } // En Cola
    };

    private ModalDetalleFacturacion? ModalDetalleFacturacionRef;

    private bool HasFE;

    #endregion Variables de Estado y Configuración

    #region Variables de Filtros

    /// <summary>
    /// Estado actual de facturación (1=PorRevisar,2=Facturadas,3=Anuladas,4=EnCola)
    /// </summary>
    private int FiltroEstado = (int)EnumEstadoFacturacionElectronica.PorRevisar;

    /// <summary>
    /// Fecha inicial del rango de búsqueda (por defecto: hace15 días)
    /// </summary>
    private DateTime FiltroFechaDesde = DateTime.Today;

    /// <summary>
    /// Fecha final del rango de búsqueda (por defecto: hoy)
    /// </summary>
    private DateTime FiltroFechaHasta = DateTime.Today;

    /// <summary>
    /// Representa el estado del modal al mostrar
    /// </summary>
    /// <remarks>The value indicates the current state or mode of the modal dialog. The meaning of specific
    /// values should be defined elsewhere in the documentation or code comments.</remarks>
    private int MostrarModal = 0;

    /// <summary>
    /// Representa el numero total de pines procesados.
    /// </summary>
    /// <remarks>This field is intended for internal use and is not exposed to consumers of the
    /// class.</remarks>
    private int Total = 0;

    /// <summary>
    /// Representa el numero de pines procesados exitosamente.
    /// </summary>
    /// <remarks>This field is intended for internal tracking of successful outcomes within the containing
    /// class. The value is incremented or updated as operations succeed.</remarks>
    private int TotalExitosos = 0;

    private FiltrosConsultaFacturacionModel FiltrosConsultaFacturacion = new();
    /// <summary>
    /// Indica si hay errores de validación en los campos del formulario
    /// Se calcula dinámicamente consultando el EditContext
    /// </summary>
    private bool HayErroresEnCampos => EditContext?.GetValidationMessages().Any() ?? false;
    #endregion Variables de Filtros

    #region Listas de Datos

    /// <summary>
    /// Catálogo de tipos de documento habilitados para facturación electrónica
    /// Obtenido del servicio y filtrado por VisualizarFacturacionElectronica
    /// </summary>
    private List<TipoDocumentoFacturacionElectronica> ListaDocumentos = [];

    #endregion Listas de Datos

    #region Métodos del Ciclo de Vida de Blazor

    /// <summary>
    /// Método del ciclo de vida que se ejecuta cuando cambian los parámetros
    /// Actualiza el filtro de estado según el parámetro recibido del padre
    /// Limpia los filtros cuando se cambia de pestaña para evitar resultados incorrectos
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        // Inicio código generado por GitHub Copilot
        // Guardar el estado anterior para comparar
        var estadoAnterior = FiltroEstado;

        // Si el parámetro existe y es un enum válido
        if (!string.IsNullOrEmpty(Estado) &&
            System.Enum.TryParse<EnumEstadoFacturacionElectronica>(Estado, ignoreCase: true, out var estadoEnum))
            FiltroEstado = (int)estadoEnum;
        else
            // Valor por defecto si no es válido
            FiltroEstado = (int)EnumEstadoFacturacionElectronica.PorRevisar;

        // Si el estado cambió, limpiar los filtros (excepto fechas)
        if (estadoAnterior != FiltroEstado)
        {
            // Limpiar modelo de filtros
            FiltrosConsultaFacturacion.TipoDocumento = null;
            FiltrosConsultaFacturacion.NumeroDocumento = null;
            FiltrosConsultaFacturacion.NombreCliente = null;
            FiltrosConsultaFacturacion.PIN = null;
            _parametersChanged = true;
        }

        // Solo refrescar si ya se completó el primer render Y hubo cambios
        if (_firstRenderComplete && _parametersChanged && Grid != null)
        {
            _parametersChanged = false;
            // Usar Task.Delay para diferir la ejecución y evitar conflictos
            await Task.Delay(1);
            await FilterChangedAsync(esManual: false);
        }
        // Fin código generado por GitHub Copilot
    }

    /// <summary>
    /// Método del ciclo de vida que se ejecuta al inicializar el componente
    /// Carga datos iniciales y configura el provider del grid
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Inicio código generado por GitHub Copilot
        // Suscribirse al evento de cambio de conteo total para actualizar la UI
        Pagination.TotalItemCountChanged += Pagination_TotalItemCountChanged;

        // Cargar datos de sesión del centro
        var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");

        if (centroShared.Success && centroShared.Value != null)
            GetCentroResponse = centroShared.Value.Respuesta;

        // Consultar el estado de facturación electrónica
        FacturacionResponse<EstadoFacturacion> responseEstado = await MiLicenciaService.ConsultarEstadoFacturacionElectronica(
            new ConsultaEstadoFacturacion() { IdRunt = $"{GetCentroResponse.CodigoRUNT ?? 0}" }
            //new ConsultaEstadoFacturacion() { IdRunt = "1234" }
        );

        if (responseEstado?.SolicitudExitosa == true)
            HasFE = responseEstado.Datos.HabilitadoFacturaElectronica;

        // Inicializar EditContext
        EditContext = new EditContext(FiltrosConsultaFacturacion);

        // Cargar datos compartidos de la aplicación
        var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(ApplicationShared.NameLocalStorage);

        if (result.Success && result.Value != null)
            ApplicationShared = result.Value;

        // Cargar tipos de documento habilitados para facturación electrónica
        ListaDocumentos = await MiLicenciaService.ObtenerTiposDocumentoFacturacionElectronica();

        // Configurar el provider que alimenta el QuickGrid
        ConfigurarGridProvider();
        // Fin código generado por GitHub Copilot

        IsLoading = false;
    }

    /// <summary>
    /// Método del ciclo de vida que se ejecuta después de cada renderización
    /// En el primer render, procesa los cambios de parámetros pendientes
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Inicio código generado por GitHub Copilot
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _firstRenderComplete = true;

            // Si hubo cambios de parámetros antes del primer render, aplicarlos ahora
            if (_parametersChanged && Grid != null)
            {
                _parametersChanged = false;

                // Diferir la ejecución para evitar conflictos de enumeración
                await Task.Delay(10);

                await InvokeAsync(async () =>
                {
                    await FilterChangedAsync();
                    StateHasChanged();
                });
            }
        }
        // Fin código generado por GitHub Copilot
    }

    #endregion Métodos del Ciclo de Vida de Blazor

    // Inicio refactorización/optimización por GitHub Copilot
    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        EditContext?.Validate();
    }
    // Fin refactorización/optimización por GitHub Copilot

// Inicio refactorización/optimización por GitHub Copilot
    #region Lógica dinámica de campo Número de Documento

    private void OnNumeroDocumentoInput(ChangeEventArgs e)
    {
        string raw = e.Value?.ToString() ?? string.Empty;

        // Filtrar caracteres no permitidos según el tipo de documento seleccionado
        string filtrado = DocumentoFacturacionHelper.FiltrarCaracteres(raw, FiltrosConsultaFacturacion.TipoDocumento);

        // Truncar al límite del tipo (int.MaxValue = sin límite, caso "Todos")
        int max = DocumentoFacturacionHelper.GetMaxLength(FiltrosConsultaFacturacion.TipoDocumento);
        if (max != int.MaxValue && filtrado.Length > max)
            filtrado = filtrado[..max];

        FiltrosConsultaFacturacion.NumeroDocumento = filtrado;
        EditContext?.NotifyFieldChanged(FieldIdentifier.Create(() => FiltrosConsultaFacturacion.NumeroDocumento));
    }

    #endregion Lógica dinámica de campo Número de Documento
    // Fin refactorización/optimización por GitHub Copilot
    
    #region Configuración del Grid Provider

    /// <summary>
    /// Configura el delegado que provee datos al QuickGrid
    /// Este método se ejecuta cada vez que el grid necesita datos (paginación, filtros, etc.)
    /// </summary>
    private void ConfigurarGridProvider()
    {
        // Inicio código generado por GitHub Copilot
        FacturacionProvider = async req =>
        {
            // 1. Construir objeto de consulta con los filtros actuales
            var consulta = new RequestFilter
            {
                FechaInicio = FiltrosConsultaFacturacion.FechaDesde,
                FechaFin = GetFechaHastaParaConsulta(),
                TipoDocumento = string.IsNullOrWhiteSpace(FiltrosConsultaFacturacion.TipoDocumento)
                    ? null
                    : ListaDocumentos.FirstOrDefault(x => x.IdTipoSisec.ToString() == FiltrosConsultaFacturacion.TipoDocumento).CodigoACH,
                NumeroDocumento = string.IsNullOrWhiteSpace(FiltrosConsultaFacturacion.NumeroDocumento)
                    ? null
                    : FiltrosConsultaFacturacion.NumeroDocumento.Trim(),
                Nombre = string.IsNullOrWhiteSpace(FiltrosConsultaFacturacion.NombreCliente)
                    ? null
                    : FiltrosConsultaFacturacion.NombreCliente.Trim(),
                Pin = string.IsNullOrWhiteSpace(FiltrosConsultaFacturacion.PIN)
                    ? null
                    : FiltrosConsultaFacturacion.PIN.Trim(),
                PageNumber = (int)((req.StartIndex / req.Count) + 1),
                PageSize = (int)req.Count,
                IdRunt = $"{GetCentroResponse.CodigoRUNT ?? 0}",
                //IdRunt = "1234",
                EstadoML = FiltroEstado.ToString()
            };

            //2. Llamar al servicio para obtener datos
            FacturacionResponse<DataWrapper<PaginatedData>> result =
             await MiLicenciaService.GetElectronicBillingRequests(consulta);

            //3. Validar respuesta del servicio
            if (!result.SolicitudExitosa ||
                result.Datos == null ||
                result.Datos.Datos.Resultado == null ||
                result.Datos.Datos.Resultado.Count == 0)
            {
                SinResultados = true;
                DatosPaginaActual = [];
                SeleccionarTodos = false;

                // Inicio código generado por GitHub Copilot
                // ? Mostrar notificación solo si la solicitud falló
                if (!result.SolicitudExitosa)
                {
                    // Si hay errores específicos, mostrarlos
                    if (result.Errores != null && result.Errores.Count > 0)
                    {
                        foreach (var error in result.Errores)
                        {
                            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, error);
                        }
                    }
                    // Si no hay errores específicos, mostrar mensaje genérico
                    else
                    {
                        await MiLicenciaService.ShowNotificacion(
                            NotificationStatus.Error,
                            "No se pudieron cargar las solicitudes de facturación. Por favor, intente nuevamente."
                        );
                    }
                }
                // Fin código generado por GitHub Copilot

                return GridItemsProviderResult.From(new List<PINDetail>(), 0);
            }

            //4. Actualizar contador de la pestaña activa con el total real del API
            // Inicio refactorización/optimización por GitHub Copilot
            // TotalEncontrados = totalItemCount
            // QuickGrid y Pagination.TotalItemCount reflejen el conteo desde la primera carga
            Contadores[FiltroEstado] = result.Datos.Datos.TotalEncontrados;
            // Fin refactorización/optimización por GitHub Copilot

            //5. Datos válidos, filtrar por estado
            SinResultados = false;

            // Obtener la lista de estados válidos para el filtro actual
            var estadosPermitidos = ObtenerNombresEstado(FiltroEstado);

            // Filtrar resultados según el estado actual
            // Un registro se incluye si su estado está en la lista de estados permitidos
            var datosFiltrados = result.Datos.Datos.Resultado
                .Where(x => estadosPermitidos.Contains(x.Estado, StringComparer.OrdinalIgnoreCase))
                .ToList();

            // Si después del filtrado no hay datos
            if (datosFiltrados.Count == 0)
            {
                SinResultados = true;
                DatosPaginaActual = [];
                SeleccionarTodos = false;
                return GridItemsProviderResult.From(new List<PINDetail>(), 0);
            }

            // Guardar página actual para manejo de selección múltiple
            DatosPaginaActual = datosFiltrados;

            // Inicializar selección según el estado del header
            if (SeleccionarTodos)
            {
                foreach (var item in DatosPaginaActual)
                {
                    item.Seleccionado = true;
                }
            }

            //6. Retornar datos filtrados al QuickGrid
            // Inicio refactorización/optimización por GitHub Copilot
            // TotalEncontrados = totalItemCount
            // QuickGrid y Pagination.TotalItemCount reflejen el conteo desde la primera carga
            return GridItemsProviderResult.From(
                items: datosFiltrados,
                totalItemCount: result.Datos.Datos.TotalEncontrados
            );
            // Fin refactorización/optimización por GitHub Copilot
        };

        // Fin código generado por GitHub Copilot
    }

    #endregion Configuración del Grid Provider

    #region Métodos Auxiliares

    /// <summary>
    /// Convierte el valor numérico del enum a una lista de posibles nombres de estado en texto
    /// Devuelve múltiples valores porque varios estados del backend se agrupan en una misma pestaña
    /// </summary>
    private List<string> ObtenerNombresEstado(int estadoInt)
    {
        // Inicio refactorización/optimización por GitHub Copilot
        var estadoEnum = (EnumEstadoFacturacionElectronica)estadoInt;
        return ObtenerEstadosBackend(estadoEnum);
        // Fin refactorización/optimización por GitHub Copilot
    }

    /// <summary>
    /// Obtiene la lista de estados del backend que corresponden a un estado del enum
    /// </summary>
    /// <param name="estado">Estado del enum de facturación</param>
    /// <returns>Lista de estados del backend que mapean a ese estado</returns>
    private List<string> ObtenerEstadosBackend(EnumEstadoFacturacionElectronica estado)
    {
        // Inicio código generado por GitHub Copilot
        return estado switch
        {
            EnumEstadoFacturacionElectronica.PorRevisar =>
            [
                "En error de configuración",
                "En error de conexión"
            ],

            EnumEstadoFacturacionElectronica.Facturadas =>
            [
                "Facturada"
            ],

            EnumEstadoFacturacionElectronica.Anuladas =>
            [
                "Anulado"
            ],

            EnumEstadoFacturacionElectronica.EnCola =>
            [
                "Registrado",
                "Listo para procesar",
                "Encolados",
                "Procesando"
            ],

            _ =>
            [
                "En error de configuración",
                "En error de conexión"
            ]
        };
        // Fin código generado por GitHub Copilot
    }

    /// <summary>
    /// Calcula la cantidad de registros por cada estado/pestaña
    /// </summary>
    private void CalcularContadores(List<PINDetail> todosLosRegistros)
    {
        // Inicio código generado por GitHub Copilot
        // Contar registros para cada pestaña
        for (int estadoId = 1; estadoId <= 4; estadoId++)
        {
            var estadosPermitidos = ObtenerNombresEstado(estadoId);
            var cantidad = todosLosRegistros
                .Count(x => estadosPermitidos.Contains(x.Estado, StringComparer.OrdinalIgnoreCase));

            Contadores[estadoId] = cantidad;
        }
        // Fin código generado por GitHub Copilot
    }

    /// <summary>
    /// Obtiene la clase CSS para el badge de estado del backend
    /// Retorna las clases personalizadas definidas en el archivo CSS del componente
    /// </summary>
    private string GetEstadoBadgeClass(string estado)
    {
        // Inicio código generado por GitHub Copilot
        return estado?.ToLower() switch
        {
            "en error de configuración" => "badge-warning",
            "en error de conexión" => "badge-warning",
            "registrado" => "badge-info",
            "listo para procesar" => "badge-info",
            "encolados" => "badge-info",
            "procesando" => "badge-info",
            "facturada" => "badge-success",
            "anulado" => "badge-neutral",
            _ => "badge"
        };
        // Fin código generado por GitHub Copilot
    }

    /// <summary>
    /// Mapea el estado del backend al nombre de la pestaña correspondiente del frontend
    /// Homologa múltiples estados del API a las 4 pestañas principales
    /// </summary>
    /// <param name="estadoBackend">Estado que viene de la respuesta del API</param>
    /// <returns>Nombre de la pestaña a mostrar en la columna Estado</returns>
    private string MapearEstadoAPestana(string estadoBackend)
    {
        // Inicio refactorización/optimización por GitHub Copilot
        var estadoEnum = ElectronicBillingStatus.MapearDesdeEstadoBackend(estadoBackend);
        return ElectronicBillingStatus.GetEstadoDisplayName(estadoEnum);
        // Fin refactorización/optimización por GitHub Copilot
    }



    /// <summary>
    /// Obtiene la clase CSS para el badge según el estado mapeado a pestaña
    /// </summary>
    /// <param name="estadoBackend">Estado original del backend</param>
    /// <returns>Clase CSS del badge</returns>
    private string GetEstadoBadgeClassPorPestana(string estadoBackend)
    {
        // Inicio refactorización/optimización por GitHub Copilot
        var estadoEnum = ElectronicBillingStatus.MapearDesdeEstadoBackend(estadoBackend);
        return ObtenerClaseBadge(estadoEnum);
        // Fin refactorización/optimización por GitHub Copilot
    }

    /// <summary>
    /// Obtiene la clase CSS del badge según el estado del enum
    /// Retorna las clases personalizadas definidas en el archivo CSS del componente
    /// </summary>
    /// <param name="estado">Estado del enum</param>
    /// <returns>Clase CSS del badge</returns>
    private string ObtenerClaseBadge(EnumEstadoFacturacionElectronica estado)
    {
        // Inicio código generado por GitHub Copilot
        return estado switch
        {
            EnumEstadoFacturacionElectronica.PorRevisar => "badge-warning",
            EnumEstadoFacturacionElectronica.EnCola => "badge-info",
            EnumEstadoFacturacionElectronica.Facturadas => "badge-success",
            EnumEstadoFacturacionElectronica.Anuladas => "badge-neutral",
            _ => "badge"
        };
        // Fin código generado por GitHub Copilot
    }

    /// <summary>
    /// Obtiene el nombre legible del estado para las pestañas de la UI.
    /// </summary>
    private string GetEstadoDisplayName(EnumEstadoFacturacionElectronica estado)
    {
        // Método generado por GitHub Copilot
        return ElectronicBillingStatus.GetEstadoDisplayName(estado);
    }

    /// <summary>
    /// Actualiza la selección al marcar o desmarcar "Seleccionar todos" en el encabezado
    /// </summary>
    private void OnSeleccionarTodosChanged()
    {
        // Método generado por GitHub Copilot
        if (DatosPaginaActual == null || DatosPaginaActual.Count == 0)
        {
            SeleccionarTodos = false;
            return;
        }

        foreach (var item in DatosPaginaActual)
        {
            item.Seleccionado = SeleccionarTodos;
        }
    }

    /// <summary>
    /// Genera el contenido CSV para los registros seleccionados.
    /// Columnas solicitadas:
    /// estadoML, estado, operadorRecaudo, pin, nombres, apellidos, correo,
    /// tipoDocumento, numeroIdentificacion, fechaFacturacion, fechaRegistro,
    /// valorTotal, numeroFactura
    /// </summary>
    private string GenerarCsvSeleccionados()
    {
        // Método generado por GitHub Copilot
        var seleccionados = DatosPaginaActual
            .Where(p => p.Seleccionado)
            .ToList();

        if (seleccionados.Count == 0)
            return string.Empty;

        // Inicio refactorización/optimización por GitHub Copilot
        // Agregar BOM UTF-8 para que Excel reconozca correctamente acentos y caracteres especiales.
        var sb = new StringBuilder();
        sb.Append('\uFEFF');
        // Fin refactorización/optimización por GitHub Copilot

        // Encabezados
        sb.AppendLine(string.Join(';', new[]
        {
            "estadoML",
            "estado",
            "operadorRecaudo",
            "pin",
            "nombres",
            "apellidos",
            "correo",
            "tipoDocumento",
            "numeroIdentificacion",
            "fechaFacturacion",
            "fechaRegistro",
            "valorTotal",
            "numeroFactura"
        }));

        // Valor textual del estado ML (p.ej. "PorRevisar")
        var estadoMlTexto = ((EnumEstadoFacturacionElectronica)FiltroEstado).ToString();

        foreach (var item in seleccionados)
        {
            // Inicio refactorización/optimización por GitHub Copilot
            // Excel tiende a interpretar números largos como notación científica.
            // Para evitarlo, exportamos el PIN como texto prefijando un tab (\t).
            // Esto fuerza a Excel a tratar el valor como texto sin alterar el contenido visible.
            var pinComoTextoParaExcel = string.IsNullOrWhiteSpace(item.Pin) ? null : $"\t{item.Pin.Trim()}";
            // Fin refactorización/optimización por GitHub Copilot

            sb.AppendLine(string.Join(';', new[]
            {
                estadoMlTexto,
                item.Estado,
                item.OperadorRecaudo,
                pinComoTextoParaExcel,
                item.Nombres,
                item.Apellidos,
                item.Correo,
                item.TipoDocumento,
                item.NumeroIdentificacion,
                item.FechaFacturacion.HasValue ? item.FechaFacturacion.Value.ToString("dd-MM-yyyy") : null,
                item.FechaRegistro.ToString("dd-MM-yyyy"),
                item.ValorTotal.ToString("0.##"),
                item.NumeroFactura
            }.Select(EscaparCsv)));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Genera el contenido CSV para los registros seleccionados
    /// </summary>
    private async Task<FacturacionResponse<CancelBillingRequestResult>> AnularSeleccionados()
    {
        // Método generado por GitHub Copilot
        var seleccionados = DatosPaginaActual
            .Where(p => p.Seleccionado)
            .ToList();

        if (seleccionados.Count == 0)
            return new();

        FacturacionResponse<CancelBillingRequestResult> response = await MiLicenciaService.CancelBillingRequest(new UpdateElectronicBillingRequest()
        {
            IdRunt = GetCentroResponse.CodigoRUNT?.ToString() ?? "",
            UsuarioPortal = ApplicationShared.UserName ?? "",
            Pines = [.. seleccionados.Select(s => s.Pin)]
        });

        return response;
    }

    /// <summary>
    /// Escapa un valor para ser usado en CSV
    /// </summary>
    private string EscaparCsv(string valor)
    {
        // Método generado por GitHub Copilot
        if (valor == null)
        {
            return string.Empty;
        }

        if (valor.Contains('"'))
        {
            valor = valor.Replace("\"", "\"\"");
        }

        // Como usamos ';' como separador, escapamos si contiene ';' o saltos de línea
        if (valor.Contains(';') || valor.Contains('\n') || valor.Contains('\r'))
        {
            return $"\"{valor}\"";
        }

        return valor;
    }

    /// <summary>
    /// Exporta los registros seleccionados a CSV y dispara la descarga en el navegador.
    /// </summary>
    private async Task ExportarSeleccionAsync()
    {
        // Método generado por GitHub Copilot
        if (!HaySeleccionados)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay registros seleccionados para exportar.");
            return;
        }

        var csv = GenerarCsvSeleccionados();
        if (string.IsNullOrWhiteSpace(csv))
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No se pudo generar el archivo de exportación.");
            return;
        }

        var cantidad = CantidadSeleccionados;
        DescripcionModal = $"{cantidad} solicitudes exportadas en formato CSV exitosamente";
        await InvokeAsync(StateHasChanged);

        // Nombre: solicitudesfacturacion_ddmmyyyy_hora.csv
        // Se incluye hora con hhmmss para evitar colisiones.
        var fileName = $"solicitudesfacturacion_{DateTime.Now:ddMMyyyy_HHmmss}.csv";

        //1) Intentar descarga (si falla, sí mostramos error)
        try
        {
            await JsRuntime.InvokeVoidAsync("downloadHelper.downloadCsv", fileName, csv);
        }
        catch (JSException jsEx)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Ocurrió un error al iniciar la descarga del archivo.");
        }
        catch (Exception ex)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Ocurrió un error inesperado al exportar la información.");
        }

        try
        {
            await JsRuntime.InvokeVoidAsync("downloadHelper.openExportSuccessModal");
        }
        catch (JSException jsEx)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, $"No se pudo abrir el modal (JS): {jsEx.Message}");
        }
        catch (Exception)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "No se pudo abrir el modal de confirmación.");
        }
    }

    /// <summary>
    /// Anula los registros seleccionados.
    /// </summary>
    private async Task AnularSeleccionAsync()
    {
        if (!HaySeleccionados)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay registros seleccionados para exportar.");
            return;
        }

        try
        {
            FacturacionResponse<CancelBillingRequestResult> anulados = await AnularSeleccionados();

            if (!anulados.SolicitudExitosa)
            {
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No se pudo procesar la solicitud correctamente.");
                return;
            }
        }
        catch (Exception)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Ocurrió un error inesperado al procesar la solicitud.");
        }
    }

    #endregion Métodos Auxiliares

    #region Métodos de Filtrado y Paginación

    private bool CanGoBack => Pagination.CurrentPageIndex > 0;

    private bool CanGoForwards => Pagination.CurrentPageIndex < Pagination.LastPageIndex;

    /// <summary>
    /// Calcula la fecha final para la consulta
    /// </summary>
    private DateTime GetFechaHastaParaConsulta()
    {
        // Inicio refactorización/optimización por GitHub Copilot
        var hoy = DateTime.Today;
        var fechaHasta = FiltrosConsultaFacturacion.FechaHasta.Date;

        if (fechaHasta == hoy)
        {
            return fechaHasta.AddDays(2).AddTicks(-1);
        }

        return fechaHasta.AddDays(1).AddTicks(-1);
        // Fin refactorización/optimización por GitHub Copilot
    }

    /// <summary>
    /// Se ejecuta cuando el usuario aplica filtros o cambia de pestaña
    /// </summary>
    /// <param name="esManual">Indica si la búsqueda fue iniciada manualmente por el usuario (true) o automáticamente por el sistema (false)</param>
    private async Task FilterChangedAsync(bool esManual = true)
    {
        // Inicio código generado por GitHub Copilot
        SeleccionarTodos = false;
        DatosPaginaActual = [];

        // Primero cambiar la página (esto disparará el refresh automáticamente)
        if (Pagination != null)
        {
            await Pagination.SetCurrentPageIndexAsync(0);
        }
        
        // Solo marcar como búsqueda manual si el usuario la inició explícitamente
        if (esManual)
        {
            BusquedaManualRealizada = true;
        }
        try
        {   
            if(Pagination.TotalItemCount != null)
                await Grid!.RefreshDataAsync();
        }
        catch (Exception ex)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Ocurrió un error inesperado al procesar la solicitud.");
        }
        
        // Fin código generado por GitHub Copilot
    }

    /// <summary>
    /// Maneja el submit del formulario de filtros
    /// </summary>
    private async Task OnConsultarAsync()
    {
        if (!EditContext.Validate())
        {
            return;
        }

        await FilterChangedAsync(esManual: true);
    }

    /// <summary>
    /// Limpia todos los filtros y restablece a valores por defecto
    /// </summary>
    private async Task LimpiarFiltrosAsync()
    {
        // Inicio código generado por GitHub Copilot
        
        // Limpiar filtros de fechas
        FiltroFechaDesde = DateTime.Today;
        FiltroFechaHasta = DateTime.Today;

        FiltrosConsultaFacturacion.FechaDesde = DateTime.Today;
        FiltrosConsultaFacturacion.FechaHasta = DateTime.Today;

        // Limpiar filtros del modelo de validación
        FiltrosConsultaFacturacion.TipoDocumento = null;
        FiltrosConsultaFacturacion.NumeroDocumento = null;
        FiltrosConsultaFacturacion.NombreCliente = null;
        FiltrosConsultaFacturacion.PIN = null;

        SeleccionarTodos = false;
        DatosPaginaActual = [];

        EditContext.Validate();

        // Marcar que el usuario ha realizado una búsqueda manual
        BusquedaManualRealizada = true;

        // Solo refrescar si el grid ya está inicializado
        if (_firstRenderComplete && Grid != null)
        {
            await Task.Delay(1);
            await FilterChangedAsync();
        }
        // Fin código generado por GitHub Copilot
    }

    private void Pagination_TotalItemCountChanged(object sender, int? e) => StateHasChanged();

    private async Task GoToPageAsync(int pageIndex) => await Pagination.SetCurrentPageIndexAsync(pageIndex);

    private string PageButtonClass(int pageIndex) => Pagination.CurrentPageIndex == pageIndex ? "btn-active" : null;

    private string AriaCurrentValue(int pageIndex) => Pagination.CurrentPageIndex == pageIndex ? "page" : null;

    private Task GoFirstAsync() => GoToPageAsync(0);

    private Task GoPreviousAsync() => GoToPageAsync(Pagination.CurrentPageIndex - 1);

    private Task GoNextAsync() => GoToPageAsync(Pagination.CurrentPageIndex + 1);

    private Task GoLastAsync() => GoToPageAsync(Pagination.LastPageIndex.GetValueOrDefault(0));

    #endregion Métodos de Filtrado y Paginación

    #region Métodos para manejo del modal

    // Inicio refactorización/optimización por GitHub Copilot
    /// <summary>
    /// Muestra el modal de confirmación al usuario antes de proceder con la anulación.
    /// </summary>
    /// <remarks>
    /// Este método valida que haya registros seleccionados antes de mostrar el modal.
    /// Si no hay selección, muestra una notificación de advertencia y no abre el modal.
    /// Actualiza el estado del componente para reflejar los cambios en la UI.
    /// </remarks>
    private void MostrarModalConfirmacion()
    {
        if (!HaySeleccionados)
        {
            _ = MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay registros seleccionados para anular.");
            return;
        }

        MostrarModal = 1;
        StateHasChanged();
    }

    /// <summary>
    /// Cierra el modal de diálogo y restablece el estado del componente.
    /// </summary>
    /// <remarks>
    /// Establece la propiedad <see cref="MostrarModal"/> en 0 para indicar que el modal está cerrado.
    /// Limpia los contadores de totales procesados y exitosos.
    /// Dispara una actualización de la interfaz de usuario.
    /// </remarks>
    private void CerrarModal()
    {
        MostrarModal = 0;
        Total = 0;
        TotalExitosos = 0;
        StateHasChanged();
    }

    /// <summary>
    /// Confirma y procesa la anulación de las solicitudes de facturación seleccionadas.
    /// Actualiza el estado del resultado según el éxito o fallo de la operación.
    /// </summary>
    /// <remarks>
    /// Este método procesa de forma asíncrona la anulación de las solicitudes seleccionadas y actualiza
    /// las propiedades de resumen como el número total de registros procesados y anulaciones exitosas.
    /// Establece el estado del modal según el resultado de la operación (éxito total, error total o éxito parcial).
    /// Incluye manejo robusto de excepciones para errores de conexión, timeout y errores inesperados.
    /// Después de una anulación exitosa, refresca automáticamente el grid para mostrar los cambios.
    /// </remarks>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    private async Task ConfirmarAnulacion()
    {
        try
        {
            if (!HaySeleccionados)
            {
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay registros seleccionados para anular.");
                CerrarModal();
                return;
            }

            FacturacionResponse<CancelBillingRequestResult> response = await AnularSeleccionados();

            // Validar que la respuesta no sea nula
            if (response == null)
            {
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "No se recibió respuesta del servidor.");
                MostrarModal = 3; // Modal de error total
                StateHasChanged();
                return;
            }

            // Validar que los datos no sean nulos
            if (response.Datos == null)
            {
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "La respuesta del servidor no contiene datos válidos.");
                MostrarModal = 3; // Modal de error total
                StateHasChanged();
                return;
            }

            // Asignar totales de forma segura
            Total = response.Datos.TotalProcesados;
            TotalExitosos = response.Datos.TotalExitosos;
            var totalFallidos = response.Datos.TotalFallidos;

            // Determinar el tipo de modal según los resultados
            if (TotalExitosos == Total && totalFallidos == 0)
            {
                MostrarModal = 2; // Éxito total
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Success, $"Se anularon exitosamente {TotalExitosos} solicitud(es) de facturación.");
            }
            else if (totalFallidos == Total)
            {
                MostrarModal = 3; // Error total
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "No se pudo anular ninguna solicitud de facturación.");
            }
            else if (totalFallidos > 0 && totalFallidos < Total)
            {
                MostrarModal = 4; // Éxito parcial
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, $"Se anularon {TotalExitosos} de {Total} solicitud(es). {totalFallidos} fallaron.");
            }
            else
            {
                // Caso por defecto si no se cumple ninguna condición esperada
                MostrarModal = 0;
                await MiLicenciaService.ShowNotificacion(NotificationStatus.Info, "La operación finalizó con un resultado inesperado.");
            }

            // Refrescar el grid para mostrar los cambios
            if (Grid != null)
            {
                await Task.Delay(500); // Dar tiempo para que el backend actualice
                await Grid.RefreshDataAsync();
            }

            StateHasChanged();
        }
        catch (HttpRequestException httpEx)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Error de conexión con el servidor. Por favor, verifique su conexión a internet.");
            MostrarModal = 3;
            StateHasChanged();
        }
        catch (TaskCanceledException)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "La operación ha tardado demasiado tiempo. Por favor, intente nuevamente.");
            MostrarModal = 3;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "Ocurrió un error inesperado al procesar la anulación.");
            MostrarModal = 3;
            StateHasChanged();
        }
    }

    // Fin refactorización/optimización por GitHub Copilot

    private async Task ShowModal(int idPtesaPIN)
    {
        // Inicio refactorización/optimización por GitHub Copilot
        if (ModalDetalleFacturacionRef != null)
        {
            await ModalDetalleFacturacionRef.AbrirModalAsync(idPtesaPIN);
        }
        // Fin refactorización/optimización por GitHub Copilot
    }

    #endregion Métodos de Filtrado y Paginación

    // Fin código generado por GitHub Copilot
}


