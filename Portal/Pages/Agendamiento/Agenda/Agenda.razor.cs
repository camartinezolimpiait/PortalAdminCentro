using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Services.Agendamiento.Agenda;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Services.Agendamiento;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Util;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Agenda
{
    public partial class Agenda
    {
        #region Constructor
        CultureInfo ci = new CultureInfo("es-ES");

        [Inject]
        public IAgendaService _agendaService { get; set; }
        [Inject]
        public IHorarioAtencionService _horarioAtencionService { get; set; }

        [Inject]
        public IParametrizacionHorarioService _parametrizacionHorarioService { get; set; }

        [Inject]
        private IToastService toastService { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        DataInformation dataInformation { get; set; }
        [Inject]
        NavigationManager Navigation { get; set; }

        #endregion
        #region Variables

        List<string> listDays = new List<string>();
        List<DateTime> listDates = new List<DateTime>();
        Dictionary<string, bool> DaysToBlock = new Dictionary<string, bool>();
        public bool isLoading = true;
        public bool viewBehind = true;

        bool shouldRender = false;
        public string selectedWeek;
        public int countDays = 6;
        public int countParameterization = 0;
        public short lunchConfiguration = 0;
        public int controlIntervalo;
        public int minimumHourSchedule;
        public int maximumHourSchedule;
        int Columns = 1;
        DateTime? FirstDate { get; set; }
        DateTime? LastDate { get; set; }
        public short intervalo;
        public DateTime startDate;
        public DateTime dateDay;
        public string formattedDate;
        public bool DeleteDialogOpen { get; set; }
        public List<CitasIntervalo> listCitasIntervalos = new List<CitasIntervalo>();
        public List<CitasIntervalo> listCitasIntervalosToSave = new List<CitasIntervalo>();
        //public List<ParametrizacionAgenda> parametrizationAgenda = new List<ParametrizacionAgenda>();
        //public List<ParametrizacionAgenda> parametrizationAgendaAlmuerzo = new List<ParametrizacionAgenda>();
        public List<AgendaCentro> agendaCentro = new List<AgendaCentro>();

        public AgendaDTO AgendaDTOObtained { get; set; }
        public OptionAppoiment OptionAppoiment { get; set; }
        public ToolButtonOptions OptionAgendaMode { get; set; }

        public List<ResultConsultaAgendaDTO> ConsultaNombre = new List<ResultConsultaAgendaDTO>();
        string parametro = string.Empty;

        public bool IsShowForBlock { get; set; }
        public bool IsShowForSearch { get; set; }
        public bool ShowAppoimentForm { get; set; } = false;
        public bool HandlerAllBlock { get; set; } = false;
        public string DayBlock { get; set; }

        //new config
        public List<ConfiguracionHorario> parametrizationAgenda = new List<ConfiguracionHorario>();
        public List<ConfiguracionHorario> parametrizationAgendaTemp = new List<ConfiguracionHorario>();
        public List<ConfiguracionHorario> parametrizationAgendaAlmuerzo = new List<ConfiguracionHorario>();
        private ParametrizacionHorario ParametrizacionHorario { get; set; }
        private ApplicationShared applicationShared = new ApplicationShared();

        #endregion
        #region Metodos

        protected override async Task OnInitializedAsync()
        {
            var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
            applicationShared = result.Value;
            OptionAppoiment = new OptionAppoiment();
            OptionAgendaMode = new ToolButtonOptions
            {
                AgendaMode = AgendaMode.Schedule,
                ShowBlockSchedule = false
            };
            dataInformation.OnChange += StateHasChanged;
            isLoading = true;
            startDate = DateTime.Now;
            selectedWeek = SetWeekRange(startDate);
            await InitLoadSchedule();
            isLoading = false;
        }

        public void Dispose()
        {
            dataInformation.OnChange -= StateHasChanged;
        }

        public async Task BuscarPorNombre()
        {

            if(parametro == null || parametro.Trim() == "")
            {
                toastService.ShowError("Por favor digite un nombre");
            }
            else
            {
                isLoading = true;
                RenderManager();
                ConsultaNombre = await _agendaService.GetAppoimentByParametro(applicationShared.IdPerfil.Value, FirstDate.Value.ToString("ddMMyyyy"), parametro);
                isLoading = false;
                RenderManager();
            }
        }
              
           

        public async Task BuscarPorNombreEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await BuscarPorNombre();
            }
        }

        public void ShowScheduleMode()
        {
            OptionAgendaMode = new ToolButtonOptions
            {
                AgendaMode = AgendaMode.Schedule,
                ShowBlockSchedule = false
            };
            RenderManager();
        }

        public async Task<List<AgendaCentro>> GetScheduleCentro(int idPerfil, DateTime fromDate, DateTime toDate)
        {
            return await _agendaService.GetScheduleCentro(idPerfil, fromDate, toDate);//hacer el metodo con fechas
        }

        // Inicio código generado por GitHub Copilot
        // Método generado por GitHub Copilot: cálculo del número de semana ISO
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Usar la implementación integrada para obtener la semana ISO
            return System.Globalization.ISOWeek.GetWeekOfYear(time);
        }
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        // Método generado por GitHub Copilot: calcular la fecha base (domingo) de una semana ISO
        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            // Calcular el lunes de la semana ISO (la semana 1 contiene el 4 de enero)
            // Luego devolver el día anterior (domingo) para mantener compatibilidad
            DateTime jan4 = new DateTime(year, 1, 4);
            int jan4Day = (int)jan4.DayOfWeek;
            int jan4IsoDay = jan4Day == 0 ? 7 : jan4Day; // convertir Domingo(0) a 7
            DateTime week1Monday = jan4.AddDays(1 - jan4IsoDay);
            DateTime desiredMonday = week1Monday.AddDays((weekOfYear - 1) * 7);
            DateTime sundayBefore = desiredMonday.AddDays(-1);
            return sundayBefore;
        }
        // Fin código generado por GitHub Copilot

        // Inicio código generado por GitHub Copilot
        // Método generado por GitHub Copilot: establece el rango de la semana mostrado en la UI
        string SetWeekRange(DateTime fecha)
        {
            var numberWeek = GetIso8601WeekOfYear(fecha);
            // Determinar el año ISO correspondiente a la semana (puede pertenecer al año anterior o siguiente)
            int yearForWeek = fecha.Year;
            if (fecha.Month == 1 && numberWeek >= 52)
            {
                // Enero que pertenece a la última semana del año anterior
                yearForWeek = fecha.Year - 1;
            }
            else if (fecha.Month == 12 && numberWeek == 1)
            {
                // Diciembre que pertenece a la primera semana del año siguiente
                yearForWeek = fecha.Year + 1;
            }

            var firstDateWeek = FirstDateOfWeek(yearForWeek, numberWeek, CultureInfo.CurrentCulture);
            FirstDate = firstDateWeek.AddDays(1);
            LastDate = firstDateWeek.AddDays(7);
            formattedDate = fecha.ToString("MMMM", ci);
            SetDaysWeek(firstDateWeek);

            return TitleDate();
        }
        // Fin código generado por GitHub Copilot

        string TitleDate()
        {
            string monthFirstDate = FirstDate.Value.ToString("MMMM", ci),
                monthLastDate = LastDate.Value.ToString("MMMM", ci);
            int yearFirstDate = FirstDate.Value.Year,
                yearLastDate = LastDate.Value.Year;
            string titleDate = string.Empty;

            if (monthFirstDate.Equals(monthLastDate) && yearFirstDate == yearLastDate)
            {
                titleDate = string.Format("{0} {1} a {2} de {3}", string.Concat(char.ToUpper(monthFirstDate[0]), monthFirstDate.Substring(1)),
                    FirstDate.Value.Day, LastDate.Value.Day, FirstDate.Value.Year);
            }
            else if (!monthFirstDate.Equals(monthLastDate) && yearFirstDate == yearLastDate)
            {
                titleDate = string.Format("{0} de {1} a {2} de {3} de {4}",
                     FirstDate.Value.Day,
                     monthFirstDate,
                     LastDate.Value.Day,
                     monthLastDate,
                     LastDate.Value.Year);
            }
            else if (!monthFirstDate.Equals(monthLastDate) && yearFirstDate != yearLastDate)
            {
                titleDate = string.Format("{0} de {1} de {2} a {3} de {4} de {5}",
                    FirstDate.Value.Day,
                    monthFirstDate,
                    FirstDate.Value.Year,
                    LastDate.Value.Day,
                    monthLastDate,
                    LastDate.Value.Year);
            }

            return titleDate;
        }

        private void SetDaysWeek(DateTime firstDateWeek)
        {
            listDays = new List<string>();
            listDates = new List<DateTime>();
            DaysToBlock = new Dictionary<string, bool>();

            for(int i = 1; i <=7; i++)
            {
                DateTime currentDate = firstDateWeek.AddDays(i);
                listDays.Add(currentDate.ToString("dd", ci));
                listDates.Add(DateTime.Parse(currentDate.ToShortDateString()));
                DaysToBlock.Add(currentDate.ToString("ddMMyyyy"), false);

            }
        }

        private async Task PreviusWeek()
        {
            isLoading = true;
            startDate = startDate.AddDays(-7);
            selectedWeek = SetWeekRange(startDate);
            RenderManager();
            await InitLoadSchedule();
             isLoading = false;
            RenderManager();
        }

        private async Task NextWeek()
        {
            startDate = startDate.AddDays(7);
            selectedWeek = SetWeekRange(startDate);
            isLoading = true;
            RenderManager();
            await InitLoadSchedule();
            isLoading = false;
            RenderManager();
        }

        private async Task TodayWeek()
        {

            startDate = DateTime.Now;
            selectedWeek = SetWeekRange(startDate);
            isLoading = true;
            RenderManager();
            await InitLoadSchedule();
            isLoading = false;
            RenderManager();
        }

        public void NewApointment(DateTime fechaCita) //, 
        {

            StateHasChanged();
        }

        public void AppointmentDetail(long idAgenda)
        {

            StateHasChanged();
        }

        private async Task InitLoadSchedule()
        {
            try
            {
                InitObjects();
                parametrizationAgenda = await _horarioAtencionService.GetConfiguracionHorario((int)applicationShared.IdPerfil, (DateTime)FirstDate, false);
                if (parametrizationAgenda.Any())
                {
                    viewBehind = true;
                    parametrizationAgenda = parametrizationAgenda.Where(x => x.IsActivo).ToList();
                    var firstHorarioAtencion = parametrizationAgenda.FirstOrDefault();
                    if (firstHorarioAtencion != null)
                    {
                        ParametrizacionHorario = await _parametrizacionHorarioService.GetParametrizationScheduleByProfileIdAndParametroHorario(applicationShared.IdPerfil.Value,
                            firstHorarioAtencion.IdParametroHorario);
                    }

                    if (ParametrizacionHorario != null && ParametrizacionHorario.Intervalo > 0)
                    {
                        countParameterization = parametrizationAgenda.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Domingo).Select(a => a.IdHorarioCita).FirstOrDefault();
                        parametrizationAgendaAlmuerzo = parametrizationAgenda.Where(x => x.IdHorarioCita == (int)FranjaHoraria.Almuerzo).ToList();
                        minimumHourSchedule = parametrizationAgenda.Where(x => x.IdHorarioCita != (int)FranjaHoraria.Almuerzo).Min(x => x.HorarioAperturaAgenda);
                        maximumHourSchedule = parametrizationAgenda.Where(x => x.IdHorarioCita != (int)FranjaHoraria.Almuerzo).Max(x => x.HorarioCierreAgenda);
                        Columns = (parametrizationAgenda.Where(x => x.IdHorarioCita != (int)FranjaHoraria.Almuerzo).Count());

                        intervalo = ParametrizacionHorario.Intervalo;
                        controlIntervalo = intervalo;
                        if (FirstDate.HasValue && LastDate.HasValue)
                        {
                            agendaCentro = await GetScheduleCentro((int)applicationShared.IdPerfil, FirstDate.Value, LastDate.Value);
                            InitCheckAllDaysBlocked(agendaCentro);
                        }
                        DrawSchedule();
                    }
                    else
                    {
                        parametrizationAgenda = null;
                        toastService.ShowWarning("No es posible mostrar la agenda porque falta configuración en la parametrización de horario", "Información");
                    }
                    parametrizationAgendaTemp = parametrizationAgenda;
                }
                else
                {
                    if(parametrizationAgendaTemp.Any())
                    {
                        parametrizationAgenda = parametrizationAgendaTemp;
                        selectedWeek = SetWeekRange(parametrizationAgenda[0].FechaInicioParametrizacion);
                        viewBehind = false;
                    }
                    else
                    {
                        Navigation.NavigateTo("/configuracion/horario-atencion", false);
                    }
                    
                    //hacer el direccionamiento hacia la parametrizacion
                    //configuracion/horario-atencion
                }
                IsShowForSearch = false;
                IsShowForBlock = false;
            }
            catch (Exception ex)
            {
                toastService.ShowError("Ha ocurrido un error cargando la agenda");
            };
        }

        private void InitObjects()
        {
            applicationShared.CitasIntervaParaBloquear = new List<CitasIntervalo>();
            activesCheckbox = new List<ActiveCheckboxAllDay>();
        }

        public bool DrawSchedule()
        {
            try
            {
                string nombreIntervalo = string.Empty;
                string nombreIntervaloEditado = string.Empty;
                listCitasIntervalos = new List<CitasIntervalo>();
                CitasIntervalo citasIntervalos = new CitasIntervalo();
                DiasIntevalo diasIntevalo = new DiasIntevalo();
                List<DiasIntevalo> listDiasIntervalo = new List<DiasIntevalo>();
                Cita cita = new Cita();
                List<Cita> citas = new List<Cita>();
                int nuevahora = 0;
                string nuevosMinutos = string.Empty;


                for (int i = minimumHourSchedule; i <= maximumHourSchedule; i += controlIntervalo)
                {
                    //se validan los intervalos
                    string numbers = i.ToString();
                    string hora = numbers.Length == 3 ? numbers.Substring(0, 1) : numbers.Substring(0, 2);
                    string minutos = numbers.Substring((numbers.Length - 2), 2);
                    nuevahora = int.Parse(hora);
                    nombreIntervalo = string.Concat(hora, ":", minutos); ;

                    // Se valida cuanto se debe aumentar el control de intervalo
                    int proximaMinutos = (int.Parse(minutos) + intervalo) - 60;
                    if (proximaMinutos >= 0) // Ya avanza a la siguiente hora
                    {
                        nuevahora = nuevahora + 1;
                        nombreIntervaloEditado = string.Concat(nuevahora.ToString(), ":", proximaMinutos == 60 || proximaMinutos == 0 ? "00" : proximaMinutos > 60 ? (proximaMinutos - 60).ToString() : proximaMinutos > 9 ? proximaMinutos : "0" + proximaMinutos.ToString());
                        string nuevocontador = nombreIntervaloEditado.Replace(":", "");
                        controlIntervalo = int.Parse(nuevocontador) - @i;
                    }
                    else  //se mantiene en la misma hora
                    {
                        controlIntervalo = intervalo;
                    }

                    int countloop = 0;
                    bool esHoraAlmuerzo = false;
                    //Se recorren los dias de la semana
                    foreach (var param in parametrizationAgenda)
                    {
                        if (param.IdHorarioCita != (short)FranjaHoraria.Almuerzo)
                        {
                            //dateDay = listDates[countloop];
                            int sublistDates = DateTimeExtension.GetindicelistDates(param.Dia);
                            dateDay = listDates[sublistDates];
                            List<AgendaCentro> agendaProcesar = new List<AgendaCentro>();
                            agendaProcesar = agendaCentro.Where(x => x.FechaAgenda == dateDay && x.HorarioInicioAgenda == i).ToList();
                            bool hasDisabledDay = agendaCentro.Any(x => x.FechaAgenda == dateDay && x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado);

                            if (parametrizationAgenda[countloop].HorarioAperturaAgenda <= i && i <= parametrizationAgenda[countloop].HorarioCierreAgenda)
                            {
                                if (parametrizationAgendaAlmuerzo.Any())
                                {
                                    if (i >= parametrizationAgendaAlmuerzo[0].HoraApertura && i < parametrizationAgendaAlmuerzo[0].HoraCierre && parametrizationAgendaAlmuerzo[0].IsActivo)
                                    {
                                        cita = new Cita { IdAgenda = 0, FechaAgenda = dateDay, HoraInicioAgenda = parametrizationAgenda[countloop].HorarioAperturaAgenda, IdEstadoAgenda = 0, TipoCita = TipoCita.Almuerzo, NombreUsuario = null };
                                        esHoraAlmuerzo = true;
                                    }
                                    else
                                    {
                                        bool hasIndividuals = false;
                                        int quotasByInterval = 0;
                                        foreach (var item in agendaProcesar)
                                        {
                                            if (item.FechaAgenda == dateDay && item.HorarioInicioAgenda == i)
                                            {
                                                if (item.IdEstadoAgenda != (int)EstadoAgenda.Inhabilitado
                                                    && item.IdEstadoAgenda != (int)EstadoAgenda.DiaInhabilitado)
                                                {
                                                    hasIndividuals = true;
                                                    citas.Add(new Cita
                                                    {
                                                        IdAgenda = item.IdAgenda,
                                                        FechaAgenda = dateDay,
                                                        HoraInicioAgenda = parametrizationAgenda[countloop].HorarioAperturaAgenda,
                                                        IdEstadoAgenda = item.IdEstadoAgenda,
                                                        TipoCita = TipoCita.Ocupado,
                                                        NombreUsuario = item.NombreUsuario,
                                                        TipoCitaAgenda = item.IdTipoCita,
                                                        AgendaOtroCliente = item.AgendaOtroCliente,
                                                    });
                                                    quotasByInterval++;
                                                }
                                            }
                                        }
                                        bool isScheduleBlock = agendaCentro.Any(x => (x.FechaAgenda == dateDay && x.HorarioInicioAgenda == i)
                                                                                    && ((x.IdEstadoAgenda == (int)EstadoAgenda.Inhabilitado
                                                                                    || x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado)));
                                        bool isDisabledDay = agendaCentro.Any(x => (x.FechaAgenda == dateDay)
                                                                                    && x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado);
                                        TipoCita tipoCita = SetTipoCita(isScheduleBlock, hasIndividuals, hasDisabledDay);
                                        if (tipoCita == TipoCita.Disponible &&
                                            (ParametrizacionHorario.CuposPorIntervalo > 0 && quotasByInterval == ParametrizacionHorario.CuposPorIntervalo))
                                        {
                                            tipoCita = TipoCita.CupoLleno;
                                        }
                                        cita = new Cita
                                        {
                                            IdAgenda = 0,
                                            FechaAgenda = dateDay,
                                            HoraInicioAgenda = i,
                                            IdEstadoAgenda = (isDisabledDay) ? (short)EstadoAgenda.DiaInhabilitado : default,
                                            TipoCita = tipoCita,
                                            IsCheckedForBlock = isScheduleBlock || isDisabledDay,
                                            NombreUsuario = null,
                                            IdHorarioAtencion = param.IdHorarioAtencion
                                        };
                                    }
                                }
                                else
                                {
                                    bool hasIndividuals = false;
                                    int quotasByInterval = 0;
                                    foreach (var item in agendaProcesar)
                                    {
                                        if (item.FechaAgenda == dateDay && item.HorarioInicioAgenda == i)
                                        {
                                            if (item.IdEstadoAgenda != (int)EstadoAgenda.Inhabilitado
                                                && item.IdEstadoAgenda != (int)EstadoAgenda.DiaInhabilitado)
                                            {
                                                hasIndividuals = true;
                                                citas.Add(new Cita
                                                {
                                                    IdAgenda = item.IdAgenda,
                                                    FechaAgenda = dateDay,
                                                    HoraInicioAgenda = parametrizationAgenda[countloop].HorarioAperturaAgenda,
                                                    IdEstadoAgenda = item.IdEstadoAgenda,
                                                    TipoCita = TipoCita.Ocupado,
                                                    NombreUsuario = item.NombreUsuario,
                                                    TipoCitaAgenda = item.IdTipoCita,
                                                    AgendaOtroCliente = item.AgendaOtroCliente
                                                });
                                                quotasByInterval++;
                                            }
                                        }
                                    }
                                    bool isScheduleBlock = agendaCentro.Any(x => (x.FechaAgenda == dateDay && x.HorarioInicioAgenda == i)
                                                                                   && ((x.IdEstadoAgenda == (int)EstadoAgenda.Inhabilitado
                                                                                   || x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado)));
                                    bool isDisabledDay = agendaCentro.Any(x => (x.FechaAgenda == dateDay)
                                                                                && x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado);

                                    TipoCita tipoCita = SetTipoCita(isScheduleBlock, hasIndividuals, hasDisabledDay);
                                    if (tipoCita == TipoCita.Disponible &&
                                        (ParametrizacionHorario.CuposPorIntervalo > 0 && quotasByInterval == ParametrizacionHorario.CuposPorIntervalo))
                                    {
                                        tipoCita = TipoCita.CupoLleno;
                                    }

                                    cita = new Cita
                                    {
                                        IdAgenda = 0,
                                        FechaAgenda = dateDay,
                                        HoraInicioAgenda = i,
                                        IdEstadoAgenda = (isDisabledDay) ? (short)EstadoAgenda.DiaInhabilitado : default,
                                        TipoCita = tipoCita,
                                        IsCheckedForBlock = isScheduleBlock || isDisabledDay,
                                        NombreUsuario = null,
                                        IdHorarioAtencion = param.IdHorarioAtencion
                                    };
                                }

                            }
                            else
                            {
                                cita = new Cita
                                {
                                    IdAgenda = 0,
                                    FechaAgenda = dateDay,
                                    HoraInicioAgenda = i,
                                    IdEstadoAgenda = 0,
                                    TipoCita = TipoCita.BloqueadoPorConfiguracion,
                                    NombreUsuario = null,
                                    IdHorarioAtencion = param.IdHorarioAtencion
                                };
                            }

                            citas.Add(cita);

                            diasIntevalo = new DiasIntevalo
                            {
                                Cita = citas,
                                FechaAgenda = dateDay,
                                DiaInhabilitado = agendaCentro.Any(x => (x.FechaAgenda == dateDay) && x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado)
                            };
                            listDiasIntervalo.Add(diasIntevalo);
                            citas = new List<Cita>();
                            diasIntevalo = new DiasIntevalo();
                        }

                        countloop++;
                    }

                    listCitasIntervalos.Add(new CitasIntervalo
                    {
                        DiasIntervalo = listDiasIntervalo,
                        NombreIntervalo = nombreIntervalo,
                        EsHoraAlmuerzo = esHoraAlmuerzo,
                        MostrarIntervalo = true,
                        HoraIntervalo = i
                    });

                    listDiasIntervalo = new List<DiasIntevalo>();
                }

                int indexItervaloAlmuerzo = 0;
                controlIntervalo = ParametrizacionHorario.Intervalo;
                listCitasIntervalos?.Where(item => item.EsHoraAlmuerzo)?.ToList().ForEach(item =>
                {
                    item.MostrarIntervalo = !(indexItervaloAlmuerzo > 0);
                    indexItervaloAlmuerzo++;
                });
                listCitasIntervalos = listCitasIntervalos.Where(citasIntervalo => citasIntervalo.MostrarIntervalo)?.ToList();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ButtonBlockSelected(ToolButtonOptions options)
        {
            if (options is null) return;
            OptionAgendaMode = options;
            IsShowForBlock = options.ShowBlockSchedule && options.AgendaMode == AgendaMode.Schedule;
            ConsultaNombre = new List<ResultConsultaAgendaDTO>();

            if (!options.ShowBlockSchedule && options.AgendaMode == AgendaMode.Schedule)
            {
                listCitasIntervalos.Where(x => x.DiasIntervalo.Any(x => x.Cita.Any(ct => ct.IsChange)))
                                   .ToList()
                                   .ForEach(x =>
                                   {
                                       x.DiasIntervalo.ForEach(y =>
                                       {
                                           y.IsChange = false;
                                           y.DiaInhabilitado = false;
                                           y.Cita.ForEach(c =>
                                           {
                                               c.IsChange = false;
                                               c.IsCheckedForBlock = false;
                                           });
                                       });

                                   });
                dataInformation._CitasIntervalos = listCitasIntervalos;
                HandlerAllBlock = false;
            }

            parametro = string.Empty;
            shouldRender = true;
            StateHasChanged();
            shouldRender = false;
        }

        private void ButtonSearchSelected(bool isForConsulta)
        {
            IsShowForSearch = isForConsulta;
            shouldRender = true;
            StateHasChanged();
            shouldRender = false;
        }

        private void BlockEntireDay(ChangeEventArgs e, int selectedDay, ref bool isBloked)
        {
            //var daysColumns = new List<int>();
            //foreach (var day in parametrizationAgenda.Where(x => x.IdHorarioCita !=
            //                    (int)FranjaHoraria.Almuerzo))
            //{
            //    int sublistDates = DateTimeExtension.GetindicelistDates1(day.Dia);
            //    daysColumns.Add(sublistDates);
            //}

            //int indexColum = daysColumns.FindIndex(d => d == selectedDay);
            var dateSelected = listDates[selectedDay-1];
            isBloked = (bool)e.Value;
            DaysToBlock[dateSelected.ToString("ddMMyyyy")] = (bool)e.Value;
            HandlerAllBlock = true;
            DayBlock = dateSelected.ToString("ddMMyyyy");
            RenderManager();
        }

        private async Task AppoimentClick(OptionAppoiment options)
        {
            OptionAppoiment = options;
            if (options.ScheduleForm == ScheduleForms.None)
            {
                await ReloadDataSchedule();
            }
            ShowAppoimentForm = (options.ScheduleForm != ScheduleForms.None);
            RenderManager();
        }

        private async Task ClickResultado(long idAgenda, string timeString)
        {
            OptionAppoiment options = new OptionAppoiment
            {
                TimeString = timeString,
                IdAgenda = idAgenda,
                ScheduleForm = ScheduleForms.ScheduleDetail,
                ToolButtonOptions = OptionAgendaMode
            };
            await AppoimentClick(options);
        }

        private async Task BlockScheduleRefresh(bool refresh)
        {
            if (refresh)
            {
                IsShowForBlock = false;
                OptionAppoiment.ScheduleForm = ScheduleForms.None;
                OptionAgendaMode = new ToolButtonOptions
                {
                    AgendaMode = AgendaMode.Schedule,
                    ShowBlockSchedule = false
                };
                await ReloadDataSchedule();
            }
        }

        private void CancelNewAppoiment(bool isCancel)
        {
            ShowAppoimentForm = isCancel;
            RenderManager();
        }

        private async Task SavedNewAppoiment(bool isSaved)
        {
            if (isSaved)
            {
                await ReloadDataSchedule();
            }
        }

        private bool IsAllDayBlock(int day)
        {
            //var daysColumns = new List<int>();
            //foreach (var dayParam in parametrizationAgenda.Where(x => x.IdHorarioCita !=
            //                    (int)FranjaHoraria.Almuerzo))
            //{
            //    daysColumns.Add(dayParam.IdHorarioCita);
            //}

            //int indexColum = daysColumns.FindIndex(d => d == day);
            var blockedDay = listDates[day-1];
            bool isChecked = agendaCentro.Any(x => x.FechaAgenda == blockedDay && x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado);
            if (!isChecked || activesCheckbox.Any())
            {
                // Oscar M: Hack for activate checkbox when selecting each appoiment
                var checkTop = activesCheckbox.FirstOrDefault(x => x.Day == blockedDay.ToString("ddMMyyyy"));
                if (checkTop != null)
                {
                    isChecked = checkTop.Status;
                }
            }
            return isChecked;
        }

        private TipoCita SetTipoCita(bool isBlocked, bool hasIndividuals, bool hasDisabledDay)
        {
            if (hasDisabledDay)
                return TipoCita.BloqueadoPorRecepcion;

            var tipoCita = TipoCita.Disponible;
            if (isBlocked)
            {
                tipoCita = TipoCita.BloqueadoPorRecepcion;
                if (hasIndividuals)
                    tipoCita = TipoCita.CupoLleno;
            }

            return tipoCita;
        }

        List<ActiveCheckboxAllDay> activesCheckbox = new List<ActiveCheckboxAllDay>();
        private void ActivateEnabledDay(ActiveCheckboxAllDay activeCheckboxAllDay)
        {
            if (activesCheckbox.Any(x => x.Day == activeCheckboxAllDay.Day))
                activesCheckbox.Where(x => x.Day == activeCheckboxAllDay.Day)
                               .ToList()
                               .ForEach(x => x.Status = activeCheckboxAllDay.Status);
            else
                activesCheckbox.Add(activeCheckboxAllDay);
            RenderManager();
        }

        private void InitCheckAllDaysBlocked(List<AgendaCentro> agendaCentro)
        {
            foreach (var item in DaysToBlock)
            {
                DaysToBlock[item.Key] = agendaCentro.Any(x => x.FechaAgenda.ToString("ddMMyyyy") == item.Key && x.IdEstadoAgenda == (int)EstadoAgenda.DiaInhabilitado);
            }
        }

        private async Task ReloadDataSchedule()
        {
            isLoading = true;
            RenderManager();
            ShowAppoimentForm = false;
            agendaCentro = await GetScheduleCentro((int)applicationShared.IdPerfil, FirstDate.Value, LastDate.Value);
            DrawSchedule();
            isLoading = false;
            RenderManager();
        }

       
        protected override void OnParametersSet()
        {
            shouldRender = true;
        }

        protected override bool ShouldRender()
        {
            if (shouldRender)
            {
                shouldRender = false;
                return true;
            }
            else return false;
        }

        void RenderManager()
        {
            shouldRender = true;
            StateHasChanged();
            shouldRender = false;
        }
        
        private async Task ObtainAgendaAsync(AgendaDTO agendaDTO)
        {
            AgendaDTOObtained = agendaDTO;
        }
        #endregion
    }
}
