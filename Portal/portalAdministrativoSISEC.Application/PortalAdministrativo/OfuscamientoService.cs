namespace portalAdministrativoSISEC.Application.PortalAdministrativo;

public class OfuscamientoService : IOfuscamientoService
{
    public async Task<string> Ofuscamiento(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return data;
        }

        return data.Contains("@")
            ? await Task.Run(() => OfuscarCorreo(data))
            : await Task.Run(() => OfuscarNombreCompleto(data));
    }

    private static string OfuscarCorreo(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return email;
        }

        var name = parts[0];
        var domain = parts[1];

        return $"{name.Substring(0, 4)}{"*".PadRight(name.Length - 4, '*')}@{domain}";
    }

    private static string OfuscarNombreCompleto(string nombreCompleto)
    {
        var palabras = nombreCompleto.Split(' ');
        var resultado = new string[palabras.Length];

        for (var i = 0; i < palabras.Length; i++)
        {
            resultado[i] = palabras[i].Length <= 3
                ? palabras[i]
                : $"{palabras[i].Substring(0, 3)}{"*".PadRight(palabras[i].Length - 3, '*')}";
        }

        return string.Join(" ", resultado);
    }
}
