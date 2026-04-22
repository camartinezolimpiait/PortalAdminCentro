using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.Agendamiento;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Services.Agendamiento.Agenda;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Util.Helpers;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Agenda.Components
{
	public partial class ReScheduleConfirmationComponent
	{
		CultureInfo ci = new CultureInfo("es-ES");
		[Inject]
		private IToastService toastService { get; set; }
		[Inject]
		public IAgendaService _agendaService { get; set; }
        [Inject]
        public IMiLicenciaService MiLicenciaService { get; set; }
        [Parameter]
		public OptionAppoiment OptionAppoiment { get; set; }
        [Parameter]
        public AgendaDTO AgendaDTO { get; set; }

        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private ApplicationShared applicationShared = new ApplicationShared();

        private GetDataResponseCentro getCentroResponse = new();
        public List<TramiteDTO> Tramites { get; set; }

        [Parameter]
		public EventCallback<OptionAppoiment> ReScheduleAppoimentCallback { get; set; }
		public AgendaDTO AgendaDto { get; set; }
		public bool Confirmation { get; set; } = false;
		public bool isArmas = false;		

		protected override void OnParametersSet()
		{
			AgendaDto = OptionAppoiment.Agenda;
			isArmas = AgendaDto.IdTipoCliente == applicationShared.IdClienteArmas;
			Confirmation = (OptionAppoiment.ScheduleForm == ScheduleForms.ReScheduleConfirmation);
		}

		private string StringDate(DateTime fechaAgenda)
		{
			string result = string.Empty;
			if (AgendaDto != null)
			{
				string formattedDate = fechaAgenda.ToString("MMMM", ci);
                DayOfWeek diaSemana = fechaAgenda.DayOfWeek;
                int dia = (diaSemana == DayOfWeek.Sunday) ? 7 : (int)diaSemana;
                string day = ((FranjaHoraria)dia).ToString();
				result = string.Concat(day, " ", fechaAgenda.Day, " de ", char.ToUpper(formattedDate[0]) + formattedDate.Substring(1), ", ", OptionAppoiment.TimeString);
			}
			return result;
		}

		private async Task CancelReScheduleClick(MouseEventArgs e)
		{
			OptionAppoiment.ScheduleForm = ScheduleForms.ScheduleDetail;
			if (OptionAppoiment.ToolButtonOptions == null)
			{
				OptionAppoiment.ToolButtonOptions = new ToolButtonOptions
				{
					AgendaMode = AgendaMode.Schedule,
					ShowBlockSchedule = false
				};
				OptionAppoiment.IdAgenda = AgendaDto.IdAgenda;
			}
			OptionAppoiment.ToolButtonOptions.ShowBlockSchedule = false;
			await ReScheduleAppoimentCallback.InvokeAsync(OptionAppoiment);
		}

		private async Task ConfirmReScheduleClick(MouseEventArgs e)
		{
			try
			{
                var agenda = AgendaDTO;
                bool result = await _agendaService.ReSchedule(new AgendaDTO
				{
					IdAgenda = AgendaDto.IdAgenda,
					IdHorarioAtencion = OptionAppoiment.IdHorarioAtencion.Value,
					HorarioInicioAgenda = OptionAppoiment.ScheduleTime.Value,
					HorarioFinAgenda = 0,
					FechaAgenda = OptionAppoiment.ScheduleDay.Value,
					UsuarioCreacion = applicationShared.UserName,
				});

                
                if (result)
				{
                    this.sendRescheduleMail();
                    OptionAppoiment.ScheduleForm = ScheduleForms.None;
                    toastService.ShowSuccess(@"Se reprogramó la cita correctamente", "Reprogramar cita");
					await ReScheduleAppoimentCallback.InvokeAsync(OptionAppoiment);
				}
				else
				{
					toastService.ShowWarning(@"No fue posible reprogramar la cita", "Información");
				}
			}
			catch (Exception ex)
			{
				toastService.ShowError(@"No fue posible reprogramar la cita");
			}
		}

		private async Task CancelFormReScheduleClick(MouseEventArgs e)
		{
			OptionAppoiment.ScheduleForm = ScheduleForms.ReSchedule;
			await ReScheduleAppoimentCallback.InvokeAsync(OptionAppoiment);
		}

        private string FormatFecha(DateTime tiempo)
        {

            string fechaFormateada = tiempo.ToString("dddd dd 'de' MMMM, yyyy", new System.Globalization.CultureInfo("es-ES"));

            return fechaFormateada;
        }

        private string FormatHora(short hora)
        {
            DateTime fecha = new DateTime(2024, 1, 1, hora / 100, hora % 100, 0);
            return fecha.ToString("h:mm tt", new CultureInfo("en-US"));
        }

        //Se omite esta notificacion para evitar duplicidad
        private async void notificacionAgenda() 
		{
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            Tramites = await _agendaService.GetTramiteId(1);
            var notificacionCita = new RequestNotificacionCitas()
            {
                Aspirante = AgendaDto?.DatosPersonaDTO.Nombres + " " + AgendaDto?.DatosPersonaDTO.Apellidos,
                Correo = AgendaDto.DatosPersonaDTO.Email,
                NombreCentro = getCentroResponse.Nombre,
                Categoria = AgendaDto.Categoria,
                NumeroTelefono = AgendaDto?.DatosPersonaDTO.NumeroTelefono,
                Tramite = Tramites.Where(x => x.IdTramite == AgendaDto.IdTramite).Select(x => x.Nombre).FirstOrDefault(),
                Plataforma = result.Value.Plataforma,
                TipoCliente = "",
                FechaAgenda = FormatFecha(OptionAppoiment.ScheduleDay.Value),
                HoraAgenda = FormatHora(OptionAppoiment.ScheduleTime.Value)

            };
            notificacionCita.TipoCliente = "CENTRO";
            notificacionCita.Correo = getCentroResponse.Email;
            var resultCitas = await MiLicenciaService.ConstruirCorreoCitas(notificacionCita);
            notificacionCita.TipoCliente = "ASPIRANTE";
            notificacionCita.Correo = AgendaDto?.DatosPersonaDTO.Email;
            var resultCitasAsp = await MiLicenciaService.ConstruirCorreoCitas(notificacionCita);
        }

        protected override async Task OnInitializedAsync()
        {
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            applicationShared = result.Value;
        }

        //envio de correo aspirante
        private async Task sendRescheduleMail()
        {
            try
            {
                var datos = new RequestNotificationCancelation()
                {
                    AplicantName = AgendaDTO.DatosPersonaDTO.Nombres + " " + AgendaDTO.DatosPersonaDTO.Apellidos,
                    StatusAppointment = "reagendada",// agendada , cancelada, reprogramada
                    ShortDate = DateTimeTransformations.FormatShortDate(OptionAppoiment.ScheduleDay.Value),//  8 de abril
                    Hour = DateTimeTransformations.FormatHour(OptionAppoiment.ScheduleTime.Value.ToString()),// 9:30 am
                    Day = DateTimeTransformations.FormatDay(OptionAppoiment.ScheduleDay.Value),// Sabado 
                    Year = DateTimeTransformations.FormatYear(OptionAppoiment.ScheduleDay.Value),// 2025
                    CenterName = !string.IsNullOrEmpty(getCentroResponse.Nombre) ? getCentroResponse.Nombre : "No se pudo obtener",// nombre debe aceptar espacios, un guion, # , acentos, numeros,  
                    CenterAddress = !string.IsNullOrEmpty(getCentroResponse.Direccion) ? getCentroResponse.Direccion : "Sin dirección",//// nombre debe aceptar espacios, un guion, # , acentos, numeros,  
                    CenterPhone = !string.IsNullOrEmpty(getCentroResponse.Fijo) ? getCentroResponse.Fijo : "Sin registro",// // Solo numeros
                    Email = AgendaDTO.DatosPersonaDTO.Email
                };
                var result = await MiLicenciaService.SendRescheduleNotification(datos);
                toastService.ShowInfo(@"Notificación enviada exitosamente", "Información");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                toastService.ShowWarning(@"No se pudo enviar la notificación.", "Información");
            }
        }
    }
}
