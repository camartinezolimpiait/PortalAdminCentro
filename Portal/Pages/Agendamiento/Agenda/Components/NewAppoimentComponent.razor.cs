using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.Agendamiento;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Pages.CompraPin.Models;
using portalAdministrativoSISEC.Services.Agendamiento.Agenda;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Agenda.Components
{
	public partial class NewAppoimentComponent
	{
		[Inject]
		public NavigationManager Navigation { get; set; }
		[Inject]
		public IAgendaService _agendaService { get; set; }
		[Inject]
		private IToastService toastService { get; set; }
		[Inject]
		ProtectedSessionStorage ProtectedSessionStore { get; set; }

		[Inject]
		public IMiLicenciaService MiLicenciaService { get; set; }
		[Parameter]
		public EventCallback<bool> AppoimentSaved { get; set; }
		[Parameter]
		public OptionAppoiment OptionAppoiment { get; set; }
		[Parameter]
		public EventCallback<bool> CancelForm { get; set; }

		#region Variables

		public bool isLoading = true;

		private GetDataResponseCentro getCentroResponse = new();
		public List<TipoDocumentoDTO> TiposDeDocumento { get; set; }
		public List<TramiteDTO> Tramites { get; set; }
		public List<TipoCitaDTO> TiposCita { get; set; }
		public List<MotivoDTO> Motivos { get; set; }
		public List<CategoriaDTO> Categorias { get; set; }
		private AgendaNuevaCita dataAgenda = new AgendaNuevaCita();
		private string FormId = "newAppoimentForm";
		private List<string> CategoriesSelected { get; set; } = new List<string>();
		private ApplicationShared applicationShared = new ApplicationShared();
		public bool selectTi = false;
		public bool isArmas = false;
		public  List<TramiteDTO> TramiteTI { get; set; }
		public  List<CategoriaDTO> CategoriaTI { get; set; }

		#endregion

		protected override async Task OnInitializedAsync()
		{

			try
			{
				isLoading = true;
				var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
				var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
				getCentroResponse = centroShared.Value.Respuesta;
				applicationShared = result.Value;

				isArmas = applicationShared.Plataforma.Equals("Armas");

				dataAgenda.IdGenero = (short)Genero.Masculino;
				TiposDeDocumento = await _agendaService.GetTipoDocumento();
				TiposDeDocumento = filtrarTipoDocumento(TiposDeDocumento, applicationShared.Plataforma == EnumTipoCliente.CEA.ToString() ? 2 : 1);
				Tramites = await _agendaService.GetTramiteId(1);
				TiposCita = await _agendaService.GetTipoCita();
				Motivos = await _agendaService.GetMotivos();
				Categorias = await _agendaService.GetCategoriaId(1);
				//Enrolamiento
				dataAgenda.IdMotivo = 1;
				dataAgenda.IdTramite = 1;
				dataAgenda.IdTipoCita = 1;
				CategoriesSelected = new List<string>();
				isLoading = false;
			}
			catch (Exception ex)
			{
				toastService.ShowError(@"Se ha presentado un error inicializando el formulario, error: " + ex.Message);
			}
		}

		private async Task OnSubmitSaveSchedule()
		{
			try
			{
				isLoading = true;
				StateHasChanged();
				string validation = string.Empty;
				dataAgenda.Categoria = string.Join(",", CategoriesSelected);
				validation = await ValidationForm(dataAgenda);
				if (validation != string.Empty)
				{
					isLoading = false;
					StateHasChanged();
					toastService.ShowWarning(validation, "Información");
					return;
				}
				var scheduledPerson = await ScheduledPerson(applicationShared.IdPerfil.Value, asignarTipoDocumento(TiposDeDocumento, applicationShared.Plataforma == EnumTipoCliente.CEA.ToString() ? 2 : 1, dataAgenda.TipoDocumento), dataAgenda.NumeroDocumento, OptionAppoiment.ScheduleStartDate.Value);
				if (scheduledPerson != null && scheduledPerson.IdAgenda > 0)
				{
					isLoading = false;
					StateHasChanged();
					string message = string.Concat("El usuario con este Tipo de identificación y número ya tiene fecha agendada el dia ", scheduledPerson.FechaAgenda.ToResultadoBusqueda());
					toastService.ShowWarning(message, "Información");
					return;
				}

				if (dataAgenda.IdTipoCita == (int)TipoCitaAgenda.ContinuarProceso)
				{
					if (string.IsNullOrEmpty(dataAgenda.Observaciones))
					{
						isLoading = false;
						StateHasChanged();
						toastService.ShowWarning(@"Debe proporcionar las observaciones para continuar", "Información");
						return;
					}
				}
				var result = await _agendaService.SaveSchedule(new AgendaDTO
				{
					IdEstadoAgenda = EstadoAgenda.Activo,
					IdHorarioAtencion = OptionAppoiment.IdHorarioAtencion.Value,
					IdPerfil = applicationShared.IdPerfil.Value,
					FechaAgenda = OptionAppoiment.ScheduleDay.Value,
					HorarioInicioAgenda = OptionAppoiment.ScheduleTime.Value,
					HorarioFinAgenda = 0,
					IdTipoCita = (TipoCitaAgenda)dataAgenda.IdTipoCita,
					IdTipoAgendaCliente = TipoAgendaCliente.PortalAdministrativo,
					Categoria = dataAgenda.Categoria,
					IdTramite = (int)(dataAgenda.IdTramite ?? 1),
					IdMotivo = (short)(dataAgenda.IdMotivo != null? dataAgenda.IdMotivo : 1),
					UsuarioCreacion = applicationShared.UserName,
					Observaciones = dataAgenda.Observaciones ?? "",
					DatosPersonaDTO = new DatosPersonaDTO
					{
						Nombres = dataAgenda.Nombres,
						Apellidos = dataAgenda.Apellidos,
						Email = dataAgenda.Correo,
						NumeroTelefono = dataAgenda.Telefono,
						IdGenero = dataAgenda.IdGenero,
						IdTipoIdentificacion = asignarTipoDocumento(TiposDeDocumento, applicationShared.Plataforma == EnumTipoCliente.CEA.ToString() ? 2 : 1, dataAgenda.TipoDocumento),
						NumeroIdentificacion = dataAgenda.NumeroDocumento,
					}
				});

				isLoading = false;
				StateHasChanged();
				if (result)
				{
					this.notificacionAgenda();
					toastService.ShowSuccess(@"Se agendó la cita correctamente", "Nueva cita");
					await AppoimentSaved.InvokeAsync(result);
				}
				else
				{
					toastService.ShowWarning(@"No se pudo agendar la cita correctamente, por favor verifique los datos nuevamente", "Información");
				}
			}
			catch (Exception ex)
			{
				isLoading = false;
				toastService.ShowWarning(@"No se pudo agendar la cita correctamente, por favor verifique los datos nuevamente", "Información");
			}
			
		}

		private async Task<string> ValidationForm(AgendaNuevaCita dataAgenda)
		{

			string message = string.Empty;
			if (dataAgenda.Nombres == null || dataAgenda.Nombres.Trim() == "")
			{
				message = "Complete el campo Nombre";
				return await Task.FromResult(message);
			}
			if (!Regex.IsMatch(dataAgenda.Nombres, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
			{
				message = "Nombres no válidos. Solo se permiten letras y espacios.";
				return await Task.FromResult(message);
			}
			if (dataAgenda.Apellidos == null || dataAgenda.Apellidos.Trim() == "")
			{
				message = "Complete el campo Apellido";
				return await Task.FromResult(message);
			}


			if (!Regex.IsMatch(dataAgenda.Apellidos, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
			{
				message = "Nombres no válidos. Solo se permiten letras y espacios.";
				return await Task.FromResult(message);
			}
			if (dataAgenda.TipoDocumento == 0)
			{
				message = "Seleccione un tipo de documento";
				return await Task.FromResult(message);
			}
			if (dataAgenda.NumeroDocumento == null || dataAgenda.NumeroDocumento.Trim() == "")
			{
				message = "Complete el campo  número de documento";
				return await Task.FromResult(message);
			}
			if (!isArmas && dataAgenda.IdTramite == 0)
			{
				message = "Complete el campo tipo tramite";
				return await Task.FromResult(message);
			}
			if (!isArmas && (dataAgenda.Categoria == null || dataAgenda.Categoria.Trim() == ""))
			{
				message = "Seleccione mínimo una categoria";
				return await Task.FromResult(message);
			}
			if (dataAgenda.Telefono == null || dataAgenda.Telefono.Trim() == "")
			{
				message = "Complete el campo teléfono";
				return await Task.FromResult(message);
			}
			if (dataAgenda.Correo == null || dataAgenda.Correo.Trim() == "")
			{
				message = "Complete el campo correo";
				return await Task.FromResult(message);
			}
			return await Task.FromResult(message);
		}

		private async Task CancelFormClick()
		{
			await CancelForm.InvokeAsync(false);
		}
        /// <summary>
        ///  // Crear una nueva lista para almacenar la categoría seleccionada
        /// </summary>
        /// <param name="args"></param>
        /// <param name="category"></param>
        private void CategoryOnChange(ChangeEventArgs args, string category)
		{
			CategoriesSelected = new List<string>();

			if ((bool)args.Value){
				CategoriesSelected.Add(category);
			}
			else CategoriesSelected.Remove(category);
			CategoriesSelected = CategoriesSelected ?? new List<string>();

		}

        private void GenreOnChange(ChangeEventArgs args)
		{
			dataAgenda.IdGenero = int.Parse(args.Value.ToString());
		}

		private async Task<AgendaDTO> ScheduledPerson(int idPerfil, int idType, string idNumber, DateTime startSearchDate)
		{
			return await _agendaService.CheckScheduleAvailableByDatosPersonaId(idPerfil, idType, idNumber, startSearchDate.ToString("ddMMyyyy"));
		}

		private string FormatFecha(DateTime tiempo)
		{
			string fechaFormateada = tiempo.ToString("dddd dd 'de' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES"));
			return fechaFormateada;
		}

		private string FormatHora(short hora)
		{
			int currentYear = DateTime.Now.Year;
            DateTime fecha = new DateTime(currentYear, 1, 1, hora / 100, hora % 100, 0);
			return fecha.ToString("h:mm tt", new System.Globalization.CultureInfo("en-US"));
		}

        /// <summary>
        /// Método para construir la notificación con parámetros dinámicos
        /// </summary>
        /// <param name="tipoCliente"></param>
        /// <param name="correo"></param>
        /// <returns></returns>
        RequestNotificacionCitas ConstruirNotificacionCita(string tipoCliente, string correo)
        {
            return new RequestNotificacionCitas
            {
                Aspirante = (dataAgenda.Nombres + " " + dataAgenda.Apellidos).Length > 80
							? (dataAgenda.Nombres + " " + dataAgenda.Apellidos).Substring(0, 50) + "..."
							: (dataAgenda.Nombres + " " + dataAgenda.Apellidos),
                Correo = correo,
                NombreCentro = getCentroResponse.Nombre,
                Categoria = dataAgenda.Categoria,
                NumeroTelefono = dataAgenda.Telefono,
                Tramite = Tramites.Where(x => x.IdTramite == dataAgenda.IdTramite).Select(x => x.Nombre).FirstOrDefault(),
                Plataforma = applicationShared.Plataforma,
                TipoCliente = tipoCliente,
                FechaAgenda = FormatFecha(OptionAppoiment.ScheduleDay.Value),
                HoraAgenda = FormatHora(OptionAppoiment.ScheduleTime.Value)
            };
        }

        /// <summary>
        /// // Construir y enviar notificaciones
        /// </summary>
        private async void notificacionAgenda()
		{
            var notificacionCentro = ConstruirNotificacionCita("CENTRO", getCentroResponse.Email);
            var resultCitas = await MiLicenciaService.ConstruirCorreoCitas(notificacionCentro);

            var notificacionAspirante = ConstruirNotificacionCita("ASPIRANTE", dataAgenda.Correo);
            var resultCitasAsp = await MiLicenciaService.ConstruirCorreoCitas(notificacionAspirante);
        }
        

        private List<TipoDocumentoDTO> filtrarTipoDocumento(List<TipoDocumentoDTO> filtroDocumento, int tipoCliente)
		{
			List<TipoDocumentoDTO> documentos = new List<TipoDocumentoDTO>();
			if (filtroDocumento.Count() > 0)
			{
				var codigosValidos = new[] { 1, 2, 3, 5, 13 };
				switch (tipoCliente)
				{
					case 1:
						documentos = filtroDocumento.Where(doc => doc.ClienteId == tipoCliente && doc.CodigoEquCRC.HasValue && codigosValidos.Contains(doc.CodigoEquCRC.Value)).Select(doc => new TipoDocumentoDTO
						{
							abreviatura = doc.abreviatura,
							ClienteId = doc.ClienteId,
							Codigo = doc.Codigo,
							CodigoEquCEA = doc.CodigoEquCEA,
							CodigoEquCRC = doc.CodigoEquCRC,
							descripcion = doc.descripcion,
							Id = doc.Id
						}).ToList();

						break;
					case 2:

						documentos = filtroDocumento.Where(doc => doc.ClienteId == tipoCliente && doc.CodigoEquCEA.HasValue && codigosValidos.Contains(doc.CodigoEquCEA.Value)).Select(doc => new TipoDocumentoDTO
						{
							abreviatura = doc.abreviatura,
							ClienteId = doc.ClienteId,
							Codigo = doc.Codigo,
							CodigoEquCEA = doc.CodigoEquCEA,
							CodigoEquCRC = doc.CodigoEquCRC,
							descripcion = doc.descripcion,
							Id = doc.Id
						}).ToList();

						break;
				}

			}
			return documentos;

		}

		private async void selectDocumento(ChangeEventArgs evento) 
		{

			var valorSeleccionado = evento.Value.ToString();
			var CategoriasValidas = new[] { "A1", "A2", "B1" };
			if (valorSeleccionado != null) 
			{
				dataAgenda.TipoDocumento = int.Parse(TiposDeDocumento.Where(doc => doc.Id == valorSeleccionado).Select(doc => doc.Id).FirstOrDefault());
				string selected = TiposDeDocumento.Where(doc => doc.Id == valorSeleccionado).Select(doc => doc.abreviatura).FirstOrDefault();

				if (selected == "TI")
				{
					CategoriesSelected = new List<string>();
					dataAgenda.IdTramite = null;
					TramiteTI = null;
					selectTi = true;
					TramiteTI = Tramites.Where(x => x.Nombre == "Primera Vez").Select(x => new TramiteDTO
					{
						Activo = x.Activo,
						Nombre = x.Nombre,
						IdTipoCliente = x.IdTipoCliente,
						IdTramite = x.IdTramite,
						IdTramiteCliente = x.IdTramiteCliente,
					}).ToList();

					CategoriaTI = Categorias.Where(x => CategoriasValidas.Contains(x.Codigo)).Select(x => new CategoriaDTO
					{
						Id = x.Id,
						Nombre = x.Nombre,
						Codigo = x.Codigo,
						ClienteId = x.ClienteId,
						Activo = x.Activo,
						GrupoId = x.GrupoId

					}).ToList();


						

				}
				else 
				{
					selectTi = false;
					dataAgenda.TipoDocumento =int.Parse(TiposDeDocumento.Where(doc => doc.Id == valorSeleccionado).Select(doc => doc.Id).FirstOrDefault());
				}
			}

		}

		public int asignarTipoDocumento(List<TipoDocumentoDTO> filtroDocumento, int tipoCliente, int Iddocument)
		{
			List<TipoDocumentoDTO> documentos = new List<TipoDocumentoDTO>();

			int codigoDocumento = 0;
			int Id = 0;
				switch(tipoCliente)
				{
				case 1:
				 codigoDocumento = (int)filtroDocumento.Where(doc => doc.Id == Iddocument.ToString()).Select(doc => doc.CodigoEquCRC).FirstOrDefault();
					var IdFiltradoCRC = filtroDocumento.Where(doc => doc.CodigoEquCRC.ToString() == codigoDocumento.ToString()).Select(doc => doc.Id).FirstOrDefault();
					Id = int.Parse(IdFiltradoCRC);
					break;
				case 2:
				 codigoDocumento=(int)filtroDocumento.Where(doc => doc.Id == Iddocument.ToString()).Select(doc => doc.CodigoEquCEA).FirstOrDefault();
					var IdFiltradoCEA = filtroDocumento.Where(doc => doc.CodigoEquCEA.ToString() == codigoDocumento.ToString()).Select(doc => doc.Id).FirstOrDefault();
					Id = int.Parse(IdFiltradoCEA);
					break;
				default:
					throw new ArgumentException("tipoCliente no es válido", nameof(tipoCliente));
				}
			return Id;
		}

		public string convertirHoraEstandar(string Hora) 
		{
			string tiempo = "";
			if (int.TryParse(Hora.Replace(":", ""), out int h) && h >= 0 && h <= 2359)
			{
				int horas = h / 100;
				int minutos = h % 100;
				string formato = horas >= 12 ? "PM" : "AM";
				horas = horas > 12 ? horas - 12 : horas;
				horas = horas == 0 ? 12 : horas;
				tiempo= $"{horas:D2}:{minutos:D2} {formato}";
			}
			return tiempo;
		}
		private async void OnChangeTramite(ChangeEventArgs evento)
		{
			var result = evento.Value;
		}
	}
}
