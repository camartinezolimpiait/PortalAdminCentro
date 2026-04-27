using portalAdministrativoSISEC.Application.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Contracts.Agendamiento.Perfil
{
    public interface IPerfilAgendaService
    {
        Task<GetCentroResponse> GetInformationCentroId(GetCentroRequest consultaCentro);
        Task<GetComercioResponse> GetInformationComercio(GetCentroRequest consultaCentro);
        Task<CentroResponse> UpdateCentroId(DataContact centro);
    }
}



