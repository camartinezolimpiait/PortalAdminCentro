using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Shared
{
    public partial class NavMenuPrincipal
    {
        #region Fields

        public List<ConvenioCentro> listadoConvenios = new List<ConvenioCentro>();

        public ConsultaCentoId IdCentro;

        private ApplicationSevice menuService = new ApplicationSevice();

        private bool isLoading = false;

        private GetDataResponseCentro getCentroResponse = new GetDataResponseCentro();

        #endregion Fields

        #region Properties

        [Inject]
        private IMiLicenciaService _miLicenciaService { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        [Inject]
        private DataInformation dataInformation { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        #endregion Properties

        #region Protected Methods

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>(menuService.NameLocalStorage);
            menuService = protectedSessionStore.Value;
            var centroShared = await ProtectedSessionStore.GetAsync<GetCentroResponse>("centroStorage");
            getCentroResponse = centroShared.Value.Respuesta;
            isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (menuService != null && menuService.Autenticado && menuService.menuList == null)
            {
                menuService.menuList = await dataInformation.GetMenuData(menuService.UserID);
                menuService.MenuListPrincipal = menuService.menuList.Where(x => x.Padre == 0).ToList();
                IdCentro = new ConsultaCentoId
                {
                    IdCentro = getCentroResponse.IdCentro,
                    CodigoDestino = menuService.Plataforma == EnumTipoCliente.CRC.ToString()
                        ? getCentroResponse.CodigoRUNT.ToString()
                        : null // si es CEA, queda null
                };
                if (menuService.Plataforma != "CDA")
                {
                    // Obtener convenios según plataforma, con switch para claridad
                    listadoConvenios = menuService.Plataforma == EnumTipoCliente.CEA.ToString()
                        ? await _miLicenciaService.ObtenerConveniosCEA(IdCentro)
                        : await _miLicenciaService.ObtenerConveniosCRC(IdCentro);

                    // ConvenioCompraPin es true solo si hay algún convenio con esos orígenes; si la lista es null, será false
                    bool convenioCompraPin =
                        listadoConvenios?.Any(c =>
                            c.IdOrigenPin == (int)EnumTipoPago.PinDirecto ||
                            c.IdOrigenPin == (int)EnumTipoPago.PSEColpatria) == true;

                    if (!convenioCompraPin)
                    {
                        // Quitar "compradepin" de una sola vez, sin foreach
                        menuService.MenuListPrincipal =
                            menuService.MenuListPrincipal?.Where(m => m.Ruta != "compradepin").ToList()
                            ?? new List<MenuInfo>();
                    }
                }
            }
            await base.OnParametersSetAsync();

            var CurrentPath = Navigation.ToBaseRelativePath(Navigation.Uri);

            if (!menuService.menuList.Any(x => CurrentPath.Contains(x.Ruta)))
            {
                await _miLicenciaService.ShowNotificacion(NotificationStatus.Warning, "No se encontro la ruta especificada");
                Navigation.NavigateTo("/configuracion/PerfilMilicencia", false);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void GetIsCliked(MenuInfo mn)
        {
            if (!string.IsNullOrEmpty(mn.Ruta))
            {
                menuService.SubMenuList = [.. menuService.menuList.Where(x => x.Padre == mn.Id)];
            }
        }

        #endregion Private Methods
    }
}