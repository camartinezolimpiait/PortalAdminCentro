using portalAdministrativoSISEC.Application.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util.Helpers
{
    public interface IApiService
    {
        Task<ApiResponse<T>> CallApiAsync<T>(string baseAddress, string apiEndpoint, HttpMethod method, object content = null, string apiKey = "SisecAdmin");
    }
}

