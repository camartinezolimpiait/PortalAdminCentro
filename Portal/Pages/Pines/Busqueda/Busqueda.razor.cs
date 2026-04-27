using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Entidades.Pago.Wompi;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data.Pines;
using System.Linq;
using System.Globalization;
using OfficeOpenXml;
using System.IO;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using portalAdministrativoSISEC.Entidades.Common;

namespace portalAdministrativoSISEC.Pages.Pines.Busqueda
{
	public partial class Busqueda
	{
		#region Inyeccion Dependencias

		[Inject]
		public IMiLicenciaService MiLicenciaService { get; set; }

		[Inject]
		private ProtectedSessionStorage ProtectedSessionStore { get; set; }

		[Inject]
		private IJSRuntime JS { get; set; }

		#endregion Inyeccion Dependencias

		#region Variables

		public bool isLoading = true;

		public ConsultaInfoPinEstado busquedaPines = new();
		public List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = new();
		public List<AgenteDispersionResponseDTO> ListaAgenteDispersion { get; set; } = new();
		public List<Centro> ListaCentros { get; set; } = new();
		public List<CanalVentaResponseDTO> ListaCanalVenta { get; set; } = new();
		private GetDataResponseCentro getCentroResponse = new GetDataResponseCentro();
		private ApplicationSevice menuService = new ApplicationSevice();
		private bool camposDeshabilitados = false;
		private DateTime fechaActual = DateTime.Now;
		private string fechaFormateada { get; set; } = "";
		private string fechaAyerFormateada { get; set; } = "";
		private EnumTipoInput variableTipoDocumento { get; set; } = EnumTipoInput.NumerosYLetras;

		[Parameter]
		public int tipoBusqueda { get; set; }

		[Parameter]
		public ConsultaInfoPinEstado DatosFiltros { get; set; }

		[Parameter]
		public EventCallback<ConsultaInfoPinEstado> DatosFiltrosChanged { get; set; }

		[Parameter]
		public EventCallback<ConsultaDescarga> DescargarDatos { get; set; }

		#endregion Variables

		#region Methods

		protected override async Task OnInitializedAsync()
		{
			isLoading = true;
			ObtenerFechaActual();
			var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
			getCentroResponse = centroShared.Value.Respuesta;
			await ObtenerCombosBusqueda();
			isLoading = false;
		}

		public async Task ConsultarPines(ConsultaInfoPinEstado datos)
		{
			bool isValid = true;
			if (tipoBusqueda == (int)EnumTipoFormBusqueda.BusquedaDevoluciones)
			{
				isValid = await ValidacionFiltrosConsultaDevoluciones();
			}
			else
			{
				isValid = await ValidacionFiltrosConsulta();
			}

			if (isValid)
			{
				await ActualizaDatosPin(datos);
			}
		}

