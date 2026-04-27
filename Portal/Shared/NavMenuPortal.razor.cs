using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Application.Data;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Shared
{
    public partial class NavMenuPortal
    {
        #region Constructor
        [Inject]
        DataInformation dataInformation { get; set; }
        [Inject]
        NavigationManager Navigation { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }

        #endregion

        #region Variables
        private bool collapseNavMenu = true;
        private bool expandSubNav;
        private int clickedMenu = 0;
        private bool hasPageName = true;
        private int prevClickedMenu = 0;
        private ApplicationSevice menuService = new ApplicationSevice();
        #endregion

        #region Metodos
        protected override async Task OnInitializedAsync()
        {
            var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>("applicationService");
            menuService = protectedSessionStore.Value;

        }
        protected override async Task OnParametersSetAsync()
        {
            if (menuService.Autenticado && menuService.menuList == null)
            {
                menuService.menuList = await dataInformation.GetMenuData(menuService.UserID);
            }
            await base.OnParametersSetAsync();
        }

        private void GetIsCliked(MenuInfo mn)
        {
            if (menuService.MustChangePassword)
                Navigation.NavigateTo("/ChangePassword", true);

            clickedMenu = mn.Id;
            if (prevClickedMenu != clickedMenu)
            {
                expandSubNav = false;
                if (mn.PageName != "" || mn.MenuName == "Home")
                {
                    hasPageName = true;
                }
                else
                {
                    expandSubNav = !expandSubNav;
                    hasPageName = false;
                }
            }
            else
            {
                expandSubNav = !expandSubNav;
            }
            prevClickedMenu = clickedMenu;

            if (!string.IsNullOrEmpty(mn.Pagina))
            {
                Navigation.NavigateTo("/" + mn.Pagina, false);
            }
        }

        #endregion
    }
}
