using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Entidades.Agendamiento.InicioAgendamiento;
using portalAdministrativoSISEC.Services.Agendamiento.InicioAgendamiento;
using System;
using portalAdministrativoSISEC.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using System.Globalization;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Services.Agendamiento;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Util;

namespace portalAdministrativoSISEC.Pages.Agendamiento.InicioAgendamiento
{
    public partial class InicioAgendamiento
    {
        #region Constructor

        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public IHorarioAtencionService HorarioAtencionService { get; set; }
        [Inject]
        private IParametrizacionHorarioService ParametrizacionHorarioService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        private IToastService ToastService { get; set; }
        [Parameter]
        public List<ConfiguracionHorario> ConfiguracionHorarios { get; set; }
        [Parameter]
        public ParametrizacionHorario ParametrizacionHorario { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; }
        [Parameter]
        public bool ExistFutureAppointment { get; set; }
        [Parameter]
        public bool AgendaFutura { get; set; }

        #endregion

        #region Variables

        private ApplicationShared applicationShared = new ApplicationShared();
        private List<int> _listIntervalos = new() { 5, 10, 15, 20, 30, 60 };
        public bool mostarGuardar = true;
        public bool isActiveIndependiente = false;
        public bool isLoading = true;
        public bool getNewSchedule = false;
        public bool isActual = true;
        private LocalStorage localStorage = new LocalStorage();
        #endregion

        #region Metodos

