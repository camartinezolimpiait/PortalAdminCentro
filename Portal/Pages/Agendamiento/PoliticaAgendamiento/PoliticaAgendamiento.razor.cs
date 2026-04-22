using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Services.Agendamiento;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Agendamiento.PoliticaAgendamiento
{
    public partial class PoliticaAgendamiento
    {
        #region Constructor

        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public IParametrizacionHorarioService ParametrizacionHorarioService { get; set; }

        [Inject]
        private IHorarioAtencionService _horarioAtencionService { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Parameter]
        public List<ConfiguracionHorario> ConfiguracionHorarios { get; set; }
        [Parameter]
        public ParametrizacionHorario ParametrizacionHorario { get; set; }
        [Parameter]
        public bool readOnly { get; set; }

        #endregion

        #region Constants
        private const string SIGUIENTE = "Guardar y continuar";
        private const string GUARDAR = "Guardar";
        #endregion


        #region Variables
        
        public string SelectedPolicy { get; set; }
        public string ButtonText { get; set; }
        public bool IsLoading = true;
        public bool getNewSchedule = false;
        public bool existFutureAppointment = false;
        public bool agendaFutura = false;
        private ApplicationShared applicationShared = new ApplicationShared();
        private LocalStorage localStorage = new LocalStorage();
        #endregion

        #region Metodos
        protected override Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
                applicationShared = result.Value;
                await LoadInitialForm();
            }
            catch (ApplicationException ae)
            {
                toastService.ShowWarning(ae.Message.ToString(), "Información");
            }
            catch (Exception ex)
            {
                toastService.ShowWarning(ex.Message.ToString(), "Información");
            }
            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// Verifica y establece si un radio button debe estar checked
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        private bool CheckedSelectedValue(Enum.PoliticaAgendamiento policy)
        {
            return (SelectedPolicy == policy.ToString());
        }

        /// <summary>
        /// Establece el texto que tendrá el boton para continuar
        /// Regla: si el radionutton seleccionado es SoloAtencionOrdenLlegada el texto debe ser Guardar, de lo contrario, Siguiente 
        /// </summary>
        /// <param name="policy">Enumerador de la politca de agendamiento</param>
        private void SetButtonText(Enum.PoliticaAgendamiento policy)
        {
            ButtonText = (policy == Enum.PoliticaAgendamiento.SoloAtencionOrdenLlegada) ?
                GUARDAR : SIGUIENTE;
        }

        /// <summary>
        /// Evento change del radio button para establecer la variable SelectedPolicy
        /// </summary>
        /// <param name="args"></param>
        private void PolicyOnChange(ChangeEventArgs args)
        {
            SelectedPolicy = args.Value.ToString();
            SetButtonText(GetEnumFromString(SelectedPolicy));
        }

        /// <summary>
        /// Evento click del div content para establecer la variable SelectedPolicy
        /// </summary>
        /// <param name="policy"></param>
        private void PolicyOnChange(Enum.PoliticaAgendamiento policy)
        {
            SelectedPolicy = policy.ToString();
            SetButtonText(GetEnumFromString(SelectedPolicy));
        }

        /// <summary>
        /// Convertir string a la opcion correspondiente dentro del enumerador
        /// Para revisar: Esto podría hacer parte de una funcion global para ser usada con Genericos
        /// </summary>
        /// <param name="optionEnum">String que corresponde a la opcion del enumerador</param>
        /// <returns></returns>
        private Enum.PoliticaAgendamiento GetEnumFromString(string optionEnum)
        {
            if (string.IsNullOrEmpty(optionEnum))
                return Enum.PoliticaAgendamiento.Mixta;
            return (Enum.PoliticaAgendamiento)System.Enum.Parse(typeof(Enum.PoliticaAgendamiento), optionEnum.Trim());
        }

        /// <summary>
        /// Funcion para guardar la parametrizacion de horario y seguir en el wizard dependiendo de lo seleccionado
        /// </summary>
        /// <returns></returns>
        public async Task SaveAndNextWizard()
        {
            if (applicationShared != null && applicationShared.IdPerfil.HasValue)
            {
                var estadoProceso = await ParametrizacionHorarioService.SaveParametrizationSchedule(new ParametrizacionHorario
                {
                    IdPerfil = applicationShared.IdPerfil.Value,
                    UsuarioCreacion = applicationShared.UserName,
                    Intervalo = ParametrizacionHorario.Intervalo,
                    DisponibilidadAgendaDias = ParametrizacionHorario.DisponibilidadAgendaDias,
                    CuposPorIntervalo = ParametrizacionHorario.CuposPorIntervalo,
                    TiempoHoraMinimoAgenda = ParametrizacionHorario.TiempoHoraMinimoAgenda,
                    TiempoHoraMinimoCancelar = ParametrizacionHorario.TiempoHoraMinimoCancelar,
                    IdPoliticaAgendamiento = ((short)GetEnumFromString(SelectedPolicy)),
                    IdTipoAgenda = ParametrizacionHorario.IdTipoAgenda,
                    IdParametroHorario = applicationShared.IdParametroHorario,
                    IdParametrizacionHorario = ParametrizacionHorario.IdParametrizacionHorario
                }, ConfiguracionHorarios[0].FechaInicioParametrizacion);

                if (await ParametrizacionHorarioService.GetEstadoPeticion())
                {
                    if (estadoProceso.IdParametroHorario != applicationShared.IdParametroHorario)
                    {
                        applicationShared.IdParametroHorario = ConfiguracionHorarios[0].IdParametroHorario;
                        applicationShared.IsAgendaFutura = true;
                    }

                    applicationShared.IdPoliticaAgendamiento = ((short)GetEnumFromString(SelectedPolicy));
                    await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
                    if (SelectedPolicy != Enum.PoliticaAgendamiento.SoloAtencionOrdenLlegada.ToString())
                    {
                        Navigation.NavigateTo("/configuracion/horario-agendamiento", false);
                    }
                    else
                    {
                        toastService.ShowSuccess(@"La configuración se ha guardado correctamente", "Información");
                        Navigation.NavigateTo("/configuracion/configurarParametrizacion", false);
                    }
                }
                else
                {
                    toastService.ShowError(@"Ha ocurrido un error al guardar la configuración", "Información");
                }
            }
        }

        /// <summary>
        /// Carga inicial de la pagina
        /// </summary>
        /// <returns></returns>
        private async Task LoadInitialForm()
        {
            IsLoading = true;
            if (applicationShared == null)
            {
                IsLoading = false;
                throw new ApplicationException("No es posible obtener la información del centro");
            }
            else
            {
                if (ConfiguracionHorarios == null)
                {
                    DateTime startDate = DateTime.Now;
                    getNewSchedule = applicationShared.IsAgendaFutura && applicationShared.CreateNewConfiguration ? true : false;
                    ConfiguracionHorarios = await _horarioAtencionService.GetConfiguracionHorario(applicationShared.IdPerfil ?? 0, startDate, getNewSchedule);
                }

                if (ConfiguracionHorarios.Any())
                {
                    ButtonText = SIGUIENTE;
                    if (ParametrizacionHorario == null)
                        ParametrizacionHorario = await ParametrizacionHorarioService.GetParametrizationScheduleByProfileId(applicationShared.IdPerfil.Value, ConfiguracionHorarios[0].IdParametroHorario);

                    SelectedPolicy = (ParametrizacionHorario != null)
                                   ? ((Enum.PoliticaAgendamiento)ParametrizacionHorario.IdPoliticaAgendamiento).ToString()
                                   : Enum.PoliticaAgendamiento.Mixta.ToString();
                    SetButtonText(GetEnumFromString(SelectedPolicy));

                    applicationShared.IdPoliticaAgendamiento = (short)((ParametrizacionHorario != null)
                                   ? ((Enum.PoliticaAgendamiento)ParametrizacionHorario.IdPoliticaAgendamiento)
                                   : Enum.PoliticaAgendamiento.Mixta);

                    existFutureAppointment = ConfiguracionHorarios[0].TotalFuturasAgenda > 0 ? true : false;
                    readOnly = !applicationShared.CreateNewConfiguration;
                    applicationShared.IdParametroHorario = ConfiguracionHorarios[0].IdParametroHorario;
                    applicationShared.IsAgendaFutura = ConfiguracionHorarios[0].TotalParametrizacionAgenda >= 2 ? true : false;
                    applicationShared.FechaInicioParametrizacion = ConfiguracionHorarios[0].FechaInicioParametrizacion;
                }
            }            
            IsLoading = false;
            StateHasChanged();
        }

        /// <summary>
        /// Avanza para configurar el horario de atencion
        /// </summary>
        private void Navigate()
        {
            ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
            Navigation.NavigateTo("/configuracion/horario-atencion",false);
        }
        #endregion
    }
}
