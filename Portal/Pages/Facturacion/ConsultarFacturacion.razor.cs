using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.Facturacion;

/// <summary>
/// Componente wrapper para la gestión de facturación electrónica
/// Maneja la navegación entre estados y delega la lógica al componente hijo
/// </summary>
public partial class ConsultarFacturacion
{
    // Inicio código generado por GitHub Copilot

    #region Properties

    [Parameter]
    public string Estado { get; set; }

    #endregion Properties

    #region Protected Methods

    protected override async Task OnInitializedAsync()
    {
        // Si no viene estado en la URL → usar "PorRevisar"
        if (string.IsNullOrWhiteSpace(Estado))
        {
            Estado = "PorRevisar";
        }

        // Cambia la URL sin refrescar la página
        NavManager.NavigateTo($"/ConsultarFacturacion/{Estado}", replace: true);

        await base.OnInitializedAsync();
    }

    #endregion Protected Methods

    // Fin código generado por GitHub Copilot
}