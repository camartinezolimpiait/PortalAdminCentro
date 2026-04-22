using portalAdministrativoSISEC.Entidades.SuperTransporte;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CEA;
using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades
{
    public class RespuestaServiciosGetCEA
    {
        public DataCEA data { get; set; }

    }
    public class RespuestaServiciosGetByRuntCEA
    {
        public List<DataCEA> data { get; set; }

    }
    public class DataCEA
    {
        public int id { get; set; }
        public AttributesCEA attributes { get; set; }
    }
    public class AttributesCEA
    {
        public string IDRUNT { get; set; }
        public string nombre { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool de_prueba { get; set; }
        public int capacidad_certificados { get; set; }
        public int horas_de_atencion_por_dia { get; set; }
        public string estado_acreditacion_onac { get; set; }
        public string nivel { get; set; }
        public bool auth_cursos_normas { get; set; }
        public string num_cert_conformidad { get; set; }
        public string fecha_aprobacion_cert { get; set; }
        public string fecha_vencimiento_cert { get; set; }
        public bool forma_instructores { get; set; }
        public string motivos { get; set; }
        public bool completado { get; set; }
        public InfoConstitucionClsCEA<soporte_adjunto> constitucion { get; set; }
        public InfoHomologadoClsCEA<soporte_adjunto> homologado { get; set; }
        public ProgramaConvenioClsCEA<soporte_adjunto> convenios { get; set; }
        public CertificacionOECClsCEA<soporte_adjunto> certificacion_oec { get; set; }
        public ResolucionHabilitacionClsCEA<soporte_adjunto> resolucion_de_habilitacion { get; set; }
        public PolizaClsCEA<soporte_adjunto, soporte_adjunto> poliza { get; set; }
        public LicenciaFuncionamientoClsCEA<soporte_adjunto> licencia_funcionamiento { get; set; }        
        public InfraestructuraClsResponseCEA infraestructura { get; set; }
        public InfoBasicaClsCEA attributes { get; set; }
        public List<InfoPropietariosDtoCEA> propietarios { get; set; }
        public List<InstructorDtoCEA<GetFile>> instructores { get; set; }
        public List<InfoVehiculosDtoCEA<GetFile>> vehiculos { get; set; }
        public RepresentanteLegalClsCEA<soporte_adjunto> representante_legal { get; set; }
        public CategoriasAutorizadas categorias_autorizadas { get; set; }
        public RespuestaServiciosGetVigilado vigilado { get; set; }

    }
}
