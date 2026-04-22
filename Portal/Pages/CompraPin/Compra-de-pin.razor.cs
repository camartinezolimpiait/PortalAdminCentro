using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.PowerBI.Api.Models;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Facturacion;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.CompraPin;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPin;

public partial class Compra_de_pin
{
	#region Constructor

	[Inject]
	private IPermisoService PermisoService { get; set; }

	[Inject]
	private NavigationManager Navigation { get; set; }

	[Inject]
	private ProtectedSessionStorage ProtectedSessionStore { get; set; }

	[Inject]
	private IMiLicenciaService MiLicenciaService { get; set; }

    [Parameter, SupplyParameterFromQuery]
    public string modo { get; set; }

    #endregion Constructor

    #region Variables

    private ApplicationSevice menuService = new();
	private GetDataResponseCentro getCentroResponse = new();
	private PagoPin pagoPin = new();
	private int pasosCompraPin = 0;
	private bool isLoading = false;
	private bool isFormValid = false;
	private readonly HashSet<int> PasosVisitados = new();
    private PasosCompraPin pasoActual = PasosCompraPin.CompraPin;
	private PasosCompraPin pasoAnterior;
    private Stack<PasosCompraPin> historialPasos = new Stack<PasosCompraPin>();
    private bool esRetroceso = false;
    private PasosCompraPin ultimoPasoEjecutado;
    private bool primeraVezTramite = true;
    ComponentesCompra componentesCompra = new ComponentesCompra();
    private bool abrirModalIrInicio;

    private static readonly HashSet<int> OrigenesPermitidos = new()
        {
            (int)EnumTipoPago.PinDirecto,
            (int)EnumTipoPago.PSEColpatria
        };

    private static readonly Dictionary<EnumTipoPago, string> NombresPorTipoPago = new()
    {
        [EnumTipoPago.PinDirecto] = "PIN Directo Davibank",
        [EnumTipoPago.PSEColpatria] = "PSE Davibank",
        // agregar más mapeos si se habilitan más orígenes
    };

    private const string IconoPorDefecto = "/images/iconos/default.png";

    public bool esModificacion { get; set; } = false;
    #endregion Variables

    #region Metodos

    protected override async Task OnInitializedAsync()
	{
		isLoading = true;
        await InfoCentro();
        await ObtenerCategorias();
        await ObtenerConvenios();

        if (!string.IsNullOrEmpty(modo) && modo.Equals("crear"))
        {
            GotoDatosBasicos();
        }
    }

    protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
	}

    private async Task GetIsCliked()
    {
        if (historialPasos.Count > 0)
        {
            pasoActual = historialPasos.Pop();
            StateHasChanged(); 
        }

		if (pasoActual == (PasosCompraPin)0)
		{
			pagoPin = new();
            await InfoCentro();
        }
    }


    private void HandleFormValidChanged(bool isValid)
    {
        if (!isValid) return;

        componentesCompra.DesactivarTodo();

        pagoPin.PasoCotizacion = PasosCompraPin.DatosBasicos;
        componentesCompra.TipoTramite = true;
        StateHasChanged();
    }

    public void HandleFormValidCuotas(bool isValid)
    {
        if (!isValid) return;
        componentesCompra.DesactivarTodo();
        componentesCompra.ConfirmarCompra = true;

    }

    public void onDatosPersonales(bool isValid)
    {
        componentesCompra.DesactivarTodo();
        if (pagoPin.EmisionOtraPersona)
        {
            componentesCompra.FacturaElectronica = true;
        }
        else
        {
            componentesCompra.MediosPago = true;
        }
    }

    public void onFacturaElectronica(bool isValid)
    {
        componentesCompra.DesactivarTodo();
        componentesCompra.MediosPago = true;
    }

    public async Task OnMedioPago(bool isValid)
    {

        // Desactivar al inicio (guard clause)
        componentesCompra.DesactivarTodo();

        // Buscar una sola vez el medio seleccionado
        var medioSeleccionado = pagoPin.MediosPago
            .FirstOrDefault(x => x.Id == pagoPin.TipoRecaudoCtrl);

        // Selección por cliente con rama clara
        if (pagoPin.ClienteCompra == (int)EnumTipoCliente.CEA)
        {
            if (medioSeleccionado.ComprasCuotas)
                componentesCompra.CuotaCeas = true;
            else
                componentesCompra.ConfirmarCompra = true;
        }
        else
        {
            componentesCompra.ConfirmarCompra = true;
            pagoPin.ObtenerPagoCrc = true;
        }

        await ObtenerCosto();
    }


    private void HandleFormValidChangedTramite(bool isValid)
    {
        if (!isValid) return;

        if (pagoPin.OpcionTramite == 1)
        {
            pagoPin.PasoCotizacion = PasosCompraPin.DatosPersonales;
        }else if (pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboMoto)
        {
            pagoPin.PasoCotizacion = PasosCompraPin.CategoriaComboMoto;
        }
        else if (pagoPin.PasoCotizacion != PasosCompraPin.CategoriaComboCarro
              && pagoPin.PasoCotizacion != PasosCompraPin.CategoriaComboMoto)
        {
            // primera vez: aún no pasó por combo → forzamos a carro
            pagoPin.PasoCotizacion = PasosCompraPin.CategoriaComboCarro;
        }

        componentesCompra.DesactivarTodo();
        componentesCompra.Categoria = true;
        StateHasChanged();
    }

    private async Task HandleFormValidChangedCategoria(bool isValid)
    {
        componentesCompra.DesactivarTodo();
        if (isValid)
        {
            await ObtenerCosto();
            componentesCompra.DatosPersonales = true;
        }
        else
        {
            pagoPin.PasoCotizacion = PasosCompraPin.CategoriaComboMoto;
            componentesCompra.TipoTramite = true;
        }


        StateHasChanged();
    }

    private void Avanzar(PasosCompraPin nuevoPaso)
    {
        // Guardar paso actual antes de avanzar
        historialPasos.Push(pasoActual);
        pasoActual = nuevoPaso;
        StateHasChanged();
    }

    private async Task OnSiguienteClicked()
	{
		
	}

    private async Task ObtenerConvenios()
    {
        ConsultaCentoId consulta = new ConsultaCentoId
        {
            IdCentro = (int)pagoPin.CentroSeleccionado.IdCentro,
            CodigoDestino = pagoPin.CentroSeleccionado.CodigoRUNT?.ToString()
        };

        List<ConvenioCentro> convenios = pagoPin.ClienteCompra switch
        {
            (int)EnumTipoCliente.CEA => await MiLicenciaService.ObtenerConveniosCEA(consulta),
            (int)EnumTipoCliente.CRC => await MiLicenciaService.ObtenerConveniosCRC(consulta),
            _ => new List<ConvenioCentro>()
        };

        pagoPin.MediosPago = convenios
            .Where(c => OrigenesPermitidos.Contains(c.IdOrigenPin))
            .Select(c => new MedioPago
            {
                Id = c.IdOrigenPin,
                Nombre = NombresPorTipoPago.TryGetValue((EnumTipoPago)c.IdOrigenPin, out var nombre)
                    ? nombre
                    : c.ConvenioNombre, // fallback si llega un id no mapeado
                IconoUrl = IconosMediosPago.Urls.TryGetValue(c.IdOrigenPin, out var url) ? url : IconoPorDefecto,
                ComprasCuotas = c.ComprasCuotas
            })
            .ToList();
    }

    private async Task PinHasChanged(PagoPin value)
	{
		// Maneja el evento y recibe el valor del componente hijo
		pagoPin = value;
		await Task.FromResult(true);
	}

	private async Task OnCuotaSelect(bool value)
	{
		// Maneja el evento y recibe el valor del componente hijo
		isFormValid = value;
	}

	private async Task PasosCompraPinChanged(int value)
	{
        await CambiarAVista((PasosCotizacion)value);
	}

	private void AsignacionCentro()
	{
		pagoPin.CentroSeleccionado = new Centro()
		{
			IdCentro = getCentroResponse != null ? getCentroResponse.IdCentro : 0,
			Nombre = getCentroResponse != null ? getCentroResponse.Nombre : ""
		};
	}

	private async Task CostoHasChanged()
	{
		// Maneja el evento y recibe el valor del componente hijo
		await ObtenerCosto();
	}

	private async Task ObtenerCosto()
	{
        CotizacionPin costo;

        if (!string.IsNullOrEmpty(pagoPin.Categoria) && pagoPin.ClienteCompra == (int)EnumTipoCliente.CEA)
		{            
			int idCategoria = pagoPin.CategoriasCea.Any() ? pagoPin.CategoriasCea.Find(x => x.Codigo == pagoPin.Categoria).IdCategoria : 0;
			costo = await MiLicenciaService.GeneracionCosto(pagoPin, idCategoria);
        }
        else
        {
            var parametrosCosto = new ParametrosCostoPin
            {
                CodigoCategoria1 = pagoPin.Categoria1,
                CodigoCategoria2 = pagoPin.Categoria2,
                Edad = pagoPin.EdadAspirante,
                Genero = pagoPin.Usuario?.Genero,
                IdCentro = pagoPin.CentroSeleccionado?.IdCentro,
                IdOrigenPin = pagoPin.ObtenerPagoCrc ? TipoOrigenPin(pagoPin) : (int)EnumTipoPago.PSE
            };

             costo = await MiLicenciaService.ObtenerPrecioPIN(parametrosCosto);
            
        }
        if (costo != null && (!string.IsNullOrEmpty(pagoPin.Categoria1) || !string.IsNullOrEmpty(pagoPin.Categoria2)))
        {
            await AsignarCosto(costo);
        }

    }

	private async Task AsignarCosto(CotizacionPin costo)
	{
		if (costo != null && await ValidarCosto(costo))
		{
			pagoPin.ValorDiscriminadoCotizacion.ValorTotal = costo.ValorTotal;
			pagoPin.ValorDiscriminadoCotizacion.Sicov = costo.SICOV;
			pagoPin.ValorDiscriminadoCotizacion.Banco = costo.BANCO;
			pagoPin.ValorDiscriminadoCotizacion.Crc = costo.CRC;
			pagoPin.ValorDiscriminadoCotizacion.Ansv = costo.ANSV;
			pagoPin.CostoCuotas = costo.CalculoCoutas;

			if (costo.CalculoCoutas.Any())
				pagoPin.ValorDiscriminadoCotizacion.CalculoCoutas = costo.CalculoCoutas;

            if (pagoPin?.TipoRecaudoCtrl.HasValue == true &&
            pagoPin.TipoRecaudoCtrl.Value > 0 &&
            pagoPin.TipoRecaudoCtrl.Value == (int)EnumTipoPago.PinDirecto)
            {
                pagoPin.ValorDiscriminadoCotizacion.Banco = pagoPin.ConfiguracionCuotas.ValorAliado;
            }
        }
		else
		{
			pagoPin.ValorDiscriminadoCotizacion.CalculoCoutas = null;
			pagoPin.Categoria = null;
			pagoPin.CostoCuotas = null;
		}
	}

	private async Task<bool> ValidarCosto(CotizacionPin cotizacionPin)
	{
		if (cotizacionPin.ANSV == 0 || cotizacionPin.ValorTotal == 0 || cotizacionPin.BANCO == 0 || cotizacionPin.CRC == 0)
		{
			await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No se encontró información relacionada para calcular " +
				"el costo del PIN para la categoría seleccionada ");
			return false;
		}
		return true;
	}

	private async Task ValidarPermisos()
	{
		int idCentro = getCentroResponse.IdCentro;
		int platataforma = menuService.ClienteId;
		if (menuService.Plataforma != "CDA")
		{
			bool tienePermiso = await PermisoService.TienePermisoParaCompraPin(idCentro, platataforma);
			if (!tienePermiso)
			{
				Navigation.NavigateTo("/configuracion/PerfilMilicencia");
				await MiLicenciaService.ShowNotificacion(NotificationStatus.Error, "No se puede acceder a la ruta especificada");
			}
		}
	}

    // Inicio refactorización/optimización por GitHub Copilot
	private async Task InfoCentro()
	{
        var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");
        menuService = protectedSessionStore.Value;

        var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
        getCentroResponse = centroShared.Value.Respuesta;
        await ValidarPermisos();
        AsignacionCentro();

        pagoPin.CentroSeleccionado.IdCentro = getCentroResponse.IdCentro;
        pagoPin.CentroSeleccionado.Nombre = getCentroResponse.Nombre;
        pagoPin.CentroSeleccionado.IdComercio = getCentroResponse.IdComercio;
        pagoPin.CentroSeleccionado.IdDepartamento = getCentroResponse.IdDepartamento;
        pagoPin.CentroSeleccionado.IdMunicipio = getCentroResponse.IdMunicipio;
        pagoPin.CentroSeleccionado.IdZona = getCentroResponse.IdZona;
        pagoPin.CentroSeleccionado.Direccion = getCentroResponse.Direccion;
        pagoPin.CentroSeleccionado.Email = getCentroResponse.Email;
        pagoPin.CentroSeleccionado.Fijo = getCentroResponse.Fijo;
        pagoPin.CentroSeleccionado.Movil = getCentroResponse.Movil;
        pagoPin.CentroSeleccionado.Latitud = getCentroResponse.Latitud;
        pagoPin.CentroSeleccionado.Longitud = getCentroResponse.Longitud;
        pagoPin.CentroSeleccionado.CodigoRUNT = getCentroResponse.CodigoRUNT;

        if (System.Enum.TryParse<EnumTipoCliente>(menuService.Plataforma, out var tipoCliente))
        {
            pagoPin.ClienteCompra = (int)tipoCliente;
        }
        else
        {
            // Manejo si no es válido
            pagoPin.ClienteCompra = 0; // o cualquier valor por defecto
        }

        _ = ActivoFacturacionElectronica();
        isLoading = false;
    }
    // Fin refactorización/optimización por GitHub Copilot

	public void NavegarResumen(int paso)
	{
		pasosCompraPin = paso;
		PasosVisitados.Add(paso);
		StateHasChanged();
	}

    // Inicio código generado por GitHub Copilot
    private bool EsVistaActual(PasosCotizacion vista)
    {
        return vista switch
        {
            PasosCotizacion.DatosBasicos => componentesCompra.DatosBasicos,
            PasosCotizacion.DatosPersonales => componentesCompra.DatosPersonales,
            PasosCotizacion.FacturaElectronica => componentesCompra.FacturaElectronica,
            PasosCotizacion.PagoCuotas => componentesCompra.CuotaCeas,
            PasosCotizacion.Pagos => componentesCompra.MediosPago,
            PasosCotizacion.Tramite => componentesCompra.TipoTramite &&
                                       (pagoPin.OpcionTramite == (int)OpcionTramitesEnum.Simple ||
                                        pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboCarro),
            PasosCotizacion.TramiteDos => componentesCompra.TipoTramite &&
                                          pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboMoto,
            PasosCotizacion.Categoria => componentesCompra.Categoria &&
                                         (pagoPin.OpcionTramite == (int)OpcionTramitesEnum.Simple ||
                                          pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro),
            PasosCotizacion.CategoriaDos => componentesCompra.Categoria &&
                                            pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto,
            _ => false
        };
    }
    // Fin código generado por GitHub Copilot

    public async Task CambiarAVista(PasosCotizacion vista)
    {
        // Inicio código generado por GitHub Copilot
        if (EsVistaActual(vista))
        {
            return;
        }
        // Fin código generado por GitHub Copilot

        componentesCompra.DesactivarTodo();
        pagoPin.CentroIpVolver = true;

        switch (vista)
        {
            case PasosCotizacion.DatosPersonales:
                componentesCompra.DatosPersonales = true;
                break;

            // Inicio código generado por GitHub Copilot
            case PasosCotizacion.FacturaElectronica:
                componentesCompra.FacturaElectronica = true;
                break;
            // Fin código generado por GitHub Copilot

            case PasosCotizacion.DatosBasicos:
                componentesCompra.DatosBasicos = true;
                break;

            case PasosCotizacion.Tramite:
                pagoPin.PasoCotizacion =
                    pagoPin.OpcionTramite == (int)OpcionTramitesEnum.Simple
                        ? PasosCompraPin.TipoTramiteSimple
                        : PasosCompraPin.TipoTramiteComboCarro;
                componentesCompra.TipoTramite = true;
                break;

            case PasosCotizacion.TramiteDos:
                pagoPin.PasoCotizacion =
                    pagoPin.OpcionTramite == (int)OpcionTramitesEnum.Simple
                        ? PasosCompraPin.TipoTramiteSimple
                        : PasosCompraPin.TipoTramiteComboMoto;
                componentesCompra.TipoTramite = true;
                break;

            case PasosCotizacion.Categoria:
                pagoPin.PasoCotizacion =
                    pagoPin.OpcionTramite == (int)OpcionTramitesEnum.Simple
                        ? PasosCompraPin.DatosPersonales
                        : PasosCompraPin.CategoriaComboCarro;
                componentesCompra.Categoria = true;
                break;

            case PasosCotizacion.CategoriaDos:
                pagoPin.PasoCotizacion =
                    pagoPin.OpcionTramite == (int)OpcionTramitesEnum.Simple
                        ? PasosCompraPin.CategoriasSimple
                        : PasosCompraPin.CategoriaComboMoto;
                componentesCompra.Categoria = true;
                break;
            case PasosCotizacion.PagoCuotas:
                pagoPin.PasoCotizacion = PasosCompraPin.CuotasCeas;
                componentesCompra.CuotaCeas = true;
                break;

            case PasosCotizacion.Pagos:
                componentesCompra.MediosPago = true;
                break;

            default:
                componentesCompra.DesactivarTodo();
                componentesCompra.DatosBasicos = true;
                break;
        }
        await ObtenerCosto();
        StateHasChanged();
    }

    private void VolverMedioDePagoCuotas()
    {
        var medio = pagoPin?.MediosPago?.FirstOrDefault(x => x.Id == pagoPin?.TipoRecaudoCtrl);
        if (medio?.ComprasCuotas == true)
        {
            GotoCuotasCeas();
            return;
        }

        GotoMedioPago();
    }


    private void FormularioDevoluciones()
	{
		NavManager.NavigateTo("/compradepin/devoluciones", false);
	}

	private void FormularioGestionPin()
	{
		NavManager.NavigateTo("/compradepin/gestionarPin", false);
	}

	private void FormularioPagoCuota()
	{
		NavManager.NavigateTo("/compradepin/pagoCuota", false);
	}

    private string ObtenerNombreCentro(string plataforma)
    {
        return plataforma switch
        {
            "CEA" => "Centro de enseñanza automovilística",
            "CRC" => "Centro de reconocimiento de conductores",
            "CDA" => "Centro de diagnóstico automotor",
            _ => "Plataforma desconocida"
        };
    }

    private string ObtenerDescripcion(string plataforma)
    {
        return plataforma switch
        {
            "CEA" => "Curso de conducción",
            "CRC" => "Examen médico",
            "CDA" => "Revisión técnico-mecánica", // opcional
            _ => ""
        };
    }

    private void GuardarPasoYAvanzar(PasosCompraPin nuevoPaso)
    {
        historialPasos.Push(pasoActual); // Guarda el paso actual
        pasoActual = nuevoPaso;          // Cambia al nuevo paso
        StateHasChanged();
    }

    #endregion Metodos

    #region pasos

    private async Task GotoCompraPin()
    {
        componentesCompra.DesactivarTodo();
        componentesCompra.CompraPin = true;
        pagoPin = new();
        await InfoCentro();
        await ObtenerCategorias();
        await ObtenerConvenios();
        StateHasChanged();
    }
    private void GotoDatosBasicos()
    {
        componentesCompra.DesactivarTodo();
        if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto)
        {
            componentesCompra.Categoria = true;
            pagoPin.PasoCotizacion = PasosCompraPin.CategoriaComboCarro;
        }else if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboCarro)
        {
            componentesCompra.Categoria = true;
        }
        else
        {
            componentesCompra.DatosBasicos = true;
        }
        
        StateHasChanged();
    }
    private void GotoCantidadTramites() 
    {
        componentesCompra.DesactivarTodo();
        if (pagoPin.PasoCotizacion == PasosCompraPin.CategoriaComboMoto)
        {
            componentesCompra.Categoria = true;
        }else if (pagoPin.PasoCotizacion == PasosCompraPin.TipoTramiteComboMoto)
        {
            componentesCompra.TipoTramite = true;
        }
        else
        {
            componentesCompra.TipoTramite = true;
        }
        
        StateHasChanged();
    }

    private int TipoOrigenPin(PagoPin pagoPin)
    {
        int resultado = 0;

        switch (pagoPin.TipoRecaudoCtrl)
        {
            case (int)EnumTipoPago.PSE:
                resultado = (int)EnumTipoPago.PSE;
                break;
            case (int)EnumTipoPago.BancolombiaWompi:
                resultado = (int)EnumTipoPago.BancolombiaWompi;
                break;
            case (int)EnumTipoPago.NequiWompi:
                resultado = (int)EnumTipoPago.NequiWompi;
                break;
            case (int)EnumTipoPago.TdCWompi:
                resultado = (int)EnumTipoPago.TdCWompi;
                break;
            case (int)EnumTipoPago.Daviplata:
                resultado = (int)EnumTipoPago.Daviplata;
                break;
            default:
                resultado = pagoPin.TipoPagoEfectivo ?? 0;
                break;
        }

        return resultado;
    }


    private void GotoTipoTramiteComboCarro()
    {
        pasoActual = PasosCompraPin.TipoTramiteComboCarro;
        StateHasChanged();
    }
    private void GotoCategoria()
    {
        componentesCompra.DesactivarTodo();
        componentesCompra.Categoria = true;
        StateHasChanged();
    }
    private void GotoTipoTramiteComboMoto()
    {
        pasoActual = PasosCompraPin.TipoTramiteComboMoto;
        StateHasChanged();
    }
    private void GotoCategoriaComboMoto()
    {
        pasoActual = PasosCompraPin.CategoriaComboMoto;
        StateHasChanged();
    }
    private void GotoTipoTramiteSimple()
    {
        pasoActual = PasosCompraPin.TipoTramiteSimple;
        StateHasChanged();
    }
    private void GotoCategoriasSimple()
    {
        pasoActual = PasosCompraPin.CategoriasSimple;
        StateHasChanged();
    }
    private void GotoSeleccionCentro()
    {
        pasoActual = PasosCompraPin.SeleccionCentro;
        StateHasChanged();
    }
    private void GotoDatosPersonales()
    {
        componentesCompra.DesactivarTodo();
        if (pagoPin.EmisionOtraPersona)
        {
            componentesCompra.FacturaElectronica = true;
        }
        else
        {
            componentesCompra.DatosPersonales = true;
        }
        StateHasChanged();
    }

    private void GotoDatosFromFacturacion()
    {
        componentesCompra.DesactivarTodo();
        componentesCompra.DatosPersonales = true;
        StateHasChanged();
    }
    private void GotoMedioPago()
    {
        componentesCompra.DesactivarTodo();
        componentesCompra.MediosPago = true;
        StateHasChanged();
    }
    private void GotoCuotasCeas()
    {
        componentesCompra.DesactivarTodo();
        componentesCompra.CuotaCeas = true;
        StateHasChanged();
    }
    private void GotoConfirmarCompra()
    {
        pasoActual = PasosCompraPin.ConfirmarCompra;
        StateHasChanged();
    }

    private async Task ObtenerCategorias() 
    {
        if (pagoPin.ClienteCompra == (int)EnumTipoCliente.CRC)
        {
            if (pagoPin.CategoriasCrc == null)
            {
                pagoPin.CategoriasCrc = await MiLicenciaService.ObtenerCategoriasCrc();
            }
        }
    }

    private void AbrirConfirmacionIrInicio()
    {
        abrirModalIrInicio = true; 
    }

    private void ConfirmarIrInicio()
    {
        abrirModalIrInicio = false; // cierra el modal
        GotoCompraPin();            // ejecuta la navegación/acción real
    }

    // Inicio refactorización/optimización por GitHub Copilot
    protected async Task ActivoFacturacionElectronica()
    {
        try
        {
            long idRunt = getCentroResponse?.CodigoRUNT ?? 0;
            if (idRunt <= 0)
                return;

            var request = new ConsultaEstadoFacturacion { IdRunt = idRunt.ToString() };
            var response = await MiLicenciaService.ConsultarEstadoFacturacionElectronica(request);

            if (response?.SolicitudExitosa == true && response.Datos is not null)
            {
                pagoPin.FacturacionActiva = response.Datos.HabilitadoFacturaElectronica;

                if (!pagoPin.FacturacionActiva)
                    return;

                var responseConfiguracion = await MiLicenciaService.ConsultarConfiguracionFacturacionElectronica(request);
                if (responseConfiguracion?.SolicitudExitosa == true && responseConfiguracion.Datos is not null &&
                    System.Enum.IsDefined(typeof(EnumEventoFacturacion), responseConfiguracion.Datos.MomentoFacturacion))
                {
                    pagoPin.MomentoFacturacion = responseConfiguracion.Datos.MomentoFacturacion;
                }
            }
        }
        catch (Exception ex)
        {
        }
    }
    // Fin refactorización/optimización por GitHub Copilot


    #endregion

}
