using Microsoft.AspNetCore.Components.Forms;
using portalAdministrativoSISEC.Entidades;
using portalAdministrativoSISEC.Entidades.SuperTransporte;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Contracts.SuperTransporte
{
    public interface ISuperTransporteService
    {
        void SetPlataforma(string plataforma);
        Task<RespuestaPutCentro> PutCentro<T>(T objeto, int idCentro);
        Task<RespuestaServiciosGetVigilado> PutVigilado<T>(T objeto, int idVigilado);
        Task<int> PostFile(IBrowserFile file);
        Task<bool> DeleteFile(int idFile);
        Task<VigiladoFilterDto> GetVigilado(string nit);
        Task<RespuestaServiciosGet> GetCentro(int idCentro, string populate);
        Task<RespuestaServiciosGetCEA> GetCentroCEA(int idCentro, string populate);
        Task<RespuestaServiciosGet> GetCentroMultiPopulate(int idCentro, List<string> populates);
        Task<RespuestaServiciosGetCEA> GetCentroMultiPopulateCEA(int idCentro, List<string> populates);
        Task<RespuestaServiciosGetByRunt> GetCentroByRunt(long runt);
        Task<RespuestaServiciosGet> PostCentro<T>(T objeto);
        Task<List<SelectSuperTransporteDTO>> GetListaMaestra(string metodo, List<SelectSuperTransporteIndex> campos);
        Task<List<SelectSuperTransporteDTO>> GetListaMaestraSuperT(string lista);
        Task<RespuestaServiciosGet> GetVigiladoByIdCentro(int idCentro);
        Task<int> PostVigilado<T>(T objeto);
        Task<string> GetBlobSasToken();
        Task<VigiladoDto> GetListaMaestraSuperVigilados(string nit);
    }
}

