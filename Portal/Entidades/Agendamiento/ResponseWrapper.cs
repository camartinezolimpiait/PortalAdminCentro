namespace portalAdministrativoSISEC.Entidades.Agendamiento
{
    public class ResponseWrapper<T> where T : class
    {
        public bool Estado { get; set; }
        public string Respuesta { get; set; }
        public T Data { get; set; }
    }
}
