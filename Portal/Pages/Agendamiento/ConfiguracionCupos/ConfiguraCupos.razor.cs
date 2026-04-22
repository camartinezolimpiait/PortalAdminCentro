using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.ConfiguracionCuposReglas;
using portalAdministrativoSISEC.Entidades.Agendamiento.HorarioAtencion;
using portalAdministrativoSISEC.Entidades.Agendamiento.Politica;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Services.Agendamiento;
using portalAdministrativoSISEC.Services.Agendamiento.ConfigruracionCuposReglas;
using portalAdministrativoSISEC.Services.Agendamiento.HorarioAtencion;

namespace portalAdministrativoSISEC.Pages.Agendamiento.ConfiguracionCupos
{
    public partial class ConfiguraCupos
    {
        #region Constructor

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IHorarioAtencionService HorarioAtencionService { get; set; }

        [Inject]
        public IConfiguracionCuposReglas ConfiguracionCuposReglasService { get; set; }

        [Inject]
        public IParametrizacionHorarioService ParametrizacionHorarioService { get; set; }

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

        [Inject]
        private IToastService ToastService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        #endregion Constructor

        #region Variables

        private ApplicationShared applicationShared = new();
        private bool IsIlimitado { get; set; } = false;
        private string CapacidadMaxima { get; set; }
        private int IntervaloCitas { get; set; } = 5;
        private int TotalCitasSemana { get; set; } = 0;
        private int TotalCitasSabado { get; set; } = 0;
        private int TotalCitasDomingo { get; set; } = 0;
        private int TotalCitasLV { get; set; } = 0;
        private int TotalCitasS { get; set; } = 0;
        private int TotalCitasD { get; set; } = 0;
        private int CantidadDiasActivosSemana { get; set; } = 0;
        private bool IsLoading { get; set; } = true;
        private bool GetNewSchedule { get; set; } = false;

        #endregion Variables

        #region Protected Methods

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;
            try
            {
                var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
                applicationShared = result.Value;
                CapacidadMaxima = "Max parametros centro";
                if (applicationShared.CuposIntervalo == 0)
                    applicationShared.CuposIntervalo = 1;

                IsIlimitado = applicationShared.IdTipoAgenda == 2;
                await CalcalularRegistroAsync();
            }
            catch (ApplicationException ae)
            {
                IsLoading = false;
                ToastService.ShowWarning(ae.Message.ToString(), "Información");
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ToastService.ShowWarning(ex.Message.ToString(), "Información");
            }
            IsLoading = false;
            await base.OnParametersSetAsync();
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Metodos para mostrar u ocultar parametrizacion de intervalos
        /// </summary>
        private void Show()
        {
            IsIlimitado = !IsIlimitado;
        }

        /// <summary>
        /// Metodo para volver a la pantalla anterior
        /// </summary>
        /// <returns></returns>
        private async Task Atras()
        {
            await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
            Navigation.NavigateTo("/configuracion/horario-agendamiento", false);
        }

        /// <summary>
        /// Metodo para almacenar los datos parametrizados y pasar a siguiente pantalla
        /// </summary>
        /// <returns></returns>
        private async Task GuardarConfiguracionReglas()
        {
            applicationShared.IdTipoAgenda = (short)(IsIlimitado ? 2 : 1);
            applicationShared.CuposIntervalo = (short)(IsIlimitado ? 0 : applicationShared.CuposIntervalo);

            var estadoProceso = await ParametrizacionHorarioService.SaveParametrizationSchedule(new ParametrizacionHorario
            {
                IdPerfil = applicationShared.IdPerfil.Value,
                UsuarioCreacion = applicationShared.UserName,
                Intervalo = ParametrizacionHorario.Intervalo,
                DisponibilidadAgendaDias = ParametrizacionHorario.DisponibilidadAgendaDias,
                CuposPorIntervalo = applicationShared.CuposIntervalo,
                TiempoHoraMinimoAgenda = ParametrizacionHorario.TiempoHoraMinimoAgenda,
                TiempoHoraMinimoCancelar = ParametrizacionHorario.TiempoHoraMinimoCancelar,
                IdPoliticaAgendamiento = ParametrizacionHorario.IdPoliticaAgendamiento,
                IdTipoAgenda = applicationShared.IdTipoAgenda,
                IdParametroHorario = applicationShared.IdParametroHorario,
                IdParametrizacionHorario = ParametrizacionHorario.IdParametrizacionHorario
            }, applicationShared.FechaInicioParametrizacion);

            if (estadoProceso.IdParametroHorario > 0)
            {
                if (estadoProceso.IdParametroHorario != applicationShared.IdParametroHorario)
                {
                    applicationShared.IdParametroHorario = ConfiguracionHorarios[0].IdParametroHorario;
                    applicationShared.IsAgendaFutura = true;
                }
                await ProtectedSessionStore.SetAsync(applicationShared.NameLocalStorage, applicationShared);
                Navigation.NavigateTo("/configuracion/configuracion-reglas", false);
            }
            else
                ToastService.ShowWarning(@"Ha ocurrido un error, intente nuevamente.", "Información");
        }

