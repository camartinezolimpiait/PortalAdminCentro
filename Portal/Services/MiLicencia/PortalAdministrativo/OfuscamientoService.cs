using portalAdministrativoSISEC.Services.MiLicencia.PortalAdministrativo;
using portalAdministrativoSISEC.Services.MiLicencia;
using System.Threading.Tasks;

public class OfuscamientoService : IOfuscamientoService
{
    private readonly IMiLicenciaService _miLicenciaService;

    public OfuscamientoService(IMiLicenciaService miLicenciaService)
    {
        _miLicenciaService = miLicenciaService;
    }

    public async Task<string> Ofuscamiento(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return data;

        if (ContieneArroba(data))
        {
            return await Task.Run(() => OfuscarCorreo(data));
        }
        else
        {
            return await Task.Run(() => OfuscarNombreCompleto(data));
        }
    }

    private bool ContieneArroba(string input)
    {
        return input.Contains("@");
    }

    private string OfuscarCorreo(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2)
            return email;

        var name = parts[0];
        var domain = parts[1];

        if (name.Length <= 4)
            return $"{name.Substring(0, 4)}{"*".PadRight(name.Length - 4, '*')}@{domain}";
        else
            return $"{name.Substring(0, 4)}{"*".PadRight(name.Length - 4, '*')}@{domain}";
    }

    private string OfuscarNombreCompleto(string nombreCompleto)
    {
        var palabras = nombreCompleto.Split(' ');
        var resultado = new string[palabras.Length];

        for (int i = 0; i < palabras.Length; i++)
        {
            if (palabras[i].Length <= 3)
            {
                resultado[i] = palabras[i];
            }
            else
            {
                resultado[i] = $"{palabras[i].Substring(0, 3)}{"*".PadRight(palabras[i].Length - 3, '*')}";
            }
        }

        return string.Join(" ", resultado);
    }
}
