namespace portalAdministrativoSISEC.Application.Data
{
    public class ApplicationSevice
    {
        public readonly string NameLocalStorage = "applicationService";
        public bool Autenticado { get; set; } = false;
        public string UserID { get; set; }
        public string Plataforma { get; set; }
        public string UserName { get; set; }
        public bool ShowCaptcha { get; set; }
        public bool MustChangePassword { get; set; } = false;
        public IEnumerable<MenuInfo> menuList { get; set; }
        public IEnumerable<MenuInfo> MenuListPrincipal { get; set; }
        public IEnumerable<MenuInfo> SubMenuList { get; set; }
        public string FirstNameUser { get; set; }
        public int CentroUsuario { get; set; }
        public long? CodigoRUNTCentro { get; set; }
        public int ClienteId { get; set; }
        public int TipoAfisId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string keyFirma { get; set; }
        public string base64Firma { get; set; }
        public List<ClienteDTO> listaClientes { get; set; }
        public EntradaEnrolamiento entradaEnrolamiento { get; set; }
        public RespuestaEnrolarPersona respuestaEnrolarPersona { get; set; }
        public int idOrigenPin { get; set; }
    }
}

