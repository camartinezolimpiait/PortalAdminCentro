using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data.Pines;
using portalAdministrativoSISEC.Entidades.Devolucion.ConsultaInfoPin;
using System;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.PagoCuota.ConfirmacionCompraCuota
{
    public partial class ConfirmacionCompraCuota
    {
        #region Variables
        [Parameter]
        public ResponsePagoCuota<RespuestaGeneric<object>> Pago { get; set; }

        [Parameter]
        public decimal ValorCompra { get; set; } = 0;

        public bool compraRealizada { get; set; }

        [Parameter]
        public ResponseDTO<Entidad> InfoPin { get; set; }

        private string Pin => InfoPin.Entidad.Pin;
        private string TodaysDate = DateTime.Now.ToString("dd-MM-yyyy");
        #endregion

        protected override async Task OnInitializedAsync()
        {

            try
            {
                compraRealizada = Pago?.Respuesta?.Codigo == 0;
               
            }
            catch (Exception ex)
            {

                compraRealizada = false;
            }
        }

        private void VolverAlInicio()
        {
            NavManager.NavigateTo("/compradepin"); // Redirige a la página de inicio
        }
    }
}

