namespace portalAdministrativoSISEC.Application.PortalAdministrativo;

public interface IPermisoService
{
    Task<bool> TienePermisoParaCompraPin(int idCentro, int plataforma);
}
