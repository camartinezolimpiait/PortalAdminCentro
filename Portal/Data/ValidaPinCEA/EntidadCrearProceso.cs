using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data.ValidaPinCEA
{
    public class EntidadCrearProceso
    {
        public string IdMatricula { get; set; }
        public EntidadCPPersona persona { get; set; }
        public List<EntidadCPExcepcionHll> excepcionesHuellas { get; set; }
        public EntidadCPUsarPin usarPin { get; set; }
        public List<EntidadCPInconsistencia> inconsistencias { get; set; }
        public byte idServicio { get; set; }
        public byte idTipoSolicitud { get; set; }
        public int idCentro { get; set; }
        public long? idProcesoPrincipal { get; set; }
        public int idCategoria { get; set; }
        public int idCategoria2 { get; set; }
        public byte dedoIzquierdo { get; set; }
        public byte dedoDerecho { get; set; }
        public Guid idCreador { get; set; }
        public int idFlujoEstado { get; set; }
        public string nombreAcompanante { get; set; }
        public string telefonoAcompanante { get; set; }
        public int nivelRiesgo { get; set; }
        public byte dedoDefecto { get; set; }
        public int idObjetivo { get; set; }
        public int IdARL { get; set; }
        public long IdEmpresaArmas { get; set; }
        public long? idProceso { get; set; }
        public int idOcp { get; set; }
        public Int64 IdNuac { get; set; }
        public bool UsaCupo { get; set; }
        public string NumeroAprobacion { get; set; }
        public bool esReEnrolado { get; set; }
        public EntidadReEnrolamiento ReEnrolamiento { get; set; }
        public int IdOrigenPin { get; set; }
        public string InfoDispositivo { get; set; }
        public int IdCargo { get; set; }
        public Int64 IdValidacionDC { get; set; }
        public long IdValidacionANI { get; set; }
        public long IdValidacionAFI { get; set; }
        public long IdInformacionPersona { get; set; }
        public bool EsMigrado { get; set; }
        public int IdDetalleAcuerdo { get; set; }
        public bool EsInstructor { get; set; }
        public int IdLiquidacion { get; set; }
        public EntidadDatosComparendo DatosComparendo { get; set; }
        public List<HuellaCandidataExcepcion> HuellasCandidatasExcepcion { get; set; }
    }
}
