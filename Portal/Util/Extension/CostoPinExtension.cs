using portalAdministrativoSISEC.Data.CompraPin;
using System;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util.Extension
{
	public static class CostoPinExtension
	{
		/// <summary>
		/// Calcula el valor a pagar del CEA para el pago relacionado
		/// </summary>
		/// <param name="configuracionCuotas"></param>
		/// <param name="valorDiscriminadoCotizacion"></param>
		/// <returns></returns>
		public static async Task<int> CalcularValorCea(this ConfiguracionCuotas configuracionCuotas, DiscriminadoValorPin valorDiscriminadoCotizacion, bool cuotas)
		{
			if (configuracionCuotas.PermiteCuotas && configuracionCuotas.CuotaSeleccionada != null && !configuracionCuotas.CuotaSeleccionada.NumeroCuotas.Equals(1) && cuotas)
			{
				int valorCrc = (int)Math.Round(valorDiscriminadoCotizacion.Crc / configuracionCuotas.CuotaSeleccionada.NumeroCuotas);
				double valorDispersion = valorDiscriminadoCotizacion.Ansv + valorDiscriminadoCotizacion.Sicov
					+ valorDiscriminadoCotizacion.Banco;

				if (valorCrc + valorDispersion == (double)configuracionCuotas.CuotaSeleccionada.PrimeraCuota)
					return await Task.FromResult(valorCrc);
				else
					return await Task.FromResult((int)Math.Round((double)configuracionCuotas.CuotaSeleccionada.PrimeraCuota - valorDispersion));
			}
			else
				return await Task.FromResult((int)Math.Round(valorDiscriminadoCotizacion.Crc));
		}
	}
}
