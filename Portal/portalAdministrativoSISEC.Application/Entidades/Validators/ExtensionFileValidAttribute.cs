using Microsoft.AspNetCore.Components.Forms;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Entidades.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed class ExtensionFileValidAttribute : ValidationAttribute
    {
        readonly string _extensions;
        readonly int _size;
        
        public string Extensions
        {
            get { return _extensions; }
        }

        public int Size
        {
            get { return _size; }
        }

        public ExtensionFileValidAttribute(string extensions = ".pdf.jpg.jpeg.png.doc.docx", int size = 20)
        {
            _extensions = extensions;
            _size = size; // Por Default 20 MB
        }
        public override bool IsValid(object value)
        {
            bool result = true;
            var file = (IBrowserFile)value;

            if (!Extensions.Contains(Path.GetExtension(file.Name))) {
                this.ErrorMessage = $"Adjunto NO cumple con las extensiones ({this.Extensions})";
                result = false;
            } else if ((file.Size / 1e+6) > Size) {
                this.ErrorMessage = $"Adjunto NO cumple con el tamaño max de {this.Size}MB";
                result = false;
            } else if (file.Size == 0) {
                this.ErrorMessage = $"Adjunto NO puede tener un tamaño igual a 0MB";
                result = false;
            }
                
            
            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, "Adjunto", this.Extensions, this.Size);
        }
    }
}