        protected override Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }
        protected override async Task<Task> OnParametersSetAsync()
        {
            isLoading = true;
            try
            {               
                var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
                applicationShared = result.Value;
                await GetConfigurationSchedule();               
            }
            catch (ApplicationException ae)
            {
                isLoading = false;
                ToastService.ShowWarning(ae.Message.ToString(), "Información");
            }
            catch (Exception ex)
            {
                isLoading = false;
                ToastService.ShowWarning(ex.Message.ToString(), "Información");
            }
            isLoading = false;
            return base.OnParametersSetAsync();
        }
        /// <summary>
        /// Evento para realizar el cambio de horario por cada dia.
        /// </summary>
        public async Task HorarioDiferente(ChangeEventArgs args)
        {
            ConfiguracionHorario horarioAlmuerzo = ConfiguracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).FirstOrDefault();
            ConfiguracionHorarios.Remove(horarioAlmuerzo);
            if ((bool)args.Value)
            {
                //Horario Independiente
                isActiveIndependiente = true;
                ConfiguracionHorarios = llenarConfiguracionHorarios(2, ConfiguracionHorarios);
            }
            else
            {
                //Horario Semana
                isActiveIndependiente = false;
                ConfiguracionHorarios = llenarConfiguracionHorarios(1, ConfiguracionHorarios);
            }
            if (horarioAlmuerzo != null)
            {
                ConfiguracionHorarios.Add(horarioAlmuerzo);
            }
        }


        /// <summary>
        /// Evento para habilidar e inhabilitar el campo de dia.
        /// </summary>

        public async Task HabilitarDia(ChangeEventArgs args, ConfiguracionHorario configHorarioDia)
        {
            ConfiguracionHorario confDia = ConfiguracionHorarios.Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita).FirstOrDefault();
            if (confDia != null)
            {
                confDia.IsActivo = (bool)args.Value;
            }
        }

        /// <summary>
        /// Evento para actualizar los valores de las horas.
        /// </summary>

        public async Task ActualizarHoraDia(ChangeEventArgs args, ConfiguracionHorario configHorarioDia, int tipoActualizacion)
        {
            if (ConfiguracionHorarios.Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita).FirstOrDefault() != null && !string.IsNullOrEmpty(args.Value?.ToString()))
            {
                //TODO: Actualizar para cuando este seleccionado de lunes a viernes cambie en toda la franje de la semana
                Int16 horaSeleccionada = 0;
                string horaParametro = args.Value.ToString();
                string horaFormateda = string.Concat(horaParametro[0], horaParametro[1], horaParametro[3], horaParametro[4]);
                Int16.TryParse(horaFormateda, out horaSeleccionada);
                switch (tipoActualizacion)
                {
                    case 1:
                        if (configHorarioDia.IdHorarioCita == (int)FranjaHoraria.LunesViernes)
                        {
                            ConfiguracionHorarios
                                .Where(x => x.IdHorarioCita >= (int)FranjaHoraria.LunesViernes && x.IdHorarioCita < (int)FranjaHoraria.Sabado)
                                .Select(s => { s.HorarioAperturaAgenda = horaSeleccionada; s.IsActivo = true; return s; }).ToList();
                        }
                        else
                        {
                            ConfiguracionHorarios
                                .Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita)
                                .Select(s => { s.HorarioAperturaAgenda = horaSeleccionada; return s; }).ToList();
                        }
                        break;
                    case 2:
                        if (configHorarioDia.IdHorarioCita == (int)FranjaHoraria.LunesViernes)
                        {
                            ConfiguracionHorarios
                                .Where(x => x.IdHorarioCita >= (int)FranjaHoraria.LunesViernes && x.IdHorarioCita < (int)FranjaHoraria.Sabado)
                                .Select(s => { s.HorarioCierreAgenda = horaSeleccionada; s.IsActivo = true; return s; }).ToList();
                        }
                        else
                        {
                            ConfiguracionHorarios
                                .Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita)
                                .Select(s => { s.HorarioCierreAgenda = horaSeleccionada; return s; }).ToList();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Evento para enviar a guardar la configuracion de horarios.
        /// </summary>

        public async Task GuardarConfiguracionhorarios()
        {
            int cantidadActivos = ConfiguracionHorarios.Where(x => x.IsActivo == true && x.IdHorarioCita != (int)FranjaHoraria.LunesViernes && x.IdHorarioCita != (int)FranjaHoraria.Almuerzo).ToList().Count();
            DateTime fechainicioPrametrizacion = DateTime.Now;
            var cantidaConfiguraciones = ConfiguracionHorarios
                .Where(x =>
                    x.IsActivo == true
                    && x.IdHorarioCita != (int)FranjaHoraria.LunesViernes
                    && x.HorarioAperturaAgenda > 0
                    && x.HorarioCierreAgenda > 0
                    && x.HorarioAperturaAgenda >= x.HoraApertura
                    && x.HorarioCierreAgenda <= x.HoraCierre
                    && x.IdHorarioCita != (int)FranjaHoraria.Almuerzo
                    && DateTime.Compare(DateTime.ParseExact(x.ConvertirHorarioCierreAgenda(), "HH:mm", CultureInfo.InvariantCulture), DateTime.ParseExact(x.ConvertirHorarioAperturaAgenda(), "HH:mm", CultureInfo.InvariantCulture)) > 0
                ).ToList();
            if (cantidadActivos == cantidaConfiguraciones.Count())
            {
                if (!_listIntervalos.Contains(ParametrizacionHorario.Intervalo))
                {
                    ToastService.ShowWarning(@"El valor del intervalo de citas no corresponde a los permitidos", "Información");
                    return;
                }

                isLoading = true;
                //TODO: Actualizar el id de perfil a consultar
                ConfiguracionHorarios = ConfiguracionHorarios.Where(x => x.IdHorarioCita != (int)FranjaHoraria.LunesViernes).ToList();

                var estadoProceso = await HorarioAtencionService.SaveConfiguracionHorario(applicationShared.IdPerfil ?? 0, applicationShared.UserName, 1, ConfiguracionHorarios);

                if(await HorarioAtencionService.GetEstadoPeticion())
                {
                    if(estadoProceso.IdParametroHorario != applicationShared.IdParametroHorario)
                    {
                        DateTime startDate = DateTime.Now;
                        applicationShared.IdParametroHorario = estadoProceso.IdParametroHorario;
                        applicationShared.IsAgendaFutura = true;
                        ConfiguracionHorarios = await HorarioAtencionService.GetConfiguracionHorario(applicationShared.IdPerfil ?? 0, startDate, true);
                    }
                    fechainicioPrametrizacion = ConfiguracionHorarios.Where(x => x.IsActivo == true).FirstOrDefault().FechaInicioParametrizacion;
                    var estadoProcesoParametrizacion = await ParametrizacionHorarioService.SaveParametrizationSchedule(new ParametrizacionHorario
                    {
                        IdPerfil = applicationShared.IdPerfil.Value,
                        UsuarioCreacion = applicationShared.UserName,
                        Intervalo = ParametrizacionHorario.Intervalo,
                        DisponibilidadAgendaDias = ParametrizacionHorario.DisponibilidadAgendaDias,
                        CuposPorIntervalo = ParametrizacionHorario.CuposPorIntervalo,
                        TiempoHoraMinimoAgenda = ParametrizacionHorario.TiempoHoraMinimoAgenda,
                        TiempoHoraMinimoCancelar = ParametrizacionHorario.TiempoHoraMinimoCancelar,
                        IdPoliticaAgendamiento = ParametrizacionHorario.IdPoliticaAgendamiento,
                        IdTipoAgenda = ParametrizacionHorario.IdTipoAgenda,
                        IdParametroHorario = applicationShared.IdParametroHorario,
                        IdParametrizacionHorario = ParametrizacionHorario.IdParametrizacionHorario
                    }, fechainicioPrametrizacion);

                    if (await ParametrizacionHorarioService.GetEstadoPeticion())
                    {
                        if(estadoProcesoParametrizacion.IdParametroHorario != applicationShared.IdParametroHorario)
                        {
                            applicationShared.IdParametroHorario = estadoProcesoParametrizacion.IdParametroHorario;
                            applicationShared.IsAgendaFutura = true;
                        }
                        await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
                        Navigation.NavigateTo("/configuracion/configuracion-cupos", false);
                    }
                    else
                    {
                        ToastService.ShowWarning(@"Ha ocurrido un error, intente nuevamente.", "Información");
                    }
                }
                else
                {
                    ToastService.ShowWarning(@"Ha ocurrido un error, intente nuevamente.", "Información");
                }
                



                
            }
            else
            {
                ToastService.ShowWarning(@"Valide que la configuración de los horarios de las citas estan  dentro del horario de atención.", "Información");
            }
            isLoading = false;
        }

        /// <summary>
        /// Metodo para crear el objetos con los diferentes dias de configuracion horaria.
        /// estadoVisualizacion = Semana 1,  independiente 2
        /// </summary>
        private List<ConfiguracionHorario> llenarConfiguracionHorarios(int tipo, List<ConfiguracionHorario>? configuracionPrecarga)
        {
            List<ConfiguracionHorario> configuracionHorariosResult = (configuracionPrecarga == null) ? new List<ConfiguracionHorario>() : configuracionPrecarga;
            bool estadoVisualizacion = (tipo == 1 ? false : true);
            switch (tipo)
            {
                case 1:
                case 2:
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.LunesViernes), !estadoVisualizacion) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.LunesViernes, "Lunes a viernes", true, !estadoVisualizacion));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Lunes), estadoVisualizacion) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Lunes, "Lunes", true, estadoVisualizacion));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Martes), estadoVisualizacion) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Martes, "Martes", true, estadoVisualizacion));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Miercoles), estadoVisualizacion) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Miercoles, "Miercoles", true, estadoVisualizacion));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Jueves), estadoVisualizacion) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Jueves, "Jueves", true, estadoVisualizacion));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Viernes), estadoVisualizacion) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Viernes, "Viernes", true, estadoVisualizacion));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Sabado), true) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Sabado, "Sabado", true, true));
                    }
                    if (quitarVistaDia(buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Domingo), true) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Domingo, "Domingos", false, true));
                    }
                    configuracionHorariosResult = configuracionHorariosResult.OrderBy(o => o.IdHorarioCita).ToList();
                    break;
                case 3:
                    if (buscarDia(configuracionHorariosResult, (int)FranjaHoraria.Almuerzo) == null)
                    {
                        configuracionHorariosResult.Add(asignacionDia((int)FranjaHoraria.Almuerzo, "Almuerzo", true, true));
                    }
                    break;
            }
            return configuracionHorariosResult;
        }

        /// <summary>
        /// Metodo para crear el objeto de configuracion horaria basica.
        /// </summary>
        private ConfiguracionHorario asignacionDia(int IdHorarioCita, string Dia, bool estado, bool IsVisible)
        {
            return new ConfiguracionHorario()
            {
                IdHorarioCita = IdHorarioCita,
                Dia = Dia,
                IsActivo = estado,
                IsVisible = IsVisible
            };
        }
        /// <summary>
        /// Metodo para buscar objeto de configuracion horaria.
        /// </summary>
        private ConfiguracionHorario buscarDia(List<ConfiguracionHorario> listado, int IdHorarioCita)
        {
            return ConfiguracionHorarios.Where(x => x.IdHorarioCita == IdHorarioCita).FirstOrDefault();
        }
        /// <summary>
        /// Metodo para remover objecto de configuracion cuando la vista no coincidan.
        /// </summary>
        private ConfiguracionHorario quitarVistaDia(ConfiguracionHorario horario, bool estadoNuevo)
        {
            if (horario != null)
            {
                ConfiguracionHorarios.Where(x => x.IdHorarioCita == horario.IdHorarioCita).Select(s => { s.IsVisible = estadoNuevo; return s; }).ToList();
                //ConfiguracionHorarios.Remove(horario);
                return horario;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Regresa ala pagina de politica de agendamiento
        /// </summary>
        private async Task Regregar()
        {
            await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
            Navigation.NavigateTo("/configuracion/politicaAgendamiento", false);
        }

        /// <summary>
        /// Obtiene la configuracion del horario
        /// </summary>
        /// <returns></returns>
        private async Task GetConfigurationSchedule()
        {
            //TODO: Actualizar el id de perfil a consultar
            DateTime startDate = DateTime.Now;
            if (ConfiguracionHorarios == null)
            {
                getNewSchedule = applicationShared.IsAgendaFutura && applicationShared.CreateNewConfiguration ? true : false;
                ConfiguracionHorarios = await HorarioAtencionService.GetConfiguracionHorario(applicationShared.IdPerfil ?? 0, startDate, getNewSchedule);

                applicationShared.IdParametroHorario = ConfiguracionHorarios[0].IdParametroHorario;
                applicationShared.IsAgendaFutura = ConfiguracionHorarios[0].TotalParametrizacionAgenda >= 2 ? true : false;
                applicationShared.FechaInicioParametrizacion = ConfiguracionHorarios[0].FechaInicioParametrizacion;

                ExistFutureAppointment = ConfiguracionHorarios[0].TotalFuturasAgenda > 0 ? true : false;
                ReadOnly = !applicationShared.CreateNewConfiguration;
            }

            if (await HorarioAtencionService.GetEstadoPeticion())
            {
                if (!ConfiguracionHorarios.Any())
                {
                    ConfiguracionHorarios = llenarConfiguracionHorarios(1, null);
                }
                else
                {
                    if (ParametrizacionHorario == null)
                        ParametrizacionHorario = await ParametrizacionHorarioService.GetParametrizationScheduleByProfileId
                                                           (applicationShared.IdPerfil.Value, ConfiguracionHorarios[0].IdParametroHorario);

                    ConfiguracionHorarios = ConfiguracionHorarios.Select(x => { x.IsVisible = true; return x; }).OrderBy(o => o.IdHorarioCita).ToList();
                    ConfiguracionHorario primerDiaSemana = ConfiguracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Lunes).FirstOrDefault();
                    if (primerDiaSemana != null
                         && ConfiguracionHorarios
                         .Where(x => x.IdHorarioCita > (int)FranjaHoraria.LunesViernes
                                 && x.IdHorarioCita < (int)FranjaHoraria.Sabado
                                 && x.HoraApertura == primerDiaSemana.HoraApertura
                                 && x.HoraCierre == primerDiaSemana.HoraCierre
                                 && x.HoraApertura > 0
                                 && x.HoraCierre > 0
                                 && x.IsActivo == true
                         ).ToList().Count == 5
                     )
                    {
                        //configuracion Semana
                        ConfiguracionHorarios = llenarConfiguracionHorarios(1, ConfiguracionHorarios);
                        ConfiguracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.LunesViernes).Select(s => { s.HorarioAperturaAgenda = primerDiaSemana.HorarioAperturaAgenda; s.HorarioCierreAgenda = primerDiaSemana.HorarioCierreAgenda; return s; }).ToList();
                        //configuracionAgenda.FirstOrDefault();
                    }
                    else
                    {
                        //configuracion idependiente
                        isActiveIndependiente = true;
                        ConfiguracionHorarios = llenarConfiguracionHorarios(2, ConfiguracionHorarios);
                        //configuracionAgenda.FirstOrDefault();
                    }
                }
            }
            else
            {
                mostarGuardar = false;
                ToastService.ShowWarning(@"No se puedo cargar el horario", "Información");
            }
        }

        #endregion
    }
}
