using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Data
{
    public class ResetPassword
    {
        public string userName { get; set; }
        public string passWord { get; set; }
        public string plataforma { get; set; }
    }

    public class ResetPasswordResponse
    {
        public bool Respuesta { get; set; }
        public string RespuestaTexto { get; set; }
    }

    public class ChangePassword
    {
        public string userName { get; set; }
        public string oldPassWord { get; set; }
        public string passWord { get; set; }
        public string plataforma { get; set; }
    }

    public class SecurityPolitics
    {
        private readonly int MinLenght;
        private readonly int MaxLenght;
        private readonly bool ContainsUppercase;
        private readonly bool ContainsLowercase;
        private readonly bool ContainsDigit;
        private readonly bool SpecialCharacter;
        private readonly bool RepeatedCharacter;

        public SecurityPolitics()
        {
             MinLenght = 10;
             MaxLenght = 128;
             ContainsUppercase = true;
             ContainsLowercase = true;
             ContainsDigit = true;
             SpecialCharacter = true;
             RepeatedCharacter = true;
        }

        public string ValidatorPassword(string password)
        {
            string message = "<ul>";
            SecurityPolitics politics = new SecurityPolitics();

            if (password.Length < politics.MinLenght || password.Length > politics.MaxLenght)
                message += "<li>La longitud de la contraseña debe estar entre 10 y 128 caracteres</li>";

            if (politics.ContainsUppercase)
                if (!password.Any(c => char.IsUpper(c)))
                    message += "<li>Debe existir al menos un carácter en mayúscula (A-Z)</li>";

            if (politics.ContainsLowercase)
                if (!password.Any(c => char.IsLower(c)))
                    message += "<li>Debe existir al menos un carácter en minúscula (a-z)</li>";

            if (politics.ContainsDigit)
                if (!password.Any(c => char.IsDigit(c)))
                    message += "<li>Debe existir al menos un dígito (0-9)</li>";

            if (politics.SpecialCharacter)
                if (!password.Any(c => char.IsSymbol(c)) && !password.Any(c => char.IsPunctuation(c)))
                    message += "<li>Debe existir al menos un carácter especial (*$-+? _=!.)</li>";

            if (politics.RepeatedCharacter)
            {
                int repeat = 0;
                for(var i = 0; i < (password.Length -1); i++)
                {
                    if (password[i].Equals(password[i + 1]))
                        repeat++;
                }

                if(repeat > 2)
                    message += "<li>No debe haber más de 2 caracteres idénticos en fila</li>";
            }

            message += "</ul>";

            return message;
        }

    }

    

}
