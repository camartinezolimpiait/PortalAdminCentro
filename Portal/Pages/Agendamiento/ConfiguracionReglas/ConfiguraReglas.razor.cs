using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Horario;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.ConfiguracionCuposReglas;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Util;

namespace portalAdministrativoSISEC.Pages.Agendamiento.ConfiguracionReglas
{
    public partial class ConfiguraReglas
    {
        #region Constructor
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public IConfiguracionCuposReglas ConfiguracionCuposReglasService { get; set; }
        [Inject]
        public IParametrizacionHorarioService ParametrizacionHorarioService { get; set; }
        [Inject]
        private IHorarioAtencionService HorarioAtencionService { get; set; }

        [Inject]
        private IToastService ToastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; }
        [Parameter]
        public bool ExistFutureAppointment { get; set; }
        [Parameter]
        public bool AgendaFutura { get; set; }
        [Parameter]
        public ParametrizacionHorario ParametrizacionHorario { get; set; }
        [Parameter]
        public List<ConfiguracionHorario> ConfiguracionHorarios { get; set; }

        #endregion

        #region Variables
        public short TiempoMinAgenda = 1;
        public short TiempoMinCancelar = 1;
        private short _horas = 24;
        public string TipoTiempoAgenda = "Días";
        public string TipoTiempoCancelar = "Días";
        public bool IsLoading = true;
        public bool getNewSchedule = false;
        public bool createNewSchedule = false;
        public bool isActual = true;
        private LocalStorage localStorage = new LocalStorage();
        private ApplicationShared applicationShared = new ApplicationShared();

        #endregion

        #region Metodos
        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
        protected override async Task<Task> OnParametersSetAsync()
        {
            try
            {
                IsLoading = true;
                var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
                applicationShared = result.Value;
                await getConfiguration();
                IsLoading = false;
            }
            catch (ApplicationException ae)
            {
                IsLoading = false;
                ToastService.ShowWarning(ae.Message, "Información");
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ToastService.ShowWarning(ex.Message, "Error");
            }
            return base.OnParametersSetAsync();
        }

        /// <summary>
        /// Metodo para volver a la pantalla anterior
        /// </summary>
        /// <returns></returns>
        private async Task Atras()
        {
           await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
            Navigation.NavigateTo("/configuracion/configuracion-cupos", false);
        }
        private async Task getConfiguration()
        {
            if (ConfiguracionHorarios == null)
            {
                DateTime startDate = DateTime.Now;
                getNewSchedule = applicationShared.IsAgendaFutura && applicationShared.CreateNewConfiguration ? true : false;
                ConfiguracionHorarios = await HorarioAtencionService.GetConfiguracionHorario(applicationShared.IdPerfil ?? 0, startDate, getNewSchedule);
                applicationShared.FechaInicioParametrizacion = ConfiguracionHorarios[0].FechaInicioParametrizacion;
            }
            if (ParametrizacionHorario == null)
                ParametrizacionHorario = await ParametrizacionHorarioService.GetParametrizationScheduleByProfileId(applicationShared.IdPerfil.Value, ConfiguracionHorarios[0].IdParametroHorario);


            if (applicationShared != null)
            {
                if (applicationShared.TiempoHoraMinAgenda <= 23)
                {
                    TipoTiempoAgenda = "Horas";
                    TiempoMinAgenda = applicationShared.TiempoHoraMinAgenda;
                }
                else
                {
                    TipoTiempoAgenda = "Días";
                    TiempoMinAgenda = (short)(applicationShared.TiempoHoraMinAgenda / _horas);
                }

                if (applicationShared.TiempoHoraMinCancelar <= 23)
                {
                    TipoTiempoCancelar = "Horas";
                    TiempoMinCancelar = applicationShared.TiempoHoraMinCancelar;
                }
                else
                {
                    TipoTiempoCancelar = "Días";
                    TiempoMinCancelar = (short)(applicationShared.TiempoHoraMinCancelar / _horas);
                }
            }
        }
        /// <summary>
        /// Metodo para almacenar los datos parametrizados y pasar a siguiente pantalla
        /// </summary>
        /// <returns></returns>
        private async Task GuardarConfiguracionReglas()
        {
            applicationShared.TiempoHoraMinAgenda = (short)(TipoTiempoAgenda == "Días" ? TiempoMinAgenda * _horas : TiempoMinAgenda);
            applicationShared.TiempoHoraMinCancelar = (short)(TipoTiempoCancelar == "Días" ? TiempoMinCancelar * _horas : TiempoMinCancelar);

            if(applicationShared.TiempoHoraMinAgenda > (ParametrizacionHorario.DisponibilidadAgendaDias * _horas))
            {
                ToastService.ShowWarning(@"El tiempo mínimo para agendar no puede ser mayor al valor de agenda disponible.", "Información");
                return;
            }

            var estadoProceso = await ParametrizacionHorarioService.SaveParametrizationSchedule(new ParametrizacionHorario
            {
                IdPerfil = applicationShared.IdPerfil.Value,
                UsuarioCreacion = applicationShared.UserName,
                Intervalo = ParametrizacionHorario.Intervalo,
                DisponibilidadAgendaDias = ParametrizacionHorario.DisponibilidadAgendaDias,
                CuposPorIntervalo = ParametrizacionHorario.CuposPorIntervalo,
                TiempoHoraMinimoAgenda = applicationShared.TiempoHoraMinAgenda,
                TiempoHoraMinimoCancelar = applicationShared.TiempoHoraMinCancelar,
                IdPoliticaAgendamiento = ParametrizacionHorario.IdPoliticaAgendamiento,
                IdTipoAgenda = ParametrizacionHorario.IdTipoAgenda,
                IdParametroHorario = applicationShared.IdParametroHorario,
                IdParametrizacionHorario = ParametrizacionHorario.IdParametrizacionHorario
            }, applicationShared.FechaInicioParametrizacion);
            await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
            if (estadoProceso.IdParametroHorario > 0)
            {
                ToastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                Navigation.NavigateTo("/configuracion/configurarParametrizacion", false);
            }
            else
            {
                ToastService.ShowWarning(@"Ha ocurrido un error, intente nuevamente.", "Información");
            }
        }
    }
    #endregion
}



