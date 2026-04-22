using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Services.MiLicencia
{
    public interface IApiMilicenciaService
    {
        Task<T> ConsumirServicioGET<T>(string url, bool createToken = true, string googleCaptcha = "");

        Task<T> ConsumirServicioPOST<T>(string metodo, object json, bool createToken = true,string googleCaptcha="");

		void LogMessage(string v);
	}
}
