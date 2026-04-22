using portalAdministrativoSISEC.Entidades.PortalAdministrativo;
using System.Threading.Tasks;


namespace portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo
{
	public interface IPortalAdministrativoService
	{
		Task<T> ConsumirServicioGET<T>(string url, bool createToken = true);

		Task<T> ConsumirServicioPOST<T>(string metodo, object json, bool createToken = true);

		Task<bool> RegistrarLogCotizador(DetalleLog detalleLog);

		Task RegisterExceptionLog(ExceptionLog exceptionLog);
		Task RegisterInfoLog(InfoLog infoLog);


    }
}