using System.Collections.Generic;

namespace portalAdministrativoSISEC.Entidades.Recaptcha
{
	public class RespuestaRecaptchaDinamica<T>
	{
		public bool Ok { get; set; }
		public string Mensaje { get; set; } = "";
		public T Data { get; set; }
	}
    public class RecaptchaResponse
    {
        public bool Success { get; set; }
        public string ChallengeTs { get; set; }
        public string Hostname { get; set; }
        public List<string> ErrorCodes { get; set; }
    }
}