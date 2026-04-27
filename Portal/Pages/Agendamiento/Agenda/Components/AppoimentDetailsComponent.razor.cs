using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using portalAdministrativoSISEC.Application.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.Agendamiento.Agenda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Agendamiento.Agenda.Components
{
	public partial class AppoimentDetailsComponent
	{
		[Inject]
		public IAgendaService _agendaService { get; set; }

		[Parameter]
		public OptionAppoiment OptionAppoiment { get; set; }
		[Inject]
		private IToastService toastService { get; set; }

		[Parameter]
		public EventCallback<OptionAppoiment> CancelScheduleCallback { get; set; }

        [Parameter]
        public EventCallback<AgendaDTO> AgendaDTOCallback { get; set; }

            [Inject]
		ProtectedSessionStorage ProtectedSessionStore { get; set; }

		private ApplicationShared applicationShared = new ApplicationShared();
		public AgendaDTO AgendaDto { get; set; }
		public TipoDocumentoDTO TipoDocumentoDto { get; set; }
		public TramiteDTO TramiteDto { get; set; }
		public MotivoDTO MotivoDto { get; set; }
		public bool IsLoading { get; set; }
		public string TipoCita { get; set; }
		public List<TipoDocumentoDTO> TiposDeDocumento { get; private set; }

		public bool isArmas = false;
		private TipoDocumentoDTO resultadoDocumento;

		protected override async Task OnInitializedAsync()
		{
			var result = await ProtectedSessionStore.GetAsync<ApplicationShared>(applicationShared.NameLocalStorage);
			applicationShared = result.Value;
		}

		protected override async Task OnParametersSetAsync()
		{
			IsLoading = true;
			try
			{
				if (OptionAppoiment.IdAgenda.HasValue)
				{
					AgendaDto = await _agendaService.GetAppoimentById(OptionAppoiment.IdAgenda.Value);
					isArmas = AgendaDto.IdTipoCliente == applicationShared.IdClienteArmas;

					if (AgendaDto != null && AgendaDto.IdAgenda>0)
					{
                        await ObtainAgendaDTODetails();
                        TipoCita = (AgendaDto.IdTipoCita == TipoCitaAgenda.Enrolamiento) ? "Enrolamiento" : "Continuación";
						if (AgendaDto.DatosPersonaDTO != null)
						{
							// Problematica en estudio ->
							// 
							// Servicio de consulta de por idtipodocumento -> Implementacion Antigua
							TipoDocumentoDto = await _agendaService.GetDocumentTypeById(AgendaDto.DatosPersonaDTO.IdTipoIdentificacion);
							// Servicios de obtencion de tipos de documento para nueva implementacion 
							TiposDeDocumento = await _agendaService.GetTipoDocumento();
							TiposDeDocumento = filtrarTipoDocumento(TiposDeDocumento, applicationShared.Plataforma == EnumTipoCliente.CEA.ToString() ? 2 : 1);
							// Validamos si existe en los ids segun el tipo de negocio
							ObtenerTipoDocumento(AgendaDto.DatosPersonaDTO.IdTipoIdentificacion);

						}
						TramiteDto = await _agendaService.GetTramitePorId((Int16)AgendaDto.IdTramite);
						AgendaDto.TramiteDto = TramiteDto;
						MotivoDto = await _agendaService.GetMotivoPorId(AgendaDto.IdMotivo);
						await ObtainAgendaDTODetails();
					}
				}
				IsLoading = false;
			}
			catch (Exception ex)
			{
				toastService.ShowError(@"Se ha presentado un error inicializando el formulario, error: " + ex.Message);
				IsLoading = false;
			}

		}

		private async Task CancelScheduleClick(MouseEventArgs e)
		{
			OptionAppoiment.ScheduleForm = ScheduleForms.CancelSchedule;
			await CancelScheduleCallback.InvokeAsync(OptionAppoiment);
		}

		private async Task CancelScheduleClick(MouseEventArgs e, bool noAction)
		{
			OptionAppoiment.ScheduleForm = ScheduleForms.None;
			await CancelScheduleCallback.InvokeAsync(OptionAppoiment);
		}

		private async Task ReScheduleClick(MouseEventArgs e)
		{
			OptionAppoiment.ScheduleForm = ScheduleForms.ReSchedule;
			OptionAppoiment.ToolButtonOptions.AgendaMode = AgendaMode.Schedule;
			OptionAppoiment.Agenda = AgendaDto;
			await CancelScheduleCallback.InvokeAsync(OptionAppoiment);
		}

		public string convertirHoraEstandar(string Hora)
		{
			string tiempo = "";
			if (int.TryParse(Hora.Replace(":", ""), out int h) && h >= 0 && h <= 2359)
			{
				int horas = h / 100;
				int minutos = h % 100;
				string formato = horas >= 12 ? "PM" : "AM";
				horas = horas > 12 ? horas - 12 : horas;
				horas = horas == 0 ? 12 : horas;
				tiempo = $"{horas:D2}:{minutos:D2} {formato}";
			}
			return tiempo;
		}
		private async Task ObtainAgendaDTODetails()
		{
			await AgendaDTOCallback.InvokeAsync(AgendaDto);
		}
		private void ObtenerTipoDocumento(int tipoDocumento)
		{
			switch (applicationShared.Plataforma == EnumTipoCliente.CEA.ToString() ? 2 : 1)
			{
				case 1:
					resultadoDocumento = TiposDeDocumento.Find(x => x.CodigoEquCRC == tipoDocumento);
					break;
				case 2:
					resultadoDocumento = TiposDeDocumento.Find(x => x.CodigoEquCEA == tipoDocumento);
					break;
			}

			
		}

		private List<TipoDocumentoDTO> filtrarTipoDocumento(List<TipoDocumentoDTO> filtroDocumento, int tipoCliente)
		{
			List<TipoDocumentoDTO> documentos = new List<TipoDocumentoDTO>();
			if (filtroDocumento.Count() > 0)
			{
				var codigosValidos = new[] { 1, 2, 3, 5, 13 };
				switch (tipoCliente)
				{
					case 1:
						documentos = filtroDocumento.Where(doc => doc.ClienteId == tipoCliente && doc.CodigoEquCRC.HasValue && codigosValidos.Contains(doc.CodigoEquCRC.Value)).Select(doc => new TipoDocumentoDTO
						{
							abreviatura = doc.abreviatura,
							ClienteId = doc.ClienteId,
							Codigo = doc.Codigo,
							CodigoEquCEA = doc.CodigoEquCEA,
							CodigoEquCRC = doc.CodigoEquCRC,
							descripcion = doc.descripcion,
							Id = doc.Id
						}).ToList();

						break;
					case 2:

						documentos = filtroDocumento.Where(doc => doc.ClienteId == tipoCliente && doc.CodigoEquCEA.HasValue && codigosValidos.Contains(doc.CodigoEquCEA.Value)).Select(doc => new TipoDocumentoDTO
						{
							abreviatura = doc.abreviatura,
							ClienteId = doc.ClienteId,
							Codigo = doc.Codigo,
							CodigoEquCEA = doc.CodigoEquCEA,
							CodigoEquCRC = doc.CodigoEquCRC,
							descripcion = doc.descripcion,
							Id = doc.Id
						}).ToList();

						break;
				}

			}
			return documentos;

		}

	}
}


