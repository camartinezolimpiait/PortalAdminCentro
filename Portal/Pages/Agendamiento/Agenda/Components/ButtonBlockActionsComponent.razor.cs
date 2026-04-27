using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Agenda;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Agenda.Components
{
    public partial class ButtonBlockActionsComponent
    {
        [Inject]
        public IAgendaService _agendaService { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        DataInformation dataInformation { get; set; }

        [Parameter]
        public ToolButtonOptions ToolButtonOptions { get; set; }

        [Parameter]
        public EventCallback<ToolButtonOptions> ButtonToolActionOnClick { get; set; }

        [Parameter]
        public EventCallback<bool> BlockScheduleEvent { get; set; }

        private ApplicationShared applicationShared = new ApplicationShared();

        private bool isActiveForBlock { get; set; } = false;
        public bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            dataInformation.OnChange += StateHasChanged;
            isLoading = false;
        }
        public void Dispose()
        {
            dataInformation.OnChange -= StateHasChanged;
        }
        protected override async Task<Task> OnParametersSetAsync()
        {
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            isActiveForBlock = ToolButtonOptions.ShowBlockSchedule;
            return base.OnParametersSetAsync();
        }

        protected async Task ActionSheduleToolClick(MouseEventArgs e, bool isActive, AgendaMode mode)
        {
            isActiveForBlock = isActive;
            await ButtonToolActionOnClick.InvokeAsync(new ToolButtonOptions
            {
                AgendaMode = mode,
                ShowBlockSchedule = isActive
            });
        }

        protected async Task SaveBlockShedule()
        {
            isLoading = true;
            StateHasChanged();
            //var filteredToSave = applicationShared.CitasIntervaParaBloquear?.Where(x => !x.EsHoraAlmuerzo).ToList();
            var filteredToSave = dataInformation._CitasIntervalos?.Where(x => !x.EsHoraAlmuerzo).ToList();

            if (filteredToSave.Any())
            {
               
                int numIntervals = filteredToSave.Count;
                var filteredLockedIntervals = filteredToSave.Where(x => x.DiasIntervalo.Any(x => x.Cita.Any(ct => ct.IsChange
                                                                                                        && (ct.IdEstadoAgenda == 0 || ct.IdEstadoAgenda == 4))) && !x.EsHoraAlmuerzo)
                                                    .SelectMany(x => x.DiasIntervalo.SelectMany(x => x.Cita.Where(ct => ct.IsChange && (ct.IdEstadoAgenda == 0 || ct.IdEstadoAgenda == 4))
                                                                                    .ToList()))
                                                    .AsEnumerable()
                                                    .OrderBy(x => x.FechaAgenda)
                                                    .ToList();
                var daysWithBlocks = filteredLockedIntervals.GroupBy(x => x.FechaAgenda).Select(x => new { FechaAgenda = x.Key }).ToList();
                List<bool> savedList = new List<bool>();
                var scheduleList = new List<AgendaDTO>();
                var intervals = filteredToSave.Select(x => x.HoraIntervalo).ToList();

                daysWithBlocks.ForEach(day =>
                {
                    // Oscar Melgarejo: Hack to validate if the block is per schedule or day
                    var AllFilteredByDay = filteredLockedIntervals.Where(x => x.FechaAgenda == day.FechaAgenda);
                    if (numIntervals == AllFilteredByDay.Count())
                    {
                        var daySelected = AllFilteredByDay.FirstOrDefault();
                        //All day should be blocked
                        scheduleList.Add(new AgendaDTO
                        {
                            IdEstadoAgenda = (AllFilteredByDay.All(d => d.IsCheckedForBlock)) ? EstadoAgenda.DiaInhabilitado : EstadoAgenda.Activo,
                            IdHorarioAtencion = daySelected.IdHorarioAtencion,
                            IdPerfil = applicationShared.IdPerfil.Value,
                            FechaAgenda = day.FechaAgenda,
                            HorarioInicioAgenda = (short)daySelected.HoraInicioAgenda,
                            IdTipoCita = TipoCitaAgenda.Enrolamiento,
                            IdTipoAgendaCliente = TipoAgendaCliente.PortalAdministrativo,
                            UsuarioCreacion = applicationShared.UserName
                        });
                        // savedList.Add(result);
                    }
                    else
                    {
                        //Each schedule should be blocked

                        scheduleList.AddRange(filteredLockedIntervals.Where(x => x.FechaAgenda == day.FechaAgenda)?
                                                .ToList()
                                                .Select(x => new AgendaDTO
                                                {
                                                    IdEstadoAgenda = (x.IsCheckedForBlock) ? EstadoAgenda.Inhabilitado : EstadoAgenda.Activo,
                                                    IdHorarioAtencion = x.IdHorarioAtencion,
                                                    IdPerfil = applicationShared.IdPerfil.Value,
                                                    FechaAgenda = day.FechaAgenda,
                                                    HorarioInicioAgenda = (short)x.HoraInicioAgenda,
                                                    HorarioFinAgenda = 0,
                                                    IdTipoCita = TipoCitaAgenda.Enrolamiento,
                                                    IdTipoAgendaCliente = TipoAgendaCliente.PortalAdministrativo,
                                                    Intervalos = intervals,
                                                    UsuarioCreacion = applicationShared.UserName
                                                }));
                    }
                });
                
                if (scheduleList.Any())
                {
                    bool _result = await _agendaService.BlockScheduleList(scheduleList);
                    savedList.Add(_result);
                }

                if (savedList.Any(x => x))
                {
                    toastService.ShowSuccess(@"El bloqueo de horario ha sido guardado correctamente", "Bloqueo Horarios");
                    isActiveForBlock = false;
                    await BlockScheduleEvent.InvokeAsync(true);
                }
                else
                {
                    toastService.ShowWarning(@"No fue posible guardar el bloqueo de horarios", "Error");
                }
            }
            isLoading = false;
            StateHasChanged();
            applicationShared.CitasIntervaParaBloquear = new List<CitasIntervalo>();
        }
    }
}


