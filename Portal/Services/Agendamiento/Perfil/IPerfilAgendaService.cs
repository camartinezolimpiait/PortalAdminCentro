using portalAdministrativoSISEC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.Agendamiento.Perfil
{
    public interface IPerfilAgendaService
    {
        Task<GetCentroResponse> GetInformationCentroId(GetCentroRequest consultaCentro);
        Task<GetComercioResponse> GetInformationComercio(GetCentroRequest consultaCentro);
        Task<CentroResponse> UpdateCentroId(DataContact centro);
    }
}

