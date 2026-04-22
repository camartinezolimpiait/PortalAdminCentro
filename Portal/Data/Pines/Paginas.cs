using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Data.Pines
{
	public class Paginas
	{
		public Paginas()
		{
			RegistroInicialPagina = (PaginaActual - 1) * RegistrosPorPagina + 1;
			RegistroFinalPagina = Math.Min(PaginaActual * RegistrosPorPagina, TotalRegistros);
		}

		public int PaginaActual { get; set; } = 1;
		public int RegistrosPorPagina { get; set; } = 30;
		public int TotalRegistros { get; set; } = 300;
		public int TotalPaginas { get; set; } = 10;
		public int PaginaInicial { get; set; } = 1;
		public int PaginaFinal { get; set; } = 10;
		public int RegistroInicialPagina { get; set; }
		public int RegistroFinalPagina { get; set; }
	}
}