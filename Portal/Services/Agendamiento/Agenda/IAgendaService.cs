using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.Agenda
{
    public interface IAgendaService
    {
        Task<List<AgendaCentro>> GetScheduleCentro(int idPerfil, DateTime fromDate, DateTime toDate);
        Task<List<TipoDocumentoDTO>> GetTipoDocumento();
        Task<List<CategoriaDTO>> GetCategoriaId(int id);
        Task<List<TipoCitaDTO>> GetTipoCita();
        Task<List<TramiteDTO>> GetTramiteId(int idCliente);
        Task<List<MotivoDTO>> GetMotivos();
        Task<bool> GuardarAgenda(NuevaAgendaMLDTO agendaNuevaCitas);
        Task<bool> SaveSchedule(AgendaDTO schedule);
        Task<bool> BlockSchedule(AgendaDTO schedule);
        Task<bool> BlockScheduleList(List<AgendaDTO> scheduleList);
        Task<bool> CancelSchedule(long scheduleId, string usuarioCancelacion);
        Task<bool> ReSchedule(AgendaDTO reschedule);
        Task<AgendaDTO> GetAppoimentById(long scheduleId);
        Task<List<ResultConsultaAgendaDTO>> GetAppoimentByParametro(int idPerfil, string startDate, string parametro);
        Task<TipoDocumentoDTO> GetDocumentTypeById(int idDocumentType);
        Task<TramiteDTO> GetTramitePorId(Int16 idTramite);
        Task<MotivoDTO> GetMotivoPorId(Int16 idMotivo);
        Task<AgendaDTO> CheckScheduleAvailableByDatosPersonaId(int idPerfil, int idIdentificationType, string numberIdentification, string startSearchDate);
    }
}
