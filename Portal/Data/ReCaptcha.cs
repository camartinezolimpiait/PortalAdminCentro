using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class ReCaptcha
    {
        public string Success { get; set; } = "false";
        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
        public string Score { get; set; }
        public string ChallengeTs { get; set; }
        public string HostName { get; set; }
    }
}