		public async Task DescargarArchivo(bool isDownload, ConsultaInfoPinEstado consulta)
		{
			if (consulta.FechaFinal == null || consulta.FechaInicial == null)
			{
                //consulta = DatosFiltros;
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "Debe incluir fecha inicial y fecha final para poder realizar la descarga.");
			}
			else
			{
                TimeSpan? diferencia = consulta.FechaFinal - consulta.FechaInicial;

                if (diferencia.Value.TotalDays < 31)
                {
					
                    await DescargarDatos.InvokeAsync(new ConsultaDescarga() { IsDownload=isDownload,Consulta=consulta});
                }
                else
                {
                    await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "El rango de fechas de descarga no puede ser mayor a 30 días");
                }
            }
		}

		private async Task ObtenerCombosBusqueda()
		{
			ListaDocumentos = await MiLicenciaService.ObtenerTipoDocumentos();
            ListaDocumentos = FilterDocumentsAviable.FilterDocumentsTypes(ListaDocumentos);
            ProtectedBrowserStorageResult<ApplicationSevice> menuServiceShared = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");
			ConsultaCentroPorComercio consultaCentro = new ConsultaCentroPorComercio() { IdComercio = getCentroResponse.IdComercio, ConsultaCentro = new ConsultaCentroPorId() { Plataforma = menuServiceShared.Value.Plataforma, Id = 0 } };
			ListaCentros = await MiLicenciaService.ObtenerTodosCentrosxComercio(consultaCentro);

			ListaAgenteDispersion = [
				new() { IdAgenteDispersion = 1, Nombre = "Bancolombia" },
                new() { IdAgenteDispersion = 3, Nombre = "Colpatria" },
            ];

			ListaCanalVenta = [
				new() { Id = (int)EnumOrigenCotizacion.Centro, Nombre = PagoPinConst.DescripcionCanalVenta[EnumOrigenCotizacion.Centro] },
				new() { Id = (int)EnumOrigenCotizacion.MiLicencia, Nombre = PagoPinConst.DescripcionCanalVenta[EnumOrigenCotizacion.MiLicencia] },
				new() { Id = (int)EnumOrigenCotizacion.PAO, Nombre = PagoPinConst.DescripcionCanalVenta[EnumOrigenCotizacion.PAO] },
                new() { Id = (int)EnumOrigenCotizacion.PortalAdministrativo, Nombre = PagoPinConst.DescripcionCanalVenta[EnumOrigenCotizacion.PortalAdministrativo] }
            ];
		}

		private async Task ActualizaDatosPin(ConsultaInfoPinEstado datos)
		{
			datos.Opcion = 1;
			await DatosFiltrosChanged.InvokeAsync(datos);
		}

		private async Task<bool> ValidacionFiltrosConsulta()
		{
			bool valido = await ValidacionFiltrosVacios();
			if (valido)
			{
				int tipoFiltro = await AsignaTipoFiltroConsulta();
				switch (tipoFiltro)
				{
					case (int)EnumTipoFiltro.FecIniFecFin:
						valido = await ValidacionFiltrosFechas();
						break;

					case (int)EnumTipoFiltro.FecIniFecFinCentro:
						valido = await ValidacionFiltrosFechas();
						break;

					case (int)EnumTipoFiltro.FecIniFecFinCanal:
						valido = await ValidacionFiltrosFechas();
						break;

					case (int)EnumTipoFiltro.TipoDocNumDoc:
						valido = await ValidacionTipoDocumento();
						break;

					case (int)EnumTipoFiltro.NumDoc:
						valido = true;
						break;

					case (int)EnumTipoFiltro.Pin:
						valido = true;
						break;

					default:
						await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay filtros válidos para la consulta");
						valido = false;
						break;
				}
			}

			return valido;
		}

		private async Task<bool> ValidacionFiltrosVacios()
		{
			// Verificar si al menos uno de los campos no es nulo o vacío
			if (!(
				string.IsNullOrEmpty(busquedaPines.FechaInicial?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines.FechaFinal?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines.IdCentro?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines.Canal?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines.IdTipoDocumento?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines.Documento?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines.Pin?.ToString())
			))
			{
				return true;
			}
			await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay filtros para la consulta");
			return false;
		}

		private async Task<bool> ValidacionFiltrosFechas()
		{
			bool valido = true;
			if (!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) || !string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					if (!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()))
					{
						if (busquedaPines?.FechaInicial > busquedaPines?.FechaFinal)
						{
							await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "La fecha inicial no puede ser mayor a la fecha final");
							valido = false;
						}
						TimeSpan? diferencia = busquedaPines?.FechaFinal - busquedaPines?.FechaInicial;

						if (diferencia.Value.TotalDays > 90)
						{
							await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "El rango de fechas no puede ser mayor a 90 días");
							valido = false;
						}
					}
					else
					{
						await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "La fecha inicial no es válida");
						valido = false;
					}
				}
				else
				{
					await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "La fecha final no es válida");
					valido = false;
				}
			}
			else
			{
				await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "La fecha inicial no es válida");
				valido = false;
			}
			return valido;
		}

		private async Task<int> AsignaTipoFiltroConsulta()
		{
			int tipoFiltro = (int)EnumTipoFiltro.NoValido;
			if (!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) || !string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.IdTipoDocumento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltro.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltro.FecIniFecFin;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.IdCentro?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.IdTipoDocumento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltro.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltro.FecIniFecFinCentro;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.Canal?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.IdTipoDocumento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltro.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltro.FecIniFecFinCanal;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.IdTipoDocumento?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.Canal?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.IdCentro?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltro.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltro.TipoDocNumDoc;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.Canal?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.IdCentro?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltro.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltro.NumDoc;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines.Pin?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.Canal?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.IdCentro?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.IdTipoDocumento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltro.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltro.Pin;
				}
			}
			else
			{
				await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay filtros válidos para la consulta");
			}

			return tipoFiltro;
		}

		private async Task<bool> ValidacionTipoDocumento()
		{
			if (!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()))
			{
				return true;
			}
			else
			{
				await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "Debe diligenciar el número de documento");
				return false;
			}
		}

		private async Task<bool> ValidacionFiltrosConsultaDevoluciones()
		{
			bool valido = await ValidacionFiltrosVaciosDevoluciones();
			if (valido)
			{
				int tipoFiltro = await AsignaTipoFiltroConsultaDevoluciones();
				switch (tipoFiltro)
				{
					case (int)EnumTipoFiltroDevoluciones.FecIniFecFin:
						valido = await ValidacionFiltrosFechas();
						break;

					case (int)EnumTipoFiltroDevoluciones.FecIniFecFinCanal:
						valido = await ValidacionFiltrosFechas();
						break;

					case (int)EnumTipoFiltroDevoluciones.FecIniFecFinCanalNumDoc:
						valido = await ValidacionFiltrosFechas();
						break;

					case (int)EnumTipoFiltroDevoluciones.FecIniFecFinCanalNumDocPin:
						valido = await ValidacionTipoDocumento();
						break;

					case (int)EnumTipoFiltro.NumDoc:
						valido = true;
						break;

					case (int)EnumTipoFiltro.Pin:
						valido = true;
						break;

					default:
						await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay filtros válidos para la consulta");
						valido = false;
						break;
				}
			}

			return valido;
		}

		private async Task<bool> ValidacionFiltrosVaciosDevoluciones()
		{
			// Verificar si al menos uno de los campos no es nulo o vacío
			if (!(
				string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines?.IdAgenteDispersion?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines?.Canal?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()) &&
				string.IsNullOrEmpty(busquedaPines?.Pin?.ToString())
			))
			{
				return true;
			}
			await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No hay filtros para la consulta");
			return false;
		}

		private async Task<int> AsignaTipoFiltroConsultaDevoluciones()
		{
			int tipoFiltro = (int)EnumTipoFiltroDevoluciones.NoValido;
			if (!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) || !string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
			{
				tipoFiltro = (int)EnumTipoFiltroDevoluciones.FecIniFecFin;
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.Canal?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltroDevoluciones.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltroDevoluciones.FecIniFecFinCanal;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.IdAgenteDispersion?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltroDevoluciones.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltroDevoluciones.NumDoc;
				}
			}
			else if (!string.IsNullOrEmpty(busquedaPines?.Pin?.ToString()))
			{
				if (!string.IsNullOrEmpty(busquedaPines?.IdAgenteDispersion?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.Documento?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaInicial?.ToString()) ||
					!string.IsNullOrEmpty(busquedaPines?.FechaFinal?.ToString()))
				{
					tipoFiltro = (int)EnumTipoFiltroDevoluciones.NoValido;
				}
				else
				{
					tipoFiltro = (int)EnumTipoFiltroDevoluciones.Pin;
				}
			}

			return tipoFiltro;
		}

		private void ActualizarCampos()
		{
			if (busquedaPines?.Canal == (int)EnumOrigenCotizacion.MiLicencia || busquedaPines?.Canal == (int)EnumOrigenCotizacion.PAO)
			{
				camposDeshabilitados = true;
				busquedaPines.Documento = "";
			}
			else
			{
				camposDeshabilitados = false;
			}
		}

		private void ObtenerFechaActual()
		{
			fechaFormateada = fechaActual.ToString("yyyy-MM-dd");
			fechaAyerFormateada = fechaActual.AddDays(-1).ToString("yyyy-MM-dd");
		}

		/// <summary>
		/// Metodo para filtrar el tipo de input onkeypress
		/// </summary>
		/// <param name="e"></param>
		/// <param name="inputType"></param>
		/// <returns></returns>
		private async Task FiltrarInput(KeyboardEventArgs e, EnumTipoInput inputType)
		{
			// Lista de teclas especiales siempre permitidas
			// Lista de teclas especiales permitidas
			var allowedSpecialKeys = new[] {
				"Backspace",  // Para borrar caracteres
				"Delete",     // Para eliminar caracteres
				"ArrowLeft",  // Para mover el cursor a la izquierda
				"ArrowRight", // Para mover el cursor a la derecha
				"Home",       // Para mover el cursor al inicio del campo
				"End",        // Para mover el cursor al final del campo
				"Tab",        // Para navegar entre campos
				"Enter",      // Para confirmar la entrada (si es necesario)
				"Escape"  };
			bool isAllowed = allowedSpecialKeys.Contains(e.Key);

			if (!isAllowed && e.Key.Length == 1)
			{
				switch (inputType)
				{
					case EnumTipoInput.SoloNumeros:
						isAllowed = char.IsDigit(e.Key[0]);
						break;

					case EnumTipoInput.NumerosYLetras:
						isAllowed = char.IsLetterOrDigit(e.Key[0]);
						break;

					case EnumTipoInput.SoloLetras:
						isAllowed = char.IsLetter(e.Key[0]);
						break;
						// Puedes agregar más casos según sea necesario
				}
			}

			if (!isAllowed)
			{
				await JS.InvokeVoidAsync("preventDefaultKeyPress", e.Key);
			}
		}

		/// <summary>
		/// Metodo para onchange tipo de documento para incluir validaciones segun tipo de documento
		/// </summary>
		/// <param name="e"></param>
		private void CambiarTipoDocumento(ChangeEventArgs e)
		{
			if (int.TryParse(e.Value.ToString(), out int idTipoDocumento))
			{
				busquedaPines.IdTipoDocumento = idTipoDocumento;

				// Aquí defines la lógica para cambiar el tipo de input según el tipo de documento
				switch (idTipoDocumento)
				{
					case (int)EnumTipoDocumento.Pasaporte:
					case (int)EnumTipoDocumento.PermisoPorProteccionTemporal:
						variableTipoDocumento = EnumTipoInput.NumerosYLetras;
						busquedaPines.Documento = "";
						break;
					// Caso tipo de documento CC CE TI
					default:
						variableTipoDocumento = EnumTipoInput.SoloNumeros;
						busquedaPines.Documento = "";
						break;
				}
			}
		}

		#endregion Methods
	}
}

