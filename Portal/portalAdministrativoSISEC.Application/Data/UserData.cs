using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Application.Data
{
    public class UserData
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string userName { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string passWord { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string plataforma { get; set; }
    }

    public class EmailRecoverPassResponse
    {
        public string userName { get; set; }
        public string email { get; set; }
        public bool emailConfirmed { get; set; }
    }
}

