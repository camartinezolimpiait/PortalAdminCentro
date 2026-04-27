namespace portalAdministrativoSISEC.Application.PortalAdministrativo;

public interface IOfuscamientoService
{
    Task<string> Ofuscamiento(string data);
}
