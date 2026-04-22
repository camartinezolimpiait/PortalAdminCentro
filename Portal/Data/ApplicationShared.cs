using portalAdministrativoSISEC.Entidades.Agendamiento.Agenda;
using portalAdministrativoSISEC.Entidades.Agendamiento.ConfiguracionCuposReglas;
using System;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Data
{
	public class ApplicationShared : ConfiguracionCupoRegla
	{
		public readonly string NameLocalStorage = "dataStorage";
		public readonly int IdClienteArmas = 3;
		public string Plataforma { get; set; }
		public string IdRunt { get; set; }
		public int IdCentro { get; set; }
		public int? IdPerfil { get; set; }
		public string UserName { get; set; }
		public short IdPoliticaAgendamiento { get; set; }
		public int IdParametroHorario { get; set; }
		public List<CitasIntervalo> CitasIntervaParaBloquear { get; set; }
		public bool CreateNewConfiguration { get; set; }
		public bool IsAgendaFutura { get; set; }
		public DateTime FechaInicioParametrizacion { get; set; }
		public int IdCentroStrappi { get; set; }
	}
}
