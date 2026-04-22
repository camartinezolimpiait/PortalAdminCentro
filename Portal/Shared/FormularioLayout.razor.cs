using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using portalAdministrativoSISEC.Data;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Shared
{
    public partial class FormularioLayout
    {
        [Inject]
        NavigationManager navigation { get; set; }
        [Inject]
        ProtectedSessionStorage ProtectedSessionStore { get; set; }
        ApplicationSevice menuService = new ApplicationSevice();
        private string url = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var protectedSessionStore = await ProtectedSessionStore.GetAsync<ApplicationSevice>(menuService.NameLocalStorage);
                menuService = protectedSessionStore.Value;
                if (menuService == null || !menuService.Autenticado)
                    await LogOutAsync();

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                navigation.NavigateTo("/", true);
                // throw;
            }
        }
        protected async Task LogOutAsync()
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
