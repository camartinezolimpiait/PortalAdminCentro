using portalAdministrativoSISEC.Entidades.Settings;

namespace portalAdministrativoSISEC.Aplication
{
    public class AppSettings
    {
        public string uriSisecAuth { get; set; }
        public string NombreAplicativo { get; set; }
        public string uriSisecParametization { get; set; }
        public string CantidadHuellas { get; set; }
        public string UrlMapaCentros { get; set; }
        public string GuidAplicacion { get; set; }
        public string EndPointApiStrapi { get; set; }
        public string EndPointApiSuperT { get; set; }
        public string EndPointApiVigilados { get; set; }
        public string tokenStrapi { get; set; }
        public string tokenSuperVigilados { get; set; }
        public string StogreAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string ContainerName { get; set; }
        public string SecretKey { get; set; }
        public ApiPortalAdministrativo ApiPortalAdministrativo { get; set; }
        public ApiFrontMiLicencia ApiFrontMiLicencia { get; set; }
        public ConfigPowerBI ConfigPowerBI { get; set; }
    }
}
