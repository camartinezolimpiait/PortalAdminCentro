using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class ResponseResult
    {
        [JsonConverter(typeof(InfoToStringConverter))]
        public string result { get; set; }
        public int id { get; set; }
        public int? exception { get; set; }
        public int status { get; set; }
        public bool isCanceled { get; set; }
        public bool isCompleted { get; set; }
        public bool isCompletedSuccessfully { get; set; }
        public int creationOptions { get; set; }
        public int? asyncState { get; set; }
        public bool isFaulted { get; set; }
    }
}

