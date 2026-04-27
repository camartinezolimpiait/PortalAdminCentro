namespace portalAdministrativoSISEC.Entidades.Settings
{
    public class ApiFrontMiLicencia : ApiPortalAdministrativo
    {
        public Encrypt Encrypt { get; set; }
    }

    public class Encrypt
    {
        public string Key { get; set; }
        public string Iv { get; set; }
    }
}
