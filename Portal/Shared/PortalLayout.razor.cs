using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Shared
{
    public partial class PortalLayout
    {
        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private DataInformation dataInformation { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        [Inject]
        private ProtectedSessionStorage ProtectedSessionStore { get; set; }

        private ApplicationSevice menuService = new ApplicationSevice();

        private IJSObjectReference _module;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>(menuService.NameLocalStorage);
                menuService = protectedSessionStore.Value;
                if (menuService == null || !menuService.Autenticado)
                    await LogOut();
                else
                {
                    if (!navigation.Uri.Contains("ChangePassword") && menuService.MustChangePassword)
                        navigation.NavigateTo("/ChangePassword", true);

                    if (menuService.Autenticado && string.IsNullOrEmpty(menuService.FirstNameUser))
                        menuService.FirstNameUser = await dataInformation.GetUserName(menuService.UserID);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                navigation.NavigateTo("/", true);
                // throw;
            }
        }

        protected async Task LogOut()
        {
            if (menuService != null)
            {
                menuService.Autenticado = false;
                menuService.ShowCaptcha = true;
                menuService.UserID = string.Empty;
                await ProtectedSessionStore.DeleteAsync(menuService.NameLocalStorage);
            }

            navigation.NavigateTo("/", true);
        }
    }
}