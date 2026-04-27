using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Horario;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.HorarioAtencion;

namespace portalAdministrativoSISEC.Pages.Agendamiento
{
    public partial class ConfigurarParametrizacion
    {
        #region Inyeccion Dependencias

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        private IHorarioAtencionService HorarioAtencionService { get; set; }

        [Inject]
        private IParametrizacionHorarioService ParametrizacionHorarioService { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        #endregion Inyeccion Dependencias

        #region Variables

        private bool IsLoading { get; set; } = true;
        private bool GetNewSchedule { get; set; } = false;
        private bool ReadOnly { get; set; } = true;
        private bool ExistFutureAppointment { get; set; } = false;
        private bool IsActual { get; set; } = false;
        private string FechaParametrizacion { get; set; } = string.Empty;
        private ApplicationShared ApplicationShared { get; set; } = new();
        private ParametrizacionHorario ParametrizacionHorario { get; set; } = new();
        private List<ConfiguracionHorario> ConfiguracionHorarios { get; set; } = [];

        #endregion Variables

        #region Metodos

        protected override async Task OnInitializedAsync() => await Task.FromResult(true);

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;

            try
            {
                var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(ApplicationShared.NameLocalStorage);
                ApplicationShared = result.Value;
                ApplicationShared.CreateNewConfiguration = false;
                ConfiguracionHorarios = await GetConfigurationSchedule();

                if (ConfiguracionHorarios!=null && ConfiguracionHorarios.Count != 0)
                    ApplicationShared.IsAgendaFutura = ConfiguracionHorarios[0].TotalParametrizacionAgenda >= 2;
            }
            catch (ApplicationException ae)
            {
                ToastService.ShowWarning(ae.Message.ToString(), "Información");
            }
            catch (Exception ex)
            {
                ToastService.ShowWarning(ex.Message.ToString(), "Información");
            }
            IsLoading = false;
            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Realiza el cambio entre la configuracion actual y la futura
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task ConfigurationOnChange(ChangeEventArgs args)
        {
            int opcion = int.Parse(args.Value.ToString());
            if (opcion == 1)
            {
                IsActual = true;
                GetNewSchedule = false;
                await GetConfigurationSchedule();
                IsActual = false;
            }
            else
            {
                IsActual = false;
                GetNewSchedule = true;
                await GetConfigurationSchedule();
                IsActual = true;
            }
        }

        /// <summary>
        /// Obtiene la configuracion de la agenda
        /// </summary>
        /// <returns></returns>
        private async Task<List<ConfiguracionHorario>> GetConfigurationSchedule()
        {
            DateTime startDate = DateTime.Now;
            ConfiguracionHorarios = await HorarioAtencionService.GetConfiguracionHorario(ApplicationShared.IdPerfil.Value, startDate, GetNewSchedule);
            if (ConfiguracionHorarios != null && ConfiguracionHorarios.Count != 0)
            {
                ApplicationShared.IsAgendaFutura = ConfiguracionHorarios[0].TotalParametrizacionAgenda >= 2;
                ExistFutureAppointment = ConfiguracionHorarios[0].TotalFuturasAgenda > 0;
                ReadOnly = true;
                FechaParametrizacion = ConfiguracionHorarios[0].FechaInicioParametrizacion.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("es-ES"));
                ApplicationShared.IdParametroHorario = ConfiguracionHorarios[0].IdParametroHorario;
                ApplicationShared.CreateNewConfiguration = !ReadOnly;

                ParametrizacionHorario = await ParametrizacionHorarioService.GetParametrizationScheduleByProfileId(ApplicationShared.IdPerfil.Value, ConfiguracionHorarios[0].IdParametroHorario);
                if (ParametrizacionHorario != null)
                {
                    ApplicationShared.CuposIntervalo = ParametrizacionHorario.CuposPorIntervalo;
                    ApplicationShared.IdTipoAgenda = ParametrizacionHorario.IdTipoAgenda;
                    ApplicationShared.TiempoHoraMinAgenda = ParametrizacionHorario.TiempoHoraMinimoAgenda;
                    ApplicationShared.TiempoHoraMinCancelar = ParametrizacionHorario.TiempoHoraMinimoCancelar;
                    ApplicationShared.FechaInicioParametrizacion = ConfiguracionHorarios[0].FechaInicioParametrizacion;
                }
                else
                {
                    ApplicationShared.CuposIntervalo = 0;
                    ApplicationShared.IdTipoAgenda = 0;
                    ApplicationShared.TiempoHoraMinAgenda = 0;
                    ApplicationShared.TiempoHoraMinCancelar = 0;
                }
            }
            await ProtectedSessionStore.SetAsync(ApplicationShared.NameLocalStorage, ApplicationShared);
            StateHasChanged();
            return ConfiguracionHorarios;
        }

        /// <summary>
        /// Direcciona para crear una nueva configuracion.
        /// </summary>
        private async Task Navigate()
        {
            ApplicationShared.CreateNewConfiguration = !ApplicationShared.CreateNewConfiguration;
            await ProtectedSessionStore.SetAsync(ApplicationShared.NameLocalStorage, ApplicationShared);
            Navigation.NavigateTo("/configuracion/politicaAgendamiento", false);
        }

        /// <summary>
        /// Avanza para configurar el horario de atencion
        /// </summary>
        private async Task NavigateSchedule()
        {
            await ProtectedSessionStore.SetAsync(ApplicationShared.NameLocalStorage, ApplicationShared);
            Navigation.NavigateTo("/configuracion/horario-atencion", false);
        }

        #endregion Metodos
    }
}


