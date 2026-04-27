namespace portalAdministrativoSISEC.Application.Data
{
    public class MenuInfo
    {
        public int Id { get; set; }
        public int Padre { get; set; }
        public int AplicacionId { get; set; }
        public string EsOpcionMenu { get; set; }
        public string Ruta { get; set; }
        public int Orden { get; set; }
        public string Activo { get; set; }
        public string Pagina { get; set; }
        public string UserId { get; set; }
        public string PageName { get; set; }
        public string MenuName { get; set; }
        public string IconoNombre { get; set; }
    }
}

