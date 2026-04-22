using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Shared
{
	public partial class GridDinamico
	{
		#region Variables

		[Parameter]
		public List<object> Listado { get; set; }


		[Parameter]
		public EventCallback<int> paginaChanged { get; set; }

		private bool isLoading = false;
		private string[] propertyNames;
		private int seleccion = 0;
		#endregion Variables
		protected override async Task OnInitializedAsync()
		{
			isLoading = true;
			
			Listado = await cargarlistado(0);
			ObtenerCabecera();
			isLoading = false;
		}

		public async Task<List<object>> cargarlistado(int tipolista)
		{
			List<object> listaFinal = new List<object>(); ;
			switch (tipolista)
			{
					case 0:
					var listado = new List<object>() { new { Nombre = "Antonio",Apellido="Garcia", TipoDocumento = 1, NumeroDocumento = "1231231231" }, new { Nombre = "Jose", Apellido = "Ramirez", TipoDocumento = 2 , NumeroDocumento="1212121212" } };
					listaFinal = listado;
					break;
					case 1:
					var listado2 = new List<object>() { new { Nombre = "Jose", Apellido = "Garcia", CodigoCliente = 1, Estado = "Activo", ActivoCompra=false }, new { Nombre = "Jose", Apellido = "Garcia", CodigoCliente = 2, Estado = "Deshabilitado", ActivoCompra = false }, new { Nombre = "Ramon", Apellido = "Martinez", CodigoCliente = 3, Estado = "Eliminado", ActivoCompra = false } };
					listaFinal = listado2;
					break;
					case 2:
					var listado3 = new List<object>() { new { Id = 10, Pin = "92139891293891823", Valor = "120.544", Retencion = "3%" }, new { Id = 11, Pin = "92139891293891824", Valor = "125.544", Retencion = "4%" }, new { Id = 12, Pin = "92139891293891825", Valor = "130.544", Retencion = "6%" } };
					listaFinal = listado3;
					break;
					default:

						break;

					
			}
			return listaFinal;
		}
		public void ObtenerCabecera()
		{
			if (Listado.Any())
			{
				propertyNames = Listado[0].GetType().GetProperties().Select(p => p.Name).ToArray();
				// Modificar los nombres que necesitemos 
			}
			
		}
		public async Task OnSiguienteClicked(int valor)
		{
			
			if (valor <= 2)
			{
				if (valor != 2)
				{
                    valor = valor + 1;
                }
				seleccion = valor;
				await paginaChanged.InvokeAsync(seleccion);
                Listado = await cargarlistado(valor);
                ObtenerCabecera();
            }
            
        }
        public async Task OnAtrasClicked(int valor)
        {

			if (valor > 0)
			{
				valor = valor - 1;
				seleccion = valor;
                Listado = await cargarlistado(valor);
                await paginaChanged.InvokeAsync(seleccion);
                ObtenerCabecera();
            }
            
        }
    }
}
