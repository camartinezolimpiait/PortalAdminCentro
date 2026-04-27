using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.ResumenCDA
{
	public partial class ResumenCDA
	{
		#region Variables

		[Parameter]
		public PagoPinCDA PagoPinCda { get; set; }

		[Parameter]
		public int PasosCompraPin { get; set; }

		[Parameter]
		public Action<int> OnNavegarResumen { get; set; }

		[Parameter]
		public HashSet<int> PasosVisitados { get; set; } = new HashSet<int>();

		private bool MostrarDetalle = false;

		#endregion Variables

		#region Metodos

		protected override async Task OnInitializedAsync() 
		{
            _ = PagoPinCda.ClienteCompra == (int)EnumTipoCliente.CDA;

        }


        private void MostrarOcultardetalle()
		{
			MostrarDetalle = !MostrarDetalle;
		}


		#endregion Metodos
	}
}

