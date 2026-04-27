using portalAdministrativoSISEC.Application.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Contracts
{
    public interface ITokenService
    {
        Task<TokenBearer> GetToken();
    }
}


