namespace portalAdministrativoSISEC.Entidades.Devolucion
{
    public class RespuestaAcciones
    {
        public bool Aprobado { get; set; }
        public long IdProceso { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public string Legal { get; set; }
    }
}
