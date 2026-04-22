using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Data.CompraPin;
using portalAdministrativoSISEC.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Util.Const.ApiPortalAdministrativo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.TramiteCDA
{
	public partial class Tramite
	{
		#region Variables

		[Parameter]
		public PagoPinCDA PagoPinCda { get; set; }

		[Parameter]
		public EventCallback<PagoPinCDA> PagoPinChanged { get; set; }

		public List<SeleccionTramites> ListadoTramites { get; set; } = new();

		private bool isLoading = false;

		#endregion Variables

		#region Metodos

		protected override async Task OnInitializedAsync()
		{
			isLoading = true;

			await InicializacionTramites();

			isLoading = false;
		}

		private async Task InicializacionTramites()
		{
			await AddSelectionTramite(EnumTramite.PrimeraVez, false);
			if (PagoPinCda.MayorEdad)
			{
				await AddSelectionTramite(EnumTramite.Recategorizar, false);
			}
			else if(PagoPinCda.TipoTramite != (int)EnumTramite.PrimeraVez)
			{
				PagoPinCda.TipoTramite = 0;
			}
			await AddSelectionTramite(EnumTramite.PrimeraVezInstructor, true);
			await AddSelectionTramite(EnumTramite.RecategorizarInstructor, true);

			if (PagoPinCda.OpcionTramite != null && PagoPinCda.TipoTramite != 0)
			{
				int tipoTramite = 0;
				if (!PagoPinCda.TramiteInstructor && PagoPinCda.TipoTramite == (int)EnumTramite.PrimeraVez)
					tipoTramite = (int)EnumTramite.PrimeraVez;

				if (!PagoPinCda.TramiteInstructor && PagoPinCda.TipoTramite == (int)EnumTramite.Recategorizar)
					tipoTramite = (int)EnumTramite.Recategorizar;

				//Se cambia el tramite a 1 (Primera vez) porque en bd esta configurado como tipo de tramite 1 para instructor
				if (PagoPinCda.TramiteInstructor && PagoPinCda.TipoTramite == (int)EnumTramite.PrimeraVez)
					tipoTramite = (int)EnumTramite.PrimeraVezInstructor;

				//Se cambia el tramite a 3 (Recategorizar) porque en bd esta configurado como tipo de tramite 3 para instructor
				if (PagoPinCda.TramiteInstructor && PagoPinCda.TipoTramite == (int)EnumTramite.Recategorizar)
					tipoTramite = (int)EnumTramite.RecategorizarInstructor;

				ListadoTramites.Find(x => x.Id == tipoTramite).Selected = true;
			}
		}

		private async Task RadioSelection(ChangeEventArgs args)
		{
			if (int.TryParse(args.Value.ToString(), out int seleccionTramite) && seleccionTramite != 0)
				await SetPagoPinTramiteCategoria(seleccionTramite, seleccionTramite == 4 || seleccionTramite == 5);
		}

		private async Task SetPagoPinTramiteCategoria(int seleccionTramite, bool isInstructor)
		{
			ListadoTramites.Find(x => x.Id != seleccionTramite).Selected = false;
			ListadoTramites.Find(x => x.Id == seleccionTramite).Selected = true;

			//Se cambia el tramite a 1 (Primera vez) porque en bd esta configurado como tipo de tramite 1 para instructor
			if (isInstructor && seleccionTramite == (int)EnumTramite.PrimeraVezInstructor)
				seleccionTramite = (int)EnumTramite.PrimeraVez;

			//Se cambia el tramite a 3 (Recategorizar) porque en bd esta configurado como tipo de tramite 3 para instructor
			if (isInstructor && seleccionTramite == (int)EnumTramite.RecategorizarInstructor)
				seleccionTramite = (int)EnumTramite.Recategorizar;

			PagoPinCda.OpcionTramite = 1; //Por defecto para pin directo siempre sera Tramite simple
			PagoPinCda.TipoTramite = seleccionTramite;
			PagoPinCda.TramiteInstructor = isInstructor;
			PagoPinCda.CategoriaSeleccionada = string.Empty;
			PagoPinCda.Categoria1 = string.Empty;
			PagoPinCda.CategoriaVehiculo = 0;
			PagoPinCda.PreSeleccionado = false;
					
			await PagoPinChanged.InvokeAsync(PagoPinCda);
		}

		private async Task AddSelectionTramite(EnumTramite enumTramite, bool isInstructor)
		{
			ListadoTramites.Add(new SeleccionTramites()
			{
				Id = (int)enumTramite,
				Nombre = PagoPinConst.DescripcionTramite[enumTramite],
				Instructor = isInstructor,
				Selected = false
			});

			await Task.FromResult(true);
		}

		#endregion Metodos
	}
}