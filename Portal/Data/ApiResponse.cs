using System.Net.Http;

namespace portalAdministrativoSISEC.Data
{
    public class ApiResponse<T>
    {
        public HttpResponseMessage HttpResponse { get; set; }
        public T Content { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }
    }
}
