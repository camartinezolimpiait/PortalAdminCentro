using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Enum;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Agenda.Components
{
    public partial class ReScheduleAppoimentComponent
    {
        [Inject]
        private IToastService toastService { get; set; }
        [Parameter]
        public OptionAppoiment OptionAppoiment { get; set; }
        [Parameter]
        public List<CitasIntervalo> CitasIntervaloListado { get; set; }

        [Parameter]
        public EventCallback<OptionAppoiment> NewAppoimentCallback { get; set; }

        [Parameter]
        public DateTime StartDate { get; set; }

        protected async Task NewAppoiment(MouseEventArgs e, DateTime scheduleDay, short scheduleTime, string timeString, long IdHorarioAtencion)
        {
            if (OptionAppoiment.Agenda.FechaAgenda == scheduleDay && scheduleTime == OptionAppoiment.Agenda.HorarioInicioAgenda)
            {
                toastService.ShowWarning("Debe seleccionar una fecha y hora diferente a la original", "Información");
                return;
            }
            await NewAppoimentCallback.InvokeAsync(new OptionAppoiment
            {
                ScheduleDay = scheduleDay,
                ScheduleTime = scheduleTime,
                TimeString = timeString,
                IdHorarioAtencion = IdHorarioAtencion,
                ScheduleForm = ScheduleForms.ReScheduleConfirmation,
                Agenda = OptionAppoiment.Agenda
            });
        }


    }
}
