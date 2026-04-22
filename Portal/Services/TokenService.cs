using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Const;

namespace portalAdministrativoSISEC.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient httpClient;
        public TokenService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<TokenBearer> GetToken()
        {
			
			var credenciales = new Credenciales { userName= CredencialesConst.DescripcionCredenciales[EnumCredenciales.ClientId].ToString(), userPassword = CredencialesConst.DescripcionCredenciales[EnumCredenciales.ClientSecret].ToString() };

            var token = await httpClient.PostJsonAsync<TokenBearer>("Token/Login2", credenciales);

            return token;
        }
    }
}
