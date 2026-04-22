using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using portalAdministrativoSISEC.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Services.MiLicencia;

namespace portalAdministrativoSISEC.Shared
{
    public partial class NavSubMenuPrincipal
    {
        [Inject]
        IApiMilicenciaService MilicenciaService { get; set; }

        [Inject]
        NavigationManager Navigation { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        DataInformation dataInformation { get; set; }
        ApplicationSevice menuService = new ApplicationSevice();
        bool isLoading = false;
        string padre = "";
        string currentPath = "";
		protected override async Task OnInitializedAsync()
        {
			currentPath = Navigation.ToBaseRelativePath(Navigation.Uri);
			isLoading = true;
            var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>(menuService.NameLocalStorage);
            menuService = protectedSessionStore.Value;
            if (menuService != null && menuService.Autenticado && menuService.menuList == null)
            {
                menuService.menuList = await dataInformation.GetMenuData(menuService.UserID);
            }

            isLoading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
             
			isLoading = true;
			var menuPadre = menuService.menuList.FirstOrDefault(m => currentPath.StartsWith(m.Ruta.ToLower()));

			// Si se encuentra un menú padre y no se han cargado aún los submenús
			if (menuService != null && menuService.SubMenuList == null && menuPadre != null)
			{
				int idPadre = menuPadre.Id;  // Obtener el ID del menú padre
											 // Cargar los submenús basados en el ID del padre
				menuService.SubMenuList = menuService.menuList.Where(x => x.Padre == idPadre).ToList();
				padre = menuPadre.Pagina;  // Guardar el nombre de la página del menú padre
			}
			isLoading = false;
			await base.OnParametersSetAsync();

        }
    }
}
