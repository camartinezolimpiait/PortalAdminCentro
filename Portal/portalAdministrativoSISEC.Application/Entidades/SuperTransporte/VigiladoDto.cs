using System.Globalization;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class VigiladoDto
    {
        public string nit { get; set; }
        public string razon_social { get; set; }
        public int codigo_Departamento { get; set; }
        public string nombre_Departamento { get; set; }
        public int codigo_Ciudad { get; set; }
        public string nombre_Ciudad { get; set; }
        public string telefono { get; set; }
        public string mail_Establecimiento { get; set; }
        public string dir_Establecimiento { get; set; }
        public string tipo_Sociedad { get; set; }
        public string tipo_Sociedad_Nombre { get; set; }
        public string id1 { get; set; }
        public string nombre_Documento1 { get; set; }
        public string nro_Documento { get; set; }
        public string nombres_Apellidos { get; set; }
        public string telefono_Rep { get; set; }
        public string email { get; set; }
        public int num_Est_VIGIA { get; set; }

    }

    public class VigiladoResponseDto
    {
        public int id { get; set; }
        //public AttributesVigilado attributes { get; set; }
    }

    public class attributes
    {
        public string NIT { get; set; }
        public string razon_social { get; set; }

    }
}