        /// <summary>
        /// Calcula  los cupos diarios
        /// </summary>
        /// <returns></returns>
        private async Task CalcalularRegistroAsync()
        {
            TotalCitasSemana = 0;
            TotalCitasSabado = 0;
            TotalCitasDomingo = 0;

            if (ConfiguracionHorarios == null)
            {
                DateTime startDate = DateTime.Now;
                GetNewSchedule = applicationShared.IsAgendaFutura && applicationShared.CreateNewConfiguration;
                ConfiguracionHorarios = await HorarioAtencionService.GetConfiguracionHorario(applicationShared.IdPerfil ?? 0, startDate, GetNewSchedule);
                applicationShared.FechaInicioParametrizacion = ConfiguracionHorarios[0].FechaInicioParametrizacion;
            }

            ParametrizacionHorario ??= await ParametrizacionHorarioService.GetParametrizationScheduleByProfileId(applicationShared.IdPerfil.Value, ConfiguracionHorarios[0].IdParametroHorario);

            IntervaloCitas = ParametrizacionHorario.Intervalo;

            CalcularCuposDiarios(ConfiguracionHorarios);
            TotalCitasLV = (TotalCitasSemana * applicationShared.CuposIntervalo) / CantidadDiasActivosSemana;
            TotalCitasS = (TotalCitasSabado * applicationShared.CuposIntervalo);
            TotalCitasD = (TotalCitasDomingo * applicationShared.CuposIntervalo);
        }

        /// <summary>
        /// Calcula los cupos diarios
        /// </summary>
        /// <param name="configuracionHorarios"></param>
        private void CalcularCuposDiarios(List<ConfiguracionHorario> configuracionHorarios)
        {
            var diasHabiles = new int[] { 1, 2, 3, 4, 5 };
            var existeConfiguracionHoraAlmuerzo = configuracionHorarios.Any(t => t.IdHorarioCita == (int)FranjaHoraria.Almuerzo && t.IsActivo) ? 1 : 0;
            CantidadDiasActivosSemana = configuracionHorarios.Count(t => t.IsActivo && diasHabiles.Contains(t.IdHorarioCita));
            foreach (var item in configuracionHorarios.OrderBy(t => t.IdHorarioCita))
            {
                if (item.IdHorarioCita == (int)FranjaHoraria.Almuerzo)
                {
                    TotalCitasSemana = TotalCitasSemana != 0 ? TotalCitasSemana - CantidadDiasActivosSemana : 0;
                    TotalCitasSabado = TotalCitasSabado != 0 ? TotalCitasSabado - existeConfiguracionHoraAlmuerzo : 0;
                    TotalCitasDomingo = TotalCitasDomingo != 0 ? TotalCitasDomingo - existeConfiguracionHoraAlmuerzo : 0;
                }
                else
                {
                    int Contador;
                    for (int i = item.HorarioAperturaAgenda; i <= item.HorarioCierreAgenda; i += Contador)
                    {
                        if (i == 0)
                            break;
                        string numbers = i.ToString();
                        string hora = numbers.Length == 3 ? numbers[..1] : numbers[..2];
                        string minutos = numbers.Substring((numbers.Length - 2), 2);
                        int nuevahora = int.Parse(hora);

                        // Se valida cuanto se debe aumentar el control de intervalo
                        int proximaMinutos = (int.Parse(minutos) + IntervaloCitas) - 60;

                        if (proximaMinutos >= 0)
                        {
                            nuevahora++;

                            string partx = proximaMinutos > 9
                                ? $"{proximaMinutos}"
                                : "0" + proximaMinutos.ToString();

                            string party = proximaMinutos > 60
                                    ? (proximaMinutos - 60).ToString()
                                    : partx;

                            string partz = proximaMinutos == 60 || proximaMinutos == 0
                                ? "00"
                                : party;

                            string nombreIntervaloEditado = string.Concat(nuevahora.ToString(), ":", partz);
                            string nuevocontador = nombreIntervaloEditado.Replace(":", "");
                            Contador = int.Parse(nuevocontador) - @i;
                        }
                        else
                            Contador = IntervaloCitas;

                        if (item.Dia == "Sabado" && item.IsActivo)
                            TotalCitasSabado++;
                        else if (item.Dia == "Domingo" && item.IsActivo)
                            TotalCitasDomingo++;
                        else
                            TotalCitasSemana++;
                    }
                }
            }
        }

        #endregion Private Methods
    }
}