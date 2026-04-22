using portalAdministrativoSISEC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services
{
    public interface ICategoriaService
    {
        Task<ResponseBody> GetCategoria(String token);
    }
}
