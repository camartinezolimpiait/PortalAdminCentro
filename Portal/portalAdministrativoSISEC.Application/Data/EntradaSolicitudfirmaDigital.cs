using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class EntradaSolicitudfirmaDigital
    {
        public string NombreArchivo { get; set; }
        public string Archivo { get; set; }
        public bool Estampado { get; set; }
        public bool FirmaVisible { get; set; }
        public string[] Data { get; set; }
        public int TipoRespuesta { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public Guid CodigoAplicacion { get; set; }
        public string Imagen { get; set; }
        public bool ImagenVisible { get; set; }
        public int PuntoX { get; set; }
        public int PuntoY { get; set; }
    }
}

