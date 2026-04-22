using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Agendamiento.HorarioAtencion
{
    public partial class HorarioAtencion
    {
        #region Constructor
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IHorarioAtencionService _horarioAtencionService { get; set; }
        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        #endregion

        #region Variables

        public List<ConfiguracionHorario> configuracionHorarios = new List<ConfiguracionHorario>();
        private ApplicationShared applicationShared = new ApplicationShared();
        public bool mostarGuardar = false;
        public bool isActiveIndependiente = false;
        public bool isActiveHoraAlmuerzo = false;
        public bool isLoading = true;
        public bool getNewSchedule = false;
        public bool readOnly = true;
        public bool existFutureAppointment = false;
        public bool isActual = true;
        public bool agendaFutura = false;
        public bool createNewConfiguration = false;
        private LocalStorage localStorage = new LocalStorage();
        private string fechaParametrizacion = string.Empty;

        #endregion

        #region Metodos
        protected override async Task OnInitializedAsync()
        {
        }

        protected override async Task OnParametersSetAsync()
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
                toastService.ShowWarning(ae.Message.ToString(), "Información");
            }
            catch (Exception ex)
            {
                isLoading = false;
                toastService.ShowWarning(ex.Message.ToString(), "Información");
            }

            isLoading = false;
            await base.OnParametersSetAsync();
        }
        /// <summary>
        /// Evento para realizar el cambio de horario por cada dia.
        /// </summary>
        public async Task HorarioDiferente(ChangeEventArgs args)
        {
            ConfiguracionHorario horarioAlmuerzo = configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).FirstOrDefault();
            configuracionHorarios.Remove(horarioAlmuerzo);
            if ((bool)args.Value)
            {
                //Horario Independiente
                isActiveIndependiente = true;
                configuracionHorarios = llenarConfiguracionHorarios(2, configuracionHorarios);
            }
            else
            {
                //Horario Semana
                isActiveIndependiente = false;
                configuracionHorarios = llenarConfiguracionHorarios(1, configuracionHorarios);
            }
            if (horarioAlmuerzo != null)
            {
                configuracionHorarios.Add(horarioAlmuerzo);
            }
            await Task.FromResult(configuracionHorarios);
        }

        /// <summary>
        /// Evento para activar el horario de almuerzo.
        /// </summary>

        public async Task HorarioAlmuerzo(ChangeEventArgs args)
        {
            if ((bool)args.Value)
            {
                if (configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).FirstOrDefault() != null)
                {
                    configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).Select(s => { s.IsVisible = true; s.IsActivo = true; return s; }).ToList();
                }
                else
                {
                    configuracionHorarios = llenarConfiguracionHorarios(3, configuracionHorarios);
                }
                isActiveHoraAlmuerzo = true;
            }
            else
            {
                if (configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).FirstOrDefault() != null)
                {
                    configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).Select(s => { s.IsVisible = false; s.IsActivo = false; return s; }).ToList();
                }
                isActiveHoraAlmuerzo = false;
            }
          await Task.FromResult(configuracionHorarios);
        }

        /// <summary>
        /// Evento para habilidar e inhabilitar el campo de dia.
        /// </summary>
        public async Task HabilitarDia(ChangeEventArgs args, ConfiguracionHorario configHorarioDia)
        {
            ConfiguracionHorario confDia = configuracionHorarios.Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita).FirstOrDefault();
            if (confDia != null)
            {
                confDia.IsActivo = (bool)args.Value;
            }
            await Task.FromResult(confDia);
        }

        /// <summary>
        /// Evento para actualizar los valores de las horas.
        /// </summary>
        public async Task ActualizarHoraDia(ChangeEventArgs args, ConfiguracionHorario configHorarioDia, int tipoActualizacion)
        {
            if (configuracionHorarios.Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita).FirstOrDefault() != null && !string.IsNullOrEmpty(args.Value?.ToString()))
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
                            configuracionHorarios
                                .Where(x => x.IdHorarioCita >= (int)FranjaHoraria.LunesViernes && x.IdHorarioCita < (int)FranjaHoraria.Sabado)
                                .Select(s => { s.HoraApertura = horaSeleccionada; s.IsActivo = true; return s; }).ToList();
                        }
                        else
                        {
                            configuracionHorarios
                                .Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita)
                                .Select(s => { s.HoraApertura = horaSeleccionada; return s; }).ToList();
                        }
                        break;
                    case 2:
                        if (configHorarioDia.IdHorarioCita == (int)FranjaHoraria.LunesViernes)
                        {
                            configuracionHorarios
                                .Where(x => x.IdHorarioCita >= (int)FranjaHoraria.LunesViernes && x.IdHorarioCita < (int)FranjaHoraria.Sabado)
                                .Select(s => { s.HoraCierre = horaSeleccionada; s.IsActivo = true; return s; }).ToList();
                        }
                        else
                        {
                            configuracionHorarios
                                .Where(x => x.IdHorarioCita == configHorarioDia.IdHorarioCita)
                                .Select(s => { s.HoraCierre = horaSeleccionada; return s; }).ToList();
                        }
                        break;
                }
            }
            await Task.FromResult(configuracionHorarios);
        }

        /// <summary>
        /// Evento para enviar a guardar la configuracion de horarios.
        /// </summary>
        public async Task GuardarConfiguracionhorarios()
        {
            isLoading = true;
            int cantidadActivos = configuracionHorarios.Where(x => x.IsActivo == true && x.IdHorarioCita != (int)FranjaHoraria.LunesViernes).ToList().Count();
            var cantidaConfiguraciones = configuracionHorarios
                .Where(x =>
                    x.IsActivo == true
                    && x.IdHorarioCita != (int)FranjaHoraria.LunesViernes
                    && x.HoraApertura > 0
                    && x.HoraCierre > 0
                    && DateTime.Compare(DateTime.ParseExact(x.ConvertirHoraFin(), "HH:mm", CultureInfo.InvariantCulture), DateTime.ParseExact(x.ConvertirHoraInicio(), "HH:mm", CultureInfo.InvariantCulture)) > 0
                ).ToList();
            if (cantidadActivos == cantidaConfiguraciones.Count())
            {
                //TODO: Actualizar el id de perfil a consultar
                configuracionHorarios = configuracionHorarios.Where(x => x.IdHorarioCita != (int)FranjaHoraria.LunesViernes).ToList();
                var estadoProceso = await _horarioAtencionService.SaveConfiguracionHorario(applicationShared.IdPerfil ?? 0, applicationShared.UserName, 1, configuracionHorarios);

                agendaFutura = false;
                getNewSchedule = false;
                readOnly = true;
                existFutureAppointment = false;
                isActual = true;

                if (await _horarioAtencionService.GetEstadoPeticion())
                {
                    toastService.ShowSuccess(@"Se ha guardado la configuración correctamente.", "Información");
                    await OnParametersSetAsync();
                }
                else
                {
                    toastService.ShowWarning(@"Ha ocurrido un error, intente nuevamente.", "Información");
                }
            }
            else
            {
                toastService.ShowWarning(@"La configuracion no cumple con los horarios.", "Información");
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
            return configuracionHorarios.Where(x => x.IdHorarioCita == IdHorarioCita).FirstOrDefault();
        }
        /// <summary>
        /// Metodo para remover objecto de configuracion cuando la vista no coincidan.
        /// </summary>
        private ConfiguracionHorario quitarVistaDia(ConfiguracionHorario horario, bool estadoNuevo)
        {
            if (horario != null)
            {
                configuracionHorarios.Where(x => x.IdHorarioCita == horario.IdHorarioCita).Select(s => { s.IsVisible = estadoNuevo; return s; }).ToList();
                //configuracionHorarios.Remove(horario);
                return horario;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Habilita para crear una nuevo horario
        /// </summary>
        private void ButtonOnClickNewSchedule()
        {
            createNewConfiguration = true;
            readOnly = false;
            isActual = false;
        }

        /// <summary>
        /// Cancela la edicion de la nueva configuracion
        /// </summary>
        private void ButtonOnClickCancel()
        {
            createNewConfiguration = false;
            readOnly = true;
            isActual = true;
        }

        private async Task ConfigurationOnChange(ChangeEventArgs args)
        {
            int opcion = int.Parse(args.Value.ToString());
            if (opcion == 1)
            {
                createNewConfiguration = false;
                getNewSchedule = false;
                isActual = true;
                await GetConfigurationSchedule();
            }
            else
            {
                isActual = false;
                getNewSchedule = true;
                await GetConfigurationSchedule();
                createNewConfiguration = true;
            }
        }
        /// <summary>
        /// Obtiene la configuracion de la parametrizacion del horario
        /// </summary>
        /// <returns></returns>
        private async Task GetConfigurationSchedule()
        {
            // isLoading = true;
            DateTime today = DateTime.Now;
            configuracionHorarios = await _horarioAtencionService.GetConfiguracionHorario(applicationShared.IdPerfil ?? 0, today, getNewSchedule);
            if (await _horarioAtencionService.GetEstadoPeticion())
            {
                if (!configuracionHorarios.Any())
                {
                    configuracionHorarios = llenarConfiguracionHorarios(1, null);
                }
                else
                {
                    fechaParametrizacion = configuracionHorarios[0].FechaInicioParametrizacion.ToString("dd MMM yyyy", CultureInfo.CreateSpecificCulture("es-ES"));
                    agendaFutura = configuracionHorarios[0].TotalParametrizacionAgenda >= 2 ? true : false;
                    existFutureAppointment = configuracionHorarios[0].TotalFuturasAgenda > 0 ? true : false;
                    readOnly = (!isActual) ? (!existFutureAppointment && agendaFutura) ? false : true : true;
                    configuracionHorarios = configuracionHorarios.Select(x => { x.IsVisible = true; return x; }).OrderBy(o => o.IdHorarioCita).ToList();
                    ConfiguracionHorario primerDiaSemana = configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Lunes).FirstOrDefault();
                    if (primerDiaSemana != null
                        && configuracionHorarios
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
                        configuracionHorarios = llenarConfiguracionHorarios(1, configuracionHorarios);
                        configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.LunesViernes).Select(s => { s.HoraApertura = primerDiaSemana.HoraApertura; s.HoraCierre = primerDiaSemana.HoraCierre; return s; }).ToList();
                    }
                    else
                    {
                        //configuracion idependiente
                        isActiveIndependiente = true;
                        configuracionHorarios = llenarConfiguracionHorarios(2, configuracionHorarios);
                    }
                    if (configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo && x.IsActivo == true).FirstOrDefault() != null)
                    {
                        isActiveHoraAlmuerzo = true;
                    }
                    else
                    {
                        isActiveHoraAlmuerzo = false;

                    }
                    configuracionHorarios.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).Select(s => { s.IsVisible = isActiveHoraAlmuerzo; return s; }).ToList();
                }
                mostarGuardar = agendaFutura ? false : true;
            }
            else
            {
                mostarGuardar = false;
                toastService.ShowWarning(@"No se puedo cargar el horario", "Información");
            }
            // isLoading = false;
        }

        #endregion
    }
}
