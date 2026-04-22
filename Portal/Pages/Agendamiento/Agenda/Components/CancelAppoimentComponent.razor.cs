using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class CancelAppoimentComponent
    {
        [Inject]
        public IAgendaService _agendaService { get; set; }
        [Inject]
        public IMiLicenciaService _miLicenciaService { get; set; }
        [Parameter]
        public OptionAppoiment OptionAppoiment { get; set; }
        [Parameter]
        public AgendaDTO AgendaDTO { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Parameter]
        public EventCallback<OptionAppoiment> CancelScheduleCallback { get; set; }
        private ApplicationShared applicationShared = new ApplicationShared();
        private GetDataResponseCentro getCentroResponse = new();

        protected override async Task OnInitializedAsync()
        {
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            applicationShared = result.Value;
        }
        private async Task CancelScheduleClick(MouseEventArgs e)
        {
            try
            {
                var agenda = AgendaDTO;
                bool result = await _agendaService.CancelSchedule(OptionAppoiment.IdAgenda.Value, applicationShared.UserName);

                if (result)
                {
                    OptionAppoiment.ScheduleForm = ScheduleForms.None;
                    toastService.ShowInfo(@"Se canceló la cita correctamente", "Cancelar cita");
                    await SendCancelationMail();
                    // Incluir el envio del correo 
                    await CancelScheduleCallback.InvokeAsync(OptionAppoiment);
                }
                else
                {
                    toastService.ShowWarning(@"No se pudo cancelar la cita por favor intentelo nuevamente mas tarde", "Información");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task CancelForm(MouseEventArgs e)
        {
            OptionAppoiment.ScheduleForm = ScheduleForms.ScheduleDetail;
            await CancelScheduleCallback.InvokeAsync(OptionAppoiment);
        }
        private async Task SendCancelationMail()
        {
            try
            {
                var datos = new RequestNotificationCancelation()
                {
                    AplicantName = AgendaDTO.DatosPersonaDTO.Nombres + " " + AgendaDTO.DatosPersonaDTO.Apellidos,
                    StatusAppointment = "cancelada",// agendada , cancelada, reprogramada
                    ShortDate = DateTimeTransformations.FormatShortDate(AgendaDTO.FechaAgenda),//  8 de abril
                    Hour = DateTimeTransformations.FormatHour(AgendaDTO.HorarioInicioAgenda.ToString()),// 9:30 am
                    Day = DateTimeTransformations.FormatDay(AgendaDTO.FechaAgenda),// Sabado 
                    Year = DateTimeTransformations.FormatYear(AgendaDTO.FechaAgenda),// 2025
                    CenterName = !string.IsNullOrEmpty(getCentroResponse.Nombre) ? getCentroResponse.Nombre : "No se pudo obtener",// nombre debe aceptar espacios, un guion, # , acentos, numeros,  
                    CenterAddress = !string.IsNullOrEmpty(getCentroResponse.Direccion) ? getCentroResponse.Direccion : "Sin dirección",//// nombre debe aceptar espacios, un guion, # , acentos, numeros,  
					CenterPhone = !string.IsNullOrEmpty(getCentroResponse.Fijo) ? getCentroResponse.Fijo : "Sin registro",// // Solo numeros
                    Email=AgendaDTO.DatosPersonaDTO.Email
                };
                var result = await _miLicenciaService.SendCancelationNotification(datos);
            }
            catch (Exception ex)
            {

                toastService.ShowWarning(@"No se pudo enviar la notificación.", "Información");

            }

        }
    }
}
