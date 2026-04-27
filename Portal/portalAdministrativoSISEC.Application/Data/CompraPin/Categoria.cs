namespace portalAdministrativoSISEC.Application.Data.CompraPin
{
    public class Categoria
    {
		public int IdCategoria { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public short IdGrupo { get; set; }

        public bool Seleccionada {  get; set; }
        public int IdTramite { get;set; }
    }
}

