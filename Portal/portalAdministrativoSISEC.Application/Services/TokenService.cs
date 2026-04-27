using portalAdministrativoSISEC.Application.Contracts;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

            var response = await httpClient.PostAsJsonAsync("Token/Login2", credenciales);
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadFromJsonAsync<TokenBearer>();

            return token ?? new TokenBearer();
        }
    }
}


