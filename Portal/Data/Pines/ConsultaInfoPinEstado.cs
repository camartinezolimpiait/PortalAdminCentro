using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using System;
using System.Linq;

namespace portalAdministrativoSISEC.Data.Pines
{
    public class ConsultaInfoPinEstado
    {
        public string? IdRunt { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }
        public int? Estado { get; set; }
        public int? NumPagina { get; set; }
        public int? NumRegistros { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string? Documento { get =>_documento; set {
                if (_documento != value)
                {
                    _documento = value;
                    OnDocumentoChanged();
                }
            } }
        public string? Pin {
            get => _pin; set
            {
                if (_pin != value)
                {
                    _pin = value;
                    OnPinChanged();
                }
            }
        }
        public int? IdAgenteDispersion { get; set; }
        public int? Canal { get; set; }
        public int? IdCentro { get; set; }
        public int? Opcion { get; set; }
        private string _documento;
        private void OnDocumentoChanged()
        {
            if (Documento != null)
            {
                switch (IdTipoDocumento)
                {
                    case (int)EnumTipoDocumento.CedulaCiudadania:
                        // Por defecto, solo permitir dígitos
                        Documento = new string(Documento.Where(char.IsDigit).ToArray());
                        break;
                    case (int)EnumTipoDocumento.CedulaExtranjeria:
                        // Por defecto, solo permitir dígitos
                        Documento = new string(Documento.Where(char.IsDigit).ToArray());
                        break;
                    case (int)EnumTipoDocumento.TarjetaIdentidad:
                        // Por defecto, solo permitir dígitos
                        Documento = new string(Documento.Where(char.IsDigit).ToArray());
                        break;
                    case (int)EnumTipoDocumento.Pasaporte:
                        Documento = new string(Documento.Where(c => char.IsLetterOrDigit(c)).ToArray()); ;
                        break;
                    case (int)EnumTipoDocumento.PermisoPorProteccionTemporal:
                        Documento = new string(Documento.Where(c => char.IsLetterOrDigit(c)).ToArray());
                        break;
                    // Agrega más casos según sea necesario
                    
                    default:
                        // Por defecto, solo permitir dígitos
                        Documento = new string(Documento.Where(c => char.IsLetterOrDigit(c)).ToArray());
                        break;
                }
            }
        }
        private string _pin;
        private void OnPinChanged()
        {
            if (Pin != null)
            {
                Pin = new string(Pin.Where(char.IsDigit).ToArray());
            }
        }
    }

}