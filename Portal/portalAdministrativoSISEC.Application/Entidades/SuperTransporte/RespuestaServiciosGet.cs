using portalAdministrativoSISEC.Entidades.SuperTransporte.CRC;
using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades
{
    public class RespuestaServiciosGet
    {
        public Data data { get; set; }

    }
    public class RespuestaServiciosGetByRunt
    {
        public List<Data> data { get; set; }

    }
    public class Data
    {
        public int id { get; set; }
        public Attributes attributes { get; set; }
    }
    public class Attributes
    {
        public string IDRUNT { get; set; }
        public string nombre { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool de_prueba { get; set; }
        public int? capacidad_certificados { get; set; }
        public int? horas_de_atencion_por_dia { get; set; }
        public string estado_acreditacion_onac { get; set; }
        public string url { get; set; }
        public string name { get; set; }
		public string motivos { get; set; }
		public bool completado { get; set; }
		public InfoHomologadoCls<soporte_adjunto> info_homologado { get; set; }
        public ResolucionHabilitacionCls<soporte_adjunto> resolucion_de_habilitacion { get; set; }
        public RegistroREPSCls<soporte_adjunto> registro_reps { get; set; }
        public InfoConstitucionCls constitucion { get; set; }
        public PolizaCls<soporte_adjunto,soporte_adjunto> poliza { get; set; }
        public InfoProfesionalesCertificadoresCls<soporte_adjunto,soporte_adjunto> profesionales_certificadores { get; set; }
        public InfoProfesionalesSaludCls<soporte_adjunto,soporte_adjunto> prof_salud { get; set; }
        public InfoInfraestructuraCls infraestructura { get; set; }
        public RepresentanteLegalCls<soporte_adjunto> representante_legal { get; set; }
        public InterconexionRUNTCls<soporte_adjunto> interconexion_runt { get; set; }
        public RespuestaServiciosGetVigilado vigilado { get; set; }


    }
}
