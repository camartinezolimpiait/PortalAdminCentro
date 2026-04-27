using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class ConvertJsonDto
    {
        public List<SelectSuperTransporteDTO> data { get; set; }
        public static List<SelectSuperTransporteDTO> leerJson(string filePath) {
            try {
                string appPath = Path.Combine(Environment.CurrentDirectory, "Pages", "SuperTransporte", "Maestro");
                var serializer = new JsonSerializer();
                ConvertJsonDto obj = new ConvertJsonDto();
                string fullpath = Path.Combine(appPath, filePath);
                using (var streamReader = new StreamReader(fullpath))
                using (var textReader = new JsonTextReader(streamReader))
                {
                    obj = serializer.Deserialize<ConvertJsonDto>(textReader);
                }
                return obj.data;
            }
            catch (Exception) {
                return null;
            }    
        }
    }
}
