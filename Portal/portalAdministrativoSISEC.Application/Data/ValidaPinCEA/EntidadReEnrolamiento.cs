using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data.ValidaPinCEA
{
    public class EntidadReEnrolamiento
    {
        private long _IdProceso;
        public long IdProceso
        {
            get { return _IdProceso; }
            set { _IdProceso = value; }
        }

        private int _IdMotivo;
        public int IdMotivo
        {
            get { return _IdMotivo; }
            set { _IdMotivo = value; }
        }

        private int _IdEstado;
        public int IdEstado
        {
            get { return _IdEstado; }
            set { _IdEstado = value; }
        }

        private int _IdCentro;
        public int IdCentro
        {
            get { return _IdCentro; }
            set { _IdCentro = value; }
        }
        private Guid _UserId;
        public Guid UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        private string _Observaciones;
        public string Observaciones
        {
            get { return _Observaciones; }
            set { _Observaciones = value; }
        }
        private bool _UsaCupo;
        public bool UsaCupo
        {
            get { return _UsaCupo; }
            set { _UsaCupo = value; }
        }
        private bool _UsaRecaudo;
        public bool UsaRecaudo
        {
            get { return _UsaRecaudo; }
            set { _UsaRecaudo = value; }
        }
    }
}

